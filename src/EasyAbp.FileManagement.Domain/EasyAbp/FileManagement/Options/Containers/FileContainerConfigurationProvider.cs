using EasyAbp.FileManagement.Containers;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace EasyAbp.FileManagement.Options.Containers;

public class FileContainerConfigurationProvider : IFileContainerConfigurationProvider, ITransientDependency
{
    private readonly FileManagementOptions _options;

    public FileContainerConfigurationProvider(IOptions<FileManagementOptions> options)
    {
        _options = options.Value;
    }

    public virtual TConfiguration Get<TConfiguration>(string fileContainerName)
        where TConfiguration : IFileContainerConfiguration
    {
        return (TConfiguration)Get(fileContainerName);
    }

    public virtual IFileContainerConfiguration Get(string fileContainerName)
    {
        return _options.Containers.GetConfiguration(fileContainerName);
    }
}