﻿using System;
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

        Task<File> ChangeAsync(File file, [NotNull] string newFileName, [CanBeNull] File oldParent, [CanBeNull] File newParent);

        Task<File> ChangeAsync(File file, [NotNull] string newFileName, [CanBeNull] string newMimeType,
            byte[] newFileContent, [CanBeNull] File oldParent, [CanBeNull] File newParent);

        Task DeleteAsync(File file, CancellationToken cancellationToken = default);

        Task<bool> TrySaveBlobAsync(File file, byte[] fileContent, bool disableBlobReuse = false,
            bool allowBlobOverriding = false, CancellationToken cancellationToken = default);
        
        Task<byte[]> GetBlobAsync(File file, CancellationToken cancellationToken = default);

        IBlobContainer GetBlobContainer(File file);
        
        IBlobContainer GetBlobContainer([NotNull] string fileContainerName);

        Task DeleteBlobAsync([NotNull] string fileContainerName, [NotNull] string blobName,
            CancellationToken cancellationToken = default);

        Task<FileDownloadInfoModel> GetDownloadInfoAsync(File file);
    }
}