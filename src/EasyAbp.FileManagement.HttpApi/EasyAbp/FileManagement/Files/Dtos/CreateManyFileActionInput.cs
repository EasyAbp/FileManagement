using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EasyAbp.FileManagement.Files.Dtos
{
    public class CreateManyFileActionInput : IValidatableObject
    {
        [Required]
        public string FileContainerName { get; set; }

        public FileType FileType { get; set; }

        public Guid? ParentId { get; set; }
        
        public Guid? OwnerUserId { get; set; }
        
        public IFormFile[] Files { get; set; }
        
        public bool GenerateUniqueFileName { get; set; }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
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