using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace EasyAbp.FileManagement.Files;

[Dependency(TryRegister = true)]
public class NullLocalFileDownloadProvider : ILocalFileDownloadProvider, ISingletonDependency
{
    public Task<FileDownloadInfoModel> CreateDownloadInfoAsync(File file)
    {
        return Task.FromResult(new FileDownloadInfoModel());
    }

    public Task CheckTokenAsync(string token, Guid fileId)
    {
        return Task.CompletedTask;
    }

    public Task<byte[]> GetDownloadBytesAsync(File file)
    {
        return Task.FromResult<byte[]>(null);
    }

    public Task<Stream> GetDownloadStreamAsync(File file)
    {
        return Task.FromResult<Stream>(null);
    }
}