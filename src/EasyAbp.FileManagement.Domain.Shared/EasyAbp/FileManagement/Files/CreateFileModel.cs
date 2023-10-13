using System;
using JetBrains.Annotations;
using Volo.Abp.ObjectExtending;

namespace EasyAbp.FileManagement.Files;

public class CreateFileModel : ExtensibleObject
{
    [NotNull]
    public string FileContainerName { get; set; }

    public Guid? OwnerUserId { get; set; }

    [NotNull]
    public string FileName { get; set; }

    [CanBeNull]
    public string MimeType { get; set; }

    public FileType FileType { get; set; }

    public Guid? ParentId { get; set; }

    public byte[] FileContent { get; set; }

    public CreateFileModel()
    {
    }

    public CreateFileModel([NotNull] string fileContainerName, Guid? ownerUserId, [NotNull] string fileName,
        [CanBeNull] string mimeType, FileType fileType, Guid? parentId, byte[] fileContent)
    {
        FileContainerName = fileContainerName;
        OwnerUserId = ownerUserId;
        FileName = fileName;
        MimeType = mimeType;
        FileType = fileType;
        ParentId = parentId;
        FileContent = fileContent;
    }
}