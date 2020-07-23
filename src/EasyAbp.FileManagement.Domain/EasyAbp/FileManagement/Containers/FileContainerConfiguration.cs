namespace EasyAbp.FileManagement.Containers
{
    public class FileContainerConfiguration
    {
        public FileContainerType FileContainerType { get; set; }

        public string AbpBlobContainerName { get; set; }
        
        public string DirectorySeparator { get; set; }
        
        public FileContainerConfiguration()
        {
        }
    }
}