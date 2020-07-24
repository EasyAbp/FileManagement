using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp.Domain.Services;

namespace EasyAbp.FileManagement.Files
{
    public interface IFileManager : IDomainService
    {
        Task<File> CreateAsync([NotNull] string fileContainerName, Guid? ownerUserId, [NotNull] string fileName,
            [CanBeNull] string mimeType, FileType fileType, Guid? parentId, byte[] fileContent);
        
        Task<File> CreateAsync([NotNull] string fileContainerName, Guid? ownerUserId, [NotNull] string filePath,
            [CanBeNull] string mimeType, FileType fileType, byte[] fileContent);

        Task<File> UpdateAsync(File file, [NotNull] string newFileName, Guid? newParentId);

        Task<File> UpdateAsync(File file, [NotNull] string newFileName, Guid? newParentId,
            [CanBeNull] string newMimeType, byte[] newFileContent);

        Task SaveBlobAsync(File file, byte[] fileContent, bool overrideExisting = false);
        
        Task<byte[]> GetBlobAsync(File file);

        Task<FileDownloadInfoModel> GetDownloadInfoAsync(File file);

        Task<bool> ExistAsync([NotNull] string fileContainerName, Guid? ownerUserId, [NotNull] string filePath,
            FileType? fileType);
    }
}