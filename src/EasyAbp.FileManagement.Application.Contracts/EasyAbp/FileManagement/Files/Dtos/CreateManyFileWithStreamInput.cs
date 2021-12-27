using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Volo.Abp.Content;

namespace EasyAbp.FileManagement.Files.Dtos
{
    public class CreateManyFileWithStreamInput : CreateFileBase
    {
        public List<IRemoteStreamContent> FileContents { get; set; } = new();

        public bool GenerateUniqueFileName { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            base.Validate(validationContext);

            if (FileContents.IsNullOrEmpty())
            {
                yield return new ValidationResult("FileContents should not be null or empty!",
                    new[] { nameof(FileContents) });
            }
        }
    }
}
