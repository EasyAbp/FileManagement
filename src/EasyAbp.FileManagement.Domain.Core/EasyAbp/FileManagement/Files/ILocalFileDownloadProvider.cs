using System;
using System.IO;
using System.Threading.Tasks;

namespace EasyAbp.FileManagement.Files;

public interface ILocalFileDownloadProvider : IFileDownloadProvider
{
    Task CheckTokenAsync(string token, Guid fileId);

    Task<byte[]> GetDownloadBytesAsync(File file);

    Task<Stream> GetDownloadStreamAsync(File file);
}