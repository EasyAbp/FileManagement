using System.Collections.Generic;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;

namespace EasyAbp.FileManagement.Web.Pages.FileManagement.Components.FileManagerWidget;

public class FileManagerStyleBundleContributor : BundleContributor
{
    public override void ConfigureBundle(BundleConfigurationContext context)
    {
        context.Files.AddIfNotContains("/Pages/FileManagement/Components/FileManagerWidget/default.css");
        context.Files.AddIfNotContains("/libs/bootstrap-fileinput/css/fileinput.min.css");
    }
}