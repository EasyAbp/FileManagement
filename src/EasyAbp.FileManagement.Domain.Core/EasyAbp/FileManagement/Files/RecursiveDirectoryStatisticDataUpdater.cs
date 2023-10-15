using System;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Uow;

namespace EasyAbp.FileManagement.Files;

public class RecursiveDirectoryStatisticDataUpdater : IRecursiveDirectoryStatisticDataUpdater, ITransientDependency
{
    protected virtual TimeSpan Timeout => TimeSpan.FromSeconds(3);

    private readonly IBackgroundJobManager _backgroundJobManager;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IAbpDistributedLock _abpDistributedLock;
    private readonly IFileRepository _fileRepository;

    public RecursiveDirectoryStatisticDataUpdater(
        IBackgroundJobManager backgroundJobManager,
        IUnitOfWorkManager unitOfWorkManager,
        IAbpDistributedLock abpDistributedLock,
        IFileRepository fileRepository)
    {
        _backgroundJobManager = backgroundJobManager;
        _unitOfWorkManager = unitOfWorkManager;
        _abpDistributedLock = abpDistributedLock;
        _fileRepository = fileRepository;
    }

    public virtual async Task HandleEventAsync(SubFilesChangedEto eventData)
    {
        if (eventData.UseBackgroundJob)
        {
            await _backgroundJobManager.EnqueueAsync(
                new SubFilesUpdateHandlingJobArgs(eventData.TenantId, eventData.DirectoryId));
        }
        else
        {
            await UpdateStatisticDataAsync(eventData.DirectoryId);
        }
    }

    protected virtual async Task UpdateStatisticDataAsync(Guid? directoryId)
    {
        while (directoryId != null)
        {
            var statisticData = await _fileRepository.GetSubFilesStatisticDataAsync(directoryId.Value);

            await using var lockHandle =
                await _abpDistributedLock.TryAcquireAsync(await GetDistributedLockKeyAsync(directoryId.Value), Timeout);

            if (lockHandle is null)
            {
                throw new AbpDbConcurrencyException();
            }

            var uow = _unitOfWorkManager.Begin(true);

            var directory = await _fileRepository.FindAsync(directoryId.Value);

            if (!directory.TryUpdateStatisticData(statisticData))
            {
                break;
            }

            await _fileRepository.UpdateAsync(directory, true);

            directoryId = directory.ParentId;

            await uow.CompleteAsync();
        }
    }

    protected virtual Task<string> GetDistributedLockKeyAsync(Guid fileId)
    {
        return Task.FromResult($"updating-dir-stat-{fileId}");
    }
}