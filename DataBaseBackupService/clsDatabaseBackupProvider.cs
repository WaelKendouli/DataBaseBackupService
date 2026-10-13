using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsServiceFileMonitoring;
using System.Threading;
namespace DataBaseBackupService
{
    public class clsDatabaseBackupProvider : IDisposable
    {
       private string _ConnectionString { get; set; }
        private string _BackupDirectory { get; set; }
        private string _DataBaseName { get; set; }
        private string _LogsDirectory { get; set; }

        private int _MinutesInterval { get; set; }
  
        private Timer _Timer { get; set; }
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);


        public clsDatabaseBackupProvider(string connectionString, string backupDirectory, string logsDirectory , string DatabaseName , int MinutesInterval)
        {
            _ConnectionString = connectionString;
            _BackupDirectory = backupDirectory;
            _LogsDirectory = logsDirectory;
            _DataBaseName = DatabaseName;
            _MinutesInterval = MinutesInterval;
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

        public void StartBackupTimer()
        {
            int intervalMs = this._MinutesInterval * 60 * 1000;

            _Timer = new Timer(RestartBackup, null, intervalMs, intervalMs);

        }

        private void RestartBackup(object state)
        {
            ProvideBackUP();
        }

        public void ProvideBackUP()
        {
            string sqlCmd = $"BACKUP DATABASE [{this._DataBaseName}] TO DISK = @backupPath WITH INIT";

            try
            {
                if (!_lock.Wait(0))
                {
                    return;
                }

                using (SqlConnection cnx = new SqlConnection(this._ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlCmd, cnx))
                    {
                        cmd.Parameters.AddWithValue("@backupPath", Path.Combine(
                    this._BackupDirectory,
                    $"{this._DataBaseName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.bak"));

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
            finally
            {
                _lock.Release();
            }
        }

        public void Dispose()
        {
            _Timer?.Dispose();
            _lock?.Dispose();
        }
    }
    
}
