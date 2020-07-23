using EasyAbp.FileManagement.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace EasyAbp.FileManagement.Permissions
{
    public class FileManagementPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(FileManagementPermissions.GroupName, L("Permission:FileManagement"));

            var filePermission = myGroup.AddPermission(FileManagementPermissions.File.Default, L("Permission:File"));
            filePermission.AddChild(FileManagementPermissions.File.Manage, L("Permission:Manage"));
            filePermission.AddChild(FileManagementPermissions.File.Create, L("Permission:Create"));
            filePermission.AddChild(FileManagementPermissions.File.Update, L("Permission:Update"));
            filePermission.AddChild(FileManagementPermissions.File.Delete, L("Permission:Delete"));
            filePermission.AddChild(FileManagementPermissions.File.Download, L("Permission:Download"));
            filePermission.AddChild(FileManagementPermissions.File.Move, L("Permission:Move"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<FileManagementResource>(name);
        }
    }
}
