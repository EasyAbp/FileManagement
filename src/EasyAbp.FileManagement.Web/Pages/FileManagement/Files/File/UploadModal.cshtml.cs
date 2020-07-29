using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Containers;
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
        
        public PublicFileContainerConfiguration Configuration { get; set; }

        private readonly IFileAppService _service;

        public UploadModalModel(IFileAppService service)
        {
            _service = service;
        }

        public virtual async Task OnGetAsync()
        {
            Configuration = await _service.GetConfigurationAsync(FileContainerName, OwnerUserId);
        }

        public virtual async Task<IActionResult> OnPostAsync()
        {
            var dto = new CreateManyFileInput {FileInfos = new List<CreateFileInput>()};
            foreach (var uploadedFile in UploadedFiles)
            {
                dto.FileInfos.Add(new CreateFileInput
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

        public virtual string GetAllowedFileExtensionsJsCode()
        {
            return Configuration.FileExtensionsConfiguration.IsNullOrEmpty()
                ? "[]"
                : ("['" + Configuration.FileExtensionsConfiguration.Where(x => x.Value).Select(x => x.Key).ToList()
                    .JoinAsString("' ,'") + "']").Replace(".", "");
        }
    }
}