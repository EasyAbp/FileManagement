namespace EasyAbp.FileManagement.Options
{
    public class LocalFileDownloadOptions
    {
        public string FileDownloadBaseUrl { get; set; } = "/";
        
        public TimeSpan TokenCacheDuration { get; set; } = TimeSpan.FromDays(1);
    }
}
