using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace EasyAbp.FileManagement.Files
{
    public class RecursiveDirectoryStatisticDataUpdater : ILocalEventHandler<SubFileUpdatedEto>, ITransientDependency
    {
        private readonly IFileRepository _fileRepository;
        private readonly ICurrentTenant _currentTenant;

        public RecursiveDirectoryStatisticDataUpdater(
            IFileRepository fileRepository,
            ICurrentTenant currentTenant)
        {
            _fileRepository = fileRepository;
            _currentTenant = currentTenant;
        }

        [UnitOfWork(true)]
        public virtual async Task HandleEventAsync(SubFileUpdatedEto eventData)
        {
            using (_currentTenant.Change(eventData.Parent.TenantId))
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
}