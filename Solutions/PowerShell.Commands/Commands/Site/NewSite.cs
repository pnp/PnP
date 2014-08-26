using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base;
using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;
using System.Net;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.New, "SPOOnPremSite")]
    [CmdletHelp("On-Premises only: Creates a new site collection.", DetailedDescription = @"
This command requires a webservice to be installed on the SharePoint farm. See https://officeams.codeplex.com for an example of such a webservice. 

Use Get/Set-SPOConfiguration to globally configure the location of this webservice, or use the ServiceUrl parameter.  
", Details = "On-Premises only")]
    public class NewSite : SPOCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Title;

        [Parameter(Mandatory = true)]
        public string Url;

        [Parameter(Mandatory = false)]
        public string Description = string.Empty;

        [Parameter(Mandatory = false)]
        public string OwnerLogin = string.Empty;

        [Parameter(Mandatory = true)]
        public string SecondaryContactLogin = string.Empty;

        [Parameter(Mandatory = false)]
        public UInt16 Lcid = 1033;

        [Parameter(Mandatory = false)]
        public string Template = "STS#0";

        [Parameter(Mandatory = false)]
        public string ServiceUrl;

        protected override void ProcessRecord()
        {
            throw new NotImplementedException();
            //string serverUrl = SPOnlineConnection.CurrentConnection.Url;
            //NetworkCredential creds = null;
            //if (SPOnlineConnection.CurrentConnection.PSCredential != null)
            //{
            //    creds = SPOnlineConnection.CurrentConnection.PSCredential.GetNetworkCredential();
            //}
            //string webServiceUrl = string.Empty;

            //if (!string.IsNullOrEmpty(ServiceUrl))
            //{
            //    webServiceUrl = ServiceUrl;
            //}
            //else
            //{
            //    webServiceUrl = Configuration.GetValue("RelativeSiteProvisionServiceUrl");
            //    if (webServiceUrl == null)
            //    {
            //        WriteWarning("Service URL not set in configuration (use Get/Set-SPOConfiguration to modify), reverting to default /_vti_bin/contoso.services.sitemanager/sitemanager.svc");
            //    }
            //    else
            //    {
            //        webServiceUrl = "/_vti_bin/contoso.services.sitemanager/sitemanager.svc";
            //    }
            //}
            //SPOnline.SPOSite.NewSite(serverUrl, webServiceUrl, creds, Title, OwnerLogin, SecondaryContactLogin, Description, Template, Url, Lcid);
        }

    }
}
