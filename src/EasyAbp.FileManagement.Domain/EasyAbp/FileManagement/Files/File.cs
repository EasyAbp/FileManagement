using System;
using System.Collections.Generic;
using EasyAbp.Abp.Trees;
using JetBrains.Annotations;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace EasyAbp.FileManagement.Files
{
    public class File : FullAuditedAggregateRoot<Guid>, ITree<File>, IMultiTenant
    {
        public virtual Guid? TenantId { get; protected set; }

        [NotNull]
        public virtual string FileContainerName { get; protected set; }

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
        
        [CanBeNull]
        public virtual string BlobName { get; protected set; }

        #region Properties in ITree

        [NotNull]
        public virtual string Code { get; set; }

        public virtual int Level { get; set; }

        public virtual Guid? ParentId { get; set; }

        public virtual File Parent { get; set; }

        public virtual ICollection<File> Children { get; set; }

        [NotNull]
        public virtual string DisplayName { get; set; }

        #endregion
        
        protected File()
        {
        }

        public File(
            Guid id,
            Guid? tenantId,
            [NotNull] string fileContainerName,
            [NotNull] string fileName,
            [NotNull] string filePath,
            [CanBeNull] string mimeType,
            FileType fileType,
            int subFilesQuantity,
            long byteSize,
            [CanBeNull] string hash,
            [CanBeNull] string blobName,
            Guid? parentId,
            [NotNull] string displayName) : base(id)
        {
            TenantId = tenantId;
            FileContainerName = fileContainerName;
            FileName = fileName;
            FilePath = filePath;
            MimeType = mimeType;
            FileType = fileType;
            SubFilesQuantity = subFilesQuantity;
            ByteSize = byteSize;
            Hash = hash;
            BlobName = blobName;
            ParentId = parentId;
            DisplayName = displayName;
        }

        public void UpdateInfo(
            [NotNull] string fileName,
            [NotNull] string filePath,
            [CanBeNull] string mimeType,
            int subFilesQuantity,
            long byteSize,
            [CanBeNull] string hash,
            [CanBeNull] string blobName,
            Guid? parentId,
            [NotNull] string displayName)
        {
            FileName = fileName;
            FilePath = filePath;
            MimeType = mimeType;
            SubFilesQuantity = subFilesQuantity;
            ByteSize = byteSize;
            Hash = hash;
            BlobName = blobName;
            ParentId = parentId;
            DisplayName = displayName;
        }
    }
}
