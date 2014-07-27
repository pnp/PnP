namespace Contoso.Core
{
    using System;
    using System.Diagnostics;
    using System.Xml.Serialization;

    /// <summary>
    /// Logger base class
    /// </summary>
    [XmlType("Log")]
    [Serializable]
    public abstract class LoggerBase
    {
        #region Properties

        [XmlArray]
        [XmlArrayItem("Trace")]
        public TraceLevel[] TraceTypes
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Cleanups this instance.
        /// </summary>
        public virtual void Cleanup()
        {
        }

        /// <summary>
        /// Initialise this instance.
        /// </summary>
        public virtual void Initialise()
        {
        }

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public virtual void LogException(string message, Exception ex)
        {
        }

        public virtual void LogVerbose(string message)
        {
        }

        public virtual void LogOutcome(string user, string value)
        {
        }
        
        #endregion Methods
    }
}