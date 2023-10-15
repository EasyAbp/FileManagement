using System;
using Volo.Abp.MultiTenancy;

namespace EasyAbp.FileManagement.Files;

[Serializable]
public class SubFilesUpdateHandlingJobArgs : IMultiTenant
{
    public Guid? TenantId { get; set; }

    public SubFilesChangedEto SubFilesChangedEto { get; set; }

    public SubFilesUpdateHandlingJobArgs()
    {
    }

    public SubFilesUpdateHandlingJobArgs(Guid? tenantId, Guid directoryId)
    {
        TenantId = tenantId;
        SubFilesChangedEto = new SubFilesChangedEto(tenantId, directoryId, false);
    }
}