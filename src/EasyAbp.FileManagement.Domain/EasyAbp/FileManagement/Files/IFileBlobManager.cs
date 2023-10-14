using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace EasyAbp.FileManagement.Files;

public interface IFileBlobManager
{
    Task<bool> TrySaveAsync(File file, byte[] fileContent, bool disableBlobReuse = false,
        bool allowBlobOverriding = false, CancellationToken cancellationToken = default);

    Task<byte[]> GetAsync(File file, CancellationToken cancellationToken = default);

    Task DeleteAsync([NotNull] string fileContainerName, [NotNull] string blobName,
        CancellationToken cancellationToken = default);
}