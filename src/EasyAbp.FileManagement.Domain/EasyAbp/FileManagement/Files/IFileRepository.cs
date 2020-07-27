using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp.Domain.Repositories;

namespace EasyAbp.FileManagement.Files
{
    public interface IFileRepository : IRepository<File, Guid>
    {
        Task<File> FindAsync(string fileName, Guid? parentId, string fileContainerName, Guid? ownerUserId,
            CancellationToken cancellationToken = default);
        
        Task<File> FindByFilePathAsync(string filePath, string fileContainerName, Guid? ownerUserId,
            FileType? specifiedFileType = null, CancellationToken cancellationToken = default);
        
        Task<File> FirstOrDefaultAsync(string hash, long byteSize, CancellationToken cancellationToken = default);
        
        Task<File> FirstOrDefaultAsync(string blobName, CancellationToken cancellationToken = default);

        Task<SubFilesStatisticDataModel> GetSubFilesStatisticDataAsync(Guid id,
            CancellationToken cancellationToken = default);

        Task DeleteSubFilesAsync([CanBeNull] File parent, string fileContainerName, Guid? ownerUserId,
            bool autoSave = false, CancellationToken cancellationToken = default);

        Task RefreshSubFilesAsync(string subFileBasePath, [CanBeNull] File parent, string fileContainerName, Guid? ownerUserId,
            bool autoSave = false, CancellationToken cancellationToken = default);
    }
}