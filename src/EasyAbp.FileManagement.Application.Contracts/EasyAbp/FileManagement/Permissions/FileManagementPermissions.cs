using Volo.Abp.Reflection;

namespace EasyAbp.FileManagement.Permissions
{
    public class FileManagementPermissions
    {
        public const string GroupName = "EasyAbp.FileManagement";

        public static string[] GetAll()
        {
            return ReflectionHelper.GetPublicConstantsRecursively(typeof(FileManagementPermissions));
        }
    }
}