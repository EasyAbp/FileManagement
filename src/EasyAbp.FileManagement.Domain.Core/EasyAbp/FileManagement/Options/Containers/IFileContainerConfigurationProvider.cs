using EasyAbp.FileManagement.Containers;

namespace EasyAbp.FileManagement.Options.Containers;

public interface IFileContainerConfigurationProvider
{
    TConfiguration Get<TConfiguration>(string fileContainerName) where TConfiguration : IFileContainerConfiguration;

    IFileContainerConfiguration Get(string fileContainerName);
}