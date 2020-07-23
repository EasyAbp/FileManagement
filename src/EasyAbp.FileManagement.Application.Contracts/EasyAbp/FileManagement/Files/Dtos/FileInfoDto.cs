using System;
using Volo.Abp.Application.Dtos;

namespace EasyAbp.FileManagement.Files.Dtos
{
    [Serializable]
    public class FileInfoDto : FullAuditedEntityDto<Guid>
    {
        public string FileName { get; set; }

        public string FilePath { get; set; }

        public string MimeType { get; set; }

        public FileType FileType { get; set; }

        public int SubFilesQuantity { get; set; }

        public long ByteSize { get; set; }

        public string Hash { get; set; }

        public string Code { get; set; }

        public int Level { get; set; }

        public Guid? ParentId { get; set; }

        public string DisplayName { get; set; }
    }
}