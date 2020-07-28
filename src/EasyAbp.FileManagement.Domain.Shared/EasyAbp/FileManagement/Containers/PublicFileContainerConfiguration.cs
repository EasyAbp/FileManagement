using System.Collections.Generic;

namespace EasyAbp.FileManagement.Containers
{
    public class PublicFileContainerConfiguration
    {
        public FileContainerType FileContainerType { get; set; }
        
        /// <summary>
        /// If there is an existing file with the same name, this feature will auto generate "($n)" and add it to the new file's name.
        /// </summary>
        public bool EnableAutoRename { get; set; }

        public long MaxByteSizeForEachFile { get; set; } = long.MaxValue;
        
        public long MaxByteSizeForEachUpload { get; set; } = long.MaxValue;
        
        public int MaxFileQuantityForEachUpload { get; set; } = int.MaxValue;
        
        // public long MaxByteSizeForEachContainer { get; set; } = long.MaxValue;

        public bool AllowOnlyConfiguredFileExtensions { get; set; }
        
        /// <summary>
        /// [".jpg" => true] means the ".jpg" extension is allowed.
        /// [".mp4" => false] means the ".mp4" extension is NOT allowed.
        /// </summary>
        public Dictionary<string, bool> FileExtensionsConfiguration { get; set; } = new Dictionary<string, bool>();
        
        public int? GetDownloadInfoTimesLimitEachUserPerMinute { get; set; }
        
        // public int? UploadTimesLimitEachUserPerMinute { get; set; }
        //
        // public int? UploadQuantityLimitEachUserPerMinute { get; set; }
        //
        // public int? UploadByteSizeLimitEachUserPerMinute { get; set; }
    }
}