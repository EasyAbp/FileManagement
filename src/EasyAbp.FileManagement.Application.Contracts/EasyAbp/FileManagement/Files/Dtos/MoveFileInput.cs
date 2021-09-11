using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.ObjectExtending;

namespace EasyAbp.FileManagement.Files.Dtos
{
    [Serializable]
    public class MoveFileInput : ExtensibleObject
    {
        public Guid? NewParentId { get; set; }
        
        [Required]
        public string NewFileName { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            base.Validate(validationContext);
            
            if (NewFileName.IsNullOrWhiteSpace())
            {
                yield return new ValidationResult("NewFileName should not be empty!",
                    new[] {nameof(NewFileName)});
            }
            
            NewFileName = NewFileName.Trim();
        }
    }
}