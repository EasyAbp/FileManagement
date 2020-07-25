using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace EasyAbp.FileManagement.Files
{
    public interface IFileRepository : IRepository<File, Guid>
    {
        Task<File> FindByFilePathAsync(string filePath, string fileContainerName, Guid? ownerUserId,
            bool includeDetails = true, CancellationToken cancellationToken = default);
    }
}