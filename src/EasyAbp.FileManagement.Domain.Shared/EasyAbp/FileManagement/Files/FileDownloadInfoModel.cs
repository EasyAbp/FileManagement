using System;
using Volo.Abp.ObjectExtending;

namespace EasyAbp.FileManagement.Files
{
    [Serializable]
    public class FileDownloadInfoModel : ExtensibleObject
    {
        public string DownloadMethod { get; set; }
        
        public string DownloadUrl { get; set; }
        
        public string ExpectedFileName { get; set; }
        
        public string Token { get; set; }
    }
}