using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DistributedLocking;
using Volo.Abp.EventBus;
using Volo.Abp.Uow;

namespace EasyAbp.FileManagement.Files;

public class RecursiveDirectoryStatisticDataUpdater : ILocalEventHandler<SubFilesChangedEto>, ITransientDependency
{
    protected virtual TimeSpan Timeout => TimeSpan.FromSeconds(3);

    private readonly IAbpDistributedLock _abpDistributedLock;
    private readonly IFileRepository _fileRepository;

    public RecursiveDirectoryStatisticDataUpdater(
        IAbpDistributedLock abpDistributedLock,
        IFileRepository fileRepository)
    {
        _abpDistributedLock = abpDistributedLock;
        _fileRepository = fileRepository;
    }

    [UnitOfWork(true)]
    public virtual async Task HandleEventAsync(SubFilesChangedEto eventData)
    {
        await UpdateStatisticDataAsync(eventData.DirectoryId);
    }

    [UnitOfWork(true)]
    protected virtual async Task UpdateStatisticDataAsync(Guid? directoryId)
    {
        while (directoryId != null)
        {
            var statisticData = await _fileRepository.GetSubFilesStatisticDataAsync(directoryId.Value);

            await using var lockHandle =
                await _abpDistributedLock.TryAcquireAsync(await GetDistributedLockKeyAsync(directoryId.Value), Timeout);

            var directory = await _fileRepository.FindAsync(directoryId.Value);

            if (!directory.TryUpdateStatisticData(statisticData))
            {
                break;
            }

            await _fileRepository.UpdateAsync(directory, true);

            directoryId = directory.ParentId;
        }
    }

    protected virtual Task<string> GetDistributedLockKeyAsync(Guid fileId)
    {
        return Task.FromResult($"updating-dir-stat-{fileId}");
    }
}