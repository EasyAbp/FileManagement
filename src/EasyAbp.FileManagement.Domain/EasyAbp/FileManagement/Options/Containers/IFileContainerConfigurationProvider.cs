namespace EasyAbp.FileManagement.Options.Containers
{
    public interface IFileContainerConfigurationProvider
    {
        FileContainerConfiguration Get(string fileContainerName);
    }
}