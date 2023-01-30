using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Volo.Abp.ObjectExtending;

namespace EasyAbp.FileManagement.Files.Dtos
{
    public class CreateFileActionInput : ExtensibleObject
    {
        [Required]
        public string FileContainerName { get; set; }

        public FileType FileType { get; set; }

        public Guid? ParentId { get; set; }
        
        public Guid? OwnerUserId { get; set; }
        
        public IFormFile File { get; set; }
        
        public bool GenerateUniqueFileName { get; set; }
        
        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            foreach (var result in base.Validate(validationContext))
            {
                yield return result;
            }
        
            if (FileContainerName.IsNullOrWhiteSpace())
            {
                yield return new ValidationResult("FileContainerName should not be empty!",
                    new[] {nameof(FileContainerName)});
            }
            
            if (!Enum.IsDefined(typeof(FileType), FileType))
            {
                yield return new ValidationResult("FileType is invalid!",
                    new[] {nameof(FileType)});
            }
        }
    }
}