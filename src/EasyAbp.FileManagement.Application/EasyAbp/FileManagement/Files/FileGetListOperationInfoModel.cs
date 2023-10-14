using System;

namespace EasyAbp.FileManagement.Files;

public class FileGetListOperationInfoModel : IFileOperationInfoModel
{
    public Guid? ParentId { get; set; }

    public string FileContainerName { get; set; }

    public Guid? OwnerUserId { get; set; }

    public FileGetListOperationInfoModel(Guid? parentId, string fileContainerName, Guid? ownerUserId)
    {
        ParentId = parentId;
        FileContainerName = fileContainerName;
        OwnerUserId = ownerUserId;
    }
}