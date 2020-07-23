using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyAbp.Abp.Trees;
using EasyAbp.FileManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace EasyAbp.FileManagement.Files
{
    public class FileRepository : EfCoreTreeRepository<FileManagementDbContext, File>, IFileRepository
    {
        public FileRepository(IDbContextProvider<FileManagementDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public virtual async Task<File> FindByFilePathAsync(string filePath, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return includeDetails
                ? await WithDetails().Where(x => x.FilePath == filePath).SingleOrDefaultAsync(cancellationToken)
                : await DbSet.Where(x => x.FilePath == filePath).SingleOrDefaultAsync(cancellationToken);
        }
    }
}