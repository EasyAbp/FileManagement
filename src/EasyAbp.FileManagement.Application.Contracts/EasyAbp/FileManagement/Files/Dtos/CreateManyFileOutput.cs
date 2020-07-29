using System;
using Volo.Abp.Application.Dtos;

namespace EasyAbp.FileManagement.Files.Dtos
{
    [Serializable]
    public class CreateManyFileOutput : ListResultDto<CreateFileOutput>
    {
    }
}