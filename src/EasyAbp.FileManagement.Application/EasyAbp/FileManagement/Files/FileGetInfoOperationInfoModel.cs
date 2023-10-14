using System;

namespace EasyAbp.FileManagement.Files;

public class FileGetInfoOperationInfoModel : IFileOperationInfoModel
{
    public File File { get; set; }

    public string FileContainerName => File.FileContainerName;

    public Guid? OwnerUserId => File.OwnerUserId;

    public FileGetInfoOperationInfoModel(File file)
    {
        File = file;
    }
}