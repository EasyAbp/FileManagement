using System;
using JetBrains.Annotations;
using Volo.Abp.ObjectExtending;

namespace EasyAbp.FileManagement.Files;

public class CreateFileModel : CreateFileModelBase
{
    public byte[] FileContent { get; set; }

    public CreateFileModel()
    {
    }

    public CreateFileModel([NotNull] string fileContainerName, Guid? ownerUserId, [NotNull] string fileName,
        [CanBeNull] string mimeType, FileType fileType, [CanBeNull] File parent, byte[] fileContent) : base(
        fileContainerName, ownerUserId, fileName, mimeType, fileType, parent)
    {
        FileContent = fileContent;
    }

    public override long GetContentLength()
    {
        return FileContent.LongLength;
    }
}