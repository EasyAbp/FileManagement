using EasyAbp.FileManagement.Files;
using EasyAbp.FileManagement.Files.Dtos;
using AutoMapper;
using EasyAbp.FileManagement.Containers;
using EasyAbp.FileManagement.Options.Containers;
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
            
            CreateMap<File, FileInfoDto>().MapExtraProperties();
            CreateMap<FileContainerConfiguration, PublicFileContainerConfiguration>(MemberList.Destination);
        }
    }
}
