using System.Collections.Generic;

namespace EasyAbp.FileManagement.Files.Dtos
{
    public class CreateManyFileDto
    {
        public List<CreateFileDto> FileInfos { get; set; }
    }
}