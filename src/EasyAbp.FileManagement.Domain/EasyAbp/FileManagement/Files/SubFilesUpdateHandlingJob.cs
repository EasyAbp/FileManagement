using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace EasyAbp.FileManagement.Files;

public class SubFilesUpdateHandlingJob : AsyncBackgroundJob<SubFilesUpdateHandlingJobArgs>, ITransientDependency
{
    private readonly IRecursiveDirectoryStatisticDataUpdater _updater;

    public SubFilesUpdateHandlingJob(IRecursiveDirectoryStatisticDataUpdater updater)
    {
        _updater = updater;
    }

    public override async Task ExecuteAsync(SubFilesUpdateHandlingJobArgs args)
    {
        await _updater.HandleEventAsync(args.SubFilesChangedEto);
    }
}