using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Utilities
{
    /// <summary>
    /// Logging class
    /// </summary>
    public class Log
    {
        #region Public Members
        /// <summary>
        /// Increases the current IndentLevel by one.
        /// </summary>
        public static void Indent()
        {
            Trace.Indent();
        }

        /// <summary>
        /// Decreases the current IndentLevel by one.
        /// </summary>
        public static void Unindent()
        {
            Trace.Unindent();
        }

        /// <summary>
        /// Writes out Debug messages
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Debug(string source, string message, params object[] args)
        {
#if DEBUG
            var log = GetLogEntry(source, message, args);
            Trace.TraceInformation(log);
#endif
        }

        /// <summary>
        /// Writes out Info Messages
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Info(string source, string message, params object[] args)
        {
            var log = GetLogEntry(source, message, args);
            Trace.TraceInformation(log);
        }

        /// <summary>
        /// Write out Warning Messages
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Warning(string source, string message, params object[] args)
        {
            var log = GetLogEntry(source, message, args);
            Trace.TraceWarning(log);
        }

        /// <summary>
        /// Write out Error Messages
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Error(string source, string message, params object[] args)
        {
            var log = GetLogEntry(source, message, args);
            Trace.TraceError(log);
        }

        /// <summary>
        /// Writes out Fatal Error Messages
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Fatal(string source, string message, params object[] args)
        {
            var log = GetLogEntry(source, message, args);
            Trace.Fail(log);
        }

     
        /// <summary>
        /// TraceAPI - trace inter-service calls (including latency) 
        /// </summary>
        /// <param name="componentName"></param>
        /// <param name="method"></param>
        /// <param name="timespan"></param>
        public static void TraceApi(string componentName, string method, TimeSpan timespan)
        {
            TraceApi(componentName, method, timespan, "");
        }

        public static void TraceApi(string componentName, string method, TimeSpan timespan, string fmt, params object[] vars)
        {
            TraceApi(componentName, method, timespan, string.Format(fmt, vars));
        }

        public static void TraceApi(string componentName, string method, TimeSpan timespan, string properties)
        {
            string message = String.Concat("Component:",
                    componentName,
                    " method:", method,
                    " timespan:", timespan.ToString(),
                    " properties:", properties);
            Trace.TraceInformation(message);
        } 

        #endregion

        #region Private Members
        /// <summary>
        /// Helper Method to format error messages
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static string GetLogEntry(string source, string message, params object[] args)
        {
            try
            {
                if (args == null || args.Length == 0)
                    message = message.Replace("{", "{{").Replace("}", "}}");
                string _msg = String.Format(System.Globalization.CultureInfo.CurrentCulture, message, args);
                string _log = string.Format(System.Globalization.CultureInfo.CurrentCulture, "[{0}] {1}", source, _msg);
                return _log;
            }
            catch (Exception e)
            {
                return string.Format("Error while generating log information, {0}", e);
            }
        }
        #endregion
    }
}
