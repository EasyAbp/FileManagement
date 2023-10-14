using System;
using JetBrains.Annotations;

namespace EasyAbp.FileManagement.Files;

public class FileGetDownloadInfoOperationInfoModel : IFileOperationInfoModel
{
    public File File { get; set; }

    public string FileContainerName => File.FileContainerName;

    public Guid? OwnerUserId => File.OwnerUserId;

    public FileGetDownloadInfoOperationInfoModel(File file)
    {
        File = file;
    }
}