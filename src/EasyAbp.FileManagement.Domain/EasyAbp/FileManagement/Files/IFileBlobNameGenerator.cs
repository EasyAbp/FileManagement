using System.Threading.Tasks;

namespace EasyAbp.FileManagement.Files
{
    public interface IFileBlobNameGenerator
    {
        Task<string> CreateAsync(FileType fileType, string fileName, File parent, string mimeType, string directorySeparator);
    }
}