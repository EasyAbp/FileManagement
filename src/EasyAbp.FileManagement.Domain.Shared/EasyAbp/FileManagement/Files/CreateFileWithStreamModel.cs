using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp.ObjectExtending;

namespace EasyAbp.FileManagement.Files;

public class CreateFileWithStreamModel : ExtensibleObject
{
    [NotNull]
    public string FileContainerName { get; set; }

    public Guid? OwnerUserId { get; set; }

    [NotNull]
    public string FileName { get; set; }

    [CanBeNull]
    public string MimeType { get; set; }

    public FileType FileType { get; set; }

    public Guid? ParentId { get; set; }

    public Stream Stream { get; set; }

    public CreateFileWithStreamModel()
    {
    }

    public CreateFileWithStreamModel([NotNull] string fileContainerName, Guid? ownerUserId, [NotNull] string fileName,
        [CanBeNull] string mimeType, FileType fileType, Guid? parentId, Stream stream)
    {
        FileContainerName = fileContainerName;
        OwnerUserId = ownerUserId;
        FileName = fileName;
        MimeType = mimeType;
        FileType = fileType;
        ParentId = parentId;
        Stream = stream;
    }

    public async Task<CreateFileModel> ToCreateFileModelAsync(CancellationToken cancellationToken = default)
    {
        var model = new CreateFileModel(FileContainerName, OwnerUserId, FileName, MimeType, FileType, ParentId,
            await Stream.GetAllBytesAsync(cancellationToken));

        this.MapExtraPropertiesTo(model, MappingPropertyDefinitionChecks.None);

        return model;
    }
}