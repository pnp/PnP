using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using OfficeDevPnP.Core.Utilities;
using File = System.IO.File;
using Resources = OfficeDevPnP.PowerShell.Commands.Properties.Resources;


namespace OfficeDevPnP.PowerShell.Commands.Branding
{
    [Cmdlet(VerbsCommon.Get, "SPOProvisioningTemplate", SupportsShouldProcess = true)]
    [CmdletHelp("Generates a provisioning template from a web", Category = "Branding")]
    public class GetProvisioningTemplate : SPOWebCmdlet
    {
        [Parameter(Mandatory = false, Position = 0, HelpMessage = "Filename to write to, optionally including full path")]
        public string Out;

        [Parameter(Mandatory = false, HelpMessage = "Overwrites the output file if it exists.")]
        public SwitchParameter Force;

        [Parameter(Mandatory = false)]
        public Encoding Encoding = System.Text.Encoding.Unicode;

        protected override void ExecuteCmdlet()
        {

            if (!string.IsNullOrEmpty(Out))
            {
                if (!Path.IsPathRooted(Out))
                {
                    Out = Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path, Out);
                }
                if (File.Exists(Out))
                {
                    if (Force || ShouldContinue(string.Format(Resources.File0ExistsOverwrite, Out), Resources.Confirm))
                    {
                        var xml = GetProvisioningTemplateXML();

                        File.WriteAllText(Out, xml, Encoding);
                    }
                }
                else
                {
                    var xml = GetProvisioningTemplateXML();

                    File.WriteAllText(Out, xml, Encoding);
                }
            }
            else
            {
                var xml = GetProvisioningTemplateXML();

                WriteObject(xml);
            }
        }

        private string GetProvisioningTemplateXML()
        {
            var template = SelectedWeb.GetProvisioningTemplate();
            return template.ToXmlString();
        }
    }
}
