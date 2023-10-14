using Volo.Abp;

namespace EasyAbp.FileManagement.Files
{
    public class UploadQuantityExceededLimitException : BusinessException
    {
        public UploadQuantityExceededLimitException(long uploadQuantity, long maxQuantity) : base(
            "UploadQuantityExceededLimit",
            $"The quantity of the files ({uploadQuantity}) exceeded the limit: {maxQuantity}.")
        {
        }
    }
}