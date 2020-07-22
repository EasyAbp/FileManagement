using EasyAbp.FileManagement.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace EasyAbp.FileManagement.Pages
{
    public abstract class FileManagementPageModel : AbpPageModel
    {
        protected FileManagementPageModel()
        {
            LocalizationResourceType = typeof(FileManagementResource);
        }
    }
}