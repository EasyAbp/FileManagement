using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Options;
using EasyAbp.FileManagement.Options.Containers;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Uow;

namespace EasyAbp.FileManagement.Files
{
    public class FileManager : FileManagerBase, IFileManager
    {
        protected IDistributedEventBus DistributedEventBus =>
            LazyServiceProvider.LazyGetRequiredService<IDistributedEventBus>();

        protected IUnitOfWorkManager UnitOfWorkManager =>
            LazyServiceProvider.LazyGetRequiredService<IUnitOfWorkManager>();

        protected IFileBlobManager FileBlobManager => LazyServiceProvider.LazyGetRequiredService<IFileBlobManager>();

        protected IFileBlobNameGenerator FileBlobNameGenerator =>
            LazyServiceProvider.LazyGetRequiredService<IFileBlobNameGenerator>();

        protected IFileContentHashProvider FileContentHashProvider =>
            LazyServiceProvider.LazyGetRequiredService<IFileContentHashProvider>();

        protected IFileContainerConfigurationProvider ConfigurationProvider =>
            LazyServiceProvider.LazyGetRequiredService<IFileContainerConfigurationProvider>();

        [UnitOfWork(true)]
        public override async Task<File> CreateAsync(CreateFileModel model,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(model.FileContainerName, nameof(File.FileContainerName));

            var configuration = ConfigurationProvider.Get<FileContainerConfiguration>(model.FileContainerName);

            if (model.FileType == FileType.RegularFile)
            {
                CheckFileSize(new Dictionary<string, long> { { model.FileName, model.FileContent.LongLength } },
                    configuration);
                CheckFileExtension(new[] { model.FileName }, configuration);
            }

            var file = await CreateFileEntityAsync(model, configuration, cancellationToken);

            await FileRepository.InsertAsync(file, true, cancellationToken);

            await TrySaveBlobAsync(file, model.FileContent, configuration.DisableBlobReuse,
                configuration.AllowBlobOverriding);

            return file;
        }

        protected virtual async Task<File> CreateFileEntityAsync(CreateFileModel model,
            FileContainerConfiguration configuration, CancellationToken cancellationToken)
        {
            CheckFileName(model.FileName, configuration);
            CheckDirectoryHasNoFileContent(model.FileType, model.FileContent);

            var hashString = FileContentHashProvider.GetHashString(model.FileContent);

            var parent = await TryGetFileByNullableIdAsync(model.ParentId);

            string blobName = null;

            if (model.FileType == FileType.RegularFile)
            {
                if (!configuration.DisableBlobReuse)
                {
                    var existingFile =
                        await FileRepository.FirstOrDefaultAsync(model.FileContainerName, hashString,
                            model.FileContent.LongLength, cancellationToken);

                    // Todo: should lock the file that provides a reused BLOB.
                    if (existingFile != null)
                    {
                        Check.NotNullOrWhiteSpace(existingFile.BlobName, nameof(existingFile.BlobName));

                        blobName = existingFile.BlobName;
                    }
                }

                blobName ??= await FileBlobNameGenerator.CreateAsync(model.FileType, model.FileName, parent,
                    model.MimeType, configuration.AbpBlobDirectorySeparator);
            }

            if (configuration.EnableAutoRename)
            {
                if (await IsFileExistAsync(model.FileName, model.ParentId, model.FileContainerName,
                        model.OwnerUserId))
                {
                    model.FileName = await FileRepository.GetFileNameWithNextSerialNumberAsync(model.FileName,
                        model.ParentId, model.FileContainerName, model.OwnerUserId, cancellationToken);
                }
            }

            await CheckFileNotExistAsync(model.FileName, model.ParentId, model.FileContainerName, model.OwnerUserId);

            var file = new File(GuidGenerator.Create(), CurrentTenant.Id, parent, model.FileContainerName,
                model.FileName, model.MimeType, model.FileType, 0, model.FileContent?.LongLength ?? 0, hashString,
                blobName, model.OwnerUserId);

            model.MapExtraPropertiesTo(file, MappingPropertyDefinitionChecks.Destination);

            if (parent is not null)
            {
                await HandleStatisticDataUpdateAsync(parent.Id);
            }

            return file;
        }

        public override async Task<File> CreateAsync(CreateFileWithStreamModel model,
            CancellationToken cancellationToken = default)
        {
            return await CreateAsync(await model.ToCreateFileModelAsync(cancellationToken), cancellationToken);
        }

        public override async Task<List<File>> CreateManyAsync(List<CreateFileModel> models,
            CancellationToken cancellationToken = default)
        {
            var files = new List<File>();

            if (models.Select(x => x.FileContainerName).Distinct().Count() > 1)
            {
                throw new FileContainerConflictException();
            }

            var configuration = ConfigurationProvider.Get<FileContainerConfiguration>(models.First().FileContainerName);

            CheckFileQuantity(models.Count, configuration);
            CheckFileSize(models.ToDictionary(x => x.FileName, x => x.FileContent.LongLength),
                configuration);

            foreach (var model in models)
            {
                Check.NotNullOrWhiteSpace(model.FileContainerName, nameof(File.FileContainerName));

                if (model.FileType == FileType.RegularFile)
                {
                    CheckFileExtension(new[] { model.FileName }, configuration);
                }

                var file = await CreateFileEntityAsync(model, configuration, cancellationToken);

                await FileRepository.InsertAsync(file, true, cancellationToken);

                files.Add(file);
            }

            for (var i = 0; i < models.Count; i++)
            {
                await TrySaveBlobAsync(files[i], models[i].FileContent, configuration.DisableBlobReuse,
                    configuration.AllowBlobOverriding);
            }

            return files;
        }

        public override async Task<List<File>> CreateManyAsync(List<CreateFileWithStreamModel> models,
            CancellationToken cancellationToken = default)
        {
            var targetModels = new List<CreateFileModel>();

            foreach (var model in models)
            {
                targetModels.Add(await model.ToCreateFileModelAsync(cancellationToken));
            }

            return await CreateManyAsync(targetModels, cancellationToken);
        }

        [UnitOfWork(true)]
        public override async Task<File> UpdateAsync(File file, string newFileName, File oldParent, File newParent,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(newFileName, nameof(File.FileName));

            if (file.ParentId != oldParent?.Id)
            {
                throw new IncorrectParentException(oldParent);
            }

            var configuration = ConfigurationProvider.Get<FileContainerConfiguration>(file.FileContainerName);

            CheckFileExtension(new[] { newFileName }, configuration);
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

            if (oldParent is not null)
            {
                await HandleStatisticDataUpdateAsync(oldParent.Id);
            }

            if (newParent is not null)
            {
                await HandleStatisticDataUpdateAsync(newParent.Id);
            }

            await FileRepository.UpdateAsync(file, true, cancellationToken);

            return file;
        }

        [UnitOfWork(true)]
        public override async Task<File> UpdateAsync(File file, UpdateFileModel model,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(model.NewFileName, nameof(File.FileName));

            var oldParent = await TryGetFileByNullableIdAsync(file.ParentId);

            var newParent = model.NewParentId == file.ParentId
                ? oldParent
                : await TryGetFileByNullableIdAsync(model.NewParentId);

            if (file.ParentId != oldParent?.Id)
            {
                throw new IncorrectParentException(oldParent);
            }

            var configuration = ConfigurationProvider.Get<FileContainerConfiguration>(file.FileContainerName);

            CheckFileName(model.NewFileName, configuration);
            CheckDirectoryHasNoFileContent(file.FileType, model.NewFileContent);
            CheckFileSize(new Dictionary<string, long> { { model.NewFileName, model.NewFileContent.LongLength } },
                configuration);
            CheckFileExtension(new[] { model.NewFileName }, configuration);

            if (model.NewFileName != file.FileName || newParent?.Id != file.ParentId)
            {
                await CheckFileNotExistAsync(model.NewFileName, newParent?.Id, file.FileContainerName,
                    file.OwnerUserId);
            }

            if (oldParent != newParent)
            {
                await CheckNotMovingDirectoryToSubDirectoryAsync(file, newParent);
            }

            var oldBlobName = file.BlobName;

            var blobName = await FileBlobNameGenerator.CreateAsync(file.FileType, model.NewFileName, newParent,
                model.NewMimeType, configuration.AbpBlobDirectorySeparator);

            await DistributedEventBus.PublishAsync(new FileBlobNameChangedEto
            {
                TenantId = file.TenantId,
                FileId = file.Id,
                FileType = file.FileType,
                FileContainerName = file.FileContainerName,
                OldBlobName = oldBlobName,
                NewBlobName = blobName
            });

            var hashString = FileContentHashProvider.GetHashString(model.NewFileContent);

            file.UpdateInfo(model.NewFileName, model.NewMimeType, file.SubFilesQuantity,
                model.NewFileContent?.LongLength ?? 0,
                hashString, blobName, oldParent, newParent);

            model.MapExtraPropertiesTo(file, MappingPropertyDefinitionChecks.Destination);

            if (oldParent is not null)
            {
                await HandleStatisticDataUpdateAsync(oldParent.Id);
            }

            if (newParent is not null)
            {
                await HandleStatisticDataUpdateAsync(newParent.Id);
            }

            await FileRepository.UpdateAsync(file, true, cancellationToken);

            await TrySaveBlobAsync(file, model.NewFileContent, configuration.DisableBlobReuse,
                configuration.AllowBlobOverriding);

            return file;
        }

        public override async Task<File> UpdateAsync(File file, UpdateFileWithStreamModel model,
            CancellationToken cancellationToken = default)
        {
            return await UpdateAsync(file, await model.ToChangeFileModelAsync(cancellationToken), cancellationToken);
        }

        [UnitOfWork(true)]
        public override async Task DeleteAsync([NotNull] File file, CancellationToken cancellationToken = default)
        {
            var parent = file.ParentId.HasValue
                ? await FileRepository.GetAsync(file.ParentId.Value, true, cancellationToken)
                : null;

            if (parent is not null)
            {
                await HandleStatisticDataUpdateAsync(parent.Id);
            }

            await FileRepository.DeleteAsync(file, true, cancellationToken);

            if (file.FileType == FileType.Directory)
            {
                await DeleteSubFilesAsync(file, file.FileContainerName, file.OwnerUserId, cancellationToken);
            }
        }

        protected virtual async Task DeleteSubFilesAsync([CanBeNull] File file, [NotNull] string fileContainerName,
            Guid? ownerUserId, CancellationToken cancellationToken = default)
        {
            var subFiles = await FileRepository.GetListAsync(file?.Id, fileContainerName, ownerUserId,
                null, cancellationToken);

            foreach (var subFile in subFiles)
            {
                if (subFile.FileType == FileType.Directory)
                {
                    await DeleteSubFilesAsync(subFile, fileContainerName, ownerUserId, cancellationToken);
                }

                await FileRepository.DeleteAsync(subFile, true, cancellationToken);
            }
        }

        protected override IFileDownloadProvider GetFileDownloadProvider(File file)
        {
            var options = LazyServiceProvider.LazyGetRequiredService<IOptions<FileManagementOptions>>().Value;

            var configuration = options.Containers.GetConfiguration(file.FileContainerName);

            var specifiedProviderType = configuration.SpecifiedFileDownloadProviderType;

            var providers = LazyServiceProvider.LazyGetService<IEnumerable<IFileDownloadProvider>>();

            return specifiedProviderType == null
                ? providers.Single(p => p.GetType() == options.DefaultFileDownloadProviderType)
                : providers.Single(p => p.GetType() == specifiedProviderType);
        }

        [UnitOfWork(true)]
        protected virtual Task HandleStatisticDataUpdateAsync(Guid directoryId)
        {
            var useBackgroundJob = DistributedEventBus is LocalDistributedEventBus;

            UnitOfWorkManager.Current.AddOrReplaceDistributedEvent(
                new UnitOfWorkEventRecord(
                    typeof(SubFilesChangedEto),
                    new SubFilesChangedEto(CurrentTenant.Id, directoryId, useBackgroundJob),
                    default));

            return Task.CompletedTask;
        }

        protected virtual async Task<bool> TrySaveBlobAsync(File file, byte[] fileContent,
            bool disableBlobReuse = false, bool allowBlobOverriding = false)
        {
            if (file.FileType is not FileType.RegularFile)
            {
                return false;
            }

            await FileBlobManager.TrySaveAsync(file, fileContent, disableBlobReuse, allowBlobOverriding);

            return true;
        }
    }
}