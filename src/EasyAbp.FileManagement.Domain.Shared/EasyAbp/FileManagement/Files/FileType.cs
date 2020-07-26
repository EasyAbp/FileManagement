using System;

namespace EasyAbp.FileManagement.Files
{
    [Flags]
    public enum FileType
    {
        Directory = 1,
        RegularFile = 2
    }
}