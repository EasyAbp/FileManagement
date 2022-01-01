using System.Threading.Tasks;
using EasyAbp.FileManagement.Options.Containers;
using JetBrains.Annotations;
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
        private readonly IFileManager _fileManager;
        private readonly IFileRepository _fileRepository;
        private readonly IFileContainerConfigurationProvider _configurationProvider;

        public UnusedBlobCleaner(
            ICurrentTenant currentTenant,
            IFileManager fileManager,
            IFileRepository fileRepository,
            IFileContainerConfigurationProvider configurationProvider)
        {
            _currentTenant = currentTenant;
            _fileManager = fileManager;
            _fileRepository = fileRepository;
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

            // This handler will be invoked always after the UOW is completed, so delete the BLOB here.
            // See: https://github.com/abpframework/abp/issues/11100
            if (await _fileRepository.FirstOrDefaultAsync(fileContainerName, blobName) == null)
            {
                await _fileManager.DeleteBlobAsync(fileContainerName, blobName);
            }
        }
    }
}