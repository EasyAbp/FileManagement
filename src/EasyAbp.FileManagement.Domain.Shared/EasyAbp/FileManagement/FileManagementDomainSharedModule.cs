using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Volo.Abp.Localization;
using EasyAbp.FileManagement.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace EasyAbp.FileManagement
{
    [DependsOn(
        typeof(AbpValidationModule),
        typeof(AbpDddDomainSharedModule)
    )]
    public class FileManagementDomainSharedModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<FileManagementDomainSharedModule>();
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<FileManagementResource>("en")
                    .AddBaseTypes(typeof(AbpValidationResource))
                    .AddVirtualJson("/EasyAbp/FileManagement/Localization");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("EasyAbp.FileManagement", typeof(FileManagementResource));
            });
        }
    }
}
