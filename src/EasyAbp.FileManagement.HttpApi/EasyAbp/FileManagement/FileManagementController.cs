using EasyAbp.FileManagement.Localization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace EasyAbp.FileManagement
{
    [Area("file-management")]
    public abstract class FileManagementController : AbpController
    {
        protected FileManagementController()
        {
            LocalizationResource = typeof(FileManagementResource);
        }
    }
}
