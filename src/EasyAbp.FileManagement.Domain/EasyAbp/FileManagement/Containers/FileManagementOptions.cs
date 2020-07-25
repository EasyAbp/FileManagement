using System;

namespace EasyAbp.FileManagement.Containers
{
    public class FileManagementOptions
    {
        public FileContainerConfigurations Containers { get; }

        public Type DefaultFileDownloadProviderType { get; set; }
        
        public FileManagementOptions()
        {
            Containers = new FileContainerConfigurations();
        }
    }
}