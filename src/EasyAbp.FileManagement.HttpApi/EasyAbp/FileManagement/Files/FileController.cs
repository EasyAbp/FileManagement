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
    public class FileController : FileManagementController, IFileAppService
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
        public Task<FileInfoDto> CreateAsync(CreateFileDto input)
        {
            return _service.CreateAsync(input);
        }

        [HttpPost]
        [Route("upload/fileName/{fileName}/fileContainerName/{fileContainerName}")]
        [Route("upload/fileName/{fileName}/fileContainerName/{fileContainerName}/parentId/{parentId}")]
        [Route("upload/fileName/{fileName}/fileContainerName/{fileContainerName}/ownerUserId/{ownerUserId}")]
        [Route("upload/fileName/{fileName}/fileContainerName/{fileContainerName}/ownerUserId/{ownerUserId}/parentId/{parentId}")]
        public async Task<FileInfoDto> UploadAsync(string fileName, string fileContainerName, Guid? ownerUserId, Guid? parentId, IFormFile file)
        {
            if (file == null)
            {
                throw new NoUploadedFileException();
            }
            
            await using var memoryStream = new MemoryStream();
            
            await file.CopyToAsync(memoryStream);

            return await _service.CreateAsync(new CreateFileDto
            {
                FileContainerName = fileContainerName,
                FileName = fileName,
                MimeType = file.ContentType,
                FileType = FileType.RegularFile,
                ParentId = parentId,
                OwnerUserId = ownerUserId,
                Content = memoryStream.ToArray()
            });
        }

        [HttpPut]
        [Route("{id}")]
        public Task<FileInfoDto> UpdateAsync(Guid id, UpdateFileDto input)
        {
            return _service.UpdateAsync(id, input);
        }
        
        [HttpPut]
        [Route("{id}/reUpload")]
        [Route("{id}/reUpload/fileName/{fileName}")]
        public async Task<FileInfoDto> ReUploadAsync(Guid id, string fileName, IFormFile file)
        {
            if (file == null)
            {
                throw new NoUploadedFileException();
            }
            
            await using var memoryStream = new MemoryStream();
            
            await file.CopyToAsync(memoryStream);

            return await _service.UpdateAsync(id, new UpdateFileDto
            {
                FileName = fileName,
                MimeType = file.ContentType,
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

        [HttpPost]
        [Route("{id}/download")]
        public Task<byte[]> DownloadAsync(Guid id, string token)
        {
            return _service.DownloadAsync(id, token);
        }
    }
}