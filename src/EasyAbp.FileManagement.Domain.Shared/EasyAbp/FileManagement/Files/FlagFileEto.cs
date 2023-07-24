using System;
using JetBrains.Annotations;
using Volo.Abp.MultiTenancy;

namespace EasyAbp.FileManagement.Files
{
    [Serializable]
    public class FlagFileEto : IMultiTenant
    {
        public Guid? TenantId { get; set; }

        public Guid FileId { get; set; }

        [CanBeNull]
        public string Flag { get; set; }
    }
}