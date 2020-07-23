using System;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Files.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace EasyAbp.FileManagement.Files
{
    public interface IFileAppService :
        ICrudAppService< 
            FileInfoDto, 
            Guid, 
            PagedAndSortedResultRequestDto,
            CreateFileDto,
            UpdateFileDto>
    {
        Task<FileInfoDto> MoveAsync(MoveFileInput input);

        Task<FileDownloadDto> GetDownloadUrlAsync(Guid id);
        
        Task<FileInfoDto> UpdateInfoAsync(Guid id, UpdateFileInfoDto input);
    }
}