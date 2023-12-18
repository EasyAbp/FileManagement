using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Containers;
using EasyAbp.FileManagement.Files.Dtos;
using EasyAbp.FileManagement.Options.Containers;
using EasyAbp.FileManagement.Permissions;
using EasyAbp.FileManagement.Users;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Caching;
using Volo.Abp.Content;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Users;

namespace EasyAbp.FileManagement.Files
{
    public class FileAppService : ReadOnlyAppService<File, FileInfoDto, Guid, GetFileListInput>, IFileAppService
    {
        private readonly IFileManager _fileManager;
        private readonly IFileRepository _repository;
        private readonly IFileUserLookupService _fileUserLookupService;
        private readonly IFileContainerConfigurationProvider _configurationProvider;

        public FileAppService(
            IFileManager fileManager,
            IFileRepository repository,
            IFileUserLookupService fileUserLookupService,
            IFileContainerConfigurationProvider configurationProvider) : base(repository)
        {
            _fileManager = fileManager;
            _repository = repository;
            _fileUserLookupService = fileUserLookupService;
            _configurationProvider = configurationProvider;
        }

        public override async Task<FileInfoDto> GetAsync(Guid id)
        {
            var file = await GetEntityByIdAsync(id);

            await AuthorizationService.CheckAsync(new FileGetInfoOperationInfoModel(file),
                new OperationAuthorizationRequirement { Name = FileManagementPermissions.File.Default });

            return await MapToGetOutputDtoAsync(file);
        }

        public override async Task<PagedResultDto<FileInfoDto>> GetListAsync(GetFileListInput input)
        {
            await AuthorizationService.CheckAsync(
                new FileGetListOperationInfoModel(input.ParentId, input.FileContainerName, input.OwnerUserId),
                new OperationAuthorizationRequirement { Name = FileManagementPermissions.File.Default });

            var query = await CreateFilteredQueryAsync(input);

            var totalCount = await AsyncExecuter.CountAsync(query);

            var entityDtos = new List<FileInfoDto>();

            if (totalCount > 0)
            {
                query = ApplySorting(query, input);
                query = ApplyPaging(query, input);

                var entities = await AsyncExecuter.ToListAsync(query);
                entityDtos = await MapToGetListOutputDtosAsync(entities);
            }

            return new PagedResultDto<FileInfoDto>(
                totalCount,
                entityDtos
            );
        }

        protected override async Task<List<FileInfoDto>> MapToGetListOutputDtosAsync(List<File> entities)
        {
            var dtos = await base.MapToGetListOutputDtosAsync(entities);

            await LoadRelatedUserInfoModelsAsync(dtos);

            return dtos;
        }

        protected override async Task<FileInfoDto> MapToGetOutputDtoAsync(File entity)
        {
            var dto = await base.MapToGetOutputDtoAsync(entity);

            await LoadRelatedUserInfoModelsAsync(new List<FileInfoDto> { dto });

            return dto;
        }

        protected virtual async Task LoadRelatedUserInfoModelsAsync(IList<FileInfoDto> dtos)
        {
            var userIds = dtos
                .Where(x => x.CreatorId.HasValue)
                .Select(x => x.CreatorId.Value)
                .Distinct()
                .Union(dtos
                    .Where(x => x.LastModifierId.HasValue)
                    .Select(x => x.LastModifierId.Value)
                    .Distinct())
                .Union(dtos
                    .Where(x => x.OwnerUserId.HasValue)
                    .Select(x => x.OwnerUserId.Value)
                    .Distinct())
                .ToList();

            var users = new Dictionary<Guid, BriefFileUserInfoModel>();

            foreach (var userId in userIds)
            {
                users[userId] = (await _fileUserLookupService.FindByIdAsync(userId)).ToBriefUserInfoModel();
            }

            foreach (var dto in dtos)
            {
                if (dto.CreatorId.HasValue)
                {
                    dto.Creator = users[dto.CreatorId.Value];
                }

                if (dto.LastModifierId.HasValue)
                {
                    dto.LastModifier = users[dto.LastModifierId.Value];
                }

                if (dto.OwnerUserId.HasValue)
                {
                    dto.Owner = users[dto.OwnerUserId.Value];
                }
            }
        }

        protected override IQueryable<File> ApplySorting(IQueryable<File> query, GetFileListInput input)
        {
            return input.Sorting.IsNullOrWhiteSpace()
                ? query.OrderBy(x => x.FileType).ThenBy(x => x.FileName)
                : query.OrderBy(x => x.FileType).ThenBy(input.Sorting!);
        }

        protected override async Task<IQueryable<File>> CreateFilteredQueryAsync(GetFileListInput input)
        {
            await Task.CompletedTask;

            return (await _repository.GetQueryableAsync())
                .Where(x => x.ParentId == input.ParentId && x.OwnerUserId == input.OwnerUserId &&
                            x.FileContainerName == input.FileContainerName)
                .WhereIf(input.DirectoryOnly, x => x.FileType == FileType.Directory);
        }

        [Authorize]
        public virtual async Task<CreateFileOutput> CreateAsync(CreateFileInput input)
        {
            var fileName = ProcessInputFileName(false, input.FileName);

            var parent = await TryGetEntityByNullableIdAsync(input.ParentId);

            await AuthorizationService.CheckAsync(
                new FileCreationOperationInfoModel(parent, input.FileContainerName, fileName, input.MimeType,
                    input.FileType, input.Content?.LongLength, input.OwnerUserId),
                new OperationAuthorizationRequirement { Name = FileManagementPermissions.File.Create });

            var model = new CreateFileModel(input.FileContainerName, input.OwnerUserId, fileName.Trim(), input.MimeType,
                input.FileType, parent, input.Content);

            input.MapExtraPropertiesTo(model, MappingPropertyDefinitionChecks.Source);

            var file = await _fileManager.CreateAsync(model);

            return await MapToCreateOutputDtoAsync(file);
        }

        [Authorize]
        public virtual async Task<CreateFileOutput> CreateWithStreamAsync(CreateFileWithStreamInput input)
        {
            var fileName = ProcessInputFileName(input.GenerateUniqueFileName, input.Content.FileName);

            var parent = await TryGetEntityByNullableIdAsync(input.ParentId);

            await AuthorizationService.CheckAsync(
                new FileCreationOperationInfoModel(parent, input.FileContainerName, fileName, input.Content.ContentType,
                    FileType.RegularFile, input.Content.ContentLength, input.OwnerUserId),
                new OperationAuthorizationRequirement { Name = FileManagementPermissions.File.Create });

            var model = new CreateFileWithStreamModel(input.FileContainerName, input.OwnerUserId, fileName,
                input.Content.ContentType, FileType.RegularFile, parent, input.Content.GetStream());

            input.MapExtraPropertiesTo(model, MappingPropertyDefinitionChecks.Source);

            var file = await _fileManager.CreateAsync(model);

            return await MapToCreateOutputDtoAsync(file);
        }

        protected virtual async Task<CreateFileOutput> MapToCreateOutputDtoAsync(File file)
        {
            var downloadInfo = file.FileType == FileType.RegularFile
                ? await _fileManager.GetDownloadInfoAsync(file)
                : null;

            return new CreateFileOutput
            {
                FileInfo = ObjectMapper.Map<File, FileInfoDto>(file),
                DownloadInfo = downloadInfo
            };
        }

        [Authorize]
        public virtual async Task DeleteAsync(Guid id)
        {
            var file = await GetEntityByIdAsync(id);

            await AuthorizationService.CheckAsync(
                new FileDeletionOperationInfoModel(file),
                new OperationAuthorizationRequirement { Name = FileManagementPermissions.File.Delete });

            await _fileManager.DeleteAsync(file);
        }

        public virtual async Task<CreateManyFileOutput> CreateManyAsync(CreateManyFileInput input)
        {
            var fileNames = new Dictionary<CreateFileInput, string>();
            var parents = new Dictionary<Guid, File>();

            foreach (var fileInfo in input.FileInfos)
            {
                fileNames[fileInfo] = ProcessInputFileName(false, fileInfo.FileName);
                if (fileInfo.ParentId != null && !parents.ContainsKey(fileInfo.ParentId.Value))
                {
                    parents[fileInfo.ParentId.Value] = await TryGetEntityByNullableIdAsync(fileInfo.ParentId.Value);
                }

                await AuthorizationService.CheckAsync(
                    new FileCreationOperationInfoModel(
                        fileInfo.ParentId.HasValue ? parents[fileInfo.ParentId.Value] : null,
                        fileInfo.FileContainerName, fileInfo.FileName, fileInfo.MimeType, fileInfo.FileType,
                        fileInfo.Content?.LongLength, fileInfo.OwnerUserId),
                    new OperationAuthorizationRequirement { Name = FileManagementPermissions.File.Create });
            }

            var models = new List<CreateFileModel>();

            foreach (var model in input.FileInfos.Select(fileInfo => new CreateFileModel(fileInfo.FileContainerName,
                         fileInfo.OwnerUserId, fileNames[fileInfo], fileInfo.MimeType, fileInfo.FileType,
                         fileInfo.ParentId.HasValue ? parents[fileInfo.ParentId.Value] : null, fileInfo.Content)))
            {
                input.MapExtraPropertiesTo(model, MappingPropertyDefinitionChecks.Source);
                models.Add(model);
            }

            var files = await _fileManager.CreateManyAsync(models);

            var outputItems = new List<CreateFileOutput>();

            foreach (var file in files)
            {
                outputItems.Add(await MapToCreateOutputDtoAsync(file));
            }

            return new CreateManyFileOutput { Items = outputItems };
        }

        public virtual async Task<CreateManyFileOutput> CreateManyWithStreamAsync(CreateManyFileWithStreamInput input)
        {
            var parent = await TryGetEntityByNullableIdAsync(input.ParentId);
            var fileNames = new Dictionary<IRemoteStreamContent, string>();

            foreach (var fileInfo in input.FileContents)
            {
                fileNames[fileInfo] = ProcessInputFileName(input.GenerateUniqueFileName, fileInfo.FileName);

                await AuthorizationService.CheckAsync(
                    new FileCreationOperationInfoModel(parent, input.FileContainerName, fileInfo.FileName,
                        fileInfo.ContentType, FileType.RegularFile, fileInfo.ContentLength, input.OwnerUserId),
                    new OperationAuthorizationRequirement { Name = FileManagementPermissions.File.Create });
            }

            var models = new List<CreateFileWithStreamModel>();

            foreach (var model in input.FileContents.Select(fileInfo => new CreateFileWithStreamModel(
                         input.FileContainerName, input.OwnerUserId, fileNames[fileInfo], fileInfo.ContentType,
                         FileType.RegularFile, parent, fileInfo.GetStream())))
            {
                input.MapExtraPropertiesTo(model, MappingPropertyDefinitionChecks.Source);
                models.Add(model);
            }

            var files = await _fileManager.CreateManyAsync(models);

            var outputItems = new List<CreateFileOutput>();

            foreach (var file in files)
            {
                outputItems.Add(await MapToCreateOutputDtoAsync(file));
            }

            return new CreateManyFileOutput { Items = outputItems };
        }

        protected virtual string ProcessInputFileName(bool generateUniqueFileName, [CanBeNull] string inputFileName)
        {
            return generateUniqueFileName || inputFileName == null
                ? GenerateUniqueFileName(inputFileName)
                : inputFileName;
        }

        [Authorize]
        public virtual async Task<FileInfoDto> MoveAsync(Guid id, MoveFileInput input)
        {
            var newFileName = ProcessInputFileName(false, input.NewFileName);

            var file = await GetEntityByIdAsync(id);
            var newParent = await TryGetEntityByNullableIdAsync(input.NewParentId);

            await AuthorizationService.CheckAsync(
                new FileMoveOperationInfoModel(file, newParent, newFileName, file.MimeType),
                new OperationAuthorizationRequirement { Name = FileManagementPermissions.File.Move });

            input.MapExtraPropertiesTo(file);

            await _fileManager.MoveAsync(file, new MoveFileModel(newParent, newFileName, file.MimeType));

            return await MapToGetOutputDtoAsync(file);
        }

        protected virtual async Task<File> TryGetEntityByNullableIdAsync(Guid? fileId)
        {
            return fileId.HasValue ? await GetEntityByIdAsync(fileId.Value) : null;
        }

        public virtual async Task<FileDownloadInfoModel> GetDownloadInfoAsync(Guid id)
        {
            var file = await GetEntityByIdAsync(id);

            await AuthorizationService.CheckAsync(new FileGetDownloadInfoOperationInfoModel(file),
                new OperationAuthorizationRequirement { Name = FileManagementPermissions.File.GetDownloadInfo });

            var downloadLimitCache =
                LazyServiceProvider.LazyGetRequiredService<IDistributedCache<UserFileDownloadLimitCacheItem>>();

            var configuration = _configurationProvider.Get(file.FileContainerName);

            if (!configuration.GetDownloadInfoTimesLimitEachUserPerMinute.HasValue)
            {
                return await _fileManager.GetDownloadInfoAsync(file);
            }

            var cacheItemKey = GetDownloadLimitCacheItemKey();

            var absoluteExpiration = Clock.Now.AddMinutes(1);

            var cacheItem = await downloadLimitCache.GetOrAddAsync(GetDownloadLimitCacheItemKey(),
                () => Task.FromResult(new UserFileDownloadLimitCacheItem
                {
                    Count = 0,
                    AbsoluteExpiration = absoluteExpiration
                }), () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = absoluteExpiration
                });

            if (cacheItem!.Count >= configuration.GetDownloadInfoTimesLimitEachUserPerMinute.Value)
            {
                throw new UserGetDownloadInfoExceededLimitException();
            }

            var infoModel = await _fileManager.GetDownloadInfoAsync(file);

            cacheItem.Count++;

            await downloadLimitCache.SetAsync(cacheItemKey, cacheItem, new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = cacheItem.AbsoluteExpiration
            });

            return infoModel;
        }

        protected virtual string GetDownloadLimitCacheItemKey()
        {
            return CurrentUser.GetId().ToString();
        }

        [Authorize]
        public virtual async Task<FileInfoDto> UpdateInfoAsync(Guid id, UpdateFileInfoInput input)
        {
            var newFileName = ProcessInputFileName(false, input.FileName);

            var file = await GetEntityByIdAsync(id);

            await AuthorizationService.CheckAsync(
                new FileUpdateInfoOperationInfoModel(file, newFileName, file.MimeType),
                new OperationAuthorizationRequirement { Name = FileManagementPermissions.File.Update });

            input.MapExtraPropertiesTo(file);

            await _fileManager.UpdateInfoAsync(file, new UpdateFileInfoModel(newFileName, file.MimeType));

            return await MapToGetOutputDtoAsync(file);
        }

        public virtual async Task<FileDownloadOutput> DownloadAsync(Guid id, string token)
        {
            var provider = LazyServiceProvider.LazyGetRequiredService<ILocalFileDownloadProvider>();

            await provider.CheckTokenAsync(token, id);

            var file = await GetEntityByIdAsync(id);

            return new FileDownloadOutput
            {
                FileName = file.FileName,
                MimeType = file.MimeType,
                Content = await provider.GetDownloadBytesAsync(file)
            };
        }

        public virtual async Task<IRemoteStreamContent> DownloadWithStreamAsync(Guid id, string token)
        {
            var provider = LazyServiceProvider.LazyGetRequiredService<ILocalFileDownloadProvider>();

            await provider.CheckTokenAsync(token, id);

            var file = await GetEntityByIdAsync(id);

            return new RemoteStreamContent(
                await provider.GetDownloadStreamAsync(file),
                fileName: file.FileName,
                contentType: file.MimeType);
        }

        [Authorize]
        public virtual Task<PublicFileContainerConfiguration> GetConfigurationAsync(string fileContainerName,
            Guid? ownerUserId)
        {
            return Task.FromResult(_configurationProvider.Get(fileContainerName).ToPublicConfiguration());
        }

        public virtual async Task<FileLocationDto> GetLocationAsync(Guid id)
        {
            var file = await GetEntityByIdAsync(id);

            var location = await _fileManager.GetFileLocationAsync(file);

            return new FileLocationDto
            {
                Id = file.Id,
                FileName = file.FileName,
                Location = location
            };
        }

        protected virtual string GenerateUniqueFileName([CanBeNull] string fileName)
        {
            return Guid.NewGuid().ToString("N") + Path.GetExtension(fileName);
        }
    }
}