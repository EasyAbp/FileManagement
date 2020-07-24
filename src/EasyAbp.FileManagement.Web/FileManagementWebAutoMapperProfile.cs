using EasyAbp.FileManagement.Files.Dtos;
using EasyAbp.FileManagement.Web.Pages.FileManagement.Files.File.ViewModels;
using AutoMapper;
using Volo.Abp.AutoMapper;

namespace EasyAbp.FileManagement.Web
{
    public class FileManagementWebAutoMapperProfile : Profile
    {
        public FileManagementWebAutoMapperProfile()
        {
            /* You can configure your AutoMapper mapping configuration here.
             * Alternatively, you can split your mapping configurations
             * into multiple profile classes for a better organization. */
            CreateMap<FileInfoDto, EditFileViewModel>();
            CreateMap<CreateFileViewModel, CreateFileDto>()
                .Ignore(dto => dto.Content);
            CreateMap<EditFileViewModel, UpdateFileDto>()
                .Ignore(dto => dto.Content);
        }
    }
}
