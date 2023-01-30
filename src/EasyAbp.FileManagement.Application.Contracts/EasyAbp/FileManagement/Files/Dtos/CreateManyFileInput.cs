using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Volo.Abp.ObjectExtending;

namespace EasyAbp.FileManagement.Files.Dtos
{
    public class CreateManyFileInput : ExtensibleObject
    {
        public List<CreateFileInput> FileInfos { get; set; }
        
        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            foreach (var result in base.Validate(validationContext))
            {
                yield return result;
            }
            
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