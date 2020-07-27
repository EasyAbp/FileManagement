using System;
using System.Linq;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Files.Dtos;
using EasyAbp.FileManagement.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace EasyAbp.FileManagement.Files
{
    public class FileAppService : CrudAppService<File, FileInfoDto, Guid, GetFileListInput, CreateFileDto, UpdateFileDto>,
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

        public override async Task<FileInfoDto> GetAsync(Guid id)
        {
            var file = await GetEntityByIdAsync(id);

            await AuthorizationService.AuthorizeAsync(CreateFileOperationInfoModel(file),
                new OperationAuthorizationRequirement {Name = FileManagementPermissions.File.Default});
            
            return MapToGetOutputDto(file);
        }

        public override async Task<PagedResultDto<FileInfoDto>> GetListAsync(GetFileListInput input)
        {
            await AuthorizationService.AuthorizeAsync(new FileOperationInfoModel
                {
                    ParentId = input.ParentId,
                    FileContainerName = input.FileContainerName,
                    OwnerUserId = input.OwnerUserId
                },
                new OperationAuthorizationRequirement {Name = FileManagementPermissions.File.Default});

            var query = CreateFilteredQuery(input);

            var totalCount = await AsyncExecuter.CountAsync(query);

            query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            var entities = await AsyncExecuter.ToListAsync(query);

            return new PagedResultDto<FileInfoDto>(
                totalCount,
                entities.Select(MapToGetListOutputDto).ToList()
            );
        }

        protected override IQueryable<File> CreateFilteredQuery(GetFileListInput input)
        {
            return _repository
                .Where(x => x.ParentId == input.ParentId && x.OwnerUserId == input.OwnerUserId &&
                            x.FileContainerName == input.FileContainerName)
                .WhereIf(input.DirectoryOnly, x => x.FileType == FileType.Directory)
                .OrderBy(x => x.FileType)
                .ThenBy(x => x.FileName);
        }

        [Authorize]
        public override async Task<FileInfoDto> CreateAsync(CreateFileDto input)
        {
            var parent = await TryGetEntityByNullableIdAsync(input.ParentId);

            var file = await _fileManager.CreateAsync(input.FileContainerName, input.OwnerUserId, input.FileName,
                input.MimeType, input.FileType, parent, input.Content);

            await AuthorizationService.AuthorizeAsync(CreateFileOperationInfoModel(file),
                new OperationAuthorizationRequirement {Name = FileManagementPermissions.File.Create});

            await _fileManager.TrySaveBlobAsync(file, input.Content);

            await _repository.InsertAsync(file, autoSave: true);

            return MapToGetOutputDto(file);
        }

        [Authorize]
        public override async Task DeleteAsync(Guid id)
        {
            var file = await GetEntityByIdAsync(id);
            
            await AuthorizationService.AuthorizeAsync(CreateFileOperationInfoModel(file),
                new OperationAuthorizationRequirement {Name = FileManagementPermissions.File.Delete});

            await _fileManager.DeleteAsync(file);
        }

        [Authorize]
        public virtual async Task<FileInfoDto> MoveAsync(Guid id, MoveFileInput input)
        {
            var file = await GetEntityByIdAsync(id);

            var oldParent = await TryGetEntityByNullableIdAsync(file.ParentId);

            var newParent = input.NewParentId == file.ParentId
                ? oldParent
                : await TryGetEntityByNullableIdAsync(input.NewParentId);
            
            await _fileManager.ChangeAsync(file, input.NewFileName, oldParent, newParent);

            await AuthorizationService.AuthorizeAsync(CreateFileOperationInfoModel(file),
                new OperationAuthorizationRequirement {Name = FileManagementPermissions.File.Move});

            await _repository.UpdateAsync(file, autoSave: true);

            return MapToGetOutputDto(file);
        }

        protected virtual async Task<File> TryGetEntityByNullableIdAsync(Guid? fileId)
        {
            return fileId.HasValue ? await GetEntityByIdAsync(fileId.Value) : null;
        }

        public virtual async Task<FileDownloadInfoModel> GetDownloadInfoAsync(Guid id)
        {
            var file = await GetEntityByIdAsync(id);

            await AuthorizationService.AuthorizeAsync(CreateFileOperationInfoModel(file),
                new OperationAuthorizationRequirement {Name = FileManagementPermissions.File.GetDownloadInfo});

            return await _fileManager.GetDownloadInfoAsync(file);
        }

        [Authorize]
        public override async Task<FileInfoDto> UpdateAsync(Guid id, UpdateFileDto input)
        {
            var file = await GetEntityByIdAsync(id);
            

            var parent = await TryGetEntityByNullableIdAsync(file.ParentId);
            
            await _fileManager.ChangeAsync(file, input.FileName, input.MimeType, input.Content, parent, parent);

            await AuthorizationService.AuthorizeAsync(CreateFileOperationInfoModel(file),
                new OperationAuthorizationRequirement {Name = FileManagementPermissions.File.Update});
            
            await _fileManager.TrySaveBlobAsync(file, input.Content);

            await _repository.UpdateAsync(file, autoSave: true);

            return MapToGetOutputDto(file);
        }
        
        [Authorize]
        public virtual async Task<FileInfoDto> UpdateInfoAsync(Guid id, UpdateFileInfoDto input)
        {
            var file = await GetEntityByIdAsync(id);

            var parent = await TryGetEntityByNullableIdAsync(file.ParentId);
            
            await _fileManager.ChangeAsync(file, input.FileName, parent, parent);

            await AuthorizationService.AuthorizeAsync(CreateFileOperationInfoModel(file),
                new OperationAuthorizationRequirement {Name = FileManagementPermissions.File.Update});

            await _repository.UpdateAsync(file, autoSave: true);

            return MapToGetOutputDto(file);
        }

        protected virtual FileOperationInfoModel CreateFileOperationInfoModel(File file)
        {
            return new FileOperationInfoModel
            {
                ParentId = file.ParentId,
                FileContainerName = file.FileContainerName,
                OwnerUserId = file.OwnerUserId,
                File = file
            };
        }
        
        public virtual async Task<FileDownloadDto> DownloadAsync(Guid id, string token)
        {
            var provider = ServiceProvider.GetRequiredService<LocalFileDownloadProvider>();

            await provider.CheckTokenAsync(token, id);

            var file = await GetEntityByIdAsync(id);

            return new FileDownloadDto
            {
                FileName = file.FileName,
                MimeType = file.MimeType,
                Content = await _fileManager.GetBlobAsync(file)
            };
        }
    }
}
