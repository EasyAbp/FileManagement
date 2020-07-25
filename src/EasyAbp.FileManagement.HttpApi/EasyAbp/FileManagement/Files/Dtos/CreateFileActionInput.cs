using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EasyAbp.FileManagement.Files.Dtos
{
    public class CreateFileActionInput : IValidatableObject
    {
        [Required]
        public string FileContainerName { get; set; }
        
        [Required]
        public string FileName { get; set; }
        
        public FileType FileType { get; set; }

        public Guid? ParentId { get; set; }
        
        public Guid? OwnerUserId { get; set; }
        
        public IFormFile File { get; set; }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FileName.IsNullOrWhiteSpace())
            {
                yield return new ValidationResult("FileName should not be empty!",
                    new[] {nameof(FileName)});
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