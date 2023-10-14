using JetBrains.Annotations;
using Volo.Abp.ObjectExtending;

namespace EasyAbp.FileManagement.Files;

public class UpdateFileInfoModel : ExtensibleObject
{
    [NotNull]
    public string NewFileName { get; set; }

    [CanBeNull]
    public string NewMimeType { get; set; }

    public UpdateFileInfoModel()
    {
    }

    public UpdateFileInfoModel([NotNull] string newFileName, [CanBeNull] string newMimeType)
    {
        NewFileName = newFileName;
        NewMimeType = newMimeType;
    }
}