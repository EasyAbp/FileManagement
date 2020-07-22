using Volo.Abp.Modularity;

namespace EasyAbp.FileManagement
{
    [DependsOn(
        typeof(FileManagementDomainSharedModule)
        )]
    public class FileManagementDomainModule : AbpModule
    {

    }
}
