using System;
using EasyAbp.FileManagement.Options.Containers;

namespace EasyAbp.FileManagement.Options;

public class FileManagementOptions : FileManagementOptionsBase<FileContainerConfiguration>
{
    public Type DefaultFileDownloadProviderType { get; set; }
}