namespace EasyAbp.FileManagement
{
    public static class FileManagementDbProperties
    {
        public static string DbTablePrefix { get; set; } = "EasyAbpFileManagement";

        public static string DbSchema { get; set; } = null;

        public const string ConnectionStringName = "EasyAbpFileManagement";
    }
}
