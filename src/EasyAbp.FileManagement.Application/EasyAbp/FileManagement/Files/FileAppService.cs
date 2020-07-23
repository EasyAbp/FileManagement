using System;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Files.Dtos;
using EasyAbp.FileManagement.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace EasyAbp.FileManagement.Files
{
    public class FileAppService : CrudAppService<File, FileInfoDto, Guid, PagedAndSortedResultRequestDto, CreateFileDto, UpdateFileDto>,
        IFileAppService
    {
        private readonly IFileManager _fileManager;
        private readonly IFileRepository _repository;
        
        public FileAppService(
            IFileManager fileManager,
            IFileRepository repository) : base(repository)
        {
            _fileManager = fileManager;
            _repository = repository;
        }

        public override Task<FileInfoDto> GetAsync(Guid id)
        {
            return base.GetAsync(id);
        }

        public override Task<PagedResultDto<FileInfoDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            return base.GetListAsync(input);
        }

        [Authorize]
        public override async Task<FileInfoDto> CreateAsync(CreateFileDto input)
        {
            var file = await _fileManager.CreateAsync(input.FileContainerName, input.FileName, input.MimeType,
                input.FileType, input.ParentId, input.Content);

            await AuthorizationService.AuthorizeAsync(file,
                new OperationAuthorizationRequirement {Name = FileManagementPermissions.File.Create});

            if (file.FileType == FileType.RegularFile)
            {
                await _fileManager.SaveBlobAsync(file, input.Content);
            }

            await _repository.InsertAsync(file, autoSave: true);

            return MapToGetOutputDto(file);
        }

        public override Task DeleteAsync(Guid id)
        {
            return base.DeleteAsync(id);
        }

        public async Task<FileInfoDto> MoveAsync(MoveFileInput input)
        {
            throw new NotImplementedException();
        }

        public async Task<FileDownloadDto> GetDownloadUrlAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public override async Task<FileInfoDto> UpdateAsync(Guid id, UpdateFileDto input)
        {
            var file = await _repository.GetAsync(id);

            await _fileManager.UpdateAsync(file, input.FileName, input.MimeType, input.Content);

            await AuthorizationService.AuthorizeAsync(file,
                new OperationAuthorizationRequirement {Name = FileManagementPermissions.File.Update});
            
            if (file.FileType == FileType.RegularFile)
            {
                await _fileManager.SaveBlobAsync(file, input.Content);
            }

            await _repository.UpdateAsync(file, autoSave: true);

            // Todo: publish an local event to remove the old blob

            return MapToGetOutputDto(file);
        }
        
        public async Task<FileInfoDto> UpdateInfoAsync(Guid id, UpdateFileInfoDto input)
        {
            var file = await _repository.GetAsync(id);

            await _fileManager.UpdateAsync(file, input.FileName);

            await AuthorizationService.AuthorizeAsync(file,
                new OperationAuthorizationRequirement {Name = FileManagementPermissions.File.Update});

            await _repository.UpdateAsync(file, autoSave: true);

            return MapToGetOutputDto(file);
        }
    }
}
