using EasyAbp.FileManagement.Files.Dtos;
using EasyAbp.FileManagement.Web.Pages.FileManagement.Components.FileManagerWidget.ViewModels;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace EasyAbp.FileManagement.Web
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class FileInfoDtoToRenameFileViewModelMapper : MapperBase<FileInfoDto, RenameFileViewModel>
    {
        public override partial RenameFileViewModel Map(FileInfoDto source);
        public override partial void Map(FileInfoDto source, RenameFileViewModel destination);
    }
}
