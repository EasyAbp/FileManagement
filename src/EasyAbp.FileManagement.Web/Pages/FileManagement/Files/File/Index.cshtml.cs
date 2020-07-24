using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EasyAbp.FileManagement.Web.Pages.FileManagement.Files.File
{
    public class IndexModel : FileManagementPageModel
    {
        [BindProperty(SupportsGet = true)]
        public Guid? OwnerUserId { get; set; }
        
        public virtual async Task OnGetAsync()
        {
            await Task.CompletedTask;
        }
    }
}
