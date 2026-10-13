using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsServiceFileMonitoring;
namespace DataBaseBackupService
{
    public class clsDatabaseBackupProvider
    {
       private string _ConnectionString { get; set; }
        private string _BackupDirectory { get; set; }
        private string _DataBaseName { get; set; }
        private string _LogsDirectory { get; set; }
  

        public clsDatabaseBackupProvider(string connectionString, string backupDirectory, string logsDirectory , string DatabaseName)
        {
            _ConnectionString = connectionString;
            _BackupDirectory = backupDirectory;
            _LogsDirectory = logsDirectory;
            _DataBaseName = DatabaseName;
        }

        public void Log(string message ,string logsPath , string LogsFileName = "DB_BackupLogs.txt")
        {
            string FullLogPath = Path.Combine(logsPath,LogsFileName);

            if (!File.Exists(FullLogPath))
            {
                File.Create(FullLogPath).Dispose();
            }
            File.AppendAllText(FullLogPath, $"[{DateTime.UtcNow}] {message}\n");
        }

        public void ProvideBackUP()
        {
            string sqlCmd = $"BACKUP DATABASE [{this._DataBaseName}] TO DISK = @backupPath WITH INIT";

            try
            {
                using (SqlConnection cnx = new SqlConnection(this._ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlCmd, cnx))
                    {
                        cmd.Parameters.AddWithValue("@backupPath", Path.Combine(
                    this._BackupDirectory,
                    $"{this._DataBaseName}_{DateTime.Now:yyyyMMdd_HHmmss}.bak"));

                        cnx.Open();
                        cmd.ExecuteNonQuery();
                        Log("Database backup provided successfully", this._LogsDirectory);
                    }
                }

            }
            catch (Exception e)
            {
                Log(e.Message, this._LogsDirectory);
            }
        }
    }
    
}
