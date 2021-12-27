﻿using Localization.Resources.AbpUi;
using EasyAbp.FileManagement.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;
using EasyAbp.FileManagement.Files.Dtos;

namespace EasyAbp.FileManagement
{
    [DependsOn(
        typeof(FileManagementApplicationContractsModule),
        typeof(AbpAspNetCoreMvcModule))]
    public class FileManagementHttpApiModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<IMvcBuilder>(mvcBuilder =>
            {
                mvcBuilder.AddApplicationPartIfNotExists(typeof(FileManagementHttpApiModule).Assembly);
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Get<FileManagementResource>()
                    .AddBaseTypes(typeof(AbpUiResource));
            });

            Configure<AbpAspNetCoreMvcOptions>(options =>
            {
                options.ConventionalControllers.FormBodyBindingIgnoredTypes.Add(typeof(CreateFileWithStreamInput));
                options.ConventionalControllers.FormBodyBindingIgnoredTypes.Add(typeof(CreateManyFileWithStreamInput));
                options.ConventionalControllers.FormBodyBindingIgnoredTypes.Add(typeof(UpdateFileWithStreamInput));
            });
        }
    }
}
