namespace Contoso.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Log Helper
    /// </summary>
    public class LogHelper : IDisposable
    {
        #region Fields

        /// <summary>
        /// The instance
        /// </summary>
        private static volatile LogHelper instance;

        /// <summary>
        /// The logger instances
        /// </summary>
        private static LoggerCollection loggerInstances;

        /// <summary>
        /// The synchronize root
        /// </summary>
        private static object syncRoot = new object();
        private static bool verbose = false;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="LogHelper"/> class from being created.
        /// </summary>
        private LogHelper()
        {
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Instances the specified loggers.
        /// </summary>
        /// <param name="loggers">The loggers.</param>
        /// <returns>An instance of the log helper</returns>
        public static LogHelper Instance(LoggerCollection loggers, bool logVerbose = false)
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        verbose = logVerbose;
                        loggerInstances = loggers;
                        instance = new LogHelper();

                        foreach (var logger in loggers)
                        {
                            logger.Initialise();
                        }
                    }
                }
            }

            return instance;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (loggerInstances != null)
            {
                foreach (var logger in loggerInstances)
                {
                    logger.Cleanup();
                }
            }
        }

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public void LogException(string message, Exception ex)
        {
            if (loggerInstances != null)
            {
                if (loggerInstances != null)
                {
                    var loggersByType = GetLoggersByType(TraceLevel.Error);

                    foreach (var logger in loggersByType)
                    {
                        logger.LogException(message, ex);
                    }
                }
            }
        }

        public void LogVerbose(string message)
        {
            if (loggerInstances != null)
            {
                var loggersByType = GetLoggersByType(TraceLevel.Verbose);

                foreach (var logger in loggersByType)
                {
                    logger.LogVerbose(message);
                }
            }
        }

        public void LogOutcome(string user, string value)
        {
            if (loggerInstances != null)
            {
                var loggersByType = GetLoggersByType(TraceLevel.Verbose);

                foreach (var logger in loggersByType)
                {
                    logger.LogOutcome(user, value);
                }
            }
        }

        private List<LoggerBase> GetLoggersByType(TraceLevel type)
        {
            List<LoggerBase> loggersToIvoke = new List<LoggerBase>();

            if (loggerInstances != null)
            {
                foreach (var logger in loggerInstances)
                {
                    if (logger.TraceTypes != null && logger.TraceTypes.Contains(type))
                    {
                        loggersToIvoke.Add(logger);
                    }
                }
            }

            return loggersToIvoke;
        }

        #endregion Methods
    }
}