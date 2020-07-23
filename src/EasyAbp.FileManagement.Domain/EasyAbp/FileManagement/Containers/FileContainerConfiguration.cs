namespace EasyAbp.FileManagement.Containers
{
    public class FileContainerConfiguration
    {
        public FileContainerType FileContainerType { get; set; }

        public string AbpBlobContainerName { get; set; }
        
        public string AbpBlobDirectorySeparator { get; set; }
        
        public FileContainerConfiguration()
        {
        }
    }
}