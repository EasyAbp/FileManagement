using System;
using JetBrains.Annotations;

namespace EasyAbp.FileManagement.Files;

public class FileUpdateInfoOperationInfoModel : IFileOperationInfoModel
{
    public File File { get; set; }

    [CanBeNull]
    public string NewFileName { get; set; }

    [CanBeNull]
    public string NewMimeType { get; set; }

    public string FileContainerName => File.FileContainerName;

    public Guid? OwnerUserId => File.OwnerUserId;

    public FileUpdateInfoOperationInfoModel(File file, [CanBeNull] string newFileName, [CanBeNull] string newMimeType)
    {
        File = file;
        NewFileName = newFileName;
        NewMimeType = newMimeType;
    }
}