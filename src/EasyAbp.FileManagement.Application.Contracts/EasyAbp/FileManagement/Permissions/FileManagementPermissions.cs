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

        public class File
        {
            public const string Default = GroupName + ".File";
            public const string Manage = Default + ".Manage";
            public const string Update = Default + ".Update";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
            public const string Download = Default + ".Download";
            public const string Move = Default + ".Move";
        }
    }
}
