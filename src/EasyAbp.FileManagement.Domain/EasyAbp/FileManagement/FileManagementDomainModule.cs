using EasyAbp.FileManagement.BackgroudWorkers;
using Volo.Abp;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.BlobStoring;
using Volo.Abp.Caching;
using Volo.Abp.Modularity;

namespace EasyAbp.FileManagement
{
    [DependsOn(
        typeof(FileManagementDomainSharedModule),
        typeof(AbpBlobStoringModule),
        typeof(AbpCachingModule),
        typeof(AbpBackgroundWorkersModule)
    )]
    public class FileManagementDomainModule : AbpModule
    {
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            context.AddBackgroundWorker<BackupDatabaseWorker>();
        }
    }
}
