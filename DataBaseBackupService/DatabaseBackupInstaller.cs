using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace DataBaseBackupService
{
    [RunInstaller(true)]
    public partial class DatabaseBackupInstaller : System.Configuration.Install.Installer
    {
        private ServiceProcessInstaller serviceProcessInstaller;
        private ServiceInstaller serviceInstaller;
        public DatabaseBackupInstaller()
        {
            InitializeComponent();

            // Configure the Service Process Installer
            serviceProcessInstaller = new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalSystem // Adjust as needed (e.g., NetworkService, LocalService)
            };

            // Configure the Service Installer
            serviceInstaller = new ServiceInstaller
            {
                ServiceName = "DataBaseBackupService", // Must match the ServiceName in your ServiceBase class
                DisplayName = "Database backup Service",
                StartType = ServiceStartMode.Manual ,// Or Automatic, depending on requirements
                ServicesDependedOn = new string[] { "MSSQLSERVER" , "RpcSs" , "EventLog" } // Specify the service dependencies here
            };

            // Add installers to the installer collection
            Installers.Add(serviceProcessInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
