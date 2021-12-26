using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Content;

namespace EasyAbp.FileManagement.Files.Dtos
{
    public class UpdateFileWithStreamInput : UpdateFileBase
    {
        public IRemoteStreamContent Content { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
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
