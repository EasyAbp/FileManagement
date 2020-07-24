using System;

namespace EasyAbp.FileManagement.Files
{
    [Serializable]
    public class FileDownloadInfoModel
    {
        public string DownloadMethod { get; set; }
        
        public string DownloadUrl { get; set; }
        
        public string Token { get; set; }
    }
}