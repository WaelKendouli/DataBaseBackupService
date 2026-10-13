using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace DataBaseBackupService
{
    public partial class DataBaseBackUpService : ServiceBase
    {
        public DataBaseBackUpService()
        {
            InitializeComponent();
        }
        clsDatabaseBackupProvider DB_Backup;

        protected override void OnStart(string[] args)
        {
            string ConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
            string BackupDirectory = ConfigurationManager.AppSettings["BackupFolder"];
            string DatabaseName = "TestDB";
            string LogsDirectory = ConfigurationManager.AppSettings["LogFolder"];
            int MinutesInterval = Convert.ToInt32(ConfigurationManager.AppSettings["BackupIntervalMinutes"]);

            DB_Backup = new clsDatabaseBackupProvider(ConnectionString , BackupDirectory , LogsDirectory , DatabaseName, MinutesInterval);
            DB_Backup.ProvideBackUP();
            DB_Backup.StartBackupTimer();
            DB_Backup.Log("Service Started", LogsDirectory);
        }

        public void StartInConsole()
        {
            OnStart(null);
            Console.WriteLine("Service started. Press any key to stop...");
            Console.ReadKey();
            OnStop();
        }
        protected override void OnStop()
        {
           
            DB_Backup.Log("Service Stopped", ConfigurationManager.AppSettings["LogFolder"]);
            DB_Backup.Dispose();
        }
    }
}
