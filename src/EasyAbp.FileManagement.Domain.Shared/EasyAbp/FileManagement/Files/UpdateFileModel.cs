using System;
using JetBrains.Annotations;
using Volo.Abp.ObjectExtending;

namespace EasyAbp.FileManagement.Files;

public class UpdateFileModel : ExtensibleObject
{
    [NotNull]
    public string NewFileName { get; set; }

    [CanBeNull]
    public string NewMimeType { get; set; }

    public Guid? OldParentId { get; set; }

    public Guid? NewParentId { get; set; }

    public byte[] NewFileContent { get; set; }

    public UpdateFileModel()
    {
    }

    public UpdateFileModel([NotNull] string newFileName, [CanBeNull] string newMimeType, Guid? oldParentId,
        Guid? newParentId, byte[] newFileContent)
    {
        NewFileName = newFileName;
        NewMimeType = newMimeType;
        OldParentId = oldParentId;
        NewParentId = newParentId;
        NewFileContent = newFileContent;
    }
}