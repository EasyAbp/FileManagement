using EasyAbp.FileManagement.Files;
using EasyAbp.FileManagement.Users;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.Users.EntityFrameworkCore;

namespace EasyAbp.FileManagement.EntityFrameworkCore
{
    [DependsOn(
        typeof(FileManagementDomainCoreModule),
        typeof(AbpEntityFrameworkCoreModule),
        typeof(AbpUsersEntityFrameworkCoreModule)
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
                options.AddRepository<FileUser, FileRepository>();
            });
        }
    }
}
