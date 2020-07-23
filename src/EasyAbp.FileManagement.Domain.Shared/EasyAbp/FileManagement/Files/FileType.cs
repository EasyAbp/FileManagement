using System;

namespace EasyAbp.FileManagement.Files
{
    [Flags]
    public enum FileType
    {
        RegularFile = 1,
        Directory = 2
    }
}