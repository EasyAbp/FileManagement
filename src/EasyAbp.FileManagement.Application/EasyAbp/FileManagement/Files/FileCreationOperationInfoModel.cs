using System;
using JetBrains.Annotations;

namespace EasyAbp.FileManagement.Files;

public class FileCreationOperationInfoModel : IFileOperationInfoModel
{
    [CanBeNull]
    public File Parent { get; set; }

    public string FileContainerName { get; set; }

    [CanBeNull]
    public string FileName { get; set; }

    [CanBeNull]
    public string MimeType { get; set; }

    public FileType? FileType { get; set; }

    public long? ByteSize { get; set; }

    public Guid? OwnerUserId { get; set; }

    public FileCreationOperationInfoModel([CanBeNull] File parent, [NotNull] string fileContainerName,
        [CanBeNull] string fileName, [CanBeNull] string mimeType, FileType? fileType, long? byteSize, Guid? ownerUserId)
    {
        Parent = parent;
        FileContainerName = fileContainerName;
        FileName = fileName;
        MimeType = mimeType;
        FileType = fileType;
        ByteSize = byteSize;
        OwnerUserId = ownerUserId;
    }
}