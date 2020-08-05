using System.Threading.Tasks;
using EasyAbp.FileManagement.Containers;
using EasyAbp.FileManagement.Options.Containers;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;

namespace EasyAbp.FileManagement.Files
{
    public class UnusedBlobCleaner : ILocalEventHandler<EntityDeletedEventData<File>>,
        ILocalEventHandler<FileBlobNameChangedEto>, ITransientDependency
    {
        private readonly IFileManager _fileManager;
        private readonly IFileRepository _fileRepository;
        private readonly IFileContainerConfigurationProvider _configurationProvider;

        public UnusedBlobCleaner(
            IFileManager fileManager,
            IFileRepository fileRepository,
            IFileContainerConfigurationProvider configurationProvider)
        {
            _fileManager = fileManager;
            _fileRepository = fileRepository;
            _configurationProvider = configurationProvider;
        }

        public virtual async Task HandleEventAsync(EntityDeletedEventData<File> eventData)
        {
            if (_configurationProvider.Get(eventData.Entity.FileContainerName).RetainDeletedBlobs)
            {
                return;
            }
            
            if (await _fileRepository.FirstOrDefaultAsync(eventData.Entity.BlobName) == null)
            {
                await _fileManager.DeleteBlobAsync(eventData.Entity);
            }
        }

        public virtual async Task HandleEventAsync(FileBlobNameChangedEto eventData)
        {
            var file = await _fileRepository.FindAsync(eventData.FileId);

            if (file == null)
            {
                return;
            }

            if (_configurationProvider.Get(file.FileContainerName).RetainDeletedBlobs)
            {
                return;
            }
            
            if (await _fileRepository.FirstOrDefaultAsync(eventData.OldBlobName) == null)
            {
                var blobContainer = _fileManager.GetBlobContainer(file);

                await blobContainer.DeleteAsync(eventData.OldBlobName);
            }
        }
    }
}