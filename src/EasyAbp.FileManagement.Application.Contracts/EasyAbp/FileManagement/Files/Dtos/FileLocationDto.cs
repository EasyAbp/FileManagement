using System;

namespace EasyAbp.FileManagement.Files.Dtos;

[Serializable]
public class FileLocationDto
{
    public Guid Id { get; set; }

    public FileLocationModel Location { get; set; }
}