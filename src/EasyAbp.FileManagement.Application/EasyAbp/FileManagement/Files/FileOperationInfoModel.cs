using System;
using JetBrains.Annotations;

namespace EasyAbp.FileManagement.Files
{
    public class FileOperationInfoModel
    {
        public Guid? ParentId { get; set; }

        [NotNull]
        public string FileContainerName { get; set; }

        [CanBeNull]
        public string FileName { get; set; }

        [CanBeNull]
        public string MimeType { get; set; }

        public FileType? FileType { get; set; }

        public long? ByteSize { get; set; }

        public Guid? OwnerUserId { get; set; }

        [CanBeNull]
        public File File { get; set; }

        public FileOperationInfoModel(Guid? parentId, [NotNull] string fileContainerName, [CanBeNull] string fileName,
            [CanBeNull] string mimeType, FileType? fileType, long? byteSize, Guid? ownerUserId, [CanBeNull] File file)
        {
            ParentId = parentId;
            FileContainerName = fileContainerName;
            FileName = fileName;
            MimeType = mimeType;
            FileType = fileType;
            ByteSize = byteSize;
            OwnerUserId = ownerUserId;
            File = file;
        }
    }
}