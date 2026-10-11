using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WindowsServiceFileMonitoring
{
    public class clsFilesMonitor
    {
        private FileSystemWatcher watcher;

        private string sourcePath { get; set; }

        private string destinationPath { get; set; }
        private string logsPath { get; set; }

        private string LogsFileName { get; set; } = "FileMonitoringLog.txt";
        public clsFilesMonitor(string source, string destination , string LogsPath , string LogFileName = "FileMonitoringLog.txt")
        {
            watcher = new FileSystemWatcher(source);
            sourcePath = source;
            destinationPath = destination;
            logsPath = LogsPath;
            watcher.Filter = "*.*";
            this.LogsFileName = LogFileName;
            watcher.EnableRaisingEvents = true;
           
        }


        public void Log(string message) {
            string FullLogPath = Path.Combine(this.logsPath , this.LogsFileName);

            if (!File.Exists(FullLogPath))
            {
                File.Create(FullLogPath).Dispose();
            }
            File.AppendAllText(FullLogPath, $"[{DateTime.UtcNow}] {message}\n");
        }

        private void DetectDeletionOfFile(object sender, FileSystemEventArgs e)
        {
            try
            { 
                Log($"Source file deleted: {e.FullPath}");
            }
            catch (Exception ex)
            {
                Log($"Error deleting source file {e.FullPath}: {ex.Message}");
            }
        }

        private void DeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    Log($"File deleted: {filePath}");
                }
                else
                {
                    Log($"File not found for deletion: {filePath}");
                }
            }
            catch (Exception ex)
            {
                Log($"Error deleting file {filePath}: {ex.Message}");
            }
        }

        private void OnFileAdded(object sender, FileSystemEventArgs e)
        {
            Log($"File was created at :{e.FullPath}");
            string sourceFilePath = this.sourcePath;
            string sourceFileName = Path.GetFileName(e.FullPath);
            string NewFileName = Guid.NewGuid().ToString() + Path.GetExtension(e.FullPath);
            try
            {
                File.Copy(Path.Combine(sourceFilePath, sourceFileName), Path.Combine(destinationPath, NewFileName), true);    
                    Log($"File {sourceFileName} copied to {this.destinationPath} successfully with New GUID [{NewFileName}].");
                    DeleteFile(Path.Combine(sourceFilePath, sourceFileName));
            }
            catch (Exception ex)
            {
               Log($"Error copying file {sourceFileName} to {NewFileName}: {ex.Message}");
            }
        }

        public void StartMonitoring()
        {
            watcher.Created += OnFileAdded;
            watcher.Deleted += DetectDeletionOfFile;
        }

        public void StopMonitoring()
        {
            watcher.Created -= OnFileAdded;
            watcher.Deleted -= DetectDeletionOfFile;
            watcher.Dispose();
            watcher.EnableRaisingEvents = false;
        }
    }
}
