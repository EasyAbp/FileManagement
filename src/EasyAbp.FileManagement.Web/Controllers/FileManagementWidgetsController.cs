using System;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Web.Pages.FileManagement.Components.FileManagerWidget;
using Microsoft.AspNetCore.Mvc;

namespace EasyAbp.FileManagement.Web.Controllers;

public class FileManagementWidgetsController : FileManagementControllerBase
{
    public virtual Task<IActionResult> FileManager(string fileContainerName, Guid? ownerUserId, Guid? parentId)
    {
        return Task.FromResult((IActionResult)ViewComponent(typeof(FileManagerWidgetViewComponent),
            new { fileContainerName, ownerUserId, parentId }));
    }
}