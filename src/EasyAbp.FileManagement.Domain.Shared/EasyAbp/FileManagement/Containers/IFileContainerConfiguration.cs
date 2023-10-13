using System.Collections.Generic;

namespace EasyAbp.FileManagement.Containers;

public interface IFileContainerConfiguration
{
    FileContainerType FileContainerType { get; set; }

    /// <summary>
    /// If there is an existing file with the same name, this feature will auto generate "($n)" and add it to the new file's name.
    /// </summary>
    bool EnableAutoRename { get; set; }

    long MaxByteSizeForEachFile { get; set; }

    long MaxByteSizeForEachUpload { get; set; }

    int MaxFileQuantityForEachUpload { get; set; }

    // long MaxByteSizeForEachContainer { get; set; }

    bool AllowOnlyConfiguredFileExtensions { get; set; }

    /// <summary>
    /// [".jpg" => true] means the ".jpg" extension is allowed.
    /// [".mp4" => false] means the ".mp4" extension is NOT allowed.
    /// </summary>
    Dictionary<string, bool> FileExtensionsConfiguration { get; set; }

    int? GetDownloadInfoTimesLimitEachUserPerMinute { get; set; }

    // int? UploadTimesLimitEachUserPerMinute { get; set; }
    //
    // int? UploadQuantityLimitEachUserPerMinute { get; set; }
    //
    // int? UploadByteSizeLimitEachUserPerMinute { get; set; }
}