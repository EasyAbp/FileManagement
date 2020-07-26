using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace EasyAbp.FileManagement.Files
{
    public interface IFileRepository : IRepository<File, Guid>
    {
        Task<File> FindByFilePathAsync(string filePath, string fileContainerName, Guid? ownerUserId,
            CancellationToken cancellationToken = default);
        
        Task<File> FirstOrDefaultAsync(string hash, long byteSize, CancellationToken cancellationToken = default);
        
        Task<File> FirstOrDefaultAsync(string blobName, CancellationToken cancellationToken = default);

        Task<SubFilesStatisticDataModel> GetSubFilesStatisticDataAsync(Guid id, CancellationToken cancellationToken = default);
    }
}