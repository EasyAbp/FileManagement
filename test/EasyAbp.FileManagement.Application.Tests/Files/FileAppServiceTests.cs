using System;
using System.Collections.Generic;
using System.IO;
using Shouldly;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Files.Dtos;
using EasyAbp.FileManagement.Options.Containers;
using Volo.Abp.BlobStoring;
using Volo.Abp.Content;
using Xunit;

namespace EasyAbp.FileManagement.Files
{
    public class FileAppServiceTests : FileManagementApplicationTestBase
    {
        private readonly IFileRepository _fileRepository;
        private readonly IFileAppService _fileAppService;
        private readonly IFileContentHashProvider _hashProvider;
        private readonly IBlobContainer<LocalFileSystemBlobContainer> _blobContainer;

        public FileAppServiceTests()
        {
            _fileRepository = GetRequiredService<IFileRepository>();
            _fileAppService = GetRequiredService<IFileAppService>();
            _hashProvider = GetRequiredService<IFileContentHashProvider>();
            _blobContainer = GetRequiredService<IBlobContainer<LocalFileSystemBlobContainer>>();
        }

        [Fact]
        public async Task Should_Create_A_File_With_Stream()
        {
            const string contentType = "text/plain";
            const string fileName = "1-1.txt";
            const string text = "Text content 1-1.";
            
            var fileContainerName = FileContainerNameAttribute.GetContainerName<TestFileContainer>();
            var bytes = text.GetBytes();
            
            var resultDto = await CreateFileWithStreamAsync(bytes, fileName, contentType, fileContainerName);

            await CheckFileIsCreatedAsync(resultDto, fileName, bytes);
        }

        [Fact]
        public async Task Should_Create_Many_Files_With_Stream()
        {
            const string contentType = "text/plain";
            const string fileName1 = "2-1.txt";
            const string text1 = "Text content 2-1.";
            const string fileName2 = "2-2.txt";
            const string text2 = "Text content 2-2.";

            var bytes1 = text1.GetBytes();
            var bytes2 = text2.GetBytes();
            
            await using var ms1 = new MemoryStream(bytes1);
            await using var ms2 = new MemoryStream(bytes2);

            var resultDto = await _fileAppService.CreateManyWithStreamAsync(new CreateManyFileWithStreamInput
            {
                FileContainerName = FileContainerNameAttribute.GetContainerName<TestFileContainer>(),
                ParentId = null,
                FileContents = new List<IRemoteStreamContent>
                {
                    new RemoteStreamContent(ms1)
                    {
                        FileName = fileName1,
                        ContentType = contentType
                    },
                    new RemoteStreamContent(ms2)
                    {
                        FileName = fileName2,
                        ContentType = contentType
                    }
                }
            });

            resultDto.ShouldNotBeNull();
            resultDto.Items.Count.ShouldBe(2);
            
            await CheckFileIsCreatedAsync(resultDto.Items[0], fileName1, bytes1);
            await CheckFileIsCreatedAsync(resultDto.Items[1], fileName2, bytes2);
        }
        
        [Fact]
        public async Task Should_Create_A_File_With_Bytes()
        {
            const string contentType = "text/plain";
            const string fileName = "3-1.txt";
            const string text = "Text content 3-1.";
            
            var fileContainerName = FileContainerNameAttribute.GetContainerName<TestFileContainer>();
            var bytes = text.GetBytes();
            
            await using var ms = new MemoryStream(bytes);

            var resultDto = await _fileAppService.CreateAsync(new CreateFileInput
            {
                FileContainerName = fileContainerName,
                ParentId = null,
                FileType = FileType.RegularFile,
                MimeType = contentType,
                FileName = fileName,
                Content = bytes,
            });

            await CheckFileIsCreatedAsync(resultDto, fileName, bytes);
        }
        
        [Fact]
        public async Task Should_Create_Many_Files_With_Bytes()
        {
            const string contentType = "text/plain";
            const string fileName1 = "4-1.txt";
            const string text1 = "Text content 4-1.";
            const string fileName2 = "4-2.txt";
            const string text2 = "Text content 4-2.";
            
            var fileContainerName = FileContainerNameAttribute.GetContainerName<TestFileContainer>();
            var bytes1 = text1.GetBytes();
            var bytes2 = text2.GetBytes();
            
            await using var ms1 = new MemoryStream(bytes1);
            await using var ms2 = new MemoryStream(bytes2);

            var resultDto = await _fileAppService.CreateManyAsync(new CreateManyFileInput
            {
                FileInfos = new List<CreateFileInput>
                {
                    new()
                    {
                        FileContainerName = fileContainerName,
                        ParentId = null,
                        FileType = FileType.RegularFile,
                        MimeType = contentType,
                        FileName = fileName1,
                        Content = bytes1,
                    },
                    new()
                    {
                        FileContainerName = fileContainerName,
                        ParentId = null,
                        FileType = FileType.RegularFile,
                        MimeType = contentType,
                        FileName = fileName2,
                        Content = bytes2,
                    }
                }
            });

            resultDto.ShouldNotBeNull();
            resultDto.Items.Count.ShouldBe(2);
            
            await CheckFileIsCreatedAsync(resultDto.Items[0], fileName1, bytes1);
            await CheckFileIsCreatedAsync(resultDto.Items[1], fileName2, bytes2);
        }
        
        [Fact]
        public async Task Should_Reuse_A_Blob()
        {
            const string contentType = "text/plain";
            const string fileName1 = "5-1.txt";
            const string fileName2 = "5-2.txt";
            const string text = "Text content for file.";

            var fileContainerName = FileContainerNameAttribute.GetContainerName<TestBlobReuseFileContainer>();
            var bytes = text.GetBytes();
            
            await CreateFileWithStreamAsync(bytes, fileName1, contentType, fileContainerName);
            await CreateFileWithStreamAsync(bytes, fileName2, contentType, fileContainerName);

            var files = await _fileRepository.GetListAsync();

            files.Count.ShouldBe(2);
            files[0].BlobName.ShouldEndWith(fileName1);
            files[1].BlobName.ShouldEndWith(fileName1);
            files[1].BlobName.ShouldNotEndWith(fileName2);
        }

        [Fact]
        public async Task Should_Not_Reuse_A_Blob()
        {
            const string contentType = "text/plain";
            const string fileName1 = "6-1.txt";
            const string fileName2 = "6-2.txt";
            const string text = "Text content for file.";

            var fileContainerName = FileContainerNameAttribute.GetContainerName<TestFileContainer>();
            var bytes = text.GetBytes();
            
            await CreateFileWithStreamAsync(bytes, fileName1, contentType, fileContainerName);
            await CreateFileWithStreamAsync(bytes, fileName2, contentType, fileContainerName);

            var files = await _fileRepository.GetListAsync();

            files.Count.ShouldBe(2);
            files[0].BlobName.ShouldEndWith(fileName1);
            files[1].BlobName.ShouldEndWith(fileName2);
        }

        private async Task<CreateFileOutput> CreateFileWithStreamAsync(byte[] bytes, string fileName,
            string contentType, string fileContainerName)
        {
            await using var ms = new MemoryStream(bytes);

            return await _fileAppService.CreateWithStreamAsync(new CreateFileWithStreamInput
            {
                FileContainerName = fileContainerName,
                ParentId = null,
                Content = new RemoteStreamContent(ms)
                {
                    FileName = fileName,
                    ContentType = contentType
                }
            });
        }

        private async Task CheckFileIsCreatedAsync(CreateFileOutput resultDto, string fileName, byte[] bytes)
        {
            resultDto.ShouldNotBeNull();

            var file = await _fileRepository.FindAsync(resultDto.FileInfo.Id);

            file.ShouldNotBeNull();

            var hashString = _hashProvider.GetHashString(bytes);

            var blobBytes = await _blobContainer.GetAllBytesOrNullAsync(file.BlobName);
            
            blobBytes.ShouldBeEquivalentTo(bytes);
            
            resultDto.FileInfo.FileName.ShouldBe(fileName);
            resultDto.FileInfo.Hash.ShouldBe(hashString);
            resultDto.FileInfo.ByteSize.ShouldBe(bytes.Length);
            
            file.FileName.ShouldBe(fileName);
            file.Hash.ShouldBe(hashString);
            file.ByteSize.ShouldBe(bytes.Length);
        }
    }
}
