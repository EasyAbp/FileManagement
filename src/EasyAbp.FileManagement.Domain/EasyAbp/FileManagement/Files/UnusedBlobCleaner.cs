using System.Threading.Tasks;
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

        public UnusedBlobCleaner(
            IFileManager fileManager,
            IFileRepository fileRepository)
        {
            _fileManager = fileManager;
            _fileRepository = fileRepository;
        }

        public virtual async Task HandleEventAsync(EntityDeletedEventData<File> eventData)
        {
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

            if (await _fileRepository.FirstOrDefaultAsync(eventData.OldBlobName) == null)
            {
                var blobContainer = _fileManager.GetBlobContainer(file);

                await blobContainer.DeleteAsync(eventData.OldBlobName);
            }
        }
    }
}