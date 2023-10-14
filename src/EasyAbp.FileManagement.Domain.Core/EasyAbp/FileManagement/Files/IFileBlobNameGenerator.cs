using System.Threading.Tasks;
using JetBrains.Annotations;

namespace EasyAbp.FileManagement.Files
{
    public interface IFileBlobNameGenerator
    {
        Task<string> CreateAsync(FileType fileType, string fileName, [CanBeNull] IFile parent, string mimeType,
            string directorySeparator);
    }
}