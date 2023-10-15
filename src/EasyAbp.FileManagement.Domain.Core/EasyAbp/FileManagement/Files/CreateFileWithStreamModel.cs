using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp.ObjectExtending;

namespace EasyAbp.FileManagement.Files;

public class CreateFileWithStreamModel : CreateFileModelBase
{
    public Stream Stream { get; set; }

    public CreateFileWithStreamModel()
    {
    }

    public CreateFileWithStreamModel([NotNull] string fileContainerName, Guid? ownerUserId, [NotNull] string fileName,
        [CanBeNull] string mimeType, FileType fileType, [CanBeNull] File parent, Stream stream) : base(
        fileContainerName, ownerUserId, fileName, mimeType, fileType, parent)
    {
        Stream = stream;
    }

    public async Task<CreateFileModel> ToCreateFileModelAsync(CancellationToken cancellationToken = default)
    {
        var model = new CreateFileModel(FileContainerName, OwnerUserId, FileName, MimeType, FileType, Parent,
            await Stream.GetAllBytesAsync(cancellationToken));

        this.MapExtraPropertiesTo(model, MappingPropertyDefinitionChecks.None);

        return model;
    }

    public override long GetContentLength()
    {
        return Stream.Length;
    }
}