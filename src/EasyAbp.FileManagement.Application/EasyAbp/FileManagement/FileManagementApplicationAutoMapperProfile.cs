using EasyAbp.FileManagement.Files;
using EasyAbp.FileManagement.Files.Dtos;
using AutoMapper;
using Volo.Abp.AutoMapper;

namespace EasyAbp.FileManagement
{
    public class FileManagementApplicationAutoMapperProfile : Profile
    {
        public FileManagementApplicationAutoMapperProfile()
        {
            /* You can configure your AutoMapper mapping configuration here.
             * Alternatively, you can split your mapping configurations
             * into multiple profile classes for a better organization. */
            CreateMap<File, FileInfoDto>();
        }
    }
}
