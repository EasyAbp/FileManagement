using Volo.Abp;

namespace EasyAbp.FileManagement.Files
{
    public class FileContainerNotExistException : BusinessException
    {
        public FileContainerNotExistException(string fileExtension) : base(
            "FileContainerNotExist",
            $"The extension {fileExtension} does not exist.")
        {
        }
    }
}