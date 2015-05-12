using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Framework
{
    public class ColorTraceListener : CustomTraceListener
    {
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            if (data is LogEntry && Formatter != null)
            {
                LogEntry le = (LogEntry)data;
                WriteLine(Formatter.Format(le), le.Severity);
            }
            else
            {
                WriteLine(data.ToString());
            }
        }
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            WriteLine(message, eventType);
        }

        public void WriteLine(string message, TraceEventType severity)
        {
            ConsoleColor color;
            switch (severity)
            {
                case TraceEventType.Critical:
                case TraceEventType.Error:
                    color = ConsoleColor.Red;
                    break;
                case TraceEventType.Warning:
                    color = ConsoleColor.Yellow;
                    break;
                case TraceEventType.Information:
                    color = ConsoleColor.Cyan;
                    break;
                case TraceEventType.Verbose:
                default:
                    color = ConsoleColor.Gray;
                    break;
            }

            System.Console.ForegroundColor = color;
            System.Console.WriteLine(message);
        }

        public override void Write(string message)
        {
            WriteLine(message);
        }

        public override void WriteLine(string message)
        {
            System.Console.WriteLine(message);

        }
    }
}
