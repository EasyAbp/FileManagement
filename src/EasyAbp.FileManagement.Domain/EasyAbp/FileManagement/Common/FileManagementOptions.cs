namespace EasyAbp.FileManagement.Common
{
    public class FileManagementOptions
    {
        public FileContainerConfigurations Containers { get; }
        
        public FileManagementOptions()
        {
            Containers = new FileContainerConfigurations();
        }
    }
}