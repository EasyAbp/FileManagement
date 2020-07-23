using System;
using System.Threading;
using System.Threading.Tasks;
using EasyAbp.Abp.Trees;
using Volo.Abp.Domain.Repositories;

namespace EasyAbp.FileManagement.Files
{
    public interface IFileRepository : ITreeRepository<File>
    {
        Task<File> FindByFilePathAsync(string filePath, bool includeDetails = true, CancellationToken cancellationToken = default);
    }
}