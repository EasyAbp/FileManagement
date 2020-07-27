using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.Uow;

namespace EasyAbp.FileManagement.Files
{
    public class RecursiveDirectoryStatisticDataUpdater : ILocalEventHandler<SubFileUpdatedEto>, ITransientDependency
    {
        private readonly IFileRepository _fileRepository;

        public RecursiveDirectoryStatisticDataUpdater(
            IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        [UnitOfWork(true)]
        public virtual async Task HandleEventAsync(SubFileUpdatedEto eventData)
        {
            var parent = eventData.Parent;

            while (parent != null)
            {
                var statisticData = await _fileRepository.GetSubFilesStatisticDataAsync(parent.Id);
            
                parent.ForceSetStatisticData(statisticData);
            
                await _fileRepository.UpdateAsync(parent, true);

                parent = parent.ParentId.HasValue ? await _fileRepository.FindAsync(parent.ParentId.Value) : null;
            }
        }
    }
}