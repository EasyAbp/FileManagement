using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyAbp.FileManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace EasyAbp.FileManagement.Files
{
    public class FileRepository : EfCoreRepository<FileManagementDbContext, File, Guid>, IFileRepository
    {
        public FileRepository(IDbContextProvider<FileManagementDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public virtual async Task<File> FindByFilePathAsync(string filePath, string fileContainerName,
            Guid? ownerUserId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(x => x.FilePath == filePath && x.FileContainerName == fileContainerName &&
                            x.OwnerUserId == ownerUserId).SingleOrDefaultAsync(cancellationToken);
        }
    }
}