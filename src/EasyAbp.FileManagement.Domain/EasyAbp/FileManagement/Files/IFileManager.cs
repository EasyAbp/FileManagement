using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Services;

namespace EasyAbp.FileManagement.Files
{
    public interface IFileManager : IDomainService
    {
        Task<File> CreateAsync([NotNull] string fileContainerName, Guid? ownerUserId, [NotNull] string fileName,
            [CanBeNull] string mimeType, FileType fileType, Guid? parentId, byte[] fileContent);
        
        Task<File> UpdateAsync(File file, [NotNull] string newFileName, Guid? newParentId);

        Task<File> UpdateAsync(File file, [NotNull] string newFileName, Guid? newParentId,
            [CanBeNull] string newMimeType, byte[] newFileContent);

        Task<bool> TrySaveBlobAsync(File file, byte[] fileContent, bool overrideExisting = false,
            CancellationToken cancellationToken = default);
        
        Task<byte[]> GetBlobAsync(File file, CancellationToken cancellationToken = default);

        IBlobContainer GetBlobContainer(File file);
        
        Task DeleteBlobAsync(File file, CancellationToken cancellationToken = default);

        Task<FileDownloadInfoModel> GetDownloadInfoAsync(File file);

        Task<bool> ExistAsync([NotNull] string fileContainerName, Guid? ownerUserId, [NotNull] string filePath,
            FileType? specifiedFileType);
    }
}