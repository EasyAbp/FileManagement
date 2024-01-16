using System;
using JetBrains.Annotations;

namespace EasyAbp.FileManagement.Files.Dtos;

[Serializable]
public class GetFileByPathOutputDto
{
    public bool Found { get; set; }

    [CanBeNull]
    public FileInfoDto FileInfo { get; set; }

    public GetFileByPathOutputDto()
    {
    }

    public GetFileByPathOutputDto(bool found, [CanBeNull] FileInfoDto fileInfo)
    {
        Found = found;
        FileInfo = fileInfo;
    }
}