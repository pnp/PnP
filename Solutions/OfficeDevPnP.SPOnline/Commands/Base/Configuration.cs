using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OfficeDevPnP.SPOnline.Commands.Base
{
    public static class Configuration
    {
        public static string GetValue(string key)
        {
            string path = null;
            XDocument document = null;

            // check for existing configuration, if not existing, create it
            string appDataFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string configFolder = System.IO.Path.Combine(appDataFolder, "Contoso.PSOnline.PowerShell");
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

            var configItems = from item in document.Descendants("item") 
                              where item.Attribute("key").Value == key
                              select item;
           
            if(configItems.FirstOrDefault() != null)
            {
                return configItems.FirstOrDefault().Value;
            }
            else
            {
                return null;
            }
        }
    }
}
