using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp.Domain.Services;

namespace EasyAbp.FileManagement.Files
{
    public interface IFileManager : IDomainService
    {
        Task<File> CreateAsync(CreateFileModel model, CancellationToken cancellationToken = default);

        Task<File> CreateAsync(CreateFileWithStreamModel model, CancellationToken cancellationToken = default);

        Task<List<File>> CreateManyAsync(List<CreateFileModel> models, CancellationToken cancellationToken = default);

        Task<List<File>> CreateManyAsync(List<CreateFileWithStreamModel> models,
            CancellationToken cancellationToken = default);

        Task<File> UpdateAsync(File file, [NotNull] string newFileName, [CanBeNull] File oldParent,
            [CanBeNull] File newParent, CancellationToken cancellationToken = default);

        Task<File> UpdateAsync(File file, UpdateFileModel model, CancellationToken cancellationToken = default);

        Task<File> UpdateAsync(File file, UpdateFileWithStreamModel model,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(File file, CancellationToken cancellationToken = default);

        Task<FileDownloadInfoModel> GetDownloadInfoAsync(File file);
    }
}