using System;

namespace EasyAbp.FileManagement.Files
{
    public class UserFileDownloadLimitCacheItem
    {
        public int Count { get; set; }
        
        public DateTime AbsoluteExpiration { get; set; }
    }
}