using System;
using JetBrains.Annotations;

namespace EasyAbp.FileManagement.Web.Pages.FileManagement.Components.FileManagerWidget;

public class FileManagerViewModel
{
    [NotNull]
    public string FileContainerName { get; set; }

    public Guid? OwnerUserId { get; set; }

    public Guid? ParentId { get; set; }

    public Guid? GrandparentId { get; set; }

    [CanBeNull]
    public string FullPath { get; set; }

    public FileManagerPolicyModel Policy { get; set; }

    public FileManagerViewModel([NotNull] string fileContainerName, Guid? ownerUserId, Guid? parentId,
        Guid? grandparentId, [CanBeNull] string fullPath, [CanBeNull] FileManagerPolicyModel policy = null)
    {
        FileContainerName = fileContainerName;
        OwnerUserId = ownerUserId;
        ParentId = parentId;
        GrandparentId = grandparentId;
        FullPath = fullPath;
        Policy = policy ?? new FileManagerPolicyModel();
    }
}