namespace EasyAbp.FileManagement.Containers
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