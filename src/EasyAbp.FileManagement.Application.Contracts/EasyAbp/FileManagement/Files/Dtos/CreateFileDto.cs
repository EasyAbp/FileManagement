using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyAbp.FileManagement.Files.Dtos
{
    [Serializable]
    public class CreateFileDto : IValidatableObject
    {
        [Required]
        public string FileName { get; set; }
        
        public string MimeType { get; set; }

        public FileType FileType { get; set; }

        public Guid? ParentId { get; set; }
        
        public byte[] Content { get; set; }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FileName.IsNullOrWhiteSpace())
            {
                yield return new ValidationResult("FileName should not be empty!", new[] {nameof(FileName)});
            }
        }
    }
}