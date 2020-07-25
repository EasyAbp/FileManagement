using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Containers;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.BlobStoring;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Services;
using Volo.Abp.Timing;
using Volo.Abp.Users;

namespace EasyAbp.FileManagement.Files
{
    public class FileManager : DomainService, IFileManager
    {
        private readonly IClock _clock;
        private readonly ICurrentUser _currentUser;
        private readonly IDistributedCache<UserFileDownloadLimitCacheItem> _downloadLimitCache;
        private readonly IBlobContainerFactory _blobContainerFactory;
        private readonly IFileRepository _fileRepository;
        private readonly IFileBlobNameGenerator _fileBlobNameGenerator;
        private readonly IFileContentHashProvider _fileContentHashProvider;
        private readonly IFileContainerConfigurationProvider _configurationProvider;

        public FileManager(
            IClock clock,
            ICurrentUser currentUser,
            IDistributedCache<UserFileDownloadLimitCacheItem> downloadLimitCache,
            IBlobContainerFactory blobContainerFactory,
            IFileRepository fileRepository,
            IFileBlobNameGenerator fileBlobNameGenerator,
            IFileContentHashProvider fileContentHashProvider,
            IFileContainerConfigurationProvider configurationProvider)
        {
            _clock = clock;
            _currentUser = currentUser;
            _downloadLimitCache = downloadLimitCache;
            _blobContainerFactory = blobContainerFactory;
            _fileRepository = fileRepository;
            _fileBlobNameGenerator = fileBlobNameGenerator;
            _fileContentHashProvider = fileContentHashProvider;
            _configurationProvider = configurationProvider;
        }

        public virtual async Task<File> CreateAsync(string fileContainerName, Guid? ownerUserId, string fileName,
            string mimeType, FileType fileType, Guid? parentId, byte[] fileContent)
        {
            Check.NotNullOrWhiteSpace(fileContainerName, nameof(File.FileContainerName));
            Check.NotNullOrWhiteSpace(fileName, nameof(File.FileName));

            var configuration = _configurationProvider.Get(fileContainerName);

            CheckFileName(fileName, configuration);
            CheckDirectoryHasNoFileContent(fileType, fileContent);

            var filePath = await GetFilePathAsync(parentId, fileContainerName, fileName);

            var blobName = await _fileBlobNameGenerator.CreateAsync(fileType, fileName, filePath, mimeType,
                configuration.AbpBlobDirectorySeparator);

            await CheckFileNotExistAsync(filePath, fileContainerName, ownerUserId);

            var hashString = _fileContentHashProvider.GetHashString(fileContent);

            var file = new File(GuidGenerator.Create(), CurrentTenant.Id, fileContainerName, fileName, filePath,
                mimeType, fileType, 0, fileContent.LongLength, hashString, blobName, parentId, fileName, ownerUserId);

            return file;
        }

        public virtual async Task<File> UpdateAsync(File file, string newFileName, Guid? newParentId)
        {
            Check.NotNullOrWhiteSpace(newFileName, nameof(File.FileName));

            var configuration = _configurationProvider.Get(file.FileContainerName);

            CheckFileName(newFileName, configuration);

            var filePath = await GetFilePathAsync(newParentId, file.FileContainerName, newFileName);

            await CheckFileNotExistAsync(filePath, file.FileContainerName, file.OwnerUserId);

            file.UpdateInfo(newFileName, filePath, file.MimeType, file.SubFilesQuantity, file.ByteSize, file.Hash,
                file.BlobName, newParentId, newFileName);
            
            return file;
        }

        public virtual async Task<File> UpdateAsync(File file, string newFileName, Guid? newParentId,
            string newMimeType, byte[] newFileContent)
        {
            Check.NotNullOrWhiteSpace(newFileName, nameof(File.FileName));

            var configuration = _configurationProvider.Get(file.FileContainerName);

            CheckFileName(newFileName, configuration);
            CheckDirectoryHasNoFileContent(file.FileType, newFileContent);
            
            var filePath = await GetFilePathAsync(newParentId, file.FileContainerName, newFileName);

            var blobName = await _fileBlobNameGenerator.CreateAsync(file.FileType, newFileName, filePath, newMimeType,
                configuration.AbpBlobDirectorySeparator);

            await CheckFileNotExistAsync(filePath, file.FileContainerName, file.OwnerUserId);

            // Todo: publish a file blobName changed local event after uow completed (try to remove the blob if no file is using the blob).

            var hashString = _fileContentHashProvider.GetHashString(newFileContent);

            file.UpdateInfo(newFileName, filePath, newMimeType, file.SubFilesQuantity, file.ByteSize, hashString,
                blobName, newParentId, newFileName);

            return file;
        }

        protected virtual void CheckFileName(string fileName, FileContainerConfiguration configuration)
        {
            if (fileName.Contains(FileManagementConsts.DirectorySeparator))
            {
                throw new FileNameContainsSeparatorException(fileName, FileManagementConsts.DirectorySeparator);
            }
        }
        
        protected virtual void CheckDirectoryHasNoFileContent(FileType fileType, byte[] fileContent)
        {
            if (fileType == FileType.Directory && fileContent.LongLength != 0)
            {
                throw new DirectoryFileContentIsNotEmptyException();
            }
        }

        public virtual async Task SaveBlobAsync(File file, byte[] fileContent, bool overrideExisting = false)
        {
            if (file.FileType != FileType.RegularFile)
            {
                throw new UnexpectedFileTypeException(file.Id, file.FileType, FileType.RegularFile);
            }
            
            var configuration = _configurationProvider.Get(file.FileContainerName);
            
            var blobContainer = _blobContainerFactory.Create(configuration.AbpBlobContainerName);
            
            await blobContainer.SaveAsync(file.BlobName, fileContent, overrideExisting);
        }

        public virtual async Task<byte[]> GetBlobAsync(File file)
        {
            if (file.FileType != FileType.RegularFile)
            {
                throw new UnexpectedFileTypeException(file.Id, file.FileType, FileType.RegularFile);
            }
            
            var configuration = _configurationProvider.Get(file.FileContainerName);
            
            var blobContainer = _blobContainerFactory.Create(configuration.AbpBlobContainerName);

            return await blobContainer.GetAllBytesAsync(file.BlobName);
        }

        public virtual async Task<FileDownloadInfoModel> GetDownloadInfoAsync(File file)
        {
            if (file.FileType != FileType.RegularFile)
            {
                throw new UnexpectedFileTypeException(file.Id, file.FileType, FileType.RegularFile);
            }
            
            var provider = GetFileDownloadProvider(file);

            var configuration = _configurationProvider.Get(file.FileContainerName);

            if (!configuration.EachUserGetDownloadInfoLimitPreMinute.HasValue)
            {
                return await provider.CreateDownloadInfoAsync(file);
            }

            var cacheItemKey = GetDownloadLimitCacheItemKey();

            var absoluteExpiration = _clock.Now.AddMinutes(1);
            
            var cacheItem = await _downloadLimitCache.GetOrAddAsync(GetDownloadLimitCacheItemKey(),
                () => Task.FromResult(new UserFileDownloadLimitCacheItem
                {
                    Count = 0,
                    AbsoluteExpiration = absoluteExpiration
                }), () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = absoluteExpiration
                });

            if (cacheItem.Count >= configuration.EachUserGetDownloadInfoLimitPreMinute.Value)
            {
                throw new UserGetDownloadInfoExceededLimitException();
            }
            
            var downloadInfoModel = await provider.CreateDownloadInfoAsync(file);

            cacheItem.Count++;

            await _downloadLimitCache.SetAsync(cacheItemKey, cacheItem, new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = cacheItem.AbsoluteExpiration
            });

            return downloadInfoModel;
        }

        public async Task<bool> ExistAsync(string fileContainerName, Guid? ownerUserId, string filePath, FileType? fileType)
        {
            return await _fileRepository.FindByFilePathAsync(filePath, fileContainerName, ownerUserId) != null;
        }

        protected virtual IFileDownloadProvider GetFileDownloadProvider(File file)
        {
            var options = ServiceProvider.GetRequiredService<IOptions<FileManagementOptions>>().Value;
            
            var configuration = options.Containers.GetConfiguration(file.FileContainerName);

            var specifiedProviderType = configuration.SpecifiedFileDownloadProviderType;

            var providers = ServiceProvider.GetServices<IFileDownloadProvider>();

            return specifiedProviderType == null
                ? providers.Single(p => p.GetType() == options.DefaultFileDownloadProvider)
                : providers.Single(p => p.GetType() == specifiedProviderType);
        }
        
        protected virtual string GetDownloadLimitCacheItemKey()
        {
            return _currentUser.GetId().ToString();
        }

        protected virtual async Task CheckFileNotExistAsync(string filePath, string fileContainerName, Guid? ownerUserId)
        {
            if (await _fileRepository.FindByFilePathAsync(filePath, fileContainerName, ownerUserId) != null)
            {
                throw new FileAlreadyExistsException(filePath);
            }
        }

        protected virtual async Task<string> GetFilePathAsync(Guid? parentId, string fileContainerName, string fileName)
        {
            if (!parentId.HasValue)
            {
                return fileName;
            }

            var parentFile = await _fileRepository.GetAsync(parentId.Value);

            if (parentFile.FileType != FileType.Directory)
            {
                throw new UnexpectedFileTypeException(parentFile.Id, parentFile.FileType, FileType.Directory);
            }

            if (parentFile.FileContainerName != fileContainerName)
            {
                throw new UnexpectedFileContainerNameException(fileContainerName, parentFile.FileContainerName);
            }

            return parentFile.FilePath.EnsureEndsWith(FileManagementConsts.DirectorySeparator) + fileName;

        }
    }
}