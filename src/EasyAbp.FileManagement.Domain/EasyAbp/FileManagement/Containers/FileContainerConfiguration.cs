using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;

namespace EasyAbp.FileManagement.Containers
{
    public class FileContainerConfiguration : PublicFileContainerConfiguration
    {
        public string AbpBlobContainerName { get; set; }

        public string AbpBlobDirectorySeparator { get; set; }
        
        /// <summary>
        /// Do not delete the BLOB even if no file is using it.
        /// </summary>
        public bool RetainDeletedBlobs { get; set; }

        [CanBeNull]
        public Type SpecifiedFileDownloadProviderType { get; set; }
    }
}