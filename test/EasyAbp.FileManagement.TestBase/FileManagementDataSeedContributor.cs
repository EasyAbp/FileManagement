using System.Threading.Tasks;
using EasyAbp.FileManagement.Files;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace EasyAbp.FileManagement
{
    public class FileManagementDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IFileManager _fileManager;

        public FileManagementDataSeedContributor(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            /*
             * Directories:
             * dir1/        dir1/dir11      dir1/dir12
             * dir2/        dir2/dir21      dir2/dir22
             *
             * Files:
             * dir1/dir11/file1.txt         dir1/dir12/file2.txt
             */

            var dir1 = await _fileManager.CreateAsync(
                new CreateFileModel("test", null, "dir1", null, FileType.Directory, null, null));
            var dir11 = await _fileManager.CreateAsync(
                new CreateFileModel("test", null, "dir11", null, FileType.Directory, dir1, null));
            var dir12 = await _fileManager.CreateAsync(
                new CreateFileModel("test", null, "dir12", null, FileType.Directory, dir1, null));
            var dir2 = await _fileManager.CreateAsync(
                new CreateFileModel("test", null, "dir2", null, FileType.Directory, null, null));
            var dir21 = await _fileManager.CreateAsync(
                new CreateFileModel("test", null, "dir21", null, FileType.Directory, dir2, null));
            var dir22 = await _fileManager.CreateAsync(
                new CreateFileModel("test", null, "dir22", null, FileType.Directory, dir2, null));

            var file1 = await _fileManager.CreateAsync(new CreateFileModel("test", null, "file1.txt", null,
                FileType.RegularFile, dir11, "content"u8.ToArray()));
            var file2 = await _fileManager.CreateAsync(new CreateFileModel("test", null, "file2.txt", null,
                FileType.RegularFile, dir12, "content1"u8.ToArray()));
        }
    }
}