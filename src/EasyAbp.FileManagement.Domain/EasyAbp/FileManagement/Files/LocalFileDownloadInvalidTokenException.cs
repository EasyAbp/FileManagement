using Volo.Abp;

namespace EasyAbp.FileManagement.Files
{
    public class LocalFileDownloadInvalidTokenException : BusinessException
    {
        public LocalFileDownloadInvalidTokenException() : base("LocalFileDownloadInvalidToken",
            "The file download token is not invalid.")
        {
        }
    }
}