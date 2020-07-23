using System;
using System.Collections.Generic;
using EasyAbp.Abp.Trees;
using JetBrains.Annotations;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace EasyAbp.FileManagement.FileInfos
{
    public class FileInfo : FullAuditedAggregateRoot<Guid>, ITree<FileInfo>, IMultiTenant
    {
        public virtual Guid? TenantId { get; protected set; }
        
        [NotNull]
        public virtual string FileName { get; protected set; }
        
        [NotNull]
        public virtual string FilePath { get; protected set; }
        
        [CanBeNull]
        public virtual string MimeType { get; protected set; }
        
        public virtual FileType FileType { get; protected set; }
        
        public virtual int SubFilesQuantity { get; protected set; }
        
        public virtual long ByteSize { get; protected set; }
        
        [CanBeNull]
        public virtual string Hash { get; protected set; }

        #region Properties in ITree

        public virtual string Code { get; set; }
        
        public virtual int Level { get; set; }
        
        public virtual Guid? ParentId { get; set; }
        
        public virtual FileInfo Parent { get; set; }
        
        public virtual ICollection<FileInfo> Children { get; set; }
        
        public virtual string DisplayName { get; set; }

        #endregion
    }
}