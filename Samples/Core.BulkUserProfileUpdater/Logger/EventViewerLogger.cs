namespace Contoso.Core.Logger
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Event Viewer Logger
    /// </summary>
    public class EventViewerLogger : LoggerBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the event identifier.
        /// </summary>
        /// <value>
        /// The event identifier.
        /// </value>
        public int EventId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the event log.
        /// </summary>
        /// <value>
        /// The name of the event log.
        /// </value>
        public string EventLogName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the event source.
        /// </summary>
        /// <value>
        /// The event source.
        /// </value>
        public string EventSource
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public override void LogException(string message, Exception ex)
        {
            if (!EventLog.SourceExists(this.EventSource))
            {
                EventLog.CreateEventSource(this.EventSource, this.EventLogName);
            }

            EventLog.WriteEntry(this.EventSource, ex.ToString(), EventLogEntryType.Error, this.EventId);
        }

        public override void LogVerbose(string message)
        {
            if (!EventLog.SourceExists(this.EventSource))
            {
                EventLog.CreateEventSource(this.EventSource, this.EventLogName);
            }

            EventLog.WriteEntry(this.EventSource, message, EventLogEntryType.Information, this.EventId);
        }

        #endregion Methods
    }
}