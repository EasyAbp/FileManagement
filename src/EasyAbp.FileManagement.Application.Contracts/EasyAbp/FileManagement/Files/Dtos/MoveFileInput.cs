using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyAbp.FileManagement.Files.Dtos
{
    [Serializable]
    public class MoveFileInput : IValidatableObject
    {
        public Guid FileId { get; set; }
        
        public Guid NewParentId { get; set; }
        
        [Required]
        public string NewFileName { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (NewFileName.IsNullOrWhiteSpace())
            {
                yield return new ValidationResult("NewFileName should not be empty!", new[] {nameof(NewFileName)});
            }
        }
    }
}