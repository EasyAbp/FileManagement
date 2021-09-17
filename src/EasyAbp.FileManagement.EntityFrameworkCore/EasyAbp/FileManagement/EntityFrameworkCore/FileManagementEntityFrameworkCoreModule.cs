using EasyAbp.FileManagement.Files;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using File = EasyAbp.FileManagement.Files.File;

namespace EasyAbp.FileManagement.EntityFrameworkCore
{
    [DependsOn(
        typeof(FileManagementDomainModule),
        typeof(AbpEntityFrameworkCoreModule)
    )]
    public class FileManagementEntityFrameworkCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<FileManagementDbContext>(options =>
            {
                /* Add custom repositories here. Example:
                 * options.AddRepository<Question, EfCoreQuestionRepository>();
                 */
                options.AddRepository<File, FileRepository>();
            });
        }
    }
}
