using Volo.Abp.Bundling;

namespace EasyAbp.FileManagement.Blazor.Host
{
    public class FileManagementBlazorHostBundleContributor : IBundleContributor
    {
        public void AddScripts(BundleContext context)
        {

        }

        public void AddStyles(BundleContext context)
        {
            context.Add("main.css", true);
        }
    }
}
