using System;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Options.Containers;
using JetBrains.Annotations;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace EasyAbp.FileManagement.Files
{
    public class UnusedBlobCleaner :
        IDistributedEventHandler<EntityDeletedEto<FileEto>>,
        IDistributedEventHandler<FileBlobNameChangedEto>,
        ITransientDependency
    {
        private readonly ICurrentTenant _currentTenant;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IFileContainerConfigurationProvider _configurationProvider;

        public UnusedBlobCleaner(
            ICurrentTenant currentTenant,
            IBackgroundJobManager backgroundJobManager,
            IFileContainerConfigurationProvider configurationProvider)
        {
            _currentTenant = currentTenant;
            _backgroundJobManager = backgroundJobManager;
            _configurationProvider = configurationProvider;
        }

        [UnitOfWork]
        public virtual async Task HandleEventAsync(EntityDeletedEto<FileEto> eventData)
        {
            using var changeTenant = _currentTenant.Change(eventData.Entity.TenantId);

            await TryEnqueueCleaningJobAsync(eventData.Entity.FileType, eventData.Entity.FileContainerName,
                eventData.Entity.BlobName);
        }

        [UnitOfWork]
        public virtual async Task HandleEventAsync(FileBlobNameChangedEto eventData)
        {
            using var changeTenant = _currentTenant.Change(eventData.TenantId);

            if (eventData.NewBlobName == eventData.OldBlobName)
            {
                return;
            }

            await TryEnqueueCleaningJobAsync(eventData.FileType, eventData.FileContainerName, eventData.OldBlobName);
        }

        protected virtual async Task TryEnqueueCleaningJobAsync(FileType fileType, [NotNull] string fileContainerName,
            [CanBeNull] string blobName)
        {
            if (fileType is not FileType.RegularFile || blobName is null)
            {
                return;
            }

            if (_configurationProvider.Get(fileContainerName).RetainUnusedBlobs)
            {
                return;
            }

            // Todo: Improve this.
            // 5s delay for local distributed event bus.
            // Since this handler may be invoked inside the transaction (when the file entity change has not been saved).
            await _backgroundJobManager.EnqueueAsync(new UnusedBlobCleaningArgs
            {
                TenantId = _currentTenant.Id,
                FileContainerName = fileContainerName,
                BlobName = blobName
            }, BackgroundJobPriority.Low, TimeSpan.FromSeconds(5));
        }
    }
}