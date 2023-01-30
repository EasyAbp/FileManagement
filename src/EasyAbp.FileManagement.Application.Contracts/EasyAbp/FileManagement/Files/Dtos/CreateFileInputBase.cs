using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.ObjectExtending;

namespace EasyAbp.FileManagement.Files.Dtos
{
    public abstract class CreateFileInputBase : ExtensibleObject
    {
        [Required]
        public string FileContainerName { get; set; }

        public Guid? ParentId { get; set; }

        public Guid? OwnerUserId { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            foreach (var result in base.Validate(validationContext))
            {
                yield return result;
            }

            if (FileContainerName.IsNullOrWhiteSpace())
            {
                yield return new ValidationResult("FileContainerName should not be empty!",
                    new[] { nameof(FileContainerName) });
            }
        }
    }
}
