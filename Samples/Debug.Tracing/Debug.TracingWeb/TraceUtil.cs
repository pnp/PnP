using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;

namespace Debug.Tracing
{
    public class TraceUtil : IDisposable
    {
        private bool _traceEnd = false;
        private const string DefaultTraceCategory = "Info";
        private const int DefaultStackFrame = 2;

        public static void TraceMessage(string message)
        {
            TraceMessage(DefaultTraceCategory, message);
        }

        public static void TraceMessage(string category, string message)
        {
            try
            {
                HttpContext.Current.Trace.Write(category, message);
            }
            catch (Exception) { } //program should never crash due to tracing code
        }

        public TraceUtil TraceMethod(params object[] parameters)
        {
            TraceMethodInternal("BEGIN", parameters, DefaultStackFrame);
            _traceEnd = true;
            return this;
        }

        private void TraceMethodInternal(string methodTracePrefix, object[] parameters, int frame)
        {
            try
            {
                if (parameters != null)
                {
                    MethodBase method = new StackTrace().GetFrame(frame).GetMethod();
                    string className = method.DeclaringType.ToString();

                    string withParameterText = string.Empty;
                    if (parameters.Length > 0)
                    {
                        withParameterText = "With Parameters: ";
                    }

                    string message = string.Format("{0} {1} {2}", methodTracePrefix, method.Name, withParameterText);
                    foreach (var parameter in parameters)
                    {
                        if (parameter != null)
                        {
                            message += parameter + ",";
                        }
                    }

                    HttpContext.Current.Trace.Write(className, message.TrimEnd(','));
                }
            }
            catch (Exception) { } //program should never crash due to tracing code
        }

        public static void EnableTracing()
        {
            Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~");
            TraceSection section = (TraceSection)configuration.GetSection("system.web/trace");
            section.Enabled = true;
            section.LocalOnly = false;
            configuration.Save();
        }

        public static void DisableTracing()
        {
            Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~");
            TraceSection section = (TraceSection)configuration.GetSection("system.web/trace");
            section.Enabled = false;
            section.LocalOnly = false;
            configuration.Save();
        }

        public static void SetMaxTraceRequests(int numberOfRequests)
        {
            Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~");
            TraceSection section = (TraceSection)configuration.GetSection("system.web/trace");
            section.RequestLimit = numberOfRequests;
            configuration.Save();
        }

        public void Dispose()
        {
            if (_traceEnd)
            {
                TraceMethodInternal("END", new object[0], DefaultStackFrame);
            }
        }
    }
}