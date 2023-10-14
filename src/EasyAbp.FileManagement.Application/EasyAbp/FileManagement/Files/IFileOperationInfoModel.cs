using System;
using JetBrains.Annotations;

namespace EasyAbp.FileManagement.Files;

public interface IFileOperationInfoModel
{
    [NotNull]
    string FileContainerName { get; }

    Guid? OwnerUserId { get; }
}