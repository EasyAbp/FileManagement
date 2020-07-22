using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Components;
using Volo.Abp.DependencyInjection;

namespace EasyAbp.FileManagement
{
    [Dependency(ReplaceServices = true)]
    public class FileManagementBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "FileManagement";
    }
}
