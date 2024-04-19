using System;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace EasyAbp.FileManagement.Files
{
    public class File : FullAuditedAggregateRoot<Guid>, IFile, IMultiTenant
    {
        public virtual Guid? TenantId { get; protected set; }

        public virtual Guid? ParentId { get; protected set; }

        public virtual string FileContainerName { get; protected set; }

        public virtual string FileName { get; protected set; }

        public virtual string MimeType { get; protected set; }

        public virtual FileType FileType { get; protected set; }

        public virtual int SubFilesQuantity { get; protected set; }

        public virtual bool HasSubdirectories { get; protected set; }

        public virtual long ByteSize { get; protected set; }

        public virtual string Hash { get; protected set; }

        public virtual string BlobName { get; protected set; }

        public virtual Guid? OwnerUserId { get; protected set; }

        public virtual string Flag { get; protected set; }

        /// <summary>
        /// It is used to accommodate both soft deletion and unique indexing.
        /// Initially, it is an empty string, and it is set to a random value during soft deletion.
        /// </summary>
        public virtual string SoftDeletionToken { get; set; }

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

            Check.NotNullOrWhiteSpace(fileContainerName, nameof(fileContainerName));
            Check.NotNullOrWhiteSpace(fileName, nameof(fileName));
            Check.Length(fileContainerName, nameof(fileContainerName), FileManagementConsts.FileContainerNameMaxLength);
            Check.Length(fileName, nameof(fileName), FileManagementConsts.File.FileNameMaxLength);
            Check.Length(mimeType, nameof(mimeType), FileManagementConsts.File.MimeTypeMaxLength);
            Check.Length(hash, nameof(hash), FileManagementConsts.File.HashMaxLength);
            Check.Length(blobName, nameof(blobName), FileManagementConsts.File.BlobNameMaxLength);

            TenantId = tenantId;
            ParentId = parent?.Id;
            FileContainerName = fileContainerName;
            FileName = fileName;
            MimeType = mimeType;
            FileType = fileType;
            SubFilesQuantity = 0;
            HasSubdirectories = false;
            ByteSize = byteSize;
            Hash = hash;
            BlobName = blobName;
            OwnerUserId = ownerUserId;
            Flag = flag;
            SoftDeletionToken = string.Empty;
        }

        internal void UpdateInfo(
            [NotNull] string fileName,
            [CanBeNull] string mimeType,
            int subFilesQuantity,
            bool hasSubdirectories,
            long byteSize,
            [CanBeNull] string hash,
            [CanBeNull] string blobName,
            [CanBeNull] File parent)
        {
            if (parent != null && parent.FileContainerName != FileContainerName)
            {
                throw new UnexpectedFileContainerNameException(parent.FileContainerName, FileContainerName);
            }

            Check.NotNullOrWhiteSpace(fileName, nameof(fileName));
            Check.Length(fileName, nameof(fileName), FileManagementConsts.File.FileNameMaxLength);
            Check.Length(mimeType, nameof(mimeType), FileManagementConsts.File.MimeTypeMaxLength);
            Check.Length(hash, nameof(hash), FileManagementConsts.File.HashMaxLength);
            Check.Length(blobName, nameof(blobName), FileManagementConsts.File.BlobNameMaxLength);

            ParentId = parent?.Id;
            FileName = fileName;
            MimeType = mimeType;
            SubFilesQuantity = subFilesQuantity;
            HasSubdirectories = hasSubdirectories;
            ByteSize = byteSize;
            Hash = hash;
            BlobName = blobName;
        }

        public bool TryUpdateStatisticData(SubFilesStatisticDataModel statisticData)
        {
            if (statisticData.SubFilesQuantity == SubFilesQuantity &&
                statisticData.HasSubdirectories == HasSubdirectories &&
                statisticData.ByteSize == ByteSize)
            {
                return false;
            }

            SubFilesQuantity = statisticData.SubFilesQuantity;
            HasSubdirectories = statisticData.HasSubdirectories;
            ByteSize = statisticData.ByteSize;

            return true;
        }

        public void SetFlag([CanBeNull] string flag)
        {
            Flag = flag;
        }

        public void TriggerAuditingChanges()
        {
            // after ABP v8, LastModificationTime doesn't change if no entity properties have changed.
            LastModificationTime = DateTime.MinValue;
        }
    }
}