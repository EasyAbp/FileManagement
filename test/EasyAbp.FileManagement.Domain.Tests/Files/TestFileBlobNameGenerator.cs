using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace EasyAbp.FileManagement.Files
{
    [Dependency(ReplaceServices = true)]
    public class TestFileBlobNameGenerator : IFileBlobNameGenerator, ITransientDependency
    {
        public virtual Task<string> CreateAsync(FileType fileType, string fileName, File parent, string mimeType, string directorySeparator)
        {
            return Task.FromResult(fileName);
        }
    }
}