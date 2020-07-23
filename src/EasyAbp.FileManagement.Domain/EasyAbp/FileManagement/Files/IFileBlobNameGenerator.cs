using System.Threading.Tasks;

namespace EasyAbp.FileManagement.Files
{
    public interface IFileBlobNameGenerator
    {
        Task<string> CreateAsync(FileType fileType, string fileName, string filePath, string mimeType, string directorySeparator);
    }
}