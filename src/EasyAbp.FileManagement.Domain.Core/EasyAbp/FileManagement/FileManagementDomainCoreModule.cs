using EasyAbp.FileManagement.Files;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Domain;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.Modularity;
using Volo.Abp.Users;

namespace EasyAbp.FileManagement
{
    [DependsOn(
        typeof(FileManagementDomainSharedModule),
        typeof(AbpAutoMapperModule),
        typeof(AbpBackgroundJobsAbstractionsModule),
        typeof(AbpDddDomainModule),
        typeof(AbpUsersDomainModule)
    )]
    public class FileManagementDomainCoreModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpDistributedEntityEventOptions>(options =>
            {
                options.EtoMappings.Add<File, FileEto>(typeof(FileManagementDomainCoreModule));
                options.AutoEventSelectors.Add<File>();
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAutoMapperObjectMapper<FileManagementDomainCoreModule>();

            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddProfile<FileManagementDomainAutoMapperProfile>(validate: true);
            });
        }
    }
}
