using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;

namespace EasyAbp.FileManagement.Files
{
    public class FileBlobNameGenerator : IFileBlobNameGenerator, ITransientDependency
    {
        private readonly IClock _clock;

        public FileBlobNameGenerator(IClock clock)
        {
            _clock = clock;
        }
        
        public virtual Task<string> CreateAsync(FileType fileType, string fileName, string filePath, string mimeType, string directorySeparator)
        {
            var now = _clock.Now;

            var blobName = now.Year + directorySeparator + now.Month + directorySeparator + now.Day +
                           directorySeparator + Guid.NewGuid().ToString("N") + Path.GetExtension(fileName);

            return Task.FromResult(blobName);
        }
    }
}