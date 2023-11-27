namespace EasyAbp.FileManagement.Web.Pages.FileManagement.Components.FileManagerWidget;

public class FileManagerPolicyModel
{
    /// <summary>
    /// If null, users with the "EasyAbp.FileManagement.File.Create" permission are allowed.
    /// </summary>
    public bool? CanUploadFile { get; set; }

    /// <summary>
    /// If null, users with the "EasyAbp.FileManagement.File.Create" permission are allowed.
    /// </summary>
    public bool? CanCreateDirectory { get; set; }

    /// <summary>
    /// If null, users with the "EasyAbp.FileManagement.File.Delete" permission are allowed.
    /// </summary>
    public bool? CanDeleteFile { get; set; }

    /// <summary>
    /// If null, users with the "EasyAbp.FileManagement.File.Delete" permission are allowed.
    /// </summary>
    public bool? CanDeleteDirectory { get; set; }

    /// <summary>
    /// If null, users with the "EasyAbp.FileManagement.File.Move" permission are allowed.
    /// </summary>
    public bool? CanMoveFile { get; set; }

    /// <summary>
    /// If null, users with the "EasyAbp.FileManagement.File.Move" permission are allowed.
    /// </summary>
    public bool? CanMoveDirectory { get; set; }

    /// <summary>
    /// If null, users with the "EasyAbp.FileManagement.File.Update" permission are allowed.
    /// </summary>
    public bool? CanRenameFile { get; set; }

    /// <summary>
    /// If null, users with the "EasyAbp.FileManagement.File.Update" permission are allowed.
    /// </summary>
    public bool? CanRenameDirectory { get; set; }

    /// <summary>
    /// If null, users with the "EasyAbp.FileManagement.File.GetDownloadInfo" permission are allowed.
    /// </summary>
    public bool? CanDownloadFile { get; set; }
}