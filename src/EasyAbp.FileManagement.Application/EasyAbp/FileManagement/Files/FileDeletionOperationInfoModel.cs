using System;
using JetBrains.Annotations;

namespace EasyAbp.FileManagement.Files;

public class FileDeletionOperationInfoModel : IFileOperationInfoModel
{
    public File File { get; set; }

    public string FileContainerName => File.FileContainerName;

    public Guid? OwnerUserId => File.OwnerUserId;

    public FileDeletionOperationInfoModel(File file)
    {
        File = file;
    }
}