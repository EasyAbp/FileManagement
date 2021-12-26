using System;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace EasyAbp.FileManagement.Files
{
    public class LocalFileDownloadProvider : IFileDownloadProvider, ISingletonDependency
    {
        public const string DownloadMethod = "Local";
        
        // Todo: should be a configuration
        public static TimeSpan TokenCacheDuration = TimeSpan.FromMinutes(1);
        
        public static string BasePath = "api/file-management/file/{0}/download";

        private readonly LocalFileDownloadOptions _options;
        private readonly IDistributedCache<LocalFileDownloadCacheItem> _cache;

        public LocalFileDownloadProvider(
            IOptions<LocalFileDownloadOptions> options,
            IDistributedCache<LocalFileDownloadCacheItem> cache)
        {
            _options = options.Value;
            _cache = cache;
        }
        
        public virtual async Task<FileDownloadInfoModel> CreateDownloadInfoAsync(File file)
        {
            var token = Guid.NewGuid().ToString("N");

            await _cache.SetAsync(token,
                new LocalFileDownloadCacheItem {FileId = file.Id},
                new DistributedCacheEntryOptions {AbsoluteExpirationRelativeToNow = TokenCacheDuration});

            var url = _options.FileDownloadBaseUrl.EnsureEndsWith('/') + string.Format(BasePath, file.Id) + $"?token={token}";

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
            var cacheItem = await _cache.GetAsync(token);

            if (cacheItem == null || cacheItem.FileId != fileId)
            {
                throw new LocalFileDownloadInvalidTokenException();
            }
        }
    }
}