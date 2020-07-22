using EasyAbp.FileManagement.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace EasyAbp.FileManagement.Web.Pages
{
    /* Inherit your PageModel classes from this class.
     */
    public abstract class FileManagementPageModel : AbpPageModel
    {
        protected FileManagementPageModel()
        {
            LocalizationResourceType = typeof(FileManagementResource);
            ObjectMapperContext = typeof(FileManagementWebModule);
        }
    }
}