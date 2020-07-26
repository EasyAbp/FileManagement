using System;

namespace EasyAbp.FileManagement.Files
{
    [Serializable]
    public class FileBlobNameChangedEto
    {
        public Guid FileId { get; set; }
        
        public string OldBlobName { get; set; }
        
        public string NewBlobName { get; set; }
    }
}