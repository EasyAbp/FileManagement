using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EasyAbp.FileManagement.Files;
using EasyAbp.FileManagement.Files.Dtos;
using EasyAbp.FileManagement.Web.Pages.FileManagement.Files.File.ViewModels;

namespace EasyAbp.FileManagement.Web.Pages.FileManagement.Files.File
{
    public class CreateModalModel : FileManagementPageModel
    {
        [BindProperty]
        public CreateFileViewModel ViewModel { get; set; }

        private readonly IFileAppService _service;

        public CreateModalModel(IFileAppService service)
        {
            _service = service;
        }

        public virtual async Task<IActionResult> OnPostAsync()
        {
            var dto = ObjectMapper.Map<CreateFileViewModel, CreateFileDto>(ViewModel);
            await _service.CreateAsync(dto);
            return NoContent();
        }
    }
}