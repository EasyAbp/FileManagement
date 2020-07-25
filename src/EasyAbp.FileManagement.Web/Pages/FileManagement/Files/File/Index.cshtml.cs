using System;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Files;
using EasyAbp.FileManagement.Files.Dtos;
using EasyAbp.FileManagement.Web.Pages.FileManagement.Files.File.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EasyAbp.FileManagement.Web.Pages.FileManagement.Files.File
{
    public class IndexModel : FileManagementPageModel
    {
        [BindProperty(SupportsGet = true)]
        public FileListFilterViewModel ViewModel { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public Guid? ParentId { get; set; }
        
        public FileInfoDto CurrentDirectory { get; set; }

        private readonly IFileAppService _service;

        public IndexModel(IFileAppService service)
        {
            _service = service;
        }
        
        public virtual async Task OnGetAsync()
        {
            if (ParentId.HasValue)
            {
                CurrentDirectory = await _service.GetAsync(ParentId.Value);
            }
        }
    }
}
