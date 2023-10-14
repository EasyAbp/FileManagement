using System;
using Volo.Abp.MultiTenancy;

namespace EasyAbp.FileManagement.Files;

public class FileEto : IFile, IMultiTenant
{
    public Guid Id { get; set; }
    
    public Guid? TenantId { get; set; }

    public Guid? ParentId { get; set; }
    
    public string FileContainerName { get; set; }

    public string FileName { get; set; }

    public string MimeType { get; set; }

    public FileType FileType { get; set; }
    
    public int SubFilesQuantity { get; set; }

    public long ByteSize { get; set; }

    public string Hash { get; set; }

    public string BlobName { get; set; }
    
    public Guid? OwnerUserId { get; set; }
        
    public string Flag { get; set; }
}