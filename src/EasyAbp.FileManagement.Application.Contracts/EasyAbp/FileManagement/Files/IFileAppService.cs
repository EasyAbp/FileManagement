using System;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Containers;
using EasyAbp.FileManagement.Files.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace EasyAbp.FileManagement.Files
{
    public interface IFileAppService :
        IReadOnlyAppService< 
            FileInfoDto, 
            Guid, 
            GetFileListInput>
    {
        Task<CreateFileOutput> CreateAsync(CreateFileInput input);

        Task<CreateManyFileOutput> CreateManyAsync(CreateManyFileInput input);

        Task<FileInfoDto> UpdateAsync(Guid id, UpdateFileInput input);

        Task<FileInfoDto> MoveAsync(Guid id, MoveFileInput input);

        Task DeleteAsync(Guid id);

        Task<FileDownloadInfoModel> GetDownloadInfoAsync(Guid id);
        
        Task<FileInfoDto> UpdateInfoAsync(Guid id, UpdateFileInfoInput input);

        Task<FileDownloadOutput> DownloadAsync(Guid id, string token);

        Task<PublicFileContainerConfiguration> GetConfigurationAsync(string fileContainerName, Guid? ownerUserId);
    }
}