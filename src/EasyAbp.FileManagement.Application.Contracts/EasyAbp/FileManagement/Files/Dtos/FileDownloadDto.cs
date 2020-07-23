using System;
using Volo.Abp.Application.Dtos;

namespace EasyAbp.FileManagement.Files.Dtos
{
    [Serializable]
    public class FileDownloadDto : FileInfoDto
    {
        public string DownloadUrl { get; set; }
    }
}