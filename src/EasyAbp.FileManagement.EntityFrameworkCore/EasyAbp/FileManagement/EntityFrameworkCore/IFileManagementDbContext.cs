using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace EasyAbp.FileManagement.EntityFrameworkCore
{
    [ConnectionStringName(FileManagementDbProperties.ConnectionStringName)]
    public interface IFileManagementDbContext : IEfCoreDbContext
    {
        /* Add DbSet for each Aggregate Root here. Example:
         * DbSet<Question> Questions { get; }
         */
    }
}