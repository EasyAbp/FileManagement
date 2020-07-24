using System.Threading.Tasks;

namespace EasyAbp.FileManagement.Files
{
    public interface IFileDownloadProvider
    {
        Task<FileDownloadInfoModel> CreateDownloadInfoAsync(File file);
    }
}