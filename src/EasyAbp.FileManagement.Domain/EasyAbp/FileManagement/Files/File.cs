using System;
using System.Linq;
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
            [CanBeNull] File parent,
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
            parent?.TryAddSubFileUpdatedDomainEvent();

            TenantId = tenantId;
            ParentId = parent?.Id;
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

        private void TryAddSubFileUpdatedDomainEvent()
        {
            if (GetLocalEvents().Any(x => x is SubFileUpdatedEto))
            {
                return;
            }
            
            AddLocalEvent(new SubFileUpdatedEto
            {
                Parent = this
            });
        }

        public void UpdateInfo(
            [NotNull] string fileName,
            [NotNull] string filePath,
            [CanBeNull] string mimeType,
            int subFilesQuantity,
            long byteSize,
            [CanBeNull] string hash,
            [CanBeNull] string blobName,
            [CanBeNull] File oldParent,
            [CanBeNull] File newParent)
        {
            if (ParentId != newParent?.Id || BlobName != blobName)
            {
                oldParent?.TryAddSubFileUpdatedDomainEvent();
                newParent?.TryAddSubFileUpdatedDomainEvent();
            }

            ParentId = newParent?.Id;
            FileName = fileName;
            FilePath = filePath;
            MimeType = mimeType;
            SubFilesQuantity = subFilesQuantity;
            ByteSize = byteSize;
            Hash = hash;
            BlobName = blobName;
        }

        public void ForceSetStatisticData(SubFilesStatisticDataModel statisticData)
        {
            SubFilesQuantity = statisticData.SubFilesQuantity;
            ByteSize = statisticData.ByteSize;
        }

    }
}
