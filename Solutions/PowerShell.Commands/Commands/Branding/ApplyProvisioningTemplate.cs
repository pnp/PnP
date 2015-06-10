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
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;

namespace OfficeDevPnP.PowerShell.Commands.Branding
{
    [Cmdlet("Apply", "SPOProvisioningTemplate")]
    [CmdletHelp("Applies a provisioning template to a web", Category = "Branding")]
    [CmdletExample(
     Code = @"
    PS:> Apply-SPOProvisioningTemplate -Path template.xml
",
     Remarks = "Applies a provisioning template in XML format to the current web.",
     SortOrder = 1)]
    [CmdletExample(
     Code = @"
    PS:> Apply-SPOProvisioningTemplate -Path template.xml -ResourceFolder c:\provisioning\resources
",
     Remarks = "Applies a provisioning template in XML format to the current web.",
     SortOrder = 2)]
    public class ApplyProvisioningTemplate : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, HelpMessage = "Path to the xml file containing the provisioning template.")]
        public string Path;

        [Parameter(Mandatory = false, HelpMessage = "Root folder where resources/files that are being referenced in the template are located. If not specified location of the provisioning template will be used.")]
        public string ResourceFolder;

        protected override void ExecuteCmdlet()
        {
            if (!SelectedWeb.IsPropertyAvailable("Url"))
            {
                ClientContext.Load(SelectedWeb, w => w.Url);
                ClientContext.ExecuteQueryRetry();
            }
            if (!System.IO.Path.IsPathRooted(Path))
            {
                Path = System.IO.Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path, Path);
            }
            if (!string.IsNullOrEmpty(ResourceFolder))
            {
                if (System.IO.Path.IsPathRooted(ResourceFolder))
                {
                    ResourceFolder = System.IO.Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path, ResourceFolder);
                }
            }

            FileInfo fileInfo = new FileInfo(Path);

            XMLTemplateProvider provider =
                new XMLFileSystemTemplateProvider(fileInfo.DirectoryName, "");

            var provisioningTemplate = provider.GetTemplate(fileInfo.Name);

            if (provisioningTemplate != null)
            {
                FileSystemConnector fileSystemConnector = null;
                if (string.IsNullOrEmpty(ResourceFolder))
                {
                    fileSystemConnector = new FileSystemConnector(fileInfo.DirectoryName, "");
                }
                else
                {
                    fileSystemConnector = new FileSystemConnector(ResourceFolder, "");
                }
                provisioningTemplate.Connector = fileSystemConnector;


                var applyingInformation = new ProvisioningTemplateApplyingInformation();

                applyingInformation.ProgressDelegate = (message, step, total) =>
                {
                    WriteProgress(new ProgressRecord(0, string.Format("Applying template to {0}", SelectedWeb.Url), message) { PercentComplete = (100 / total) * step });
                };

                applyingInformation.MessageDelegate = (message, type) =>
                {
                    if (type == ProvisioningMessageType.Warning)
                    {
                        WriteWarning(message);
                    }
                };
                SelectedWeb.ApplyProvisioningTemplate(provisioningTemplate, applyingInformation);
            }
        }
    }
}
