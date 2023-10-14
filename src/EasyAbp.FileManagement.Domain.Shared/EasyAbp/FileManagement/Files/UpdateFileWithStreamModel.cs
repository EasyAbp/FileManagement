using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp.ObjectExtending;

namespace EasyAbp.FileManagement.Files;

public class UpdateFileWithStreamModel : ExtensibleObject
{
    [NotNull]
    public string NewFileName { get; set; }

    [CanBeNull]
    public string NewMimeType { get; set; }

    public Guid? OldParentId { get; set; }

    public Guid? NewParentId { get; set; }

    public Stream NewFileStream { get; set; }

    public UpdateFileWithStreamModel()
    {
    }

    public UpdateFileWithStreamModel([NotNull] string newFileName, [CanBeNull] string newMimeType, Guid? oldParentId,
        Guid? newParentId, Stream newFileStream)
    {
        NewFileName = newFileName;
        NewMimeType = newMimeType;
        OldParentId = oldParentId;
        NewParentId = newParentId;
        NewFileStream = newFileStream;
    }

    public async Task<UpdateFileModel> ToChangeFileModelAsync(CancellationToken cancellationToken = default)
    {
        var model = new UpdateFileModel(NewFileName, NewMimeType, OldParentId, NewParentId,
            await NewFileStream.GetAllBytesAsync(cancellationToken));

        this.MapExtraPropertiesTo(model, MappingPropertyDefinitionChecks.None);

        return model;
    }
}