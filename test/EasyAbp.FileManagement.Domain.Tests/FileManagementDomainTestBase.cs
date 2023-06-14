using EasyAbp.FileManagement.Files;

namespace EasyAbp.FileManagement
{
    /* Inherit from this class for your domain layer tests.
     * See SampleManager_Tests for example.
     */
    public abstract class FileManagementDomainTestBase : FileManagementTestBase<FileManagementDomainTestModule>
    {
        protected IFileRepository FileRepository { get; }
        protected IFileManager FileManager { get; }

        public FileManagementDomainTestBase()
        {
            FileRepository = GetRequiredService<IFileRepository>();
            FileManager = GetRequiredService<IFileManager>();
        }
    }
}