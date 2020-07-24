using Volo.Abp.BlobStoring;
using Volo.Abp.Caching;
using Volo.Abp.Modularity;

namespace EasyAbp.FileManagement
{
    [DependsOn(
        typeof(FileManagementDomainSharedModule),
        typeof(AbpBlobStoringModule),
        typeof(AbpCachingModule)
    )]
    public class FileManagementDomainModule : AbpModule
    {

    }
}
