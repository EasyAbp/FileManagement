using System;
using Volo.Abp.MultiTenancy;

namespace EasyAbp.FileManagement.Files;

public class SubFilesChangedEto : IMultiTenant
{
    public Guid? TenantId { get; set; }

    public Guid DirectoryId { get; set; }

    public bool UseBackgroundJob { get; set; }

    public SubFilesChangedEto()
    {
    }

    public SubFilesChangedEto(Guid? tenantId, Guid directoryId, bool useBackgroundJob)
    {
        TenantId = tenantId;
        DirectoryId = directoryId;
        UseBackgroundJob = useBackgroundJob;
    }
}