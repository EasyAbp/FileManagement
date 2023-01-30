using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyAbp.FileManagement.Files.Dtos
{
    [Serializable]
    public class UpdateFileInput : UpdateFileBase
    {
        [Required]
        public string FileName { get; set; }

        public string MimeType { get; set; }

        public byte[] Content { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            foreach (var result in base.Validate(validationContext))
            {
                yield return result;
            }

            if (FileName.IsNullOrWhiteSpace())
            {
                yield return new ValidationResult("FileName should not be empty!",
                    new[] { nameof(FileName) });
            }

            FileName = FileName.Trim();
        }
    }
}