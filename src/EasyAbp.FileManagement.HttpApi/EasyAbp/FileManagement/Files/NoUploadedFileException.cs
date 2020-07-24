using Volo.Abp;

namespace EasyAbp.FileManagement.Files
{
    public class NoUploadedFileException : BusinessException
    {
        public NoUploadedFileException() : base("NoUploadedFile")
        {
            
        }
    }
}