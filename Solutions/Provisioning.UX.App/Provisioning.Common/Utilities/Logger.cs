using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Utilities
{
    /// <summary>
    /// Logging Implementation class.
    /// </summary>
    internal class Logger : ILog
    {
        private static readonly Logger _instance = new Logger();

        public static ILog Instance
        {
            get { return _instance; }
        }

        public void Information(string message)
        {
            Trace.TraceInformation(message);
        }

        public void Information(string format, params object[] args)
        {
            Trace.TraceInformation(format, args);
        }

        public void Information(Exception exception, string format, params object[] args)
        {
            var msg = String.Format(format, args);
            Trace.TraceInformation(string.Format(format, args) + " Exception Details={0}", ExceptionUtils.FormatException(exception, includeContext: true));
        }

        public void Warning(string message)
        {
            Trace.TraceWarning(message);
        }

        public void Warning(string format, params object[] args)
        {
            Trace.TraceWarning(format, args);
        }

        public void Warning(Exception exception, string format, params object[] vars)
        {
            var msg = String.Format(format, vars);
            Trace.TraceWarning(string.Format(format, vars) + " Exception Details={0}", ExceptionUtils.FormatException(exception, includeContext: true));
        }

        public void Error(string message)
        {
            Trace.TraceError(message);
        }

        public void Error(string format, params object[] vars)
        {
            Trace.TraceError(format, vars);
        }

        public void Error(Exception exception, string format, params object[] vars)
        {
            var msg = String.Format(format, vars);
            Trace.TraceError(string.Format(format, vars) + " Exception Details={0}", ExceptionUtils.FormatException(exception, includeContext: true));
        }

        // 
        // TraceAPI - trace inter-service calls (including latency) 
        public void TraceApi(string componentName, string method, TimeSpan timespan)
        {
            TraceApi(componentName, method, timespan, "");
        }

        public void TraceApi(string componentName, string method, TimeSpan timespan, string fmt, params object[] vars)
        {
            TraceApi(componentName, method, timespan, string.Format(fmt, vars));
        }

        public void TraceApi(string componentName, string method, TimeSpan timespan, string properties)
        {
            string message = String.Concat("component:", 
                    componentName, 
                    " method:", method, 
                    " timespan:", timespan.ToString(), 
                    " properties:", properties);
            Trace.TraceInformation(message);
        }


        private string GetLogEntry(string source, string message, params object[] args)
        {
            try
            {
                if (args == null || args.Length == 0)
                    message = message.Replace("{", "{{").Replace("}", "}}");
                string _msg = String.Format(System.Globalization.CultureInfo.CurrentCulture, message, args);
                string _log = string.Format(System.Globalization.CultureInfo.CurrentCulture, "[[{0}]] {1}", source, _msg);
                return _log;
            }
            catch (Exception e)
            {
                return string.Format("Error while generating log information, {0}", e);
            }
        }
    }
}
