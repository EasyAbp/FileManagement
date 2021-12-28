using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Containers;
using Microsoft.AspNetCore.Mvc;
using EasyAbp.FileManagement.Files;
using EasyAbp.FileManagement.Files.Dtos;
using EasyAbp.FileManagement.Web.Pages.FileManagement.Files.File.ViewModels;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Content;

namespace EasyAbp.FileManagement.Web.Pages.FileManagement.Files.File
{
    public class ReUploadModalModel : FileManagementPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public IFormFile UploadedFile { get; set; }
        
        public PublicFileContainerConfiguration Configuration { get; set; }

        public string OriginalFileExtension { get; set; }
        
        private readonly IFileAppService _service;

        public ReUploadModalModel(IFileAppService service)
        {
            _service = service;
        }

        public virtual async Task OnGetAsync()
        {
            var dto = await _service.GetAsync(Id);

            OriginalFileExtension = Path.GetExtension(dto.FileName);
            
            Configuration = await _service.GetConfigurationAsync(dto.FileContainerName, dto.OwnerUserId);
        }

        public virtual async Task<IActionResult> OnPostAsync()
        {
            var dto = await _service.GetAsync(Id);

            if (Path.GetExtension(UploadedFile.FileName) != Path.GetExtension(dto.FileName))
            {
                throw new ReUploadWithDifferentExtensionException();
            }

            var updateFileDto = new UpdateFileWithStreamInput
            {
                Content = new RemoteStreamContent(
                    stream: UploadedFile.OpenReadStream(),
                    fileName: dto.FileName,
                    contentType: UploadedFile.ContentType)
            };
            
            await _service.UpdateWithStreamAsync(Id, updateFileDto);

            return NoContent();
        }
        
        public virtual string GetAllowedFileExtensionsJsCode()
        {
            return OriginalFileExtension.IsNullOrWhiteSpace()
                ? "null"
                : ("['" + OriginalFileExtension + "']").Replace(".", "");
        }
    }
}
