using EasyAbp.FileManagement.Files;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace EasyAbp.FileManagement
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class FileToFileEtoMapper : MapperBase<File, FileEto>
    {
        public override partial FileEto Map(File source);
        public override partial void Map(File source, FileEto destination);
    }
}
