using System;
using System.IO;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace EasyAbp.FileManagement.Files
{
    public class LocalFileDownloadProvider : ILocalFileDownloadProvider, ISingletonDependency
    {
        public const string DownloadMethod = "Local";

        // Todo: should be a configuration
        public static TimeSpan TokenCacheDuration = TimeSpan.FromMinutes(1);

        public static string BasePath = "api/file-management/file/{0}/download";

        public IAbpLazyServiceProvider LazyServiceProvider { get; set; }

        protected LocalFileDownloadOptions Options =>
            LazyServiceProvider.LazyGetRequiredService<IOptions<LocalFileDownloadOptions>>().Value;

        protected IDistributedCache<LocalFileDownloadCacheItem> Cache => LazyServiceProvider
            .LazyGetRequiredService<IDistributedCache<LocalFileDownloadCacheItem>>();

        protected IFileBlobManager FileBlobManager => LazyServiceProvider.LazyGetRequiredService<IFileBlobManager>();

        public virtual async Task<FileDownloadInfoModel> CreateDownloadInfoAsync(File file)
        {
            var token = Guid.NewGuid().ToString("N");

            await Cache.SetAsync(token,
                new LocalFileDownloadCacheItem { FileId = file.Id },
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TokenCacheDuration });

            var url = Options.FileDownloadBaseUrl.EnsureEndsWith('/') + string.Format(BasePath, file.Id) +
                      $"?token={token}";

            return new FileDownloadInfoModel
            {
                DownloadMethod = DownloadMethod,
                DownloadUrl = url,
                ExpectedFileName = file.FileName,
                Token = token
            };
        }

        public virtual async Task CheckTokenAsync(string token, Guid fileId)
        {
            var cacheItem = await Cache.GetAsync(token);

            if (cacheItem == null || cacheItem.FileId != fileId)
            {
                throw new LocalFileDownloadInvalidTokenException();
            }
        }

        public virtual Task<byte[]> GetDownloadBytesAsync(File file)
        {
            return FileBlobManager.GetAsync(file);
        }

        public virtual async Task<Stream> GetDownloadStreamAsync(File file)
        {
            return new MemoryStream(await GetDownloadBytesAsync(file));
        }
    }
}