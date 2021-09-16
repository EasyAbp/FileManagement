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
            [CanBeNull] string mimeType, FileType fileType, [CanBeNull] File parent, byte[] fileContent);

        /// <summary>
        /// 文件已经插入到FileSystem的create方法
        /// </summary>
        /// <param name="fileContainerName"></param>
        /// <param name="ownerUserId"></param>
        /// <param name="blobName"></param>
        /// <param name="fileName"></param>
        /// <param name="fileLength"></param>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        Task<File> CreateAsync(string fileContainerName, Guid? ownerUserId, File parent,string blobName,
            string fileName, string mimeType);

        /// <summary>
        /// database backup directory
        /// </summary>
        /// <param name="fileContainerName"></param>
        /// <param name="ownerUserId"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        File CreateAsync(string fileContainerName, Guid? ownerUserId, string fileName);
        Task<File> ChangeAsync(File file, [NotNull] string newFileName, [CanBeNull] File oldParent, [CanBeNull] File newParent);

        Task<File> ChangeAsync(File file, [NotNull] string newFileName, [CanBeNull] string newMimeType,
            byte[] newFileContent, [CanBeNull] File oldParent, [CanBeNull] File newParent);

        Task DeleteAsync(File file, CancellationToken cancellationToken = default);

        Task<bool> TrySaveBlobAsync(File file, byte[] fileContent, bool overrideExisting = false,
            CancellationToken cancellationToken = default);
        
        Task<byte[]> GetBlobAsync(File file, CancellationToken cancellationToken = default);

        IBlobContainer GetBlobContainer(File file);
        
        Task DeleteBlobAsync(File file, CancellationToken cancellationToken = default);

        Task<FileDownloadInfoModel> GetDownloadInfoAsync(File file);
    }
}