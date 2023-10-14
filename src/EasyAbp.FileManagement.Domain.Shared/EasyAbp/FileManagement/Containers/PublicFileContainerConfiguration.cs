using System.Collections.Generic;

namespace EasyAbp.FileManagement.Containers;

public class PublicFileContainerConfiguration : IFileContainerConfiguration
{
    public virtual FileContainerType FileContainerType { get; set; }

    /// <summary>
    /// If there is an existing file with the same name, this feature will auto generate "($n)" and add it to the new file's name.
    /// </summary>
    public virtual bool EnableAutoRename { get; set; }

    public virtual long MaxByteSizeForEachFile { get; set; } = long.MaxValue;

    public virtual long MaxByteSizeForEachUpload { get; set; } = long.MaxValue;

    public virtual int MaxFileQuantityForEachUpload { get; set; } = int.MaxValue;

    public virtual bool AllowOnlyConfiguredFileExtensions { get; set; }

    /// <summary>
    /// [".jpg" => true] means the ".jpg" extension is allowed.
    /// [".mp4" => false] means the ".mp4" extension is NOT allowed.
    /// </summary>
    public virtual Dictionary<string, bool> FileExtensionsConfiguration { get; set; } = new();

    public virtual int? GetDownloadInfoTimesLimitEachUserPerMinute { get; set; }

    public virtual PublicFileContainerConfiguration ToPublicConfiguration()
    {
        return this;
    }
}