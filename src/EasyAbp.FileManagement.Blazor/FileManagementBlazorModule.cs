using Microsoft.Extensions.DependencyInjection;
using EasyAbp.FileManagement.Blazor.Menus;
using Volo.Abp.AspNetCore.Components.Web.Theming;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;

namespace EasyAbp.FileManagement.Blazor
{
    [DependsOn(
        typeof(FileManagementApplicationContractsModule),
        typeof(AbpAspNetCoreComponentsWebThemingModule),
        typeof(AbpMapperlyModule)
        )]
    public class FileManagementBlazorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddMapperlyObjectMapper<FileManagementBlazorModule>();

            Configure<AbpNavigationOptions>(options =>
            {
                options.MenuContributors.Add(new FileManagementMenuContributor());
            });

            Configure<AbpRouterOptions>(options =>
            {
                options.AdditionalAssemblies.Add(typeof(FileManagementBlazorModule).Assembly);
            });
        }
    }
}