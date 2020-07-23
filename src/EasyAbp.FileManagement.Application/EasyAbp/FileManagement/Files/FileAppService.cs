using System;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Files.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace EasyAbp.FileManagement.Files
{
    public class FileAppService : CrudAppService<File, FileInfoDto, Guid, PagedAndSortedResultRequestDto, CreateFileDto, UpdateFileDto>,
        IFileAppService
    {
        private readonly IFileRepository _repository;
        
        public FileAppService(IFileRepository repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<FileInfoDto> MoveAsync(MoveFileInput input)
        {
            throw new NotImplementedException();
        }

        public async Task<FileDownloadDto> GetDownloadUrlAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
