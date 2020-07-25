using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EasyAbp.FileManagement.Files;
using EasyAbp.FileManagement.Files.Dtos;
using EasyAbp.FileManagement.Web.Pages.FileManagement.Files.File.ViewModels;
using Microsoft.AspNetCore.Http;

namespace EasyAbp.FileManagement.Web.Pages.FileManagement.Files.File
{
    public class ReUploadModalModel : FileManagementPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public IFormFile UploadedFile { get; set; }
        
        private readonly IFileAppService _service;

        public ReUploadModalModel(IFileAppService service)
        {
            _service = service;
        }

        public virtual async Task OnGetAsync()
        {
        }

        public virtual async Task<IActionResult> OnPostAsync()
        {
            
            var dto = new UpdateFileDto
            {
                FileName = UploadedFile.FileName,
                MimeType = UploadedFile.ContentType,
                Content = await UploadedFile.GetAllBytesAsync()
            };
            
            await _service.UpdateAsync(Id, dto);

            return NoContent();
        }
    }
}