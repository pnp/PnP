using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Utilities 
{
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
    [Obsolete("Use System.Diagnostics.TraceEventType instead.")]
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
    public static partial class LoggingUtility 
    {
        const int InitializeBehaviourEventId = 100;

        static TraceSource source = new TraceSource("OfficeDevPnP.Core");

        /// <summary>
        /// Logs verbose message to event log.
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="category">Category to be used for the logged message</param>
        public static void LogVerbose(string message, EventCategory category) 
        {
            InitializeBehaviour();
            LogBase(message, null, TraceEventType.Verbose, category);
        }

        /// <summary>
        /// Logs information message to the event log.
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="category">Category to be used for the logged message</param>
        public static void LogInformation(string message, EventCategory category)
        {
            InitializeBehaviour();
            LogBase(message, null, TraceEventType.Information, category);
        }

        /// <summary>
        /// Logs warning message to the event log.
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="ex">Exception to be logged, null can be passed if there are no exception details</param>
        /// <param name="category">Category to be used for the logged message</param>
        public static void LogWarning(string message, Exception ex, EventCategory category)
        {
            InitializeBehaviour();
            LogBase(message, ex, TraceEventType.Warning, category);
        }

        /// <summary>
        /// Logs error message to the event log.
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="ex">Exception to be logged, null can be passed if there are no exception details</param>
        /// <param name="category">Category to be used for the logged message</param>
        public static void LogError(string message, Exception ex, EventCategory category)
        {
            InitializeBehaviour();
            LogBase(message, ex, TraceEventType.Error, category);
        }

        // Initial behaviour compatible with old logging, which hard hard coded to write to console and debug
        static void InitializeBehaviour()
        {
            if (source.Listeners.Count == 1 && source.Listeners[0].Name.Equals("Default"))
            {
                source.Listeners.Clear();
                source.Listeners.Add(new ConsoleTraceListener() { Name = "Console" });
                source.Listeners.Add(new DefaultTraceListener() { Name = "Default" });
                source.Switch.Level = SourceLevels.Information;
                source.TraceEvent(TraceEventType.Information, InitializeBehaviourEventId, "Trace initialized to write all events to Console and Default.");
            }
        }

        /// <summary>
        /// Base logging implementation
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="ex">Exception to be logged, null can be passed if there are no exception details</param>
        /// <param name="level">Level to be used for the logged message</param>
        /// <param name="category">Category to be used for the logged message</param>
        static void LogBase(string message, Exception ex, TraceEventType level, EventCategory category)
        {
            int traceId = 100 + (int)category;
            if (ex == null)
            {
                source.TraceEvent(level, traceId, "{0,-15}: {1}", category, message);
            }
            else 
            {
                source.TraceEvent(level, traceId, "{0,-15}: {1}\r\nEXCEPTION: {2}", category, message, ex);
            }
        }

    }
}
