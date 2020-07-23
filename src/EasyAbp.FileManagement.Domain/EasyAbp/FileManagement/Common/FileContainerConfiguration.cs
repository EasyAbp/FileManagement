namespace EasyAbp.FileManagement.Common
{
    public class FileContainerConfiguration
    {
        public string AbpBlobContainerName  { get; set; }
        
        public FileContainerType FileContainerType { get; set; }

        public FileContainerConfiguration()
        {
        }
    }
}