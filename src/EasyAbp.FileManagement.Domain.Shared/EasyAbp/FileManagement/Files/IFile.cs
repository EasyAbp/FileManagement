using System;
using JetBrains.Annotations;

namespace EasyAbp.FileManagement.Files;

public interface IFile
{
    Guid Id { get; }

    Guid? ParentId { get; }

    [NotNull]
    string FileContainerName { get; }

    [NotNull]
    string FileName { get; }

    [CanBeNull]
    string MimeType { get; }

    FileType FileType { get; }

    int SubFilesQuantity { get; }

    bool HasSubdirectories { get; }

    long ByteSize { get; }

    [CanBeNull]
    string Hash { get; }

    [CanBeNull]
    string BlobName { get; }

    Guid? OwnerUserId { get; }

    [CanBeNull]
    string Flag { get; }
}