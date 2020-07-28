using Volo.Abp;

namespace EasyAbp.FileManagement.Files
{
    public class FileExtensionIsNotAllowedException : BusinessException
    {
        public FileExtensionIsNotAllowedException(string fileExtension) : base(
            "FileExtensionIsNotAllowed",
            $"The extension {fileExtension} is not allowed.")
        {
        }
    }
}