using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EasyAbp.FileManagement.Files;
using EasyAbp.FileManagement.Files.Dtos;
using EasyAbp.FileManagement.Web.Pages.FileManagement.Files.File.ViewModels;

namespace EasyAbp.FileManagement.Web.Pages.FileManagement.Files.File
{
    public class MoveModalModel : FileManagementPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public MoveFileViewModel ViewModel { get; set; }

        private readonly IFileAppService _service;

        public MoveModalModel(IFileAppService service)
        {
            _service = service;
        }

        public virtual async Task OnGetAsync()
        {
            var dto = await _service.GetAsync(Id);
        }

        public virtual async Task<IActionResult> OnPostAsync()
        {
            var dto = new MoveFileInput
            {
                NewParentId = ViewModel.NewParentId,
                NewFileName = ViewModel.NewFileName
            };
            
            await _service.MoveAsync(Id, dto);
            
            return NoContent();
        }
    }
}