using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public virtual async Task<File> FindAsync(string fileName, Guid? parentId, string fileContainerName, Guid? ownerUserId,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(x => x.ParentId == parentId && x.OwnerUserId == ownerUserId &&
                            x.FileContainerName == fileContainerName && x.FileName == fileName)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<File> FindByFilePathAsync(string filePath, string fileContainerName,
            Guid? ownerUserId, FileType? specifiedFileType = null, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(x => x.FilePath == filePath && x.OwnerUserId == ownerUserId &&
                            x.FileContainerName == fileContainerName)
                .WhereIf(specifiedFileType.HasValue, x => x.FileType == specifiedFileType)
                .SingleOrDefaultAsync(cancellationToken);
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

        [Obsolete]
        public override Task DeleteAsync(Expression<Func<File, bool>> predicate, bool autoSave = false,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotSupportedException();
        }

        public override async Task DeleteAsync(File file, bool autoSave = false, CancellationToken cancellationToken = new CancellationToken())
        {
            var parent = file.ParentId.HasValue
                ? await GetAsync(file.ParentId.Value, true, cancellationToken)
                : null;

            parent?.TryAddSubFileUpdatedDomainEvent();
            
            await base.DeleteAsync(file, autoSave, cancellationToken);
        }

        public override async Task DeleteAsync(Guid id, bool autoSave = false, CancellationToken cancellationToken = new CancellationToken())
        {
            var file = await FindAsync(id, cancellationToken: cancellationToken);
            
            if (file == null)
            {
                return;
            }

            await DeleteAsync(file, autoSave, cancellationToken);
        }

        public virtual async Task DeleteSubFilesAsync(File parent, string fileContainerName, Guid? ownerUserId,
            bool autoSave = false, CancellationToken cancellationToken = default)
        {
            var basePath = parent?.FilePath.EnsureEndsWith(FileManagementConsts.DirectorySeparator);

            await base.DeleteAsync(
                x => x.FilePath.StartsWith(basePath) && x.FileContainerName == fileContainerName &&
                     x.OwnerUserId == ownerUserId, autoSave, cancellationToken);
        }

        public virtual async Task RefreshSubFilesAsync(string subFileBasePath, File parent, string fileContainerName,
            Guid? ownerUserId, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            var basePath = subFileBasePath.EnsureEndsWith(FileManagementConsts.DirectorySeparator);

            var subFiles = await DbSet
                .Where(x => x.FilePath.StartsWith(basePath) && x.FileContainerName == fileContainerName &&
                            x.OwnerUserId == ownerUserId).ToListAsync(cancellationToken);

            foreach (var subFile in subFiles)
            {
                subFile.RefreshFilePath(parent);

                await UpdateAsync(subFile, autoSave, cancellationToken);
            }
        }
    }
}