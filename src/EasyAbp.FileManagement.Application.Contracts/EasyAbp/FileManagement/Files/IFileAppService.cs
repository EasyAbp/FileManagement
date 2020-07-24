using System;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Files.Dtos;
using Volo.Abp.Application.Services;

namespace EasyAbp.FileManagement.Files
{
    public interface IFileAppService :
        ICrudAppService< 
            FileInfoDto, 
            Guid, 
            GetFileListInput,
            CreateFileDto,
            UpdateFileDto>
    {
        Task<FileInfoDto> MoveAsync(Guid id, MoveFileInput input);

        Task<FileDownloadInfoModel> GetDownloadInfoAsync(Guid id);
        
        Task<FileInfoDto> UpdateInfoAsync(Guid id, UpdateFileInfoDto input);

        Task<byte[]> DownloadAsync(Guid id, string token);
    }
}