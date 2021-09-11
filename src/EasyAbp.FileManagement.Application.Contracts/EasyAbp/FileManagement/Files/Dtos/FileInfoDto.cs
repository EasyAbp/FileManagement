using System;
using Volo.Abp.Application.Dtos;

namespace EasyAbp.FileManagement.Files.Dtos
{
    [Serializable]
    public class FileInfoDto : ExtensibleFullAuditedEntityDto<Guid>
    {
        public Guid? ParentId { get; set; }

        public string FileContainerName { get; set; }

        public string FileName { get; set; }

        public string MimeType { get; set; }

        public FileType FileType { get; set; }

        public int SubFilesQuantity { get; set; }

        public long ByteSize { get; set; }

        public string Hash { get; set; }

        public Guid? OwnerUserId { get; set; }
    }
}