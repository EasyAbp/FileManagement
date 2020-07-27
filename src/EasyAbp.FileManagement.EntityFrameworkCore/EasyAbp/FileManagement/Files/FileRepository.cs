using System;
using System.Collections.Generic;
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

        public virtual async Task<List<File>> GetListAsync(Guid? parentId, string fileContainerName, Guid? ownerUserId,
            FileType? specifiedFileType = null, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(x => x.ParentId == parentId && x.OwnerUserId == ownerUserId &&
                            x.FileContainerName == fileContainerName).WhereIf(specifiedFileType.HasValue,
                    x => x.FileType == specifiedFileType.Value).ToListAsync(cancellationToken);
        }

        public virtual async Task<File> FindAsync(string fileName, Guid? parentId, string fileContainerName, Guid? ownerUserId,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(x => x.ParentId == parentId && x.OwnerUserId == ownerUserId &&
                            x.FileContainerName == fileContainerName && x.FileName == fileName)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<File> FirstOrDefaultAsync(string hash, long byteSize, CancellationToken cancellationToken = default)
        {
            return await DbSet.Where(x => x.Hash == hash && x.ByteSize == byteSize)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<File> FirstOrDefaultAsync(string blobName, CancellationToken cancellationToken = default)
        {
            return await DbSet.Where(x => x.BlobName == blobName).FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<SubFilesStatisticDataModel> GetSubFilesStatisticDataAsync(Guid id,
            CancellationToken cancellationToken = default)
        {
            return await DbSet.Where(x => x.ParentId == id).GroupBy(x => true).Select(x =>
                new SubFilesStatisticDataModel
                {
                    SubFilesQuantity = x.Count(),
                    ByteSize = x.Sum(y => y.ByteSize)
                }).FirstOrDefaultAsync(cancellationToken);
        }
    }
}