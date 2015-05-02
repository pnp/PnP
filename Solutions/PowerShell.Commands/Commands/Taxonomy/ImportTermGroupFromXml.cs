using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using File = System.IO.File;
using Resources = OfficeDevPnP.PowerShell.Commands.Properties.Resources;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsData.Import, "SPOTermGroupFromXml", SupportsShouldProcess = true)]
    [CmdletHelp("Imports a taxonomy TermGroup from either the input or from an XML file.", Category = "Taxonomy")]
    [CmdletExample(Code = @"PS:> Import-SPOTermGroupFromXml -Xml $xml", Remarks = "Imports the XML based termgroup information located in the $xml variable")]
    [CmdletExample(Code = @"PS:> Import-SPOTermGroupFromXml -Path input.xml", Remarks = "Imports the XML file specified by the path.")]
    public class ImportTermGroupFromXml : SPOCmdlet
    {
        [Parameter(Mandatory = false, HelpMessage = "The XML to process", Position = 0, ValueFromPipeline = true, ParameterSetName = "XML")]
        public string Xml;

        [Parameter(Mandatory = false, HelpMessage = "The XML File to import the data from", ParameterSetName = "File")]
        public string Path;


        protected override void ExecuteCmdlet()
        {
            var template = new ProvisioningTemplate();
            template.Security = null;
            template.Features = null;
            template.CustomActions = null;
            template.ComposedLook = null;
                
            template.Id = "TAXONOMYPROVISIONING";

            var outputStream = XMLPnPSchemaFormatter.LatestFormatter.ToFormattedTemplate(template);

            var reader = new StreamReader(outputStream);

            var fullXml = reader.ReadToEnd();

            var document = XDocument.Parse(fullXml);

            XElement termGroupsElement = null;
            if (this.MyInvocation.BoundParameters.ContainsKey("Xml"))
            {
                termGroupsElement = XElement.Parse(Xml);
            }
            else
            {
                if (!System.IO.Path.IsPathRooted(Path))
                {
                    Path = System.IO.Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path, Path);
                }
                termGroupsElement = XElement.Parse(File.ReadAllText(Path));
            }

            XNamespace pnp = XMLConstants.PROVISIONING_SCHEMA_NAMESPACE_2015_05;
            var templateElement = document.Root.Descendants(pnp + "ProvisioningTemplate").FirstOrDefault();

            if (templateElement != null)
            {
                templateElement.Add(termGroupsElement);
            }

            var stream = new MemoryStream();
            document.Save(stream);
            stream.Position = 0;

            var completeTemplate = XMLPnPSchemaFormatter.LatestFormatter.ToProvisioningTemplate(stream);

            ClientContext.Web.ApplyProvisioningTemplate(completeTemplate);

        }

    }
}
