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
        /// The CSV header
        /// </summary>
        private const string CsvHeader = "Timestamp,UserAccount,Outcome";

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
        /// Logs outcome of user account update in SPO.
        /// </summary>
        /// <param name="message">The user</param>
        /// <param name="ex">The success value</param>
        public override void LogOutcome(string user, string value)
        {
            if (!string.IsNullOrEmpty(this.LogFileLocation))
            {
                using (TextWriter writer = TextWriter.Synchronized(File.AppendText(this.LogFileLocation)))
                {
                    writer.WriteLine("\"{0}\",\"{1}\",\"{2}\"", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), user, value);
                }
            }
        }

        public override void Cleanup()
        {
            if (this.EmailSender != null)
            {
                string _CurrentTime = DateTime.Now.ToString("dd/MM/yyyy");
                string MailHeader = string.Format("{0} - User Profile Utility Outcomes", _CurrentTime);
                string MailBody = string.Format("The user profile utility was run on {0} and the attached CSV file summarises which accounts were successfully processedgh", _CurrentTime);
                EmailHelper.SendEmail(this.EmailSender.FromAddress, this.EmailSender.ToAddress, this.EmailSender.Host, this.EmailSender.Port, MailHeader, MailBody, this.LogFileLocation);
            }
        }


        #endregion Methods
    }
}