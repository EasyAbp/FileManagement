using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.ObjectExtending;

namespace EasyAbp.FileManagement.Files.Dtos
{
    [Serializable]
    public class CreateFileOutput : ExtensibleObject
    {
        public FileInfoDto FileInfo { get; set; }
        
        public FileDownloadInfoModel DownloadInfo { get; set; }
    }
}