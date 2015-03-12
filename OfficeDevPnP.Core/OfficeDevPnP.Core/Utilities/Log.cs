using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Utilities
{
    /// <summary>
    /// Logging class
    /// </summary>
    public static class Log
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
        /// <param name="source">Source of the message</param>
        /// <param name="message">Message to log</param>
        /// <param name="args">Arguments to used for message completion</param>
        public static void Debug(string source, [Localizable(false)] string message, params object[] args)
        {
#if DEBUG
            var log = GetLogEntry(source, message, args);
            Trace.TraceInformation(log);
#endif
        }

        /// <summary>
        /// Writes out Info Messages
        /// </summary>
        /// <param name="source">Source of the message</param>
        /// <param name="message">Message to log</param>
        /// <param name="args">Arguments to used for message completion</param>
        public static void Info(string source, string message, params object[] args)
        {
            var log = GetLogEntry(source, message, args);
            Trace.TraceInformation(log);
        }

        /// <summary>
        /// Write out Warning Messages
        /// </summary>
        /// <param name="source">Source of the message</param>
        /// <param name="message">Message to log</param>
        /// <param name="args">Arguments to used for message completion</param>
        public static void Warning(string source, string message, params object[] args)
        {
            var log = GetLogEntry(source, message, args);
            Trace.TraceWarning(log);
        }

        /// <summary>
        /// Write out Error Messages
        /// </summary>
        /// <param name="source">Source of the message</param>
        /// <param name="message">Message to log</param>
        /// <param name="args">Arguments to used for message completion</param>
        public static void Error(string source, string message, params object[] args)
        {
            var log = GetLogEntry(source, message, args);
            Trace.TraceError(log);
        }

        /// <summary>
        /// Writes out Fatal Error Messages
        /// </summary>
        /// <param name="source">Source of the message</param>
        /// <param name="message">Message to log</param>
        /// <param name="args">Arguments to used for message completion</param>
        public static void Fatal(string source, string message, params object[] args)
        {
            var log = GetLogEntry(source, message, args);
            Trace.Fail(log);
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
                {
                    message = message.Replace("{", "{{").Replace("}", "}}");
                }

                string msg = String.Format(System.Globalization.CultureInfo.CurrentCulture, message, args);
                string log = string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0} [[{1}]] {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), source, msg);
                return log;
            }
            catch (Exception e)
            {
                return string.Format("Error while generating log information, {0}", e);
            }
        }
        #endregion
    }
}
