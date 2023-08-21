using System.Threading.Tasks;
using EasyAbp.FileManagement.Localization;
using EasyAbp.FileManagement.Permissions;
using Volo.Abp.UI.Navigation;

namespace EasyAbp.FileManagement.Web.Menus
{
    public class FileManagementMenuContributor : IMenuContributor
    {
        public async Task ConfigureMenuAsync(MenuConfigurationContext context)
        {
            if (context.Menu.Name == StandardMenus.Main)
            {
                await ConfigureMainMenu(context);
            }
        }

        private async Task ConfigureMainMenu(MenuConfigurationContext context)
        {
            var l = context.GetLocalizer<FileManagementResource>();

            if (await context.IsGrantedAsync(FileManagementPermissions.File.Default))
            {
                context.Menu.AddItem(new ApplicationMenuItem(FileManagementMenus.Prefix,
                    l["Menu:File"], "/FileManagement/Files/File", icon: "fa fa-folder")
                );
            }
        }
    }
}
