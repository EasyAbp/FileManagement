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
using Volo.Abp.ObjectExtending;
using Volo.Abp.Uow;

namespace EasyAbp.FileManagement.Files
{
    public class FileManager : FileManagerBase, IFileManager
    {
        protected IFileBlobManager FileBlobManager => LazyServiceProvider.LazyGetRequiredService<IFileBlobManager>();

        protected IFileBlobNameGenerator FileBlobNameGenerator =>
            LazyServiceProvider.LazyGetRequiredService<IFileBlobNameGenerator>();

        protected IFileContentHashProvider FileContentHashProvider =>
            LazyServiceProvider.LazyGetRequiredService<IFileContentHashProvider>();


        [UnitOfWork(true)]
        public override async Task<File> CreateAsync(CreateFileModel model,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(model.FileContainerName, nameof(File.FileContainerName));

            var configuration =
                FileContainerConfigurationProvider.Get<FileContainerConfiguration>(model.FileContainerName);

            if (model.FileType == FileType.RegularFile)
            {
                CheckFileSize(new Dictionary<string, long> { { model.FileName, model.GetContentLength() } },
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
            CheckDirectoryHasNoFileContent(model.FileType, model.GetContentLength());

            var hashString = FileContentHashProvider.GetHashString(model.FileContent);

            string blobName = null;

            if (model.FileType == FileType.RegularFile)
            {
                if (!configuration.DisableBlobReuse)
                {
                    var existingFile =
                        await FileRepository.FirstOrDefaultAsync(model.FileContainerName, hashString,
                            model.GetContentLength(), cancellationToken);

                    // Todo: should lock the file that provides a reused BLOB.
                    if (existingFile != null)
                    {
                        Check.NotNullOrWhiteSpace(existingFile.BlobName, nameof(existingFile.BlobName));

                        blobName = existingFile.BlobName;
                    }
                }

                blobName ??= await FileBlobNameGenerator.CreateAsync(model.FileType, model.FileName, model.Parent,
                    model.MimeType, configuration.AbpBlobDirectorySeparator);
            }

            if (configuration.EnableAutoRename)
            {
                if (await IsFileExistAsync(model.FileName, model.Parent?.Id, model.FileContainerName,
                        model.OwnerUserId))
                {
                    model.FileName = await FileRepository.GetFileNameWithNextSerialNumberAsync(model.FileName,
                        model.Parent?.Id, model.FileContainerName, model.OwnerUserId, cancellationToken);
                }
            }

            await CheckFileNotExistAsync(model.FileName, model.Parent?.Id, model.FileContainerName, model.OwnerUserId);

            var file = new File(GuidGenerator.Create(), CurrentTenant.Id, model.Parent, model.FileContainerName,
                model.FileName, model.MimeType, model.FileType, 0, model.GetContentLength(), hashString,
                blobName, model.OwnerUserId);

            model.MapExtraPropertiesTo(file, MappingPropertyDefinitionChecks.Destination);

            if (model.Parent is not null)
            {
                await HandleStatisticDataUpdateAsync(model.Parent.Id);
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

            var configuration =
                FileContainerConfigurationProvider.Get<FileContainerConfiguration>(models.First().FileContainerName);

            CheckFileQuantity(models.Count, configuration);
            CheckFileSize(models.ToDictionary(x => x.FileName, x => x.GetContentLength()),
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