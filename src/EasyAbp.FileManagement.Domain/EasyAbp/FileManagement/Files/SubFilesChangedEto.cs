using System;

namespace EasyAbp.FileManagement.Files;

public class SubFilesChangedEto
{
    public Guid DirectoryId { get; set; }

    public SubFilesChangedEto()
    {
    }

    public SubFilesChangedEto(Guid directoryId)
    {
        DirectoryId = directoryId;
    }
}