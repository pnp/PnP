using System;
using System.Configuration;
using System.IO;

namespace Contoso.Office365.common
{
    public class Log
    {
        static FileStream ostrm;
        static StreamWriter writer;
        static string outputFolderLocation = string.Empty;
        static string outputFileName = string.Empty;
        const string APP_NAME = "ExternalSharing";
        static Guid transaction = Guid.NewGuid();

        public static void LogFileSystem(string message)
        {
            try
            {
                CheckLogFiles(); // Cleanup existing old logs
                string path = GetFilePath();
                using (ostrm = new FileStream(path, FileMode.Append, FileAccess.Write))
                {
                    writer = new StreamWriter(ostrm);
                    writer.WriteLine("{0}[{1}]\t{2}", DateTime.Now.ToShortTimeString(), transaction, message);
                    writer.Close();
                }
            }
            catch (Exception)
            {
            }
        }

        public static string GetFilePath()
        {
            outputFolderLocation = ConfigurationManager.AppSettings["OutputFileLocation"];
            outputFileName = string.Format("{0}_{1}.log", APP_NAME, DateTime.Now.ToString("dd-MM-yyyy"));
            return outputFolderLocation + @"\" + outputFileName;
        }

        /// <summary>
        /// Delete old log files, 3 days ago
        /// </summary>
        public static void CheckLogFiles()
        {
            try
            {
                var logFolder = ConfigurationManager.AppSettings["OutputFileLocation"];

                var logFiles = Directory.GetFiles(logFolder, "*.log", SearchOption.TopDirectoryOnly);
                if (logFiles == null || logFiles.Length == 0) return;

                //var assemblyName = GetAppAssembly().GetName().Name;
                var assemblyName = APP_NAME;

                foreach (string logFile in logFiles)
                {
                    if (logFile.Contains(assemblyName + "_"))
                    {
                        if (File.GetCreationTime(logFile) < DateTime.Now.AddDays(-3))
                        {
                            File.Delete(logFile);
                        }
                    }
                }
            }
            catch { }
        }
    }
}