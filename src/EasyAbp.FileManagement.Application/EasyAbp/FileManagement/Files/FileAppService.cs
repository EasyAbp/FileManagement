using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Containers;
using EasyAbp.FileManagement.Files.Dtos;
using EasyAbp.FileManagement.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Caching;
using Volo.Abp.Users;

namespace EasyAbp.FileManagement.Files
{
    public class FileAppService : CrudAppService<File, FileInfoDto, Guid, GetFileListInput, CreateFileDto, UpdateFileDto>,
        IFileAppService
    {
        private readonly IFileManager _fileManager;
        private readonly IFileRepository _repository;
        private readonly IFileContainerConfigurationProvider _configurationProvider;

        public FileAppService(
            IFileManager fileManager,
            IFileRepository repository,
            IFileContainerConfigurationProvider configurationProvider) : base(repository)
        {
            _fileManager = fileManager;
            _repository = repository;
            _configurationProvider = configurationProvider;
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
            var configuration = _configurationProvider.Get(input.FileContainerName);

            CheckFileSize(new Dictionary<string, long> {{input.FileName, input.Content?.LongLength ?? 0}}, configuration);

            if (input.FileType == FileType.RegularFile)
            {
                CheckFileExtension(new[] {Path.GetExtension(input.FileName)}, configuration);
            }
         
            var parent = await TryGetEntityByNullableIdAsync(input.ParentId);

            var file = await _fileManager.CreateAsync(input.FileContainerName, input.OwnerUserId, input.FileName,
                input.MimeType, input.FileType, parent, input.Content);

            await AuthorizationService.AuthorizeAsync(CreateFileOperationInfoModel(file),
                new OperationAuthorizationRequirement {Name = FileManagementPermissions.File.Create});
            
            await _repository.InsertAsync(file, autoSave: true);

            await _fileManager.TrySaveBlobAsync(file, input.Content);

            return MapToGetOutputDto(file);
        }

        protected virtual void CheckFileQuantity(int count, FileContainerConfiguration configuration)
        {
            if (count > configuration.MaxFileQuantityForEachUpload)
            {
                throw new UploadQuantityExceededLimitException(count, configuration.MaxFileQuantityForEachUpload);
            }
        }

        protected virtual void CheckFileSize(Dictionary<string, long> fileNameByteSizeMapping, FileContainerConfiguration configuration)
        {
            foreach (var pair in fileNameByteSizeMapping.Where(pair => pair.Value > configuration.MaxByteSizeForEachFile))
            {
                throw new FileSizeExceededLimitException(pair.Key, pair.Value, configuration.MaxByteSizeForEachFile);
            }

            var totalByteSize = fileNameByteSizeMapping.Values.Sum();
            
            if (totalByteSize > configuration.MaxByteSizeForEachUpload)
            {
                throw new UploadSizeExceededLimitException(totalByteSize, configuration.MaxByteSizeForEachUpload);
            }
        }
        
        protected virtual void CheckFileExtension(IEnumerable<string> fileExtensions, FileContainerConfiguration configuration)
        {
            foreach (var ext in fileExtensions.Where(ext => !IsFileExtensionAllowed(ext, configuration)))
            {
                throw new FileExtensionIsNotAllowedException(ext);
            }
        }

        protected virtual bool IsFileExtensionAllowed(string ext, FileContainerConfiguration configuration)
        {
            if (!configuration.FileExtensionsConfiguration.ContainsKey(ext))
            {
                return !configuration.AllowOnlyConfiguredFileExtensions;
            }

            return configuration.FileExtensionsConfiguration[ext];
        }

        [Authorize]
        public override async Task DeleteAsync(Guid id)
        {
            var file = await GetEntityByIdAsync(id);
            
            await AuthorizationService.AuthorizeAsync(CreateFileOperationInfoModel(file),
                new OperationAuthorizationRequirement {Name = FileManagementPermissions.File.Delete});

            await _fileManager.DeleteAsync(file);
        }

        public virtual async Task<ListResultDto<FileInfoDto>> CreateManyAsync(CreateManyFileDto input)
        {
            var configuration = _configurationProvider.Get(input.FileInfos.First().FileContainerName);

            CheckFileQuantity(input.FileInfos.Count, configuration);
            CheckFileSize(input.FileInfos.ToDictionary(x => x.FileName, x => x.Content?.LongLength ?? 0), configuration);

            CheckFileExtension(
                input.FileInfos.Where(x => x.FileType == FileType.RegularFile)
                    .Select(x => Path.GetExtension(x.FileName)).Distinct().ToList(), configuration);
            
            var files = new File[input.FileInfos.Count];

            for (var i = 0; i < input.FileInfos.Count; i++)
            {
                var fileInfo = input.FileInfos[i];
                var parent = await TryGetEntityByNullableIdAsync(fileInfo.ParentId);

                var file = await _fileManager.CreateAsync(fileInfo.FileContainerName, fileInfo.OwnerUserId,
                    fileInfo.FileName, fileInfo.MimeType, fileInfo.FileType, parent, fileInfo.Content);

                await AuthorizationService.AuthorizeAsync(CreateFileOperationInfoModel(file),
                    new OperationAuthorizationRequirement {Name = FileManagementPermissions.File.Create});

                await _repository.InsertAsync(file, autoSave: true);

                files[i] = file;
            }

            for (var i = 0; i < files.Length; i++)
            {
                await _fileManager.TrySaveBlobAsync(files[i], input.FileInfos[i].Content);
            }

            return new ListResultDto<FileInfoDto>(files.Select(MapToGetListOutputDto).ToList());
        }

        [Authorize]
        public virtual async Task<FileInfoDto> MoveAsync(Guid id, MoveFileInput input)
        {
            var newFileName = input.NewFileName;
            
            var file = await GetEntityByIdAsync(id);
            
            var configuration = _configurationProvider.Get(file.FileContainerName);
            
            CheckFileExtension(new[] {Path.GetExtension(newFileName)}, configuration);

            var oldParent = await TryGetEntityByNullableIdAsync(file.ParentId);

            var newParent = input.NewParentId == file.ParentId
                ? oldParent
                : await TryGetEntityByNullableIdAsync(input.NewParentId);
            
            await _fileManager.ChangeAsync(file, newFileName, oldParent, newParent);

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

            var downloadLimitCache =
                ServiceProvider.GetRequiredService<IDistributedCache<UserFileDownloadLimitCacheItem>>();

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

            if (cacheItem.Count >= configuration.GetDownloadInfoTimesLimitEachUserPerMinute.Value)
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
        public override async Task<FileInfoDto> UpdateAsync(Guid id, UpdateFileDto input)
        {
            var file = await GetEntityByIdAsync(id);
            
            var configuration = _configurationProvider.Get(file.FileContainerName);

            CheckFileSize(new Dictionary<string, long> {{input.FileName, input.Content?.LongLength ?? 0}}, configuration);
            CheckFileExtension(new[] {Path.GetExtension(input.FileName)}, configuration);

            var parent = await TryGetEntityByNullableIdAsync(file.ParentId);
            
            await _fileManager.ChangeAsync(file, input.FileName, input.MimeType, input.Content, parent, parent);

            await AuthorizationService.AuthorizeAsync(CreateFileOperationInfoModel(file),
                new OperationAuthorizationRequirement {Name = FileManagementPermissions.File.Update});

            await _repository.UpdateAsync(file, autoSave: true);

            await _fileManager.TrySaveBlobAsync(file, input.Content);

            return MapToGetOutputDto(file);
        }
        
        [Authorize]
        public virtual async Task<FileInfoDto> UpdateInfoAsync(Guid id, UpdateFileInfoDto input)
        {
            var fileName = input.FileName;
            
            var file = await GetEntityByIdAsync(id);

            var configuration = _configurationProvider.Get(file.FileContainerName);

            CheckFileExtension(new[] {Path.GetExtension(fileName)}, configuration);
            
            var parent = await TryGetEntityByNullableIdAsync(file.ParentId);
            
            await _fileManager.ChangeAsync(file, fileName, parent, parent);

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

        [Authorize]
        public virtual Task<PublicFileContainerConfiguration> GetConfigurationAsync(string fileContainerName,
            Guid? ownerUserId)
        {
            return Task.FromResult(
                ObjectMapper.Map<FileContainerConfiguration, PublicFileContainerConfiguration>(
                    _configurationProvider.Get(fileContainerName)));
        }
    }
}
