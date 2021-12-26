using System.IO;
using System.Reflection;
using EasyAbp.FileManagement.Containers;
using EasyAbp.FileManagement.EntityFrameworkCore;
using EasyAbp.FileManagement.Files;
using EasyAbp.FileManagement.Options;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.FileSystem;
using Volo.Abp.Modularity;

namespace EasyAbp.FileManagement
{
    /* Domain tests are configured to use the EF Core provider.
     * You can switch to MongoDB, however your domain tests should be
     * database independent anyway.
     */
    [DependsOn(
        typeof(AbpBlobStoringFileSystemModule),
        typeof(FileManagementEntityFrameworkCoreTestModule)
    )]
    public class FileManagementDomainTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<FileManagementOptions>(options =>
            {
                options.DefaultFileDownloadProviderType = typeof(LocalFileDownloadProvider);
                
                options.Containers.Configure<TestFileContainer>(container =>
                {
                    // private container never be used by non-owner users (except user who has the "File.Manage" permission).
                    container.FileContainerType = FileContainerType.Public;
                    container.AbpBlobContainerName = BlobContainerNameAttribute.GetContainerName<LocalFileSystemBlobContainer>();
                    container.AbpBlobDirectorySeparator = "/";
                    
                    container.DisableBlobReuse = true;
                    container.AllowBlobOverriding = true;
                    container.RetainUnusedBlobs = false;
                    container.EnableAutoRename = true;

                    container.MaxByteSizeForEachFile = 5 * 1024 * 1024;
                    container.MaxByteSizeForEachUpload = 10 * 1024 * 1024;
                    container.MaxFileQuantityForEachUpload = 2;

                    container.AllowOnlyConfiguredFileExtensions = true;
                    container.FileExtensionsConfiguration.Add(".txt", true);

                    container.GetDownloadInfoTimesLimitEachUserPerMinute = 10;
                });
                
                options.Containers.Configure<TestBlobReuseFileContainer>(container =>
                {
                    // private container never be used by non-owner users (except user who has the "File.Manage" permission).
                    container.FileContainerType = FileContainerType.Public;
                    container.AbpBlobContainerName = BlobContainerNameAttribute.GetContainerName<LocalFileSystemBlobContainer>();
                    container.AbpBlobDirectorySeparator = "/";
                    
                    container.DisableBlobReuse = false;
                    container.AllowBlobOverriding = true;
                });
            });
            
            Configure<AbpBlobStoringOptions>(options =>
            {
                options.Containers.Configure<LocalFileSystemBlobContainer>(container =>
                {
                    container.UseFileSystem(fileSystem =>
                    {
                        fileSystem.BasePath = GetFileBlobContainerBathPath();
                    });
                });
            });
        }
        
        private static string GetFileBlobContainerBathPath()
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
                "my-files");
        }
    }
}
