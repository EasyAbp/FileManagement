using System;

namespace EasyAbp.FileManagement.Files
{
    public class FileOperationInfoModel
    {
        public Guid? ParentId { get; set; }
        
        public string FileContainerName { get; set; }
        
        public Guid? OwnerUserId { get; set; }
        
        public File File { get; set; }
    }
}