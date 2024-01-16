using System;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Containers;
using EasyAbp.FileManagement.Files.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace EasyAbp.FileManagement.Files
{
    public interface IFileAppService :
        IReadOnlyAppService<
            FileInfoDto,
            Guid,
            GetFileListInput>
    {
        Task<CreateFileOutput> CreateAsync(CreateFileInput input);

        Task<CreateFileOutput> CreateWithStreamAsync(CreateFileWithStreamInput input);

        Task<CreateManyFileOutput> CreateManyAsync(CreateManyFileInput input);

        Task<CreateManyFileOutput> CreateManyWithStreamAsync(CreateManyFileWithStreamInput input);

        Task<FileInfoDto> MoveAsync(Guid id, MoveFileInput input);

        Task DeleteAsync(Guid id);

        Task<FileDownloadInfoModel> GetDownloadInfoAsync(Guid id);

        Task<FileInfoDto> UpdateInfoAsync(Guid id, UpdateFileInfoInput input);

        Task<FileDownloadOutput> DownloadAsync(Guid id, string token);

        Task<IRemoteStreamContent> DownloadWithStreamAsync(Guid id, string token);

        Task<PublicFileContainerConfiguration> GetConfigurationAsync(string fileContainerName, Guid? ownerUserId);

        Task<FileLocationDto> GetLocationAsync(Guid id);

        Task<GetFileByPathOutputDto> GetByPathAsync(string path, string fileContainerName, Guid? ownerUserId);
    }
}