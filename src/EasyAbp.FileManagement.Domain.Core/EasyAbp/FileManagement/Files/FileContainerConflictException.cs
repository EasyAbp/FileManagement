using Volo.Abp;

namespace EasyAbp.FileManagement.Files
{
    public class FileContainerConflictException : BusinessException
    {
        public FileContainerConflictException() : base(
            "FileContainerConflict",
            $"Multiple file upload requests attempted to save files in the same file container.")
        {
        }
    }
}