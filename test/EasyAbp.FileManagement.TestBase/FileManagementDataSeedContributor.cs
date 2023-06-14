using System.Text;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Files;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;

namespace EasyAbp.FileManagement
{
    public class FileManagementDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IGuidGenerator _guidGenerator;
        private readonly IFileManager _fileManager;
        private readonly IFileRepository _fileRepository;

        public FileManagementDataSeedContributor(
            IGuidGenerator guidGenerator,
            IFileManager fileManager,
            IFileRepository fileRepository)
        {
            _guidGenerator = guidGenerator;
            _fileManager = fileManager;
            _fileRepository = fileRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            /* Instead of returning the Task.CompletedTask, you can insert your test data
             * at this point!
             */

            var dir1 = await _fileManager.CreateAsync("test", null, "dir1", null, FileType.Directory, null, null);
            var dir11 = await _fileManager.CreateAsync("test", null, "dir11", null, FileType.Directory, dir1, null);
            var dir12 = await _fileManager.CreateAsync("test", null, "dir12", null, FileType.Directory, dir1, null);
            var dir2 = await _fileManager.CreateAsync("test", null, "dir2", null, FileType.Directory, null, null);
            var dir21 = await _fileManager.CreateAsync("test", null, "dir21", null, FileType.Directory, dir2, null);
            var dir22 = await _fileManager.CreateAsync("test", null, "dir22", null, FileType.Directory, dir2, null);

            await _fileRepository.InsertAsync(dir1, true);
            await _fileRepository.InsertAsync(dir11, true);
            await _fileRepository.InsertAsync(dir12, true);
            await _fileRepository.InsertAsync(dir2, true);
            await _fileRepository.InsertAsync(dir21, true);
            await _fileRepository.InsertAsync(dir22, true);

            var file1 = await _fileManager.CreateAsync("test", null, "file1", null, FileType.RegularFile, dir11,
                "content"u8.ToArray());
            var file2 = await _fileManager.CreateAsync("test", null, "file1", null, FileType.RegularFile, dir12,
                "content1"u8.ToArray());

            await _fileRepository.InsertAsync(file1, true);
            await _fileRepository.InsertAsync(file2, true);
        }
    }
}