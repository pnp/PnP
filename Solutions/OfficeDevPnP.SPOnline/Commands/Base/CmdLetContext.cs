using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.SPOnline.Commands.Base
{
    public sealed class CmdLetContext : ClientContext
    {
        private PSHost m_powerShellHost;

        internal PSHost Host
        {
            get
            {
                return this.m_powerShellHost;
            }
            private set
            {
                if (value == null)
                    throw new ArgumentNullException("Host");
                this.m_powerShellHost = value;
            }
        }

        internal CmdLetContext(string webFullUrl, PSHost host)
            : base(webFullUrl)
        {
            this.Host = host;
        }

        internal CmdLetContext(Uri webFullUrl, PSHost host)
            : base(webFullUrl)
        {
            this.Host = host;
        }

        internal static string GetUserAgent()
        {
            return string.Format((IFormatProvider)CultureInfo.InvariantCulture, Properties.Resources.ContosoSPOnlinePowerShellLibrary0, new object[1]
      {
        (object) FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion
      });
        }
    }
}
