using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Containers;
using EasyAbp.FileManagement.Files.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;
using Volo.Abp.ObjectExtending;

namespace EasyAbp.FileManagement.Files
{
    [RemoteService(Name = FileManagementRemoteServiceConsts.RemoteServiceName)]
    [Route("/api/file-management/file")]
    public class FileController : FileManagementController, IFileAppService
    {
        private readonly IFileAppService _service;

        public FileController(IFileAppService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<FileInfoDto> GetAsync(Guid id)
        {
            return _service.GetAsync(id);
        }

        [HttpGet]
        public virtual Task<PagedResultDto<FileInfoDto>> GetListAsync(GetFileListInput input)
        {
            return _service.GetListAsync(input);
        }

        [HttpPost]
        [Route("with-bytes")]
        public virtual Task<CreateFileOutput> CreateAsync(CreateFileInput input)
        {
            return _service.CreateAsync(input);
        }

        [HttpPost]
        [Route("with-stream")]
        public virtual Task<CreateFileOutput> CreateWithStreamAsync(CreateFileWithStreamInput input)
        {
            return _service.CreateWithStreamAsync(input);
        }


        [HttpPost]
        [Consumes("multipart/form-data")]
        public virtual async Task<CreateFileOutput> ActionCreateAsync([FromForm] CreateFileActionInput input)
        {
            if (input.File == null)
            {
                throw new NoUploadedFileException();
            }

            var fileName = input.GenerateUniqueFileName ? GenerateUniqueFileName(input.File) : input.File.FileName;
            
            await using var memoryStream = new MemoryStream();
            
            await input.File.CopyToAsync(memoryStream);
            
            var createFileInput = new CreateFileInput
            {
                FileContainerName = input.FileContainerName,
                FileName = fileName,
                MimeType = input.File.ContentType,
                FileType = input.FileType,
                ParentId = input.ParentId,
                OwnerUserId = input.OwnerUserId,
                Content = memoryStream.ToArray()
            };
            
            input.MapExtraPropertiesTo(createFileInput, MappingPropertyDefinitionChecks.None);

            return await _service.CreateAsync(createFileInput);
        }
        
        [HttpPost]
        [Route("many/with-bytes")]
        public virtual Task<CreateManyFileOutput> CreateManyAsync(CreateManyFileInput input)
        {
            return _service.CreateManyAsync(input);
        }

        [HttpPost]
        [Route("many/with-stream")]
        public virtual Task<CreateManyFileOutput> CreateManyWithStreamAsync(CreateManyFileWithStreamInput input)
        {
            return _service.CreateManyWithStreamAsync(input);
        }

        [HttpPost]
        [Route("many")]
        [Consumes("multipart/form-data")]
        public virtual async Task<CreateManyFileOutput> ActionCreateManyAsync([FromForm] CreateManyFileActionInput input)
        {
            if (input.Files.IsNullOrEmpty())
            {
                throw new NoUploadedFileException();
            }

            var createFileDtos = new List<CreateFileInput>();
            
            foreach (var file in input.Files)
            {
                var fileName = input.GenerateUniqueFileName ? GenerateUniqueFileName(file) : file.FileName;

                await using var memoryStream = new MemoryStream();
                
                await file.CopyToAsync(memoryStream);

                createFileDtos.Add(new CreateFileInput
                {
                    FileContainerName = input.FileContainerName,
                    FileName = fileName,
                    MimeType = file.ContentType,
                    FileType = input.FileType,
                    ParentId = input.ParentId,
                    OwnerUserId = input.OwnerUserId,
                    Content = memoryStream.ToArray()
                });
            }
            
            var createManyFileInput = new CreateManyFileInput
            {
                FileInfos = createFileDtos
            };
            
            input.MapExtraPropertiesTo(createManyFileInput, MappingPropertyDefinitionChecks.None);

            return await _service.CreateManyAsync(createManyFileInput);
        }

        protected virtual string GenerateUniqueFileName(IFormFile inputFile)
        {
            return Guid.NewGuid().ToString("N") + Path.GetExtension(inputFile.FileName);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _service.DeleteAsync(id);
        }

        [HttpPut]
        [Route("{id}/with-bytes")]
        public virtual Task<FileInfoDto> UpdateAsync(Guid id, UpdateFileInput input)
        {
            return _service.UpdateAsync(id, input);
        }

        [HttpPut]
        [Route("{id}/with-stream")]
        public Task<FileInfoDto> UpdateWithStreamAsync(Guid id, UpdateFileWithStreamInput input)
        {
            return _service.UpdateWithStreamAsync(id, input);
        }

        [HttpPut]
        [Route("{id}")]
        [Consumes("multipart/form-data")]
        public virtual async Task<FileInfoDto> ActionUpdateAsync(Guid id, [FromForm] UpdateFileActionInput input)
        {
            if (input.File == null)
            {
                throw new NoUploadedFileException();
            }
            
            await using var memoryStream = new MemoryStream();
            
            await input.File.CopyToAsync(memoryStream);

            var updateDto = new UpdateFileInput
            {
                FileName = input.FileName,
                MimeType = input.File.ContentType,
                Content = memoryStream.ToArray()
            };
            
            input.MapExtraPropertiesTo(updateDto, MappingPropertyDefinitionChecks.None);
            
            return await _service.UpdateAsync(id, updateDto);
        }

        [HttpPut]
        [Route("{id}/move")]
        public virtual Task<FileInfoDto> MoveAsync(Guid id, MoveFileInput input)
        {
            return _service.MoveAsync(id, input);
        }

        [HttpGet]
        [Route("{id}/download-info")]
        public virtual Task<FileDownloadInfoModel> GetDownloadInfoAsync(Guid id)
        {
            return _service.GetDownloadInfoAsync(id);
        }

        [HttpPut]
        [Route("{id}/info")]
        public virtual Task<FileInfoDto> UpdateInfoAsync(Guid id, UpdateFileInfoInput input)
        {
            return _service.UpdateInfoAsync(id, input);
        }

        [HttpGet]
        [Route("{id}/download/with-bytes")]
        public virtual Task<FileDownloadOutput> DownloadAsync(Guid id, string token)
        {
            return _service.DownloadAsync(id, token);
        }

        [HttpGet]
        [Route("{id}/download/with-stream")]
        public Task<IRemoteStreamContent> DownloadWithStreamAsync(Guid id, string token)
        {
            return _service.DownloadWithStreamAsync(id, token);
        }

        [HttpGet]
        [Route("{id}/download")]
        public virtual async Task<IActionResult> ActionDownloadAsync(Guid id, string token)
        {
            var dto = await _service.DownloadAsync(id, token);

            var memoryStream = new MemoryStream(dto.Content);
            
            return new FileStreamResult(memoryStream, dto.MimeType)
            {
                FileDownloadName = dto.FileName
            };
        }

        [HttpGet]
        [Route("configuration")]
        public virtual async Task<PublicFileContainerConfiguration> GetConfigurationAsync(string fileContainerName,
            Guid? ownerUserId)
        {
            return await _service.GetConfigurationAsync(fileContainerName, ownerUserId);
        }
    }
}