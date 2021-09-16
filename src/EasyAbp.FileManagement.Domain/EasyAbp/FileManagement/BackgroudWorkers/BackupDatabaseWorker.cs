using EasyAbp.FileManagement.Files;
using EasyAbp.FileManagement.Options.Containers;
using EasyAbp.FileManagement.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.FileSystem;
using Volo.Abp.Data;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Identity;
using Volo.Abp.Linq;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Settings;
using Volo.Abp.TenantManagement;
using Volo.Abp.Threading;
using Volo.Abp.Uow;

namespace EasyAbp.FileManagement.BackgroudWorkers
{
    public class BackupDatabaseWorker : AsyncPeriodicBackgroundWorkerBase
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ICurrentTenant _currentTenant;
        private readonly IFileManager _fileManager;
        private IFileRepository _fileRepository;
        private readonly IFileDapperRepository _fileDapperRepository;
        //private readonly AbpBlobStoringOptions _blobStoringOptions;
        private readonly AbpDbConnectionOptions _dbConnectionOptions;
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly IFileContainerConfigurationProvider _fileContainerConfigProvider;
        private readonly IBlobContainerConfigurationProvider _blobContainerConfigProvider;
        private readonly ISettingProvider _settingProvider;
        private readonly ILocalEventBus _localEventBus;

        public BackupDatabaseWorker(AbpAsyncTimer timer,
            IServiceScopeFactory serviceScopeFactory,
            ITenantRepository tenantRepository,
            ICurrentTenant currentTenant,
            IFileManager fileManager,
            IFileRepository fileRepository,
            IFileDapperRepository fileDapperRepository,
            IIdentityUserRepository identityUserRepository,
            ISettingProvider settingProvider,
            IFileContainerConfigurationProvider configurationProvider,
            IBlobContainerConfigurationProvider blobContainerConfigProvider,
            ILocalEventBus localEventBus,
            IOptions<AbpDbConnectionOptions> conOptions)
            : base(timer, serviceScopeFactory)
        {
            Timer.Period = 5 * 60 * 1000;
            _tenantRepository = tenantRepository;
            _currentTenant = currentTenant;
            _fileManager = fileManager;
            _fileRepository = fileRepository;
            _fileDapperRepository = fileDapperRepository;
            _identityUserRepository = identityUserRepository;
            _settingProvider = settingProvider;
            _fileContainerConfigProvider = configurationProvider;
            _localEventBus = localEventBus;
            _blobContainerConfigProvider = blobContainerConfigProvider;
            _dbConnectionOptions = conOptions.Value;
        }

        protected async override Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            var AutoBackup = await _settingProvider.GetAsync<bool>(FileManagementSettings.Backup.AutoBackup);
            var StartTime = await _settingProvider.GetAsync<int>(FileManagementSettings.Backup.StartTime);
            var EndTime = await _settingProvider.GetAsync<int>(FileManagementSettings.Backup.EndTime);
            if (AutoBackup && DateTime.Now.Hour >= StartTime && DateTime.Now.Hour < EndTime)
            {
                //_fileRepository = workerContext.ServiceProvider.GetRequiredService<IFileRepository>();
                var Frequency = await _settingProvider.GetAsync<double>(FileManagementSettings.Backup.Frequency);
                var MaxCount = await _settingProvider.GetAsync<int>(FileManagementSettings.Backup.MaxCount);
                var CyclicCovering = await _settingProvider.GetAsync<bool>(FileManagementSettings.Backup.CyclicCovering);
                Logger.LogInformation("BackupDatabaseWorker Starting...");
                var defaultContainerName = BlobContainerNameAttribute.GetContainerName<DefaultContainer>();
                var basePath = GetBasePath(defaultContainerName);                
                var backupDirectory = "backup";
                if (!string.IsNullOrEmpty(basePath))
                {
                    var tenants = await _tenantRepository.GetListAsync(includeDetails: true);
                    var backupedDatabaseSchemas = new HashSet<string>();
                    //backup tenant
                    foreach (var tenant in tenants)
                    {
                        using (_currentTenant.Change(tenant.Id))
                        {
                            if (tenant.ConnectionStrings.Any())
                            {
                                var tenantConnectionStrings = tenant.ConnectionStrings
                                    .Select(x => x.Value)
                                    .Distinct().ToList();

                                if (!backupedDatabaseSchemas.IsSupersetOf(tenantConnectionStrings))
                                {
                                    var ownerId = await GetOwnerId();
                                    foreach (var connectionString in tenantConnectionStrings)
                                    {
                                        if (await ShouldStartBackup(backupDirectory, defaultContainerName, ownerId, MaxCount, Frequency,CyclicCovering))
                                        {
                                            var parent = await CreateBackupDirectory(backupDirectory, defaultContainerName, ownerId);
                                            await BackupDatabase(tenant.Id, ownerId, parent, basePath, defaultContainerName, backupDirectory, connectionString);
                                            await TryDeleteBackupFiles(backupDirectory, defaultContainerName, ownerId, MaxCount);
                                        }
                                    }
                                    backupedDatabaseSchemas.AddIfNotContains(tenantConnectionStrings);
                                }
                            }
                        }
                    }

                    //backup host
                    var hostConnectionStrings = _dbConnectionOptions.ConnectionStrings.Select(x => x.Value).Distinct().ToList();
                    if (!backupedDatabaseSchemas.IsSupersetOf(hostConnectionStrings))
                    {
                        var ownerId = await GetOwnerId();
                        foreach (var hostConnectionString in hostConnectionStrings)
                        {
                            if (await ShouldStartBackup(backupDirectory, defaultContainerName, ownerId, MaxCount, Frequency, CyclicCovering))
                            {
                                var parent = await CreateBackupDirectory(backupDirectory, defaultContainerName, ownerId);
                                await BackupDatabase(null, ownerId, parent, basePath, defaultContainerName, backupDirectory, hostConnectionString);
                                await TryDeleteBackupFiles(backupDirectory, defaultContainerName, ownerId, MaxCount);
                            }
                        }
                    }
                    Logger.LogInformation("BackupDatabaseWorker Completed...");
                }
                else
                {
                    Logger.LogWarning("Could not find the Configuration of FileSystem.BasePath ...");
                }         
            }
        }
        [UnitOfWork]
        private async Task<bool> ShouldStartBackup(string backupDirectory, string defaultContainerName, Guid? ownerUserId, int maxCount, double Frequency, bool isCyclicCovering)
        {
            var directoryInfo = await _fileRepository.FindAsync(backupDirectory, null, defaultContainerName, ownerUserId);
            if (directoryInfo == null)
                return true;
            if (!isCyclicCovering && directoryInfo.SubFilesQuantity >= maxCount)
                return false;
            var lastBackupTime = (await _fileRepository.GetListAsync(o =>o.ParentId == directoryInfo.Id && o.OwnerUserId == ownerUserId && o.FileType == FileType.RegularFile))
                    .Max(o => o.CreationTime);
            var time = lastBackupTime.AddDays(Frequency);
            if (lastBackupTime.AddDays(Frequency) < DateTime.Now)
                return true;
            else
                return false;
        }
        private async Task TryDeleteBackupFiles(string backupDirectory, string defaultContainerName, Guid? ownerUserId,int maxCount)
        {
            var directoryInfo = await _fileRepository.FindAsync(backupDirectory, null, defaultContainerName, ownerUserId);
            if (directoryInfo != null && directoryInfo.SubFilesQuantity > maxCount)
            {
                var files = (await _fileRepository.GetListAsync(o => o.ParentId == directoryInfo.Id && o.OwnerUserId == ownerUserId && o.FileType == FileType.RegularFile))
                    .OrderByDescending(o => o.CreationTime).ToList();                
                var deleteFiles = files.Where(o => o.CreationTime < files[maxCount - 1].CreationTime);
                foreach(var file in deleteFiles)
                    await _fileManager.DeleteAsync(file);
            }
            await _localEventBus.PublishAsync(new SubFileUpdatedEto
            {
                Parent = directoryInfo
            });
        }

        private async Task<Guid?> GetOwnerId()
        {
            var users = await _identityUserRepository.GetPagedListAsync(0, 1, "CreationTime asc");
            if (users.Count > 0)
                return users.FirstOrDefault().Id;
            else
                return null;
        }

        private async Task<Files.File> CreateBackupDirectory(string backupDirectory,string defaultContainerName,Guid? ownerUserId)
        {
           var result = await _fileRepository.FindAsync(backupDirectory, null, defaultContainerName, ownerUserId);
            if(result == null)
            {
                var directory = _fileManager.CreateAsync(defaultContainerName, ownerUserId, backupDirectory);
                return await _fileRepository.InsertAsync(directory);
            }
            return result;
        }

        private async Task BackupDatabase(Guid? tenantId,Guid? ownerId, Files.File parent,string basePath,
            string defaultContainerName,string backupDirectory, string connectionString)
        {
            var dbTypeName = await _fileRepository.GetDataBaseTypeName();
            var databaseName = GetDatabaseName(connectionString);            
            var fileName = GetFileName(dbTypeName, databaseName);
            var fi = GetFilePath(tenantId, basePath, defaultContainerName, backupDirectory, fileName);
            await _fileDapperRepository.BackupDatabase(dbTypeName, fi.fullName, databaseName, connectionString);
            var mimiType = GetMimeMapping(fileName);
            var file = await _fileManager.CreateAsync(defaultContainerName, ownerId, parent, fi.blobName, fileName, mimiType);
            await _fileRepository.InsertAsync(file);
            Logger.LogInformation($"Successfully completed {(string.IsNullOrEmpty(_currentTenant.Name) ? "null" : _currentTenant.Name)} tenant database backup to <{fileName}>.");
        }

        private string GetBasePath(string defaultContainerName)
        {
            try
            {
                //var config = _blobStoringOptions.Containers.GetConfiguration(defaultContainerName).GetFileSystemConfiguration();
                var fsConfig = _blobContainerConfigProvider.Get(defaultContainerName).GetFileSystemConfiguration();
                if (fsConfig != null)
                {
                    return fsConfig.BasePath;
                }

            }
            catch
            {

            }
            return string.Empty;
        }
        private (string fullName,string blobName) GetFilePath(Guid? tenantId,string basePath,string defaultContainerName,string backupDir,string fileName)
        {
            var configuration = _fileContainerConfigProvider.Get(defaultContainerName);
            StringBuilder filePathBuilder = new StringBuilder(basePath);            
            if (tenantId == null)
            {
                filePathBuilder.Append(configuration.AbpBlobDirectorySeparator);
                filePathBuilder = filePathBuilder.Append("host");
            }                
            else
            {
                filePathBuilder.Append(configuration.AbpBlobDirectorySeparator);
                filePathBuilder.Append("tenants");
                filePathBuilder.Append(configuration.AbpBlobDirectorySeparator);
                filePathBuilder.Append(tenantId.ToString());
            }

            if (_blobContainerConfigProvider.Get(defaultContainerName).GetFileSystemConfiguration().AppendContainerNameToBasePath)
            {
                filePathBuilder.Append(configuration.AbpBlobDirectorySeparator);
                filePathBuilder.Append(defaultContainerName);
            }
            filePathBuilder.Append(configuration.AbpBlobDirectorySeparator);
            filePathBuilder.Append(backupDir);

            var filePath = filePathBuilder.ToString();
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            return (filePath + configuration.AbpBlobDirectorySeparator + fileName,
                backupDir + configuration.AbpBlobDirectorySeparator + fileName);
        }
        private string GetFileName(string dbTypeName, string databaseName)
        {            
            if (dbTypeName == "MySqlConnection")
                return $"{databaseName}-{DateTime.Now.ToString("yyyyyMMddHHmmss")}.sql";
            else
                return $"{databaseName}-{DateTime.Now.ToString("yyyyyMMddHHmmss")}.bak";
        }
        private string GetDatabaseName(string ConnectionString)
        {
            var res = Regex.Match(ConnectionString, "(Database=)(.*?)(;)");
            if (!res.Success)
                return string.Empty;
            else
                return res.Groups[2].ToString();
        }
        private string GetMimeMapping(string fileName)
        {
            var extension = fileName.Substring(fileName.LastIndexOf('.'));
            switch (extension)
            {
                case ".sql":
                    return "text/x-sql";
                case ".bak":
                    return "application/octet-stream";
                default:
                    return "application/octet-stream";
            }
        }
    }
}
