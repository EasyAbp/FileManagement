using System;
using Volo.Abp.MultiTenancy;

namespace EasyAbp.FileManagement.Files
{
    [Serializable]
    public class FileBlobNameChangedEto : IMultiTenant
    {
        public Guid? TenantId { get; set; }

        public Guid FileId { get; set; }
        
        public FileType FileType { get; set; }
        
        public string FileContainerName { get; set; }
        
        public string OldBlobName { get; set; }
        
        public string NewBlobName { get; set; }
    }
}