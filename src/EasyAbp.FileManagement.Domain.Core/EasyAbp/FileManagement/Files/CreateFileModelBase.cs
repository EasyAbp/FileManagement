using System;
using JetBrains.Annotations;
using Volo.Abp.ObjectExtending;

namespace EasyAbp.FileManagement.Files;

public abstract class CreateFileModelBase : ExtensibleObject
{
    [NotNull]
    public string FileContainerName { get; set; }

    public Guid? OwnerUserId { get; set; }

    [NotNull]
    public string FileName { get; set; }

    [CanBeNull]
    public string MimeType { get; set; }

    public FileType FileType { get; set; }

    [CanBeNull]
    public File Parent { get; set; }

    public CreateFileModelBase()
    {
    }

    public CreateFileModelBase([NotNull] string fileContainerName, Guid? ownerUserId, [NotNull] string fileName,
        [CanBeNull] string mimeType, FileType fileType, [CanBeNull] File parent)
    {
        FileContainerName = fileContainerName;
        OwnerUserId = ownerUserId;
        FileName = fileName;
        MimeType = mimeType;
        FileType = fileType;
        Parent = parent;
    }

    public abstract long GetContentLength();
}