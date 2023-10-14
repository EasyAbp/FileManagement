using Volo.Abp.EventBus.Distributed;

namespace EasyAbp.FileManagement.Files;

public interface IRecursiveDirectoryStatisticDataUpdater : IDistributedEventHandler<SubFilesChangedEto>
{
}