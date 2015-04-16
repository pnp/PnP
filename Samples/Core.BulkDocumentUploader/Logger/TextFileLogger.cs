namespace Contoso.Core
{
    using System;
    using System.IO;

    /// <summary>
    /// Text File Logger
    /// </summary>
    public class TextFileLogger : LoggerBase
    {
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

        private int _ExceptionCount = 0;

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
            }
        }

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public override void LogException(string message, Exception ex)
        {
            if (!string.IsNullOrEmpty(this.LogFileLocation))
            {
                using (TextWriter writer = TextWriter.Synchronized(File.AppendText(this.LogFileLocation)))
                {
                    writer.WriteLine("{0} - EXCEPTION: '{1}'", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), message);
                    _ExceptionCount++;
                }
            }
        }

        public override void LogVerbose(string message)
        {
            if (!string.IsNullOrEmpty(this.LogFileLocation))
            {
                using (TextWriter writer = TextWriter.Synchronized(File.AppendText(this.LogFileLocation)))
                {
                    writer.WriteLine("{0} - {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), message);
                }
            }
        }

        public override void Cleanup()
        {
        }

        #endregion Methods
    }
}