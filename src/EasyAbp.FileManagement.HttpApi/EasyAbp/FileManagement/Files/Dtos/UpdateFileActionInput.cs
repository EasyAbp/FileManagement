using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Volo.Abp.ObjectExtending;

namespace EasyAbp.FileManagement.Files.Dtos
{
    [Serializable]
    public class UpdateFileActionInput : ExtensibleObject
    {
        [Required]
        public string FileName { get; set; }

        public IFormFile File { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            base.Validate(validationContext);
            
            if (FileName.IsNullOrWhiteSpace())
            {
                yield return new ValidationResult("FileName should not be empty!",
                    new[] {nameof(FileName)});
            }
        }
    }
}