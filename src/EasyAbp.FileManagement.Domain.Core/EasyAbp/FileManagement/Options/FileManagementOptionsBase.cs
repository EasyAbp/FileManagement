using System;
using EasyAbp.FileManagement.Containers;
using EasyAbp.FileManagement.Options.Containers;

namespace EasyAbp.FileManagement.Options;

public class FileManagementOptionsBase<TConfiguration> where TConfiguration : IFileContainerConfiguration, new()
{
    public FileContainerConfigurations<TConfiguration> Containers { get; } = new();

    public Type DefaultFileDownloadProviderType { get; set; }
}