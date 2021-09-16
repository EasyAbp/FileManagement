using Dapper;
using EasyAbp.FileManagement.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;


namespace EasyAbp.FileManagement.Files
{
    public class FileDapperRepository : ITransientDependency, IFileDapperRepository
    {
        public FileDapperRepository()
        {
        }

        public async Task BackupDatabase(string dbTypeName, string fullName,string databaseName, string connectionString)
        {            
            switch (dbTypeName)
            {
                case "MySqlConnection":
                    await BackupMySql(fullName, connectionString);
                    break;
                default:
                    await BackupSqlServer(databaseName, fullName, connectionString);
                    break;
            }
        }

        

        private async Task BackupSqlServer(string databaseName, string fileName, string connectionString)
        {
            using (IDbConnection db = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                var backupStr = $"BACKUP DATABASE {databaseName} TO DISK = '{fileName}'";
                await db.ExecuteAsync(backupStr);
            }
        }
        private async Task BackupMySql(string fileName, string connectionString)
        {            
            // Important Additional Connection Options
            connectionString += "charset=utf8;convertzerodatetime=true;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd))
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        mb.ExportToFile(fileName);
                        conn.Close();
                    }
                }
            }
            await Task.CompletedTask;
        }
    }
}
