using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Containers;
using EasyAbp.FileManagement.Files.Dtos;
using EasyAbp.FileManagement.Options.Containers;
using EasyAbp.FileManagement.Permissions;
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
        private readonly IFileBlobManager _fileBlobManager;
        private readonly IFileContainerConfigurationProvider _configurationProvider;

        public FileAppService(
            IFileManager fileManager,
            IFileRepository repository,
            IFileBlobManager fileBlobManager,
            IFileContainerConfigurationProvider configurationProvider) : base(repository)
        {
            _fileManager = fileManager;
            _repository = repository;
            _fileBlobManager = fileBlobManager;
            _configurationProvider = configurationProvider;
        }

        public override async Task<FileInfoDto> GetAsync(Guid id)
        {
            var file = await GetEntityByIdAsync(id);

            await AuthorizationService.CheckAsync(new FileOperationInfoModel(file.ParentId, file.FileContainerName,
                    file.FileName, file.MimeType, file.FileType, file.ByteSize, file.OwnerUserId, file),
                new OperationAuthorizationRequirement { Name = FileManagementPermissions.File.Default });

            return await MapToGetOutputDtoAsync(file);
        }

        public override async Task<PagedResultDto<FileInfoDto>> GetListAsync(GetFileListInput input)
        {
            await AuthorizationService.CheckAsync(
                new FileOperationInfoModel(input.ParentId, input.FileContainerName, null, null, null, null,
                    input.OwnerUserId, null),
                new OperationAuthorizationRequirement { Name = FileManagementPermissions.File.Default });

            var query = await CreateFilteredQueryAsync(input);

            var totalCount = await AsyncExecuter.CountAsync(query);

            query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            var entities = await AsyncExecuter.ToListAsync(query);

            return new PagedResultDto<FileInfoDto>(
                totalCount,
                entities.Select(MapToGetListOutputDto).ToList()
            );
        }

        protected override async Task<IQueryable<File>> CreateFilteredQueryAsync(GetFileListInput input)
        {
            await Task.CompletedTask;

            return (await _repository.GetQueryableAsync())
                .Where(x => x.ParentId == input.ParentId && x.OwnerUserId == input.OwnerUserId &&
                            x.FileContainerName == input.FileContainerName)
                .WhereIf(input.DirectoryOnly, x => x.FileType == FileType.Directory)
                .OrderBy(x => x.FileType)
                .ThenBy(x => x.FileName);
        }

        [Authorize]
        public virtual async Task<CreateFileOutput> CreateAsync(CreateFileInput input)
        {
            var fileName = ProcessInputFileName(false, input.FileName);

            await AuthorizationService.CheckAsync(
                new FileOperationInfoModel(input.ParentId, input.FileContainerName, fileName, input.MimeType,
                    input.FileType, input.Content?.LongLength, input.OwnerUserId, null),
                new OperationAuthorizationRequirement { Name = FileManagementPermissions.File.Create });

            var model = new CreateFileModel(input.FileContainerName,
                input.OwnerUserId, fileName.Trim(), input.MimeType, input.FileType, input.ParentId, input.Content);

            input.MapExtraPropertiesTo(model, MappingPropertyDefinitionChecks.Source);

            var file = await _fileManager.CreateAsync(model);

            return await MapToCreateOutputDtoAsync(file);
        }

        [Authorize]
        public virtual async Task<CreateFileOutput> CreateWithStreamAsync(CreateFileWithStreamInput input)
        {
            var fileName = ProcessInputFileName(input.GenerateUniqueFileName, input.Content.FileName);

            await AuthorizationService.CheckAsync(new FileOperationInfoModel(input.ParentId, input.FileContainerName,
                    fileName, input.Content.ContentType, FileType.RegularFile, input.Content.ContentLength,
                    input.OwnerUserId, null),
                new OperationAuthorizationRequirement { Name = FileManagementPermissions.File.Create });

            var model = new CreateFileWithStreamModel(input.FileContainerName, input.OwnerUserId, fileName,
                input.Content.ContentType, FileType.RegularFile, input.ParentId, input.Content.GetStream());

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
                new FileOperationInfoModel(file.ParentId, file.FileContainerName, file.FileName, file.MimeType,
                    file.FileType, file.ByteSize, file.OwnerUserId, file),
                new OperationAuthorizationRequirement { Name = FileManagementPermissions.File.Delete });

            await _fileManager.DeleteAsync(file);
        }

        public virtual async Task<CreateManyFileOutput> CreateManyAsync(CreateManyFileInput input)
        {
            var fileNames = new Dictionary<CreateFileInput, string>();

            foreach (var fileInfo in input.FileInfos)
            {
                fileNames[fileInfo] = ProcessInputFileName(false, fileInfo.FileName);

                await AuthorizationService.CheckAsync(
                    new FileOperationInfoModel(fileInfo.ParentId, fileInfo.FileContainerName, fileNames[fileInfo],
                        fileInfo.MimeType, fileInfo.FileType, fileInfo.Content?.LongLength, fileInfo.OwnerUserId, null),
                    new OperationAuthorizationRequirement { Name = FileManagementPermissions.File.Create });
            }

            var models = new List<CreateFileModel>();

            foreach (var model in input.FileInfos.Select(fileInfo => new CreateFileModel(fileInfo.FileContainerName,
                         fileInfo.OwnerUserId, fileNames[fileInfo], fileInfo.MimeType, fileInfo.FileType,
                         fileInfo.ParentId, fileInfo.Content)))
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
            var fileNames = new Dictionary<IRemoteStreamContent, string>();

            foreach (var fileInfo in input.FileContents)
            {
                fileNames[fileInfo] = ProcessInputFileName(input.GenerateUniqueFileName, fileInfo.FileName);

                await AuthorizationService.CheckAsync(
                    new FileOperationInfoModel(input.ParentId, input.FileContainerName, fileNames[fileInfo],
                        fileInfo.ContentType, FileType.RegularFile, fileInfo.ContentLength, input.OwnerUserId, null),
                    new OperationAuthorizationRequirement { Name = FileManagementPermissions.File.Create });
            }

            var models = new List<CreateFileWithStreamModel>();

            foreach (var model in input.FileContents.Select(fileInfo => new CreateFileWithStreamModel(
                         input.FileContainerName, input.OwnerUserId, fileNames[fileInfo], fileInfo.ContentType,
                         FileType.RegularFile, input.ParentId, fileInfo.GetStream())))
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
            var newFileName = input.NewFileName;

            var file = await GetEntityByIdAsync(id);

            await AuthorizationService.CheckAsync(
                new FileOperationInfoModel(input.NewParentId, file.FileContainerName, input.NewFileName, file.MimeType,
                    file.FileType, file.ByteSize, file.OwnerUserId, file),
                new OperationAuthorizationRequirement { Name = FileManagementPermissions.File.Move });

            var oldParent = await TryGetEntityByNullableIdAsync(file.ParentId);

            var newParent = input.NewParentId == file.ParentId
                ? oldParent
                : await TryGetEntityByNullableIdAsync(input.NewParentId);

            input.MapExtraPropertiesTo(file);

            await _fileManager.UpdateAsync(file, newFileName, oldParent, newParent);

            return await MapToGetOutputDtoAsync(file);
        }

        protected virtual async Task<File> TryGetEntityByNullableIdAsync(Guid? fileId)
        {
            return fileId.HasValue ? await GetEntityByIdAsync(fileId.Value) : null;
        }

        public virtual async Task<FileDownloadInfoModel> GetDownloadInfoAsync(Guid id)
        {
            var file = await GetEntityByIdAsync(id);

            await AuthorizationService.CheckAsync(new FileOperationInfoModel(file.ParentId, file.FileContainerName,
                    file.FileName, file.MimeType, file.FileType, file.ByteSize, file.OwnerUserId, file),
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
        public virtual async Task<FileInfoDto> UpdateAsync(Guid id, UpdateFileInput input)
        {
            var file = await GetEntityByIdAsync(id);

            var fileName = ProcessInputFileName(false, input.FileName);

            await AuthorizationService.CheckAsync(
                new FileOperationInfoModel(file.ParentId, file.FileContainerName, fileName, input.MimeType,
                    file.FileType, input.Content?.LongLength, file.OwnerUserId, file),
                new OperationAuthorizationRequirement { Name = FileManagementPermissions.File.Update });

            var model = new UpdateFileModel(fileName, input.MimeType, file.ParentId, file.ParentId, input.Content);

            input.MapExtraPropertiesTo(model, MappingPropertyDefinitionChecks.Source);

            await _fileManager.UpdateAsync(file, model);

            return await MapToGetOutputDtoAsync(file);
        }

        [Authorize]
        public virtual async Task<FileInfoDto> UpdateWithStreamAsync(Guid id, UpdateFileWithStreamInput input)
        {
            var file = await GetEntityByIdAsync(id);

            var fileName = ProcessInputFileName(false, input.Content.FileName);

            await AuthorizationService.CheckAsync(
                new FileOperationInfoModel(file.ParentId, file.FileContainerName, fileName, input.Content.ContentType,
                    file.FileType, input.Content.ContentLength, file.OwnerUserId, file),
                new OperationAuthorizationRequirement { Name = FileManagementPermissions.File.Update });

            var model = new UpdateFileWithStreamModel(fileName, input.Content.ContentType, file.ParentId, file.ParentId,
                input.Content.GetStream());

            input.MapExtraPropertiesTo(model, MappingPropertyDefinitionChecks.Source);

            await _fileManager.UpdateAsync(file, model);

            return await MapToGetOutputDtoAsync(file);
        }

        [Authorize]
        public virtual async Task<FileInfoDto> UpdateInfoAsync(Guid id, UpdateFileInfoInput input)
        {
            var fileName = ProcessInputFileName(false, input.FileName);

            var file = await GetEntityByIdAsync(id);

            await AuthorizationService.CheckAsync(new FileOperationInfoModel(file.ParentId, file.FileContainerName,
                    fileName, file.MimeType, file.FileType, file.ByteSize, file.OwnerUserId, file),
                new OperationAuthorizationRequirement { Name = FileManagementPermissions.File.Update });

            var parent = await TryGetEntityByNullableIdAsync(file.ParentId);

            input.MapExtraPropertiesTo(file);

            await _fileManager.UpdateAsync(file, fileName, parent, parent);

            return await MapToGetOutputDtoAsync(file);
        }

        public virtual async Task<FileDownloadOutput> DownloadAsync(Guid id, string token)
        {
            var provider = LazyServiceProvider.LazyGetRequiredService<LocalFileDownloadProvider>();

            await provider.CheckTokenAsync(token, id);

            var file = await GetEntityByIdAsync(id);

            return new FileDownloadOutput
            {
                FileName = file.FileName,
                MimeType = file.MimeType,
                Content = await _fileBlobManager.GetAsync(file)
            };
        }

        public virtual async Task<IRemoteStreamContent> DownloadWithStreamAsync(Guid id, string token)
        {
            var provider = LazyServiceProvider.LazyGetRequiredService<LocalFileDownloadProvider>();

            await provider.CheckTokenAsync(token, id);

            var file = await GetEntityByIdAsync(id);

            return new RemoteStreamContent(
                new MemoryStream(await _fileBlobManager.GetAsync(file)),
                fileName: file.FileName,
                contentType: file.MimeType);
        }

        [Authorize]
        public virtual Task<PublicFileContainerConfiguration> GetConfigurationAsync(string fileContainerName,
            Guid? ownerUserId)
        {
            return Task.FromResult(
                ObjectMapper.Map<FileContainerConfiguration, PublicFileContainerConfiguration>(
                    _configurationProvider.Get<FileContainerConfiguration>(fileContainerName)));
        }

        protected virtual string GenerateUniqueFileName([CanBeNull] string fileName)
        {
            return Guid.NewGuid().ToString("N") + Path.GetExtension(fileName);
        }
    }
}