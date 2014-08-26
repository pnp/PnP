using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Properties;
using System;
using System.IO;
using System.Management.Automation;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

namespace OfficeDevPnP.PowerShell.Commands.Base
{
    [Cmdlet(VerbsCommon.Set, "SPOConfiguration")]
    public class SetConfiguration : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Key;

        [Parameter(Mandatory = false)]
        public string Value;

        protected override void ProcessRecord()
        {
            string path = null;

            XDocument document = null;
         
                // check for existing configuration, if not existing, create it
                string appDataFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string configFolder = System.IO.Path.Combine(appDataFolder, "OfficeDevPnP.PowerShell");
                if (!Directory.Exists(configFolder))
                {
                    Directory.CreateDirectory(configFolder);
                }
                path = System.IO.Path.Combine(configFolder, "configuration.xml");
            
            if (!File.Exists(path))
            {
                document = new XDocument(new XDeclaration("1.0", "UTF-8", string.Empty));
                var configElement = new XElement("items");
                var siteProvisionServiceUrlElement = new XElement("item", new XAttribute("key", "RelativeSiteProvisionServiceUrl"));
                siteProvisionServiceUrlElement.Value = "/_vti_bin/contoso.services.sitemanager/sitemanager.svc";
                configElement.Add(siteProvisionServiceUrlElement);
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
                if(items.Count() > 0)
                {
                    items.FirstOrDefault().Value = Value;
                }
                else
                {
                    var itemElement = new XElement("item", new XAttribute("key", Key));
                    itemElement.Value = Value;
                    itemsElement.Add(itemElement);
                }
            }
            else
            {
                var items = from item in document.Descendants("item")
                            where item.Attribute("key").Value == Key
                            select item;
                if(items.Count() > 0)
                {
                    items.FirstOrDefault().Remove();
                }
            }
            
            document.Save(path);

        }
    }
}
