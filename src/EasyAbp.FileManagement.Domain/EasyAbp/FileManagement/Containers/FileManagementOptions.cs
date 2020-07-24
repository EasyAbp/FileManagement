using System;

namespace EasyAbp.FileManagement.Containers
{
    public class FileManagementOptions
    {
        public FileContainerConfigurations Containers { get; }

        public Type DefaultFileDownloadProvider { get; }
        
        public FileManagementOptions()
        {
            Containers = new FileContainerConfigurations();
            DefaultFileDownloadProvider = null;
        }
    }
}