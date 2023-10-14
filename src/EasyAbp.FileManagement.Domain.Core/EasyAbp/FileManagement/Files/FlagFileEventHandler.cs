using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;

namespace EasyAbp.FileManagement.Files
{
    public class FlagFileEventHandler : IDistributedEventHandler<FlagFileEto>, ITransientDependency
    {
        private readonly IFileRepository _fileRepository;

        public FlagFileEventHandler(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }
        
        [UnitOfWork(true)]
        public virtual async Task HandleEventAsync(FlagFileEto eventData)
        {
            var file = await _fileRepository.GetAsync(eventData.FileId);

            file.SetFlag(eventData.Flag);

            await _fileRepository.UpdateAsync(file, true);
        }
    }
}