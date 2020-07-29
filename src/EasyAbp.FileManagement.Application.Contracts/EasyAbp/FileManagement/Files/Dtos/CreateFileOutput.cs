using System;
using Volo.Abp.Application.Dtos;

namespace EasyAbp.FileManagement.Files.Dtos
{
    [Serializable]
    public class CreateFileOutput
    {
        public FileInfoDto FileInfo { get; set; }
        
        public FileDownloadInfoModel DownloadInfo { get; set; }
    }
}