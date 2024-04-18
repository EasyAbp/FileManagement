using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyAbp.FileManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace EasyAbp.FileManagement.Files
{
    public class FileRepository : EfCoreRepository<IFileManagementDbContext, File, Guid>, IFileRepository
    {
        public FileRepository(IDbContextProvider<IFileManagementDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public virtual async Task<List<File>> GetListAsync(Guid? parentId, string fileContainerName, Guid? ownerUserId,
            FileType? specifiedFileType = null, CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync())
                .Where(x => x.ParentId == parentId && x.OwnerUserId == ownerUserId &&
                            x.FileContainerName == fileContainerName).WhereIf(specifiedFileType.HasValue,
                    x => x.FileType == specifiedFileType.Value).ToListAsync(cancellationToken);
        }

        public virtual async Task<File> FindAsync(string fileName, Guid? parentId, string fileContainerName,
            Guid? ownerUserId, bool forceCaseSensitive, CancellationToken cancellationToken = default)
        {
            var queryable = await GetQueryableAsync();

            if (forceCaseSensitive)
            {
                queryable = queryable.AsNoTracking().Where(x =>
                    x.ParentId == parentId && x.OwnerUserId == ownerUserId && x.FileContainerName == fileContainerName);

                var files = await queryable.ToListAsync(cancellationToken);

                var foundFile = files.FirstOrDefault(x => x.FileName == fileName);
                return foundFile is null ? null : await GetAsync(foundFile.Id, cancellationToken: cancellationToken);
            }

            queryable = queryable.Where(x =>
                x.ParentId == parentId && x.OwnerUserId == ownerUserId &&
                x.FileContainerName == fileContainerName && x.FileName == fileName);

            return await queryable.FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<bool> ExistAsync(string fileName, Guid? parentId, string fileContainerName,
            Guid? ownerUserId, bool forceCaseSensitive, CancellationToken cancellationToken = default)
        {
            var queryable = await GetQueryableAsync();

            if (forceCaseSensitive)
            {
                queryable = queryable.AsNoTracking().Where(x =>
                    x.ParentId == parentId && x.OwnerUserId == ownerUserId && x.FileContainerName == fileContainerName);

                var files = await queryable.ToListAsync(cancellationToken);

                return files.Any(x => x.FileName == fileName);
            }

            queryable = queryable.Where(x =>
                x.ParentId == parentId && x.OwnerUserId == ownerUserId &&
                x.FileContainerName == fileContainerName && x.FileName == fileName);

            return await queryable.AnyAsync(cancellationToken);
        }

        public virtual async Task<File> FirstOrDefaultAsync(string fileContainerName, string hash, long byteSize,
            CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync())
                .Where(x => x.Hash == hash && x.ByteSize == byteSize && x.FileContainerName == fileContainerName)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<File> FirstOrDefaultAsync(string fileContainerName, string blobName,
            CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync())
                .Where(x => x.BlobName == blobName && x.FileContainerName == fileContainerName)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<SubFilesStatisticDataModel> GetSubFilesStatisticDataAsync(Guid id,
            CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync()).Where(x => x.ParentId == id).GroupBy(x => true).Select(x =>
                new SubFilesStatisticDataModel
                {
                    SubFilesQuantity = x.Count(),
                    ByteSize = x.Sum(y => y.ByteSize)
                }).FirstOrDefaultAsync(cancellationToken) ?? new SubFilesStatisticDataModel();
        }

        public virtual async Task<string> GetFileNameWithNextSerialNumberAsync(string fileName, Guid? parentId,
            string fileContainerName, Guid? ownerUserId, CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(fileName, nameof(fileName));

            var ext = Path.GetExtension(fileName);

            var fileNameWithoutExt = fileName.Substring(0, fileName.LastIndexOf(ext, StringComparison.Ordinal));

            var part1 = fileNameWithoutExt + '(';

            var part2 = ')' + ext;

            var fileNames = await (await GetQueryableAsync())
                .Where(x => x.ParentId == parentId && x.OwnerUserId == ownerUserId &&
                            x.FileContainerName == fileContainerName && x.FileName.StartsWith(part1) &&
                            x.FileName.EndsWith(part2)).Select(x => x.FileName).ToListAsync(cancellationToken);

            var nextNumber =
                fileNames
                    .Select(x =>
                        x.Substring(part1.Length, x.LastIndexOf(part2, StringComparison.Ordinal) - part1.Length))
                    .Select(x => int.TryParse(x, out var number) ? number : 0).Where(x => x > 0).OrderBy(x => x)
                    .TakeWhile((x, i) => x == i + 1).LastOrDefault() + 1;

            return $"{part1}{nextNumber}{part2}";
        }
    }
}