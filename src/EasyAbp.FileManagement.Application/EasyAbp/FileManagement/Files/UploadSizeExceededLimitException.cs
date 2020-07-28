using Volo.Abp;

namespace EasyAbp.FileManagement.Files
{
    public class UploadSizeExceededLimitException : BusinessException
    {
        public UploadSizeExceededLimitException(long uploadByteSize, long maxByteSize) : base(
            "UploadSizeExceededLimit",
            $"The total size of the files ({uploadByteSize}) exceeded the limit: {maxByteSize}.")
        {
        }
    }
}