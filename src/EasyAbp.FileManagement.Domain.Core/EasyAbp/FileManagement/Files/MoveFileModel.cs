using JetBrains.Annotations;

namespace EasyAbp.FileManagement.Files;

public class MoveFileModel : UpdateFileInfoModel
{
    [CanBeNull]
    public File NewParent { get; set; }

    public MoveFileModel()
    {
    }

    public MoveFileModel([CanBeNull] File newParent, [NotNull] string newFileName,
        [CanBeNull] string newMimeType) : base(newFileName, newMimeType)
    {
        NewParent = newParent;
    }
}