using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;

namespace EasyAbp.FileManagement
{
    [DependsOn(
        typeof(FileManagementApplicationContractsModule),
        typeof(AbpHttpClientModule))]
    public class FileManagementHttpApiClientModule : AbpModule
    {
        public const string RemoteServiceName = "EasyAbpFileManagement";

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddHttpClientProxies(
                typeof(FileManagementApplicationContractsModule).Assembly,
                RemoteServiceName
            );
        }
    }
}
