using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace EasyAbp.FileManagement;

public class RealtimeBackgroundJobManager : IBackgroundJobManager, ITransientDependency
{
    private readonly AbpBackgroundJobOptions _options;
    private readonly IServiceProvider _serviceProvider;

    public RealtimeBackgroundJobManager(
        IOptions<AbpBackgroundJobOptions> options,
        IServiceProvider serviceProvider)
    {
        _options = options.Value;
        _serviceProvider = serviceProvider;
    }

    public async Task<string> EnqueueAsync<TArgs>(TArgs args,
        BackgroundJobPriority priority = BackgroundJobPriority.Normal, TimeSpan? delay = null)
    {
        var job = _serviceProvider.GetService(_options.GetJob<TArgs>().JobType);

        switch (job)
        {
            case IAsyncBackgroundJob<TArgs> asyncBackgroundJob:
                await asyncBackgroundJob.ExecuteAsync(args);
                break;
            case IBackgroundJob<TArgs> backgroundJob:
                backgroundJob.Execute(args);
                break;
        }

        return null;
    }
}