using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using OfficeDevPnP.Core.Utilities;
using System.Xml.Linq;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;

namespace OfficeDevPnP.PowerShell.Commands.Branding
{
    [Cmdlet("Apply", "SPOProvisioningTemplate")]
    [CmdletHelp("Generates a provisioning template from a web", Category = "Branding")]
    public class ApplyProvisioningTemplate : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true)]
        public string Path;

        
        protected override void ExecuteCmdlet()
        {
            XDocument doc = XDocument.Load(Path);

            ProvisioningTemplate provisioningTemplate = XMLSerializer.Deserialize<SharePointProvisioningTemplate>(doc).ToProvisioningTemplate();

            if (provisioningTemplate != null)
            {
                var fileinfo = new FileInfo(Path);
                FileSystemConnector fileSystemConnector = new FileSystemConnector(fileinfo.DirectoryName, "");
                provisioningTemplate.Connector = fileSystemConnector;
                SelectedWeb.ApplyProvisioningTemplate(provisioningTemplate);
            }
        }
    }
}
