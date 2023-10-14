using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Containers;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;

namespace EasyAbp.FileManagement.Files;

public abstract class FileManagerBase : DomainService, IFileManager
{
    protected IFileRepository FileRepository => LazyServiceProvider.LazyGetRequiredService<IFileRepository>();

    public abstract Task<File> CreateAsync(CreateFileModel model, CancellationToken cancellationToken = default);

    public abstract Task<File> CreateAsync(CreateFileWithStreamModel model,
        CancellationToken cancellationToken = default);

    public abstract Task<List<File>> CreateManyAsync(List<CreateFileModel> models,
        CancellationToken cancellationToken = default);

    public abstract Task<List<File>> CreateManyAsync(List<CreateFileWithStreamModel> models,
        CancellationToken cancellationToken = default);

    public abstract Task<File> UpdateAsync(File file, string newFileName, File oldParent, File newParent,
        CancellationToken cancellationToken = default);

    public abstract Task<File> UpdateAsync(File file, UpdateFileModel model,
        CancellationToken cancellationToken = default);

    public abstract Task<File> UpdateAsync(File file, UpdateFileWithStreamModel model,
        CancellationToken cancellationToken = default);

    public abstract Task DeleteAsync(File file, CancellationToken cancellationToken = default);

    protected abstract IFileDownloadProvider GetFileDownloadProvider(File file);

    public virtual async Task<FileDownloadInfoModel> GetDownloadInfoAsync(File file)
    {
        if (file.FileType != FileType.RegularFile)
        {
            throw new UnexpectedFileTypeException(file.Id, file.FileType, FileType.RegularFile);
        }

        var provider = GetFileDownloadProvider(file);

        return await provider.CreateDownloadInfoAsync(file);
    }

    protected virtual async Task<File> TryGetFileByNullableIdAsync(Guid? fileId)
    {
        return fileId.HasValue ? await FileRepository.GetAsync(fileId.Value) : null;
    }

    protected virtual void CheckDirectoryHasNoFileContent(FileType fileType, byte[] fileContent)
    {
        if (fileType == FileType.Directory && !fileContent.IsNullOrEmpty())
        {
            throw new DirectoryFileContentIsNotEmptyException();
        }
    }

    protected virtual async Task CheckNotMovingDirectoryToSubDirectoryAsync([NotNull] File file,
        [CanBeNull] File targetParent)
    {
        if (file.FileType != FileType.Directory)
        {
            return;
        }

        var parent = targetParent;

        while (parent != null)
        {
            if (parent.Id == file.Id)
            {
                throw new FileIsMovingToSubDirectoryException();
            }

            parent = parent.ParentId.HasValue ? await FileRepository.GetAsync(parent.ParentId.Value) : null;
        }
    }

    protected virtual void CheckFileName(string fileName, IFileContainerConfiguration configuration)
    {
        Check.NotNullOrWhiteSpace(fileName, nameof(File.FileName));

        if (fileName.Contains(FileManagementConsts.DirectorySeparator))
        {
            throw new FileNameContainsSeparatorException(fileName, FileManagementConsts.DirectorySeparator);
        }
    }

    [UnitOfWork]
    protected virtual async Task<bool> IsFileExistAsync(string fileName, Guid? parentId, string fileContainerName,
        Guid? ownerUserId)
    {
        return await FileRepository.FindAsync(fileName, parentId, fileContainerName, ownerUserId) != null;
    }

    protected virtual async Task CheckFileNotExistAsync(string fileName, Guid? parentId, string fileContainerName,
        Guid? ownerUserId)
    {
        if (await IsFileExistAsync(fileName, parentId, fileContainerName, ownerUserId))
        {
            throw new FileAlreadyExistsException(fileName, parentId);
        }
    }

    protected virtual void CheckFileQuantity(int count, IFileContainerConfiguration configuration)
    {
        if (count > configuration.MaxFileQuantityForEachUpload)
        {
            throw new UploadQuantityExceededLimitException(count, configuration.MaxFileQuantityForEachUpload);
        }
    }

    protected virtual void CheckFileSize(Dictionary<string, long> fileNameByteSizeMapping,
        IFileContainerConfiguration configuration)
    {
        foreach (var pair in fileNameByteSizeMapping.Where(
                     pair => pair.Value > configuration.MaxByteSizeForEachFile))
        {
            throw new FileSizeExceededLimitException(pair.Key, pair.Value, configuration.MaxByteSizeForEachFile);
        }

        var totalByteSize = fileNameByteSizeMapping.Values.Sum();

        if (totalByteSize > configuration.MaxByteSizeForEachUpload)
        {
            throw new UploadSizeExceededLimitException(totalByteSize, configuration.MaxByteSizeForEachUpload);
        }
    }

    protected virtual void CheckFileExtension(IEnumerable<string> fileNames, IFileContainerConfiguration configuration)
    {
        foreach (var fileName in fileNames.Where(fileName => !IsFileExtensionAllowed(fileName, configuration)))
        {
            throw new FileExtensionIsNotAllowedException(fileName);
        }
    }

    protected virtual bool IsFileExtensionAllowed(string fileName, IFileContainerConfiguration configuration)
    {
        var lowerFileName = fileName.ToLowerInvariant();

        foreach (var pair in configuration.FileExtensionsConfiguration.Where(x =>
                     lowerFileName.EndsWith(x.Key.ToLowerInvariant())))
        {
            return pair.Value;
        }

        return !configuration.AllowOnlyConfiguredFileExtensions;
    }
}