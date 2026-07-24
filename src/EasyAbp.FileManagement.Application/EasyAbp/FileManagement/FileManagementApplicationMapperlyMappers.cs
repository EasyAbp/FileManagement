using EasyAbp.FileManagement.Files;
using EasyAbp.FileManagement.Files.Dtos;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace EasyAbp.FileManagement
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class FileToFileInfoDtoMapper : MapperBase<File, FileInfoDto>
    {
        [MapperIgnoreTarget(nameof(FileInfoDto.Owner))]
        [MapperIgnoreTarget(nameof(FileInfoDto.Creator))]
        [MapperIgnoreTarget(nameof(FileInfoDto.LastModifier))]
        public override partial FileInfoDto Map(File source);

        [MapperIgnoreTarget(nameof(FileInfoDto.Owner))]
        [MapperIgnoreTarget(nameof(FileInfoDto.Creator))]
        [MapperIgnoreTarget(nameof(FileInfoDto.LastModifier))]
        public override partial void Map(File source, FileInfoDto destination);
    }
}
