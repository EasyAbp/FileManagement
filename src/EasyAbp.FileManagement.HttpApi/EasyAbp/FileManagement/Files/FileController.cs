using System;
using System.IO;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Files.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace EasyAbp.FileManagement.Files
{
    [RemoteService(Name = "EasyAbpFileManagement")]
    [Route("/api/fileManagement/file")]
    public class FileController : FileManagementController
    {
        private readonly IFileAppService _service;

        public FileController(IFileAppService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("{id}")]
        public Task<FileInfoDto> GetAsync(Guid id)
        {
            return _service.GetAsync(id);
        }

        [HttpGet]
        public Task<PagedResultDto<FileInfoDto>> GetListAsync(GetFileListInput input)
        {
            return _service.GetListAsync(input);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<FileInfoDto> CreateAsync(CreateFileActionInput input)
        {
            if (input.File == null)
            {
                throw new NoUploadedFileException();
            }

            var fileName = input.FileName.IsNullOrWhiteSpace() ? GenerateUniqueFileName(input.File) : input.FileName;
            
            await using var memoryStream = new MemoryStream();
            
            await input.File.CopyToAsync(memoryStream);

            return await _service.CreateAsync(new CreateFileDto
            {
                FileContainerName = input.FileContainerName,
                FileName = fileName,
                MimeType = input.File.ContentType,
                FileType = input.FileType,
                ParentId = input.ParentId,
                OwnerUserId = input.OwnerUserId,
                Content = memoryStream.ToArray()
            });
        }

        protected virtual string GenerateUniqueFileName(IFormFile inputFile)
        {
            return Guid.NewGuid().ToString("N") + Path.GetExtension(inputFile.FileName);
        }

        [HttpPut]
        [Route("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<FileInfoDto> UpdateAsync(Guid id, UpdateFileActionInput input)
        {
            if (input.File == null)
            {
                throw new NoUploadedFileException();
            }
            
            await using var memoryStream = new MemoryStream();
            
            await input.File.CopyToAsync(memoryStream);

            return await _service.UpdateAsync(id, new UpdateFileDto
            {
                FileName = input.FileName,
                MimeType = input.File.ContentType,
                Content = memoryStream.ToArray()
            });
        }

        [HttpDelete]
        [Route("{id}")]
        public Task DeleteAsync(Guid id)
        {
            return _service.DeleteAsync(id);
        }

        [HttpPut]
        [Route("{id}/move")]
        public Task<FileInfoDto> MoveAsync(Guid id, MoveFileInput input)
        {
            return _service.MoveAsync(id, input);
        }

        [HttpGet]
        [Route("{id}/downloadInfo")]
        public Task<FileDownloadInfoModel> GetDownloadInfoAsync(Guid id)
        {
            return _service.GetDownloadInfoAsync(id);
        }

        [HttpPut]
        [Route("{id}/info")]
        public Task<FileInfoDto> UpdateInfoAsync(Guid id, UpdateFileInfoDto input)
        {
            return _service.UpdateInfoAsync(id, input);
        }

        [HttpGet]
        [Route("{id}/download")]
        public async Task<IActionResult> DownloadAsync(Guid id, string token)
        {
            var dto = await _service.DownloadAsync(id, token);

            var memoryStream = new MemoryStream(dto.Content);
            
            return new FileStreamResult(memoryStream, dto.MimeType)
            {
                FileDownloadName = dto.FileName
            };
        }
    }
}