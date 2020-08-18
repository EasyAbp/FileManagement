using System;
using EasyAbp.FileManagement.Containers;
using JetBrains.Annotations;

namespace EasyAbp.FileManagement.Options.Containers
{
    public class FileContainerConfiguration : PublicFileContainerConfiguration
    {
        public string AbpBlobContainerName { get; set; }

        public string AbpBlobDirectorySeparator { get; set; }
        
        /// <summary>
        /// Do not delete the BLOB even if no file is using it.
        /// </summary>
        public bool RetainUnusedBlobs { get; set; }

        [CanBeNull]
        public Type SpecifiedFileDownloadProviderType { get; set; }
    }
}