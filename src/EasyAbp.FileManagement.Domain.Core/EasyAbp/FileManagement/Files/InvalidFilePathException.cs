using Volo.Abp;

namespace EasyAbp.FileManagement.Files
{
    public class InvalidFilePathException : BusinessException
    {
        public InvalidFilePathException(string filePath) : base("InvalidFilePath",
            $"The file path {filePath} is invalid.")
        {
        }
    }
}