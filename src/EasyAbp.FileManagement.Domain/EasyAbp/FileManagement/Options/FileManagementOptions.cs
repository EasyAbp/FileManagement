using System;
using EasyAbp.FileManagement.Options.Containers;

namespace EasyAbp.FileManagement.Options
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