using EasyAbp.FileManagement.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Settings;

namespace EasyAbp.FileManagement.Settings
{
    public class FileManagementSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            /* Define module settings here.
             * Use names from FileManagementSettings class.
             */
            context.Add(new SettingDefinition(
                FileManagementSettings.Backup.AutoBackup,
                "True",
                L("DisplayName:EasyAbp.FileManagement.Backup.AutoBackup"),
                L("Description:EasyAbp.FileManagement.Backup.AutoBackup"),
                isVisibleToClients: false)
                .WithProperty("Type", "select")
                .WithProperty("Options", "True|False")
                .WithProperty("Group1", "FileManagement")
                .WithProperty("Group2", "Backup")
            );

            context.Add(new SettingDefinition(
                FileManagementSettings.Backup.Frequency,
                "1",
                L("DisplayName:EasyAbp.FileManagement.Backup.Frequency"),
                L("Description:EasyAbp.FileManagement.Backup.Frequency"),
                isVisibleToClients: false)
                .WithProperty("Type", "number")
                .WithProperty("Group1", "FileManagement")
                .WithProperty("Group2", "Backup")
            );

            context.Add(new SettingDefinition(
                FileManagementSettings.Backup.MaxCount,
                "5",
                L("DisplayName:EasyAbp.FileManagement.Backup.MaxCount"),
                L("Description:EasyAbp.FileManagement.Backup.MaxCount"),
                isVisibleToClients: false)
                .WithProperty("Type", "number")
                .WithProperty("Group1", "FileManagement")
                .WithProperty("Group2", "Backup")
            );

            context.Add(new SettingDefinition(
                FileManagementSettings.Backup.StartTime,
                "2",
                L("DisplayName:EasyAbp.FileManagement.Backup.StartTime"),
                L("Description:EasyAbp.FileManagement.Backup.StartTime"),
                isVisibleToClients: false)
                .WithProperty("Type", "number")
                .WithProperty("Group1", "FileManagement")
                .WithProperty("Group2", "Backup")
            );

            context.Add(new SettingDefinition(
                FileManagementSettings.Backup.EndTime,
                "6",
                L("DisplayName:EasyAbp.FileManagement.Backup.EndTime"),
                L("Description:EasyAbp.FileManagement.Backup.EndTime"),
                isVisibleToClients: false)
                .WithProperty("Type", "number")
                .WithProperty("Group1", "FileManagement")
                .WithProperty("Group2", "Backup")
            );

            context.Add(new SettingDefinition(
                FileManagementSettings.Backup.CyclicCovering,
                "True",
                L("DisplayName:EasyAbp.FileManagement.Backup.CyclicCovering"),
                L("Description:EasyAbp.FileManagement.Backup.CyclicCovering"),
                isVisibleToClients: false)
                .WithProperty("Type", "select")
                .WithProperty("Options", "True|False")
                .WithProperty("Group1", "FileManagement")
                .WithProperty("Group2", "Backup")
            );
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<FileManagementResource>(name);
        }
    }
}