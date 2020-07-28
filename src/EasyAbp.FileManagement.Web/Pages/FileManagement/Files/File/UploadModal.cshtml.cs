using System;
using System.Collections.Generic;
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
        public IFormFile[] UploadedFiles { get; set; }

        private readonly IFileAppService _service;

        public UploadModalModel(IFileAppService service)
        {
            _service = service;
        }

        public virtual async Task<IActionResult> OnPostAsync()
        {
            var dto = new CreateManyFileDto {FileInfos = new List<CreateFileDto>()};
            foreach (var uploadedFile in UploadedFiles)
            {
                dto.FileInfos.Add(new CreateFileDto
                {
                    FileContainerName = FileContainerName,
                    OwnerUserId = OwnerUserId,
                    FileName = uploadedFile.FileName,
                    FileType = FileType.RegularFile,
                    MimeType = uploadedFile.ContentType,
                    ParentId = ParentId,
                    Content = await uploadedFile.GetAllBytesAsync()
                });
            }

            await _service.CreateManyAsync(dto);
            
            return NoContent();
        }
    }
}