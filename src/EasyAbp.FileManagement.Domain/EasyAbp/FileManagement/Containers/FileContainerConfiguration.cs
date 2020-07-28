using System;
using JetBrains.Annotations;

namespace EasyAbp.FileManagement.Containers
{
    public class FileContainerConfiguration
    {
        public FileContainerType FileContainerType { get; set; }

        public string AbpBlobContainerName { get; set; }
        
        public string AbpBlobDirectorySeparator { get; set; }
        
        /// <summary>
        /// Do not delete the BLOB even if no file is using it.
        /// </summary>
        public bool RetainDeletedBlobs { get; set; }
        
        /// <summary>
        /// If there is an existing file with the same name, this feature will auto generate "($n)" and add it to the new file's name.
        /// </summary>
        public bool EnableAutoRename { get; set; }
        
        public int? EachUserGetDownloadInfoLimitPreMinute { get; set; }
        
        [CanBeNull]
        public Type SpecifiedFileDownloadProviderType { get; set; }
        
        public FileContainerConfiguration()
        {
        }
    }
}