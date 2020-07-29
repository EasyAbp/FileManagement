using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EasyAbp.FileManagement.Files;
using EasyAbp.FileManagement.Files.Dtos;

namespace EasyAbp.FileManagement.Web.Pages.FileManagement.Files.File
{
    public class CreateDirectoryModalModel : FileManagementPageModel
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
        public string DirectoryName { get; set; }

        private readonly IFileAppService _service;

        public CreateDirectoryModalModel(IFileAppService service)
        {
            _service = service;
        }

        public virtual async Task<IActionResult> OnPostAsync()
        {
            var dto = new CreateFileInput
            {
                FileContainerName = FileContainerName,
                OwnerUserId = OwnerUserId,
                FileName = DirectoryName,
                FileType = FileType.Directory,
                MimeType = null,
                ParentId = ParentId,
                Content = null
            };
            
            await _service.CreateAsync(dto);
            
            return NoContent();
        }
    }
}