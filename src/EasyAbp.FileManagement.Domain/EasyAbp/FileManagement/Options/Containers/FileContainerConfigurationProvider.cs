using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace EasyAbp.FileManagement.Options.Containers
{
    public class FileContainerConfigurationProvider : IFileContainerConfigurationProvider, ITransientDependency
    {
        private readonly FileManagementOptions _options;

        public FileContainerConfigurationProvider(IOptions<FileManagementOptions> options)
        {
            _options = options.Value;
        }
        
        public FileContainerConfiguration Get(string fileContainerName)
        {
            return _options.Containers.GetConfiguration(fileContainerName);
        }
    }
}