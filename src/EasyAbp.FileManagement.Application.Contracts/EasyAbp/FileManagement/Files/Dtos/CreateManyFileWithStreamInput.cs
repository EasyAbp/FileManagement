using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Volo.Abp.Content;

namespace EasyAbp.FileManagement.Files.Dtos
{
    public class CreateManyFileWithStreamInput : CreateFileInputBase
    {
        public List<IRemoteStreamContent> FileContents { get; set; } = new();

        public bool GenerateUniqueFileName { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            foreach (var result in base.Validate(validationContext))
            {
                yield return result;
            }

            if (FileContents.IsNullOrEmpty())
            {
                yield return new ValidationResult("FileContents should not be null or empty!",
                    new[] { nameof(FileContents) });
            }
        }
    }
}
