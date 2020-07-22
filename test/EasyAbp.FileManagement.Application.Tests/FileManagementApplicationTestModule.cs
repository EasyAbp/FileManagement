using Volo.Abp.Modularity;

namespace EasyAbp.FileManagement
{
    [DependsOn(
        typeof(FileManagementApplicationModule),
        typeof(FileManagementDomainTestModule)
        )]
    public class FileManagementApplicationTestModule : AbpModule
    {

    }
}
