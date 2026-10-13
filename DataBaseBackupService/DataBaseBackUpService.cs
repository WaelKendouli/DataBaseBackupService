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
        string LogsDirectory = ConfigurationManager.AppSettings["LogFolder"];

        protected override void OnStart(string[] args)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string BackupDirectory = ConfigurationManager.AppSettings["BackupFolder"];
            string DatabaseName = "TestDB";

            DB_Backup = new clsDatabaseBackupProvider(ConnectionString , BackupDirectory , LogsDirectory , DatabaseName);
            DB_Backup.Log("Service Started", LogsDirectory);
        }

        protected override void OnStop()
        {
            DB_Backup.Log("Service Stopped", LogsDirectory);
        }
    }
}
