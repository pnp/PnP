using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Utilities {
    
    /// <summary>
    /// Logging event categories enumeration
    /// </summary>
    public enum EventCategory {
        Unknown,
        Mail,
        Authorization,
        Site,
        Features,
        FieldsAndContentTypes
        // TODO: Add more exception categories here
    }

    /// <summary>
    /// Logging event severity level enumeration
    /// </summary>
    public enum EventLevel {
        Information,
        Warning,
        Error,
        Verbose
    }

    /// <summary>
    /// This class is used to log events which occur in OfficeDevPnP. Note that it is a partial class,
    /// so adding another partial class to your source code to extend this class is a valuable option
    /// </summary>
    public static partial class LoggingUtility {
        /// <summary>
        /// Logs verbose message to event log.
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="category">Category to be used for the logged message</param>
        public static void LogVerbose(string message, EventCategory category) {
            LogBase(message, null, EventLevel.Verbose, category);
        }

        /// <summary>
        /// Logs information message to the event log.
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="category">Category to be used for the logged message</param>
        public static void LogInformation(string message, EventCategory category)
        {
            LogBase(message, null, EventLevel.Information, category);
        }

        /// <summary>
        /// Logs warning message to the event log.
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="ex">Exception to be logged, null can be passed if there are no exception details</param>
        /// <param name="category">Category to be used for the logged message</param>
        public static void LogWarning(string message, Exception ex, EventCategory category)
        {
            LogBase(message, ex, EventLevel.Warning, category);
        }

        /// <summary>
        /// Logs error message to the event log.
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="ex">Exception to be logged, null can be passed if there are no exception details</param>
        /// <param name="category">Category to be used for the logged message</param>
        public static void LogError(string message, Exception ex, EventCategory category)
        {
            LogBase(message, ex, EventLevel.Error, category);
        }

        /// <summary>
        /// Base logging implementation
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="ex">Exception to be logged, null can be passed if there are no exception details</param>
        /// <param name="level">Level to be used for the logged message</param>
        /// <param name="category">Category to be used for the logged message</param>
        static void LogBase(string message, Exception ex, EventLevel level, EventCategory category)
        {
            var msg = string.Format("{0} {1}: {2}", category.ToString().PadRight(15), level.ToString().PadRight(15), message);

            if (ex != null)
                msg += "\r\nEXCEPTION: " + ex;

            // Log to Console
            Console.WriteLine(msg);

            // Log to Debug
            Debug.WriteLine(msg);

            // TODO: Log to other logging providers here
        }
    }
}
