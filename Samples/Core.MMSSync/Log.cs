using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MMSSync
{
    public enum EventId
    {
        Unknown = 0,

        LanguageMismatch = 10,
        InitializationDone = 11,
        GetChangesFrom = 12,
        SyncError = 13,
        ChangeProcessingDone = 14,

        CopyTermGroup_Skip = 20,
        CopyTermGroup_IDMismatch = 21,
        CopyTermGroup_AlreadyCopied = 22,
        CopyTermGroup_Copying = 23,

        TaxonomySession_Open = 30,

        TermStore_GetChangeLog = 40,
        TermStore_ProcessChangeLogEntry = 41,
        TermStore_SkipChangeLogEntry = 42,
        TermStore_NumberOfChanges = 43,

        TermGroup_Delete = 50,
        TermGroup_Edit = 51,
        TermGroup_Add = 52,
        TermGroup_IsSystemGroup = 53,
        TermGroup_AlreadyExists = 54,
        TermGroup_IDMismatch = 55,

        TermSet_Delete = 70,
        TermSet_Edit = 71,
        TermSet_Add = 72,
        TermSet_Copy = 73,
        TermSet_Move = 74,
        TermSet_AlreadyExists = 75,
        TermSet_NotFoundCreating = 76,
        TermSet_Skip = 77,
        TermSet_Skip_Inclusion = 78,

        Term_Delete = 90,
        Term_Edit = 91,
        Term_Add = 92,
        Term_Copy = 93,
        Term_Move = 94,
        Term_Merge = 95,
        Term_AlreadyExists = 96,
        Term_NotFoundCreating = 97,
        Term_Skip = 98,
        Term_Skip_Inclusion = 99,
    }

    /// <summary>
    /// This class is used to log events which occur in OfficeDevPnP. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// Without configuration, tracing will be switched off (SourceLevels.Off) 
    /// and only configured with the Default (debug) trace listener.
    /// </para>
    /// <para>
    /// Tracing to the console can be switched on with the following code:
    /// </para>
    /// <code>
    ///   Kbc.SharePoint.MMSSync.Engine.LoggingUtility.Internal.Source.Switch.Level = SourceLevels.Information;
    ///   Kbc.SharePoint.MMSSync.Engine.LoggingUtility.Internal.Source.Listeners.Add(new ConsoleTraceListener() { Name = "Console" });
    /// </code>
    /// <para>
    /// Alternatively, the trace can be configured in App.config.
    /// </para>
    /// <para>
    /// For extended logging, including a coloured console logger, add the following nuget package:
    /// </para>
    /// <code>
    ///   Install-Package Essential.Diagnostics.Config
    /// </code>
    /// </remarks>
    public sealed partial class Log
    {
        const int InitializeBehaviourEventId = 100;
        static Log _internal;
        static readonly object _lockObj = new object();

        /// <summary>
        /// The Default trace source, which should be used for all internal logging.
        /// </summary>
        /// <remarks>
        /// Applications should use their own trace source names.
        /// </remarks>
        public static Log Internal
        {
            get
            {
                if (_internal == null)
                {
                    lock (_lockObj)
                    {
                        _internal = new Log("Kbc.SharePoint.MMSSync.Engine");
                    }
                }
                return _internal;
            }
        }

        /// <summary>
        /// Creates a new instance with the specified TraceSource name.
        /// </summary>
        public Log(string name)
        {
            Source = new TraceSource(name);
        }

        public TraceSource Source { get; private set; }

        /// <summary>
        /// Creates a new instance with the specified TraceSource name.
        /// </summary>
        public static Log Create(string name)
        {
            return new Log(name);
        }

        /// <summary>
        /// Write a verbose message with specifed args, with id 0, to the trace.
        /// </summary>
        public void TraceVerbose([Localizable(false)] string message, params object[] args)
        {
            TraceVerbose(0, message, args);
        }

        /// <summary>
        /// Write a verbose message with specifed id and args to the trace.
        /// </summary>
        public void TraceVerbose(int id, [Localizable(false)] string message, params object[] args)
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
            var messageWithException = string.Format("{0}; EXCEPTION: {{{1}}}", message, args.Length);
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
            var messageWithException = string.Format("{0}; EXCEPTION: {{{1}}}", message, args.Length);
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
            var messageWithException = string.Format("{0}; EXCEPTION: {{{1}}}", message, args.Length);
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

        //// Initial behaviour compatible with old logging, which hard hard coded to write to console and debug
        //static void InitializeBehaviour()
        //{
        //    // If default settings, then change
        //    if (Internal.Source.Listeners.Count == 1 && Internal.Source.Listeners[0].Name.Equals("Default") && Internal.Source.Switch.Level == SourceLevels.Off)
        //    {
        //        Internal.Source.Listeners.Clear();
        //        Internal.Source.Listeners.Add(new ConsoleTraceListener() { Name = "Console" });
        //        Internal.Source.Listeners.Add(new DefaultTraceListener() { Name = "Default" });
        //        Internal.Source.Switch.Level = SourceLevels.Information;
        //        Internal.Source.TraceEvent(TraceEventType.Information, InitializeBehaviourEventId, "Trace initialized to write all events to Console and Default.");
        //    }
        //}

    }

}
