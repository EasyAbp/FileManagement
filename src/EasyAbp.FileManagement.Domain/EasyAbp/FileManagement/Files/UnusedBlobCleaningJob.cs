using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace EasyAbp.FileManagement.Files
{
    public class UnusedBlobCleaningJob : IAsyncBackgroundJob<UnusedBlobCleaningArgs>, ITransientDependency
    {
        private readonly IFileManager _fileManager;
        private readonly IFileRepository _fileRepository;
        private readonly ICurrentTenant _currentTenant;

        public UnusedBlobCleaningJob(
            IFileManager fileManager,
            IFileRepository fileRepository,
            ICurrentTenant currentTenant)
        {
            _fileManager = fileManager;
            _fileRepository = fileRepository;
            _currentTenant = currentTenant;
        }

        [UnitOfWork]
        public async Task ExecuteAsync(UnusedBlobCleaningArgs args)
        {
            using var changeTenant = _currentTenant.Change(args.TenantId);

            await DeleteBlobIfNotUsedAsync(args.FileContainerName, args.BlobName);
        }
        
        protected virtual async Task DeleteBlobIfNotUsedAsync(string fileContainerName, string blobName)
        {
            if (await _fileRepository.FirstOrDefaultAsync(fileContainerName, blobName) == null)
            {
                await _fileManager.DeleteBlobAsync(fileContainerName, blobName);
            }
        }
    }
}