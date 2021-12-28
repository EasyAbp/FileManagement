using System;
using Volo.Abp.MultiTenancy;

namespace EasyAbp.FileManagement.Files;

[Serializable]
public class UnusedBlobCleaningArgs : IMultiTenant
{
    public Guid? TenantId { get; set; }
    
    public string FileContainerName { get; set; }
    
    public string BlobName { get; set; }
}