using OfficeDevPnP.PowerShell.Commands.Base;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Core;
using System.Net;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPORealm")]
    [CmdletHelp("Gets the authentication realm for the current web")]
    public class GetRealm : PSCmdlet
    {

        [Parameter(Mandatory = true)]
        public string Url;

        protected override void ProcessRecord()
        {
            WebRequest request = WebRequest.Create(new Uri(Url) + "/_vti_bin/client.svc");
            request.Headers.Add("Authorization: Bearer ");

            try
            {
                using (request.GetResponse())
                {
                }
            }
            catch (WebException e)
            {
                if (e.Response == null)
                {
                }

                string bearerResponseHeader = e.Response.Headers["WWW-Authenticate"];
                if (string.IsNullOrEmpty(bearerResponseHeader))
                {
                }

                const string bearer = "Bearer realm=\"";
                int bearerIndex = bearerResponseHeader.IndexOf(bearer, StringComparison.Ordinal);
                if (bearerIndex < 0)
                {
                }

                int realmIndex = bearerIndex + bearer.Length;

                if (bearerResponseHeader.Length >= realmIndex + 36)
                {
                    string targetRealm = bearerResponseHeader.Substring(realmIndex, 36);

                    Guid realmGuid;

                    if (Guid.TryParse(targetRealm, out realmGuid))
                    {
                        WriteObject(targetRealm);
                    }
                }
            }
        }


    }
}
