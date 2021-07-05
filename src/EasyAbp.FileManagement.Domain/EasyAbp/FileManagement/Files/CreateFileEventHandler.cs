using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;

namespace EasyAbp.FileManagement.Files
{
    public class CreateFileEventHandler : IDistributedEventHandler<CreateFileEto>, ITransientDependency
    {
        private readonly IFileRepository _fileRepository;

        private readonly IFileManager _fileManager;

        public CreateFileEventHandler(IFileRepository fileRepository, IFileManager fileManager)
        {
            _fileRepository = fileRepository;
            _fileManager = fileManager;
        }

        [UnitOfWork(true)]
        public virtual async Task HandleEventAsync(CreateFileEto eventData)
        {
            var parent = eventData.ParentId.HasValue ? await _fileRepository.FindAsync(eventData.ParentId.Value) : null;

            var file = await _fileManager.CreateAsync(eventData.FileContainerName, eventData.OwnerUserId, eventData.FileName, eventData.MimeType, eventData.FileType, parent, null);

            await _fileRepository.InsertAsync(file, true);
        }
    }
}