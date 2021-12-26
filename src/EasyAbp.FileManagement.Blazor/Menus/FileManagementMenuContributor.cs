using System.Threading.Tasks;
using Volo.Abp.UI.Navigation;

namespace EasyAbp.FileManagement.Blazor.Menus
{
    public class FileManagementMenuContributor : IMenuContributor
    {
        public async Task ConfigureMenuAsync(MenuConfigurationContext context)
        {
            if (context.Menu.Name == StandardMenus.Main)
            {
                await ConfigureMainMenuAsync(context);
            }
        }

        private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
        {
            //Add main menu items.
            context.Menu.AddItem(new ApplicationMenuItem(FileManagementMenus.Prefix, displayName: "FileManagement", "/FileManagement", icon: "fa fa-globe"));
            
            return Task.CompletedTask;
        }
    }
}