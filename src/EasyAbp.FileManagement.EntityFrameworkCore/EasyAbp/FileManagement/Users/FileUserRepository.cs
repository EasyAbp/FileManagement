using EasyAbp.FileManagement.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Users.EntityFrameworkCore;

namespace EasyAbp.FileManagement.Users;

public class FileUserRepository : EfCoreUserRepositoryBase<IFileManagementDbContext, FileUser>, IFileUserRepository
{
    public FileUserRepository(IDbContextProvider<IFileManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}