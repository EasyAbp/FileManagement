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
            FilePath = GetFilePath(parent, fileName);
            MimeType = mimeType;
            FileType = fileType;
            SubFilesQuantity = subFilesQuantity;
            ByteSize = byteSize;
            Hash = hash;
            BlobName = blobName;
            OwnerUserId = ownerUserId;
        }

        public void TryAddSubFileUpdatedDomainEvent()
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
            [CanBeNull] string mimeType,
            int subFilesQuantity,
            long byteSize,
            [CanBeNull] string hash,
            [CanBeNull] string blobName,
            [CanBeNull] File oldParent,
            [CanBeNull] File newParent)
        {
            if (newParent != null &&
                newParent.FilePath.StartsWith(FilePath.EnsureEndsWith(FileManagementConsts.DirectorySeparator)))
            {
                throw new FileIsMovedToSubDirectoryException();
            }
            
            if (ParentId != newParent?.Id || BlobName != blobName)
            {
                oldParent?.TryAddSubFileUpdatedDomainEvent();
                newParent?.TryAddSubFileUpdatedDomainEvent();
            }

            ParentId = newParent?.Id;
            FileName = fileName;
            FilePath = GetFilePath(newParent, fileName);
            MimeType = mimeType;
            SubFilesQuantity = subFilesQuantity;
            ByteSize = byteSize;
            Hash = hash;
            BlobName = blobName;
        }

        public void UpdateLocation(
            [NotNull] string newFileName,
            [CanBeNull] File oldParent,
            [CanBeNull] File newParent)
        {
            UpdateInfo(newFileName, MimeType, SubFilesQuantity, ByteSize, Hash, BlobName, oldParent, newParent);
        }

        public void RefreshFilePath(File parent)
        {
            if (parent?.Id != ParentId)
            {
                throw new IncorrectParentException(parent);
            }
            
            FilePath = GetFilePath(parent, FileName);
        }

        public void ForceSetStatisticData(SubFilesStatisticDataModel statisticData)
        {
            SubFilesQuantity = statisticData.SubFilesQuantity;
            ByteSize = statisticData.ByteSize;
        }

        private string GetFilePath(File parent, string fileName)
        {
            if (parent == null)
            {
                return fileName;
            }

            if (parent.FileType != FileType.Directory)
            {
                throw new UnexpectedFileTypeException(parent.Id, parent.FileType, FileType.Directory);
            }

            if (parent.FileContainerName != FileContainerName)
            {
                throw new UnexpectedFileContainerNameException(parent.FileContainerName, FileContainerName);
            }

            return parent.FilePath.EnsureEndsWith(FileManagementConsts.DirectorySeparator) + fileName;
        }
    }
}
