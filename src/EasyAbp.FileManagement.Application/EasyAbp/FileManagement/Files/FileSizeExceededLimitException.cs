using Volo.Abp;

namespace EasyAbp.FileManagement.Files
{
    public class FileSizeExceededLimitException : BusinessException
    {
        public FileSizeExceededLimitException(string fileName, long fileByteSize, long maxByteSize) : base(
            "FileSizeExceededLimit",
            $"The size of the file (name: {fileName}, size: {fileByteSize}) exceeded the limit: {maxByteSize}.")
        {
        }
    }
}