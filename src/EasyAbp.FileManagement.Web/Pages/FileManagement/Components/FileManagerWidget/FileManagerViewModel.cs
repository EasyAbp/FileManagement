using System;
using EasyAbp.FileManagement.Files;
using JetBrains.Annotations;

namespace EasyAbp.FileManagement.Web.Pages.FileManagement.Components.FileManagerWidget;

public class FileManagerViewModel
{
    [NotNull]
    public string FileContainerName { get; set; }

    public Guid? OwnerUserId { get; set; }

    public Guid? ParentId { get; set; }

    public Guid? GrandparentId { get; set; }

    public FileLocationModel Location { get; set; }

    public FileManagerPolicyModel Policy { get; set; }

    public FileManagerViewModel([NotNull] string fileContainerName, Guid? ownerUserId, Guid? parentId,
        Guid? grandparentId, [CanBeNull] FileLocationModel location, [CanBeNull] FileManagerPolicyModel policy = null)
    {
        FileContainerName = fileContainerName;
        OwnerUserId = ownerUserId;
        ParentId = parentId;
        GrandparentId = grandparentId;
        Location = location;
        Policy = policy ?? new FileManagerPolicyModel();
    }
}