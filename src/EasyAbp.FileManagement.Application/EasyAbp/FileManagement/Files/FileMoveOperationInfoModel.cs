using JetBrains.Annotations;

namespace EasyAbp.FileManagement.Files;

public class FileMoveOperationInfoModel : FileUpdateInfoOperationInfoModel
{
    [CanBeNull]
    public File NewParent { get; set; }

    public FileMoveOperationInfoModel(File file, File newParent, [CanBeNull] string newFileName,
        [CanBeNull] string newMimeType) : base(file, newFileName, newMimeType)
    {
        NewParent = newParent;
    }
}