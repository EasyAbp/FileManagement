using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Content;
using Volo.Abp.ObjectExtending;

namespace EasyAbp.FileManagement.Files.Dtos
{
    [Serializable]
    public class CreateFileWithStreamInput : CreateFileBase
    {
        public IRemoteStreamContent Content { get; set; }

        public bool GenerateUniqueFileName { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            base.Validate(validationContext);

            if (Content == null)
            {
                yield return new ValidationResult("Content should not be empty!",
                    new[] { nameof(Content) });
            }
            else if (Content.FileName.IsNullOrWhiteSpace())
            {
                yield return new ValidationResult("Content.FileName should not be empty!",
                    new[] { nameof(Content.FileName) });
            }
        }
    }
}