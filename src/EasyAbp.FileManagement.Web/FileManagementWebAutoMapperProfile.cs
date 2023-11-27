using AutoMapper;
using EasyAbp.FileManagement.Files.Dtos;
using EasyAbp.FileManagement.Web.Pages.FileManagement.Components.FileManagerWidget.ViewModels;

namespace EasyAbp.FileManagement.Web
{
    public class FileManagementWebAutoMapperProfile : Profile
    {
        public FileManagementWebAutoMapperProfile()
        {
            /* You can configure your AutoMapper mapping configuration here.
             * Alternatively, you can split your mapping configurations
             * into multiple profile classes for a better organization. */
            CreateMap<FileInfoDto, RenameFileViewModel>();
        }
    }
}