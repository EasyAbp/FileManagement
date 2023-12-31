using System;
using JetBrains.Annotations;

namespace EasyAbp.FileManagement.Files;

public class FileLocationModel
{
    public bool IsDirectory { get; set; }

    [NotNull]
    public string ParentPath { get; set; }

    [NotNull]
    public string FileName { get; set; }

    [NotNull]
    public string FilePath => ParentPath.IsNullOrEmpty()
        ? FileName
        : $"{ParentPath}{FileManagementConsts.DirectorySeparator}{FileName}";

    public FileLocationModel(bool isDirectory, [NotNull] string parentPath, [NotNull] string fileName)
    {
        IsDirectory = isDirectory;
        ParentPath = parentPath;
        FileName = fileName;
    }
}