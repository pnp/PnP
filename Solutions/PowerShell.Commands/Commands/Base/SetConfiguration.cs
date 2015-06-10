using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Xml.Linq;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands.Base
{
    [Cmdlet(VerbsCommon.Set, "SPOConfiguration")]
    [CmdletHelp("To be deprecated", Category = "Base Cmdlets")]
    public class SetConfiguration : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Key;

        [Parameter(Mandatory = false)]
        public string Value;

        protected override void ProcessRecord()
        {
            WriteWarning("This cmdlet will be deprecated in the August 2015 release");
            XDocument document;

            // check for existing configuration, if not existing, create it
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var configFolder = Path.Combine(appDataFolder, "OfficeDevPnP.PowerShell");
            if (!Directory.Exists(configFolder))
            {
                Directory.CreateDirectory(configFolder);
            }
            var path = Path.Combine(configFolder, "configuration.xml");

            if (!File.Exists(path))
            {
                document = new XDocument(new XDeclaration("1.0", "UTF-8", string.Empty));
                var configElement = new XElement("items");
                document.Add(configElement);
                document.Save(path);
            }
            else
            {
                document = XDocument.Load(path);
            }
            var itemsElement = document.Element("items");
            if (Value != null)
            {
                var items = from item in document.Descendants("item")
                            where item.Attribute("key").Value == Key
                            select item;
                if (items.Any())
                {
                    items.FirstOrDefault().Value = Value;
                }
                else
                {
                    var itemElement = new XElement("item", new XAttribute("key", Key)) { Value = Value };
                    itemsElement.Add(itemElement);
                }
            }
            else
            {
                var items = from item in document.Descendants("item")
                            where item.Attribute("key").Value == Key
                            select item;
                if (items.Any())
                {
                    items.FirstOrDefault().Remove();
                }
            }

            document.Save(path);

        }
    }
}
