namespace EasyAbp.FileManagement.Containers
{
    public interface IFileContainerConfigurationProvider
    {
        FileContainerConfiguration Get(string fileContainerName);
    }
}