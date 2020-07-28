using Volo.Abp;

namespace EasyAbp.FileManagement.Web.Pages.FileManagement.Files.File
{
    public class ReUploadWithDifferentExtensionException : BusinessException
    {
        public ReUploadWithDifferentExtensionException() : base("ReUploadWithDifferentExtension",
            "The re-uploaded file should has the same file extension with the original file.")
        {
        }
    }
}