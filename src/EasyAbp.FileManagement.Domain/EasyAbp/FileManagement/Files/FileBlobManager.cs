using System.Threading;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Options.Containers;
using Volo.Abp.BlobStoring;
using Volo.Abp.DependencyInjection;

namespace EasyAbp.FileManagement.Files;

public class FileBlobManager : IFileBlobManager, ITransientDependency
{
    private readonly IBlobContainerFactory _blobContainerFactory;
    private readonly IFileContainerConfigurationProvider _configurationProvider;

    public FileBlobManager(
        IBlobContainerFactory blobContainerFactory,
        IFileContainerConfigurationProvider configurationProvider)
    {
        _blobContainerFactory = blobContainerFactory;
        _configurationProvider = configurationProvider;
    }

    public virtual async Task<bool> TrySaveAsync(File file, byte[] fileContent, bool disableBlobReuse = false,
        bool allowBlobOverriding = false, CancellationToken cancellationToken = default)
    {
        if (file.FileType != FileType.RegularFile)
        {
            throw new UnexpectedFileTypeException(file.Id, file.FileType);
        }

        var blobContainer = GetBlobContainer(file);

        if (!disableBlobReuse && await blobContainer.ExistsAsync(file.BlobName, cancellationToken))
        {
            return false;
        }

        await blobContainer.SaveAsync(file.BlobName, fileContent, allowBlobOverriding, cancellationToken);

        return true;
    }

    public virtual async Task<byte[]> GetAsync(File file, CancellationToken cancellationToken = default)
    {
        if (file.FileType != FileType.RegularFile)
        {
            throw new UnexpectedFileTypeException(file.Id, file.FileType, FileType.RegularFile);
        }

        var blobContainer = GetBlobContainer(file);

        return await blobContainer.GetAllBytesAsync(file.BlobName, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(string fileContainerName, string blobName,
        CancellationToken cancellationToken = default)
    {
        var blobContainer = GetBlobContainer(fileContainerName);

        await blobContainer.DeleteAsync(blobName, cancellationToken);
    }

    protected virtual IBlobContainer GetBlobContainer(File file)
    {
        var configuration = _configurationProvider.Get<FileContainerConfiguration>(file.FileContainerName);

        return _blobContainerFactory.Create(configuration.AbpBlobContainerName);
    }

    protected virtual IBlobContainer GetBlobContainer(string fileContainerName)
    {
        var configuration = _configurationProvider.Get<FileContainerConfiguration>(fileContainerName);

        return _blobContainerFactory.Create(configuration.AbpBlobContainerName);
    }
}