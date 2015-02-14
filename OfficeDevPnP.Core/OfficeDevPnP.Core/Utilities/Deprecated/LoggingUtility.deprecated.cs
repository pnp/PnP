using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OfficeDevPnP.Core.Utilities 
{

    [Obsolete("Please use the Log.Debug, Log.Info, Log.Warning, Log.Error and Log.Fatal methods. This deprecated enum will be removed in the May release.")]
    public enum EventId
    {
        // Format: ABXX
        // A: 1 = preliminary, 2 = completion, 3 = frequent, 4 = transitive negative, 
        //    5 = permanent negative, 8 = finalization, 9 = unknown
        // B: 0 = syntax, 1 = control, 2 = connection, 3 = authentication, 9 = unknown
        //    4 = search, 5 = provisioning, 6 = branding, 7 = workflow
        // XX: sequential ids
        Unknown = 0,


        AuthenticationContext = 1301,
        ServicesTokenRefreshed = 1302,
        
        UploadFile = 1501,
        InstallSolution = 1502,
        CreateWeb = 1503,
        CreateSiteCollection = 1504,
        CreateField = 1506,
        CreateContentType = 1507,
        CreateListField = 1508,
        CreateTermGroup = 1509,
        CreateTermSet = 1510,
        ImportTermSet = 1511,
        CreateList = 1512,
        ProvisionElementFile = 1513,
        CreateFolder = 1514,

        DeployTheme = 1601,
        CreateComposedLook = 1602,
        //DeployThemeFile = 1603,
        DeployMasterPage = 1604,
        DeployPageLayout = 1605,

        SendMailCancelled = 2201,

        ServicesRegistered = 2301,

        ActivateWebFeature = 2501,
        ActivateSiteCollectionFeature = 2502,
        AddFieldToContentType = 2503,
        CreateTerm = 2504,
        UpdateFileProperties = 2505,
        
        SetTheme = 2601,
        SetMasterUrl = 2602,
        SetCustomMasterUrl = 2603,
        UpdateComposedLook = 2604,

        ClosedContextWarning = 4201,
        
        ProblemDeterminingTokenLease = 4301,

        FieldAlreadyExists = 4501,
        ContentTypeAlreadyExists = 4502,
        ProvisionTaxonomyUpdateException = 4503,
        ProvisionTaxonomyIdMismatch = 4504,

        //ThemeNotOverwritten = 4601,

        MailSendException = 5201,
        MailSendFailed = 5202,

        ServicesRegistrationFailed = 5301,

        ServicesNoCachedItem = 5301,

        SiteSearchUnhandledException = 5401,

        LibraryMissing = 5501,
        FolderMissing = 5502,
        FeatureActivationProblem = 5503,
        ProvisionTaxonomyImportErrorName = 5504,
        ProvisionTaxonomyImportErrorTagging = 5505,
        ProvisionTaxonomyImportErrorDescription = 5506,
        ProvisionTaxonomyImportErrorDelete = 5507,
        ProvisionModuleFileError = 5508,

        ThemeMissing = 5601,

        UnknownExceptionAccessingSite = 5901,

        DeleteWeb = 8501,
        DeleteSiteCollection = 8502,
        RemoveAppInstance = 8503,
        DeactivateWebFeature = 8504,
        DeactivateSiteCollectionFeature = 8505,
        UninstallSolution = 8506,
        DeleteTerm = 8507,
    }

    /// <summary>
    /// Logging event categories enumeration
    /// </summary>
    [Obsolete("Please use the Log.Debug, Log.Info, Log.Warning, Log.Error and Log.Fatal methods. This deprecated enum will be removed in the May release.")]
    public enum EventCategory
    {
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
    [Obsolete("Please use the Log.Debug, Log.Info, Log.Warning, Log.Error and Log.Fatal methods. This deprecated enum will be removed in the May release.")]
    public enum EventLevel
    {
        Information,
        Warning,
        Error,
        Verbose
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
    ///   OfficeDevPnP.Core.Utilities.LoggingUtility.Internal.Source.Switch.Level = SourceLevels.Information;
    ///   OfficeDevPnP.Core.Utilities.LoggingUtility.Internal.Source.Listeners.Add(new ConsoleTraceListener() { Name = "Console" });
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
    [Obsolete("Please use the Log.Debug, Log.Info, Log.Warning, Log.Error and Log.Fatal methods. This deprecated class will be removed in the May release.")]
    public sealed partial class LoggingUtility 
    {
        const int InitializeBehaviourEventId = 100;
        static LoggingUtility _internal;
        static readonly object _lockObj = new object();

        /// <summary>
        /// The Default trace source, which should be used for all internal logging.
        /// </summary>
        /// <remarks>
        /// Applications should use their own trace source names.
        /// </remarks>
        public static LoggingUtility Internal {
            get {
                if (_internal == null) {
                    lock (_lockObj) {
                        _internal = new LoggingUtility("OfficeDevPnP.Core");
                    }
                }
                return _internal;
            }
        }

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
        [Obsolete("Create a named instance or, internally, use LoggingUtility.Internal.TraceVerbose()")]
        [EditorBrowsable(EditorBrowsableState.Never)]
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
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "OfficeDevPnP.Core.Utilities.LoggingUtility.TraceInformation(System.Int32,System.String,System.Object[])")]
        [Obsolete("Create a named instance or, internally, use LoggingUtility.Internal.TraceInformation()")]
        [EditorBrowsable(EditorBrowsableState.Never)]
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
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "OfficeDevPnP.Core.Utilities.LoggingUtility.TraceWarning(System.Int32,System.String,System.Object[])")]
        [Obsolete("Create a named instance or, internally, use LoggingUtility.Internal.TraceWarning()")]
        [EditorBrowsable(EditorBrowsableState.Never)]
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
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "OfficeDevPnP.Core.Utilities.LoggingUtility.TraceError(System.Int32,System.String,System.Object[])")]
        [Obsolete("Create a named instance or, internally, use LoggingUtility.Internal.TraceError()")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void LogError(string message, Exception ex, EventCategory category)
        {
            InitializeBehaviour();
            Internal.TraceError(50 + (int)category, "{0,-15}: {1}\r\nEXCEPTION: {2}", category, message, ex);
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
            var messageWithException = string.Format(CoreResources.LoggingUtility_MessageWithException, message, args.Length);
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
            var messageWithException = string.Format(CoreResources.LoggingUtility_MessageWithException, message, args.Length);
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
            var messageWithException = string.Format(CoreResources.LoggingUtility_MessageWithException, message, args.Length);
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
