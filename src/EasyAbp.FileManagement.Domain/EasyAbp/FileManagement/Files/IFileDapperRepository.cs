using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EasyAbp.FileManagement.Files
{
    public interface IFileDapperRepository
    {
        Task BackupDatabase(string dbTypeName, string fullName, string databaseName, string connectionString);
    }
}
