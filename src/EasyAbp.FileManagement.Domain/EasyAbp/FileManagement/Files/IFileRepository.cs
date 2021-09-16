using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace EasyAbp.FileManagement.Files
{
    public interface IFileRepository : IRepository<File, Guid>
    {
        Task<List<File>> GetListAsync(Guid? parentId, string fileContainerName, Guid? ownerUserId,
            FileType? specifiedFileType = null, CancellationToken cancellationToken = default);
        
        Task<File> FindAsync(string fileName, Guid? parentId, string fileContainerName, Guid? ownerUserId,
            CancellationToken cancellationToken = default);

        Task<File> FirstOrDefaultAsync(string fileContainerName, string hash, long byteSize,
            CancellationToken cancellationToken = default);

        Task<File> FirstOrDefaultAsync(string fileContainerName, string blobName,
            CancellationToken cancellationToken = default);

        Task<SubFilesStatisticDataModel> GetSubFilesStatisticDataAsync(Guid id,
            CancellationToken cancellationToken = default);

        Task<string> GetFileNameWithNextSerialNumberAsync(string fileName, Guid? parentId, string fileContainerName,
            Guid? ownerUserId, CancellationToken cancellationToken = default);

        Task<string> GetDataBaseTypeName();
    }
}