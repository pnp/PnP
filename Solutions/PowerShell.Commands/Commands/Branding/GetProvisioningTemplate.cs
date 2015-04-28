using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using OfficeDevPnP.Core.Utilities;
using OfficeDevPnP.PowerShell.Commands.Enums;
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

        [Parameter(Mandatory = false, Position = 0, HelpMessage = "The schema of the output to use, defaults to the latest schema")]
        public XMLPnPSchemaVersion Schema = XMLPnPSchemaVersion.LATEST;

        [Parameter(Mandatory = false, HelpMessage = "If specified, all term groups will be included. Overrides IncludeSiteCollectionTermGroup.")]
        public SwitchParameter IncludeAllTermGroups;

        [Parameter(Mandatory = false, HelpMessage = "If specified, all the site collection term group will be included. Overridden by IncludeAllTermGroups.")]
        public SwitchParameter IncludeSiteCollectionTermGroup;

        [Parameter(Mandatory = false, HelpMessage = "If specified the files making up the composed look (background image, font file and color file) will be saved.")]
        public SwitchParameter PersistComposedLookFiles;

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
                        var xml = GetProvisioningTemplateXML(Schema, new FileInfo(Out).DirectoryName);

                        File.WriteAllText(Out, xml, Encoding);
                    }
                }
                else
                {
                    var xml = GetProvisioningTemplateXML(Schema, new FileInfo(Out).DirectoryName);

                    File.WriteAllText(Out, xml, Encoding);
                }
            }
            else
            {
                var xml = GetProvisioningTemplateXML(Schema, SessionState.Path.CurrentFileSystemLocation.Path);

                WriteObject(xml);
            }
        }

        private string GetProvisioningTemplateXML(XMLPnPSchemaVersion schema, string path)
        {
            var creationInformation = new ProvisioningTemplateCreationInformation(SelectedWeb);

            creationInformation.PersistComposedLookFiles = PersistComposedLookFiles;
            creationInformation.FileConnector = new FileSystemConnector(path, "");

            creationInformation.BaseTemplate = this.SelectedWeb.GetBaseTemplate();
            creationInformation.ProgressDelegate = (message, step, total) =>
            {
                WriteProgress(new ProgressRecord(0, "Extracting Template", message) { PercentComplete = (100 / total) * step });
            };

            if (IncludeAllTermGroups)
            {
                creationInformation.IncludeAllTermGroups = true;
            }
            else
            {
                if (IncludeSiteCollectionTermGroup)
                {
                    creationInformation.IncludeSiteCollectionTermGroup = true;
                }
            }

            var template = SelectedWeb.GetProvisioningTemplate(creationInformation);

            ITemplateFormatter formatter = null;
            switch (schema)
            {
                case XMLPnPSchemaVersion.LATEST:
                    {
                        formatter = XMLPnPSchemaFormatter.LatestFormatter;
                        break;
                    }
                case XMLPnPSchemaVersion.V201503:
                    {
                        formatter = XMLPnPSchemaFormatter.GetSpecificFormatter(XMLConstants.PROVISIONING_SCHEMA_NAMESPACE_2015_03);
                        break;
                    }
                case XMLPnPSchemaVersion.V201505:
                    {
                        formatter = XMLPnPSchemaFormatter.GetSpecificFormatter(XMLConstants.PROVISIONING_SCHEMA_NAMESPACE_2015_05);
                        break;
                    }
            }
            var _outputStream = formatter.ToFormattedTemplate(template);
            StreamReader reader = new StreamReader(_outputStream);

            return reader.ReadToEnd();

        }
    }
}
