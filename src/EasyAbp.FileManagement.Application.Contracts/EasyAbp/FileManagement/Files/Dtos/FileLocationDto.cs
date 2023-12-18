using System;

namespace EasyAbp.FileManagement.Files.Dtos;

[Serializable]
public class FileLocationDto
{
    public Guid Id { get; set; }

    public string FileName { get; set; }

    public string Location { get; set; }
}