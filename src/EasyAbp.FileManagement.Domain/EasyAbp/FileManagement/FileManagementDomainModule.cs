using Volo.Abp.BackgroundJobs;
using Volo.Abp.BlobStoring;
using Volo.Abp.Modularity;

namespace EasyAbp.FileManagement
{
    [DependsOn(
        typeof(FileManagementDomainCoreModule),
        typeof(AbpBackgroundJobsAbstractionsModule),
        typeof(AbpBlobStoringModule)
    )]
    public class FileManagementDomainModule : AbpModule
    {
    }
}