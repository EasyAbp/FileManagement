using System;
using JetBrains.Annotations;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace EasyAbp.FileManagement.Files
{
    public class File : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; protected set; }

        public virtual Guid? ParentId { get; set; }

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
        
        public virtual Guid? OwnerUserId { get; protected set; }

        protected File()
        {
        }

        public File(
            Guid id,
            Guid? tenantId,
            Guid? parentId,
            [NotNull] string fileContainerName,
            [NotNull] string fileName,
            [NotNull] string filePath,
            [CanBeNull] string mimeType,
            FileType fileType,
            int subFilesQuantity,
            long byteSize,
            [CanBeNull] string hash,
            [CanBeNull] string blobName,
            Guid? ownerUserId) : base(id)
        {
            TenantId = tenantId;
            ParentId = parentId;
            FileContainerName = fileContainerName;
            FileName = fileName;
            FilePath = filePath;
            MimeType = mimeType;
            FileType = fileType;
            SubFilesQuantity = subFilesQuantity;
            ByteSize = byteSize;
            Hash = hash;
            BlobName = blobName;
            OwnerUserId = ownerUserId;
        }

        public void UpdateInfo(
            Guid? parentId,
            [NotNull] string fileName,
            [NotNull] string filePath,
            [CanBeNull] string mimeType,
            int subFilesQuantity,
            long byteSize,
            [CanBeNull] string hash,
            [CanBeNull] string blobName)
        {
            if (parentId != ParentId)
            {
                // Todo: publish a file moved event
            }
            
            ParentId = parentId;
            FileName = fileName;
            FilePath = filePath;
            MimeType = mimeType;
            SubFilesQuantity = subFilesQuantity;
            ByteSize = byteSize;
            Hash = hash;
            BlobName = blobName;
        }
    }
}
