using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using OfficeDevPnP.Core.Utilities;

namespace OfficeDevPnP.PowerShell.Commands.Branding
{
    [Cmdlet(VerbsCommon.Get, "SPOProvisioningTemplate")]
    [CmdletHelp("Generates a provisioning template from a web", Category = "Branding")]
    public class GetProvisioningTemplate : SPOWebCmdlet
    {
        protected override void ExecuteCmdlet()
        {
            var template = SelectedWeb.GetProvisioningTemplate();
            SharePointProvisioningTemplate spProvisioningTemplate = template.ToXml();
            string xml = XMLSerializer.Serialize<SharePointProvisioningTemplate>(spProvisioningTemplate);

            WriteObject(xml);
        }
    }
}
