using System.Collections.Generic;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;

namespace EasyAbp.FileManagement.Web.Pages.FileManagement.Components.FileManagerWidget;

public class FileManagerScriptBundleContributor : BundleContributor
{
    public override void ConfigureBundle(BundleConfigurationContext context)
    {
        context.Files.AddIfNotContains("/Pages/FileManagement/Components/FileManagerWidget/default.js");
        context.Files.AddIfNotContains("/libs/popper.js/umd/popper.min.js");
        context.Files.AddIfNotContains("/libs/bootstrap-fileinput/js/fileinput.min.js");
        context.Files.AddIfNotContains("/libs/bootstrap-fileinput/js/plugins/purify.min.js");
        context.Files.AddIfNotContains("/libs/bootstrap-fileinput/js/plugins/sortable.min.js");
        context.Files.AddIfNotContains("/libs/bootstrap-fileinput/themes/fa/theme.min.js");
    }
}