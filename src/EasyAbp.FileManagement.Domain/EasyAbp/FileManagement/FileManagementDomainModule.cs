using EasyAbp.FileManagement.Files;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BlobStoring;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.Modularity;

namespace EasyAbp.FileManagement
{
    [DependsOn(
        typeof(FileManagementDomainSharedModule),
        typeof(AbpAutoMapperModule),
        typeof(AbpBackgroundJobsAbstractionsModule),
        typeof(AbpBlobStoringModule),
        typeof(AbpCachingModule)
    )]
    public class FileManagementDomainModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpDistributedEntityEventOptions>(options =>
            {
                options.EtoMappings.Add<File, FileEto>(typeof(FileManagementDomainModule));
                options.AutoEventSelectors.Add<File>();
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAutoMapperObjectMapper<FileManagementDomainModule>();

            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddProfile<FileManagementDomainAutoMapperProfile>(validate: true);
            });
        }
    }
}
