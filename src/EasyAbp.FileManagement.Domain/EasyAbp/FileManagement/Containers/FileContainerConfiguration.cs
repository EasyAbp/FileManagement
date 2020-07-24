using System;
using JetBrains.Annotations;

namespace EasyAbp.FileManagement.Containers
{
    public class FileContainerConfiguration
    {
        public FileContainerType FileContainerType { get; set; }

        public string AbpBlobContainerName { get; set; }
        
        public string AbpBlobDirectorySeparator { get; set; }
        
        public bool RetainDeletedBlobs { get; set; }
        
        public int? EachUserGetDownloadInfoLimitPreMinute { get; set; }
        
        [CanBeNull]
        public Type SpecifiedFileDownloadProviderType { get; set; }
        
        public FileContainerConfiguration()
        {
        }
    }
}