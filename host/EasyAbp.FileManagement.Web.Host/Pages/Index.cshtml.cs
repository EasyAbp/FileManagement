using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace EasyAbp.FileManagement.Pages
{
    public class IndexModel : FileManagementPageModel
    {
        public void OnGet()
        {
            
        }

        public async Task OnPostLoginAsync()
        {
            await HttpContext.ChallengeAsync("oidc");
        }
    }
}