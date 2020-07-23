using System;
using System.Linq;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Containers;
using Volo.Abp;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Services;

namespace EasyAbp.FileManagement.Files
{
    public class FileManager : DomainService, IFileManager
    {
        private readonly IBlobContainerFactory _blobContainerFactory;
        private readonly IFileRepository _fileRepository;
        private readonly IFileBlobNameGenerator _fileBlobNameGenerator;
        private readonly IFileContentHashProvider _fileContentHashProvider;
        private readonly IFileContainerConfigurationProvider _configurationProvider;

        public FileManager(
            IBlobContainerFactory blobContainerFactory,
            IFileRepository fileRepository,
            IFileBlobNameGenerator fileBlobNameGenerator,
            IFileContentHashProvider fileContentHashProvider,
            IFileContainerConfigurationProvider configurationProvider)
        {
            _blobContainerFactory = blobContainerFactory;
            _fileRepository = fileRepository;
            _fileBlobNameGenerator = fileBlobNameGenerator;
            _fileContentHashProvider = fileContentHashProvider;
            _configurationProvider = configurationProvider;
        }

        public virtual async Task<File> CreateAsync(string fileContainerName, string fileName, string mimeType,
            FileType fileType, Guid? parentId, byte[] fileContent)
        {
            Check.NotNullOrWhiteSpace(fileContainerName, nameof(File.FileContainerName));
            Check.NotNullOrWhiteSpace(fileName, nameof(File.FileName));

            var configuration = _configurationProvider.Get(fileContainerName);
            
            CheckFileName(fileName, configuration);
            CheckDirectoryHasNoFileContent(fileType, fileContent);

            var filePath = await GetFilePathAsync(parentId, fileContainerName, fileName);

            var blobName = await _fileBlobNameGenerator.CreateAsync(fileType, fileName, filePath, mimeType,
                configuration.AbpBlobDirectorySeparator);

            await CheckFileNotExistAsync(filePath);

            var hashString = _fileContentHashProvider.GetHashString(fileContent);

            var file = new File(GuidGenerator.Create(), CurrentTenant.Id, fileContainerName, fileName, filePath,
                mimeType, fileType, 0, fileContent.LongLength, hashString, blobName, parentId, fileName);

            return file;
        }

        public virtual async Task<File> UpdateAsync(File file, string newFileName)
        {
            Check.NotNullOrWhiteSpace(newFileName, nameof(File.FileName));

            var configuration = _configurationProvider.Get(file.FileContainerName);

            CheckFileName(newFileName, configuration);
            
            var filePath = await GetFilePathAsync(file.ParentId, file.FileContainerName, newFileName);

            await CheckFileNotExistAsync(filePath);

            file.UpdateInfo(newFileName, filePath, file.MimeType, file.SubFilesQuantity, file.ByteSize, file.Hash,
                file.BlobName, file.ParentId, newFileName);
            
            return file;
        }

        public virtual async Task<File> UpdateAsync(File file, string newFileName, string newMimeType, byte[] newFileContent)
        {
            Check.NotNullOrWhiteSpace(newFileName, nameof(File.FileName));

            var configuration = _configurationProvider.Get(file.FileContainerName);

            CheckFileName(newFileName, configuration);
            CheckDirectoryHasNoFileContent(file.FileType, newFileContent);
            
            var filePath = await GetFilePathAsync(file.ParentId, file.FileContainerName, newFileName);

            var blobName = await _fileBlobNameGenerator.CreateAsync(file.FileType, newFileName, filePath, newMimeType,
                configuration.AbpBlobDirectorySeparator);

            await CheckFileNotExistAsync(filePath);

            var hashString = _fileContentHashProvider.GetHashString(newFileContent);

            file.UpdateInfo(newFileName, filePath, newMimeType, file.SubFilesQuantity, file.ByteSize, hashString,
                blobName, file.ParentId, newFileName);
            
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

        protected virtual async Task CheckFileNotExistAsync(string filePath)
        {
            if (await _fileRepository.FindByFilePathAsync(filePath) != null)
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