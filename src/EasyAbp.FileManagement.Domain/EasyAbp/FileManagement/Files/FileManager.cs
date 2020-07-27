﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Containers;
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.BlobStoring;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Timing;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace EasyAbp.FileManagement.Files
{
    public class FileManager : DomainService, IFileManager
    {
        private readonly IClock _clock;
        private readonly ICurrentUser _currentUser;
        private readonly ILocalEventBus _localEventBus;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IDistributedCache<UserFileDownloadLimitCacheItem> _downloadLimitCache;
        private readonly IBlobContainerFactory _blobContainerFactory;
        private readonly IFileRepository _fileRepository;
        private readonly IFileBlobNameGenerator _fileBlobNameGenerator;
        private readonly IFileContentHashProvider _fileContentHashProvider;
        private readonly IFileContainerConfigurationProvider _configurationProvider;

        public FileManager(
            IClock clock,
            ICurrentUser currentUser,
            ILocalEventBus localEventBus,
            IUnitOfWorkManager unitOfWorkManager,
            IDistributedCache<UserFileDownloadLimitCacheItem> downloadLimitCache,
            IBlobContainerFactory blobContainerFactory,
            IFileRepository fileRepository,
            IFileBlobNameGenerator fileBlobNameGenerator,
            IFileContentHashProvider fileContentHashProvider,
            IFileContainerConfigurationProvider configurationProvider)
        {
            _clock = clock;
            _currentUser = currentUser;
            _localEventBus = localEventBus;
            _unitOfWorkManager = unitOfWorkManager;
            _downloadLimitCache = downloadLimitCache;
            _blobContainerFactory = blobContainerFactory;
            _fileRepository = fileRepository;
            _fileBlobNameGenerator = fileBlobNameGenerator;
            _fileContentHashProvider = fileContentHashProvider;
            _configurationProvider = configurationProvider;
        }

        public virtual async Task<File> CreateAsync(string fileContainerName, Guid? ownerUserId, string fileName,
            string mimeType, FileType fileType, File parent, byte[] fileContent)
        {
            Check.NotNullOrWhiteSpace(fileContainerName, nameof(File.FileContainerName));
            Check.NotNullOrWhiteSpace(fileName, nameof(File.FileName));

            var configuration = _configurationProvider.Get(fileContainerName);

            CheckFileName(fileName, configuration);
            CheckDirectoryHasNoFileContent(fileType, fileContent);

            var hashString = _fileContentHashProvider.GetHashString(fileContent);
            
            string blobName = null;
            
            if (fileType == FileType.RegularFile)
            {
                var existingFile = await _fileRepository.FirstOrDefaultAsync(hashString, fileContent.LongLength);

                if (existingFile != null)
                {
                    Check.NotNullOrWhiteSpace(existingFile.BlobName, nameof(existingFile.BlobName));
                    
                    blobName = existingFile.BlobName;
                }
                else
                {
                    blobName = await _fileBlobNameGenerator.CreateAsync(fileType, fileName, parent, mimeType,
                        configuration.AbpBlobDirectorySeparator);
                }
            }

            await CheckFileNotExistAsync(fileName, parent?.Id, fileContainerName, ownerUserId);

            var file = new File(GuidGenerator.Create(), CurrentTenant.Id, parent, fileContainerName, fileName, mimeType,
                fileType, 0, fileContent?.LongLength ?? 0, hashString, blobName, ownerUserId);

            return file;
        }

        public virtual async Task<File> ChangeAsync(File file, string newFileName, File oldParent, File newParent)
        {
            Check.NotNullOrWhiteSpace(newFileName, nameof(File.FileName));

            if (file.ParentId != oldParent?.Id)
            {
                throw new IncorrectParentException(oldParent);
            }

            var configuration = _configurationProvider.Get(file.FileContainerName);

            CheckFileName(newFileName, configuration);

            if (newFileName != file.FileName || newParent?.Id != file.ParentId)
            {
                await CheckFileNotExistAsync(newFileName, newParent?.Id, file.FileContainerName, file.OwnerUserId);
            }

            if (oldParent != newParent)
            {
                await CheckNotMovingDirectoryToSubDirectoryAsync(file, newParent);
            }

            file.UpdateInfo(newFileName, file.MimeType, file.SubFilesQuantity, file.ByteSize, file.Hash, file.BlobName,
                oldParent, newParent);
            
            return file;
        }

        public virtual async Task<File> ChangeAsync(File file, string newFileName, string newMimeType, byte[] newFileContent, File oldParent, File newParent)
        {
            Check.NotNullOrWhiteSpace(newFileName, nameof(File.FileName));

            if (file.ParentId != oldParent?.Id)
            {
                throw new IncorrectParentException(oldParent);
            }
            
            var configuration = _configurationProvider.Get(file.FileContainerName);

            CheckFileName(newFileName, configuration);
            CheckDirectoryHasNoFileContent(file.FileType, newFileContent);
            
            if (newFileName != file.FileName || newParent?.Id != file.ParentId)
            {
                await CheckFileNotExistAsync(newFileName, newParent?.Id, file.FileContainerName, file.OwnerUserId);
            }

            if (oldParent != newParent)
            {
                await CheckNotMovingDirectoryToSubDirectoryAsync(file, newParent);
            }
            
            var oldBlobName = file.BlobName;

            var blobName = await _fileBlobNameGenerator.CreateAsync(file.FileType, newFileName, newParent, newMimeType,
                configuration.AbpBlobDirectorySeparator);

            _unitOfWorkManager.Current.OnCompleted(async () =>
                await _localEventBus.PublishAsync(new FileBlobNameChangedEto
                {
                    FileId = file.Id,
                    OldBlobName = oldBlobName,
                    NewBlobName = blobName
                }));

            var hashString = _fileContentHashProvider.GetHashString(newFileContent);

            file.UpdateInfo(newFileName, newMimeType, file.SubFilesQuantity, newFileContent?.LongLength ?? 0,
                hashString, blobName, oldParent, newParent);

            return file;
        }
        
        protected virtual async Task CheckNotMovingDirectoryToSubDirectoryAsync([NotNull] File file, [CanBeNull] File targetParent)
        {
            if (file.FileType != FileType.Directory)
            {
                return;
            }

            var parent = targetParent;
            
            while (parent != null)
            {
                if (parent.Id == file.Id)
                {
                    throw new FileIsMovedToSubDirectoryException();
                }

                parent = parent.ParentId.HasValue ? await _fileRepository.GetAsync(parent.ParentId.Value) : null;
            }
        }

        public virtual async Task DeleteAsync([NotNull] File file, CancellationToken cancellationToken = default)
        {
            var parent = file.ParentId.HasValue
                ? await _fileRepository.GetAsync(file.ParentId.Value, true, cancellationToken)
                : null;

            parent?.TryAddSubFileUpdatedDomainEvent();
            
            await _fileRepository.DeleteAsync(file, true, cancellationToken);
            
            if (file.FileType == FileType.Directory)
            {
                await DeleteSubFilesAsync(file, file.FileContainerName, file.OwnerUserId, cancellationToken);
            }
        }

        protected virtual async Task DeleteSubFilesAsync([CanBeNull] File file, [NotNull] string fileContainerName,
            Guid? ownerUserId, CancellationToken cancellationToken = default)
        {
            var subFiles = await _fileRepository.GetListAsync(file?.Id, fileContainerName, ownerUserId,
                null, cancellationToken);

            foreach (var subFile in subFiles)
            {
                if (subFile.FileType == FileType.Directory)
                {
                    await DeleteSubFilesAsync(subFile, fileContainerName, ownerUserId, cancellationToken);
                }
                
                await _fileRepository.DeleteAsync(subFile, true, cancellationToken);
            }
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
            if (fileType == FileType.Directory && !fileContent.IsNullOrEmpty())
            {
                throw new DirectoryFileContentIsNotEmptyException();
            }
        }

        public virtual async Task<bool> TrySaveBlobAsync(File file, byte[] fileContent, bool overrideExisting = false, CancellationToken cancellationToken = default)
        {
            if (file.FileType != FileType.RegularFile)
            {
                return false;
            }
            
            var blobContainer = GetBlobContainer(file);

            if (!overrideExisting && await blobContainer.ExistsAsync(file.BlobName, cancellationToken))
            {
                return false;
            }

            await blobContainer.SaveAsync(file.BlobName, fileContent, overrideExisting, cancellationToken: cancellationToken);

            return true;
        }

        public virtual async Task<byte[]> GetBlobAsync(File file, CancellationToken cancellationToken = default)
        {
            if (file.FileType != FileType.RegularFile)
            {
                throw new UnexpectedFileTypeException(file.Id, file.FileType, FileType.RegularFile);
            }

            var blobContainer = GetBlobContainer(file);

            return await blobContainer.GetAllBytesAsync(file.BlobName, cancellationToken: cancellationToken);
        }

        public virtual IBlobContainer GetBlobContainer(File file)
        {
            var configuration = _configurationProvider.Get(file.FileContainerName);
            
            return _blobContainerFactory.Create(configuration.AbpBlobContainerName);
        }

        public async Task DeleteBlobAsync(File file, CancellationToken cancellationToken = default)
        {
            var blobContainer = GetBlobContainer(file);

            await blobContainer.DeleteAsync(file.BlobName, cancellationToken);
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

        protected virtual IFileDownloadProvider GetFileDownloadProvider(File file)
        {
            var options = ServiceProvider.GetRequiredService<IOptions<FileManagementOptions>>().Value;
            
            var configuration = options.Containers.GetConfiguration(file.FileContainerName);

            var specifiedProviderType = configuration.SpecifiedFileDownloadProviderType;

            var providers = ServiceProvider.GetServices<IFileDownloadProvider>();

            return specifiedProviderType == null
                ? providers.Single(p => p.GetType() == options.DefaultFileDownloadProviderType)
                : providers.Single(p => p.GetType() == specifiedProviderType);
        }
        
        protected virtual string GetDownloadLimitCacheItemKey()
        {
            return _currentUser.GetId().ToString();
        }

        protected virtual async Task CheckFileNotExistAsync(string fileName, Guid? parentId, string fileContainerName, Guid? ownerUserId)
        {
            if (await _fileRepository.FindAsync(fileName, parentId, fileContainerName, ownerUserId) != null)
            {
                throw new FileAlreadyExistsException(fileName, parentId);
            }
        }
    }
}