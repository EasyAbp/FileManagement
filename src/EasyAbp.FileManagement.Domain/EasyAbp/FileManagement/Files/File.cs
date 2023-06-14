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

        public virtual Guid? ParentId { get; protected set; }

        [NotNull]
        public virtual string FileContainerName { get; protected set; }

        [NotNull]
        public virtual string FileName { get; protected set; }

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

        [CanBeNull]
        public virtual string Flag { get; protected set; }

        protected File()
        {
        }

        internal File(
            Guid id,
            Guid? tenantId,
            [CanBeNull] File parent,
            [NotNull] string fileContainerName,
            [NotNull] string fileName,
            [CanBeNull] string mimeType,
            FileType fileType,
            int subFilesQuantity,
            long byteSize,
            [CanBeNull] string hash,
            [CanBeNull] string blobName,
            Guid? ownerUserId,
            [CanBeNull] string flag = null) : base(id)
        {
            if (parent != null && parent.FileContainerName != fileContainerName)
            {
                throw new UnexpectedFileContainerNameException(parent.FileContainerName, fileContainerName);
            }

            TenantId = tenantId;
            ParentId = parent?.Id;
            FileContainerName = fileContainerName;
            FileName = fileName;
            MimeType = mimeType;
            FileType = fileType;
            SubFilesQuantity = subFilesQuantity;
            ByteSize = byteSize;
            Hash = hash;
            BlobName = blobName;
            OwnerUserId = ownerUserId;
            Flag = flag;
        }

        internal void UpdateInfo(
            [NotNull] string fileName,
            [CanBeNull] string mimeType,
            int subFilesQuantity,
            long byteSize,
            [CanBeNull] string hash,
            [CanBeNull] string blobName,
            [CanBeNull] File oldParent,
            [CanBeNull] File newParent)
        {
            if (newParent != null && newParent.FileContainerName != FileContainerName)
            {
                throw new UnexpectedFileContainerNameException(newParent.FileContainerName, FileContainerName);
            }

            ParentId = newParent?.Id;
            FileName = fileName;
            MimeType = mimeType;
            SubFilesQuantity = subFilesQuantity;
            ByteSize = byteSize;
            Hash = hash;
            BlobName = blobName;
        }

        public bool TryUpdateStatisticData(SubFilesStatisticDataModel statisticData)
        {
            if (statisticData.SubFilesQuantity == SubFilesQuantity && statisticData.ByteSize == ByteSize)
            {
                return false;
            }

            SubFilesQuantity = statisticData.SubFilesQuantity;
            ByteSize = statisticData.ByteSize;

            return true;
        }

        public void SetFlag([CanBeNull] string flag)
        {
            Flag = flag;
        }
    }
}