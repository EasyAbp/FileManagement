using EasyAbp.FileManagement.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace EasyAbp.FileManagement.Web.Controllers;

public class FileManagementControllerBase : AbpController
{
    public FileManagementControllerBase()
    {
        LocalizationResource = typeof(FileManagementResource);
    }
}