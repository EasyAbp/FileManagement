namespace EasyAbp.FileManagement.Settings
{
    public static class FileManagementSettings
    {
        public const string GroupName = "EasyAbp.FileManagement";

        /* Add constants for setting names. Example:
         * public const string MySettingName = GroupName + ".MySettingName";
         */

        public class Backup
        {
            public const string AutoBackup = GroupName + "Backup.AutoBackup";
            public const string Frequency = GroupName + "Backup.Frequency";
            public const string MaxCount = GroupName + "Backup.MaxCount";
            public const string StartTime = GroupName + "Backup.StartTime";
            public const string EndTime = GroupName + "Backup.EndTime";
            public const string CyclicCovering = GroupName + "Backup.CyclicCovering";
        }
    }
}