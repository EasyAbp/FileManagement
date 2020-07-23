using System.Threading.Tasks;

namespace EasyAbp.FileManagement.Web.Pages.FileManagement.Files.File
{
    public class IndexModel : FileManagementPageModel
    {
        public virtual async Task OnGetAsync()
        {
            await Task.CompletedTask;
        }
    }
}
