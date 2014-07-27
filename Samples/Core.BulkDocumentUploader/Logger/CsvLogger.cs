namespace Contoso.Core
{
    using System;
    using System.IO;

    /// <summary>
    /// CSV Logger
    /// </summary>
    public class CsvLogger : LoggerBase
    {
        #region Fields

        /// <summary>
        /// The CSV file column headers
        /// </summary>
        private const string CsvHeader = "Timestamp,MySiteURL,Outcome";

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the log file location.
        /// </summary>
        /// <value>
        /// The log file location.
        /// </value>
        public string LogFileLocation
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Initialise this instance.
        /// </summary>
        public override void Initialise()
        {
            if (!string.IsNullOrEmpty(this.LogFileLocation))
            {
                if (File.Exists(this.LogFileLocation))
                {
                    File.Delete(this.LogFileLocation);
                }

                using (StreamWriter file = File.AppendText(this.LogFileLocation))
                {
                    file.WriteLine(CsvHeader);
                }
            }
        }

        /// <summary>
        /// Logs outcome of file upload to OneDrive URL in SPO.
        /// </summary>
        /// <param name="message">The OneDrive URL</param>
        /// <param name="ex">The success/failure value</param>
        public override void LogOutcome(string URL, string value)
        {
            if (!string.IsNullOrEmpty(this.LogFileLocation))
            {
                using (TextWriter writer = TextWriter.Synchronized(File.AppendText(this.LogFileLocation)))
                {
                    writer.WriteLine("\"{0}\",\"{1}\",\"{2}\"", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), URL, value);
                }
            }
        }

        public override void Cleanup()
        {
        }


        #endregion Methods
    }
}