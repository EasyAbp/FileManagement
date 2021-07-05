using System;
using System.ComponentModel.DataAnnotations;

namespace EasyAbp.FileManagement.Files
{
    [Serializable]
    public class CreateFileEto
    {
        [Required]
        public string FileContainerName { get; set; }

        [Required]
        public string FileName { get; set; }

        public string MimeType { get; set; }

        public FileType FileType { get; set; }

        public Guid? ParentId { get; set; }

        public Guid? OwnerUserId { get; set; }
    }
}