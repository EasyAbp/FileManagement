using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EasyAbp.FileManagement.Files.Dtos
{
    public class CreateManyFileInput : IValidatableObject
    {
        public List<CreateFileInput> FileInfos { get; set; }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FileInfos.IsNullOrEmpty())
            {
                yield return new ValidationResult("FileInfos should not be null or empty!",
                    new[] {nameof(FileInfos)});
            }
            
            if (FileInfos.Select(x => x.FileContainerName).Distinct().Count() > 1)
            {
                yield return new ValidationResult("FileContainerName of files should not be the same!",
                    new[] {nameof(CreateFileInput.FileContainerName)});
            }
        }
    }
}