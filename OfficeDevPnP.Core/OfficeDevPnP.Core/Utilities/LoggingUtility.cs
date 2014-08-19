using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Utilities 
{

    public enum EventId
    {
        // Format: ABXX
        // A: 1 = preliminary, 2 = completion, 3 = frequent, 4 = transitive negative, 
        //    5 = permanent negative, 8 = finalization, 9 = unknown
        // B: 0 = syntax, 1 = control, 2 = connection, 3 = authentication, 9 = unknown
        //    4 = search, 5 = provisioning, 6 = branding, 7 = workflow
        // XX: sequential ids

        AuthenticationContext = 1301,
        DeployTheme = 1601,

        SetTheme = 2601,

        SiteSearchUnhandledException = 5401,

    }

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
    /// This class is used to log events which occur in OfficeDevPnP. 
    /// </summary>
    public partial class LoggingUtility 
    {
        const int InitializeBehaviourEventId = 100;

        /// <summary>
        /// The Default trace source, which should be used for all internal logging.
        /// </summary>
        /// <remarks>
        /// Applications should use their own trace source names.
        /// </remarks>
        public static LoggingUtility Internal = new LoggingUtility("OfficeDevPnP.Core");

        /// <summary>
        /// Creates a new instance with the specified TraceSource name.
        /// </summary>
        public LoggingUtility(string name)
        {
            Source = new TraceSource(name);
        }

        public TraceSource Source { get; private set; }

        /// <summary>
        /// Creates a new instance with the specified TraceSource name.
        /// </summary>
        public static LoggingUtility Create(string name)
        {
            return new LoggingUtility(name);
        }

        /// <summary>
        /// Logs verbose message to event log.
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="category">Category to be used for the logged message</param>
        //[Obsolete("Create an instance with a named trace source and use TraceVerbose()")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void LogVerbose(string message, EventCategory category) 
        {
            InitializeBehaviour();
            Internal.TraceVerbose((int)category, "{0,-15}: {1}", category, message);
        }

        /// <summary>
        /// Logs information message to the event log.
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="category">Category to be used for the logged message</param>
        //[Obsolete("Create an instance with a named trace source and use TraceInformation()")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void LogInformation(string message, EventCategory category)
        {
            InitializeBehaviour();
            Internal.TraceInformation(20 + (int)category, "{0,-15}: {1}", category, message);
        }

        /// <summary>
        /// Logs warning message to the event log.
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="ex">Exception to be logged, null can be passed if there are no exception details</param>
        /// <param name="category">Category to be used for the logged message</param>
        //[Obsolete("Create an instance with a named trace source and use TraceWarning()")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void LogWarning(string message, Exception ex, EventCategory category)
        {
            InitializeBehaviour();
            Internal.TraceWarning(40 + (int)category, "{0,-15}: {1}\r\nEXCEPTION: {2}", category, message, ex);
        }

        /// <summary>
        /// Logs error message to the event log.
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="ex">Exception to be logged, null can be passed if there are no exception details</param>
        /// <param name="category">Category to be used for the logged message</param>
        //[Obsolete("Create an instance with a named trace source and use TraceError()")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void LogError(string message, Exception ex, EventCategory category)
        {
            InitializeBehaviour();
            Internal.TraceError(50 + (int)category, "{0,-15}: {1}\r\nEXCEPTION: {2}", category, message, ex);
        }

        /// <summary>
        /// Write a verbose message with specifed args, with id 0, to the trace.
        /// </summary>
        public void TraceVerbose(string message, params object[] args)
        {
            TraceVerbose(0, message, args);
        }

        /// <summary>
        /// Write a verbose message with specifed id and args to the trace.
        /// </summary>
        public void TraceVerbose(int id, string message, params object[] args)
        {
            Source.TraceEvent(TraceEventType.Verbose, id, message, args);
        }

        /// <summary>
        /// Write an information message with specifed id and args to the trace.
        /// </summary>
        public void TraceInformation(int id, string message, params object[] args)
        {
            Source.TraceEvent(TraceEventType.Information, id, message, args);
        }

        /// <summary>
        /// Write a warning message with specifed id, exception and args to the trace.
        /// </summary>
        public void TraceWarning(int id, Exception ex, string message, params object[] args)
        {
            var messageWithException = message + "; EXCEPTION: {" + args.Length.ToString() + "}";
            var argsWithException = args.Concat(new[] { ex }).ToArray();
            TraceWarning(id, messageWithException, argsWithException);
        }

        /// <summary>
        /// Write a warning message with specifed id and args to the trace.
        /// </summary>
        public void TraceWarning(int id, string message, params object[] args)
        {
            Source.TraceEvent(TraceEventType.Warning, id, message, args);
        }

        /// <summary>
        /// Write a error message with specifed id, exception and args to the trace.
        /// </summary>
        public void TraceError(int id, Exception ex, string message, params object[] args)
        {
            var messageWithException = message + "; EXCEPTION: {" + args.Length.ToString() + "}";
            var argsWithException = args.Concat(new[] { ex }).ToArray();
            TraceError(id, messageWithException, argsWithException);
        }

        /// <summary>
        /// Write a error message with specifed id and args to the trace.
        /// </summary>
        public void TraceError(int id, string message, params object[] args)
        {
            Source.TraceEvent(TraceEventType.Error, id, message, args);
        }

        /// <summary>
        /// Write a critical message with specifed id, exception and args to the trace.
        /// </summary>
        public void TraceCritical(int id, Exception ex, string message, params object[] args)
        {
            var messageWithException = message + "; EXCEPTION: {" + args.Length.ToString() + "}";
            var argsWithException = args.Concat(new[] { ex }).ToArray();
            TraceCritical(id, messageWithException, argsWithException);
        }

        /// <summary>
        /// Write a critical message with specifed id and args to the trace.
        /// </summary>
        public void TraceCritical(int id, string message, params object[] args)
        {
            Source.TraceEvent(TraceEventType.Critical, id, message, args);
        }

        // Initial behaviour compatible with old logging, which hard hard coded to write to console and debug
        static void InitializeBehaviour()
        {
            // If default settings, then change
            if (Internal.Source.Listeners.Count == 1 && Internal.Source.Listeners[0].Name.Equals("Default") && Internal.Source.Switch.Level == SourceLevels.Off)
            {
                Internal.Source.Listeners.Clear();
                Internal.Source.Listeners.Add(new ConsoleTraceListener() { Name = "Console" });
                Internal.Source.Listeners.Add(new DefaultTraceListener() { Name = "Default" });
                Internal.Source.Switch.Level = SourceLevels.Information;
                Internal.Source.TraceEvent(TraceEventType.Information, InitializeBehaviourEventId, "Trace initialized to write all events to Console and Default.");
            }
        }

    }
}
