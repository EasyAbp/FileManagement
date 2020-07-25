using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EasyAbp.FileManagement.Files;
using EasyAbp.FileManagement.Files.Dtos;
using EasyAbp.FileManagement.Web.Pages.FileManagement.Files.File.ViewModels;
using Microsoft.AspNetCore.Http;

namespace EasyAbp.FileManagement.Web.Pages.FileManagement.Files.File
{
    public class UploadModalModel : FileManagementPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public string FileContainerName { get; set; }
        
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid? OwnerUserId { get; set; }
        
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid? ParentId { get; set; }

        [BindProperty]
        public IFormFile UploadedFile { get; set; }

        private readonly IFileAppService _service;

        public UploadModalModel(IFileAppService service)
        {
            _service = service;
        }

        public virtual async Task<IActionResult> OnPostAsync()
        {
            var dto = new CreateFileDto
            {
                FileContainerName = FileContainerName,
                OwnerUserId = OwnerUserId,
                FileName = UploadedFile.FileName,
                FileType = FileType.RegularFile,
                MimeType = UploadedFile.ContentType,
                ParentId = ParentId,
                Content = await UploadedFile.GetAllBytesAsync()
            };
            
            await _service.CreateAsync(dto);
            
            return NoContent();
        }
    }
}