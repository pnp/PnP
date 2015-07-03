using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OfficeDevPnP.Core.Tools.UnitTest.PnPBuildExtensions
{
    public class PnPAppConfigManager
    {
        #region Private variables
        private string masterConfigurationFile;
        private XmlElement masterConfigurationRoot;
        #endregion

        #region Public properties
        public string MasterConfigurationFile
        {
            get
            {
                return masterConfigurationFile;
            }
        }
        #endregion

        #region Constructors
        public PnPAppConfigManager(string masterConfigurationFile)
        {
            this.masterConfigurationFile = masterConfigurationFile;
            this.masterConfigurationRoot = LoadMasterConfigurationFile(this.masterConfigurationFile);
        }
        #endregion

        #region Public methods
        public string GetConfigurationElement(string configuration, string element, bool attribute=false)
        {
            XmlNodeList result = this.masterConfigurationRoot.SelectNodes(string.Format("/TestConfigurations/Configuration[@Name=\"{0}\"]/{2}{1}", configuration, element, attribute ? "@" : ""));
            if (null == result || result.Count != 1)
            {
                throw new Exception(String.Format("There seem be either zero or multiple configurations with name {0} or someone forgot XML is case-sensitive. Other option can be a wrong element {1}", configuration, element));
            }
            else
            {
                return result[0].InnerText;
            }
        }

        public void GenerateAppConfig(string configuration, string appConfigFolder)
        {
            string appConfigFile = Path.Combine(appConfigFolder, "app.config");

            // If there's already an app.config file then delete it
            if (File.Exists(appConfigFile))
            {
                File.Delete(appConfigFile);
            }

            string configurationType = GetConfigurationElement(configuration, "Type", true);
            string configurationAuthentication = GetConfigurationElement(configuration, "Authentication", true);

            // Generate app.config XML file
            using (XmlWriter writer = XmlWriter.Create(appConfigFile))
            {
                writer.WriteStartElement("configuration");
                writer.WriteStartElement("appSettings");

                // These app settings property value pairs are always present
                WriteProperty(writer, configuration, "TenantUrl", "SPOTenantUrl");
                WriteProperty(writer, configuration, "TestSiteUrl", "SPODevSiteUrl");

                if (configurationType.Equals("OnPremises", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (configurationAuthentication.Equals("Credentials", StringComparison.InvariantCultureIgnoreCase))
                    {
                        WriteProperty(writer, configuration, "User", "OnPremUserName");
                        WriteProperty(writer, configuration, "Domain", "OnPremDomain");
                        WriteProperty(writer, configuration, "Password", "OnPremPassword");
                    }
                    else // App-Only
                    {
                        WriteProperty(writer, configuration, "Realm", "Realm");
                        WriteProperty(writer, configuration, "AppId", "AppId");
                        WriteProperty(writer, configuration, "AppSecret", "AppSecret");
                    }
                }
                else // Online
                {
                    WriteProperty(writer, configuration, "CredentialManagerLabel", "SPOCredentialManagerLabel");
                    if (configurationAuthentication.Equals("Credentials", StringComparison.InvariantCultureIgnoreCase))
                    {
                        WriteProperty(writer, configuration, "User", "SPOUserName");
                        WriteProperty(writer, configuration, "Password", "SPOPassword");
                    }
                    else // App-Only
                    {
                        WriteProperty(writer, configuration, "Realm", "Realm");
                        WriteProperty(writer, configuration, "AppId", "AppId");
                        WriteProperty(writer, configuration, "AppSecret", "AppSecret");
                    }
                    WriteProperty(writer, configuration, "Azure/StorageAccount", "AzureStorageKey");
                }
                writer.WriteEndElement(); //appSettings

                writer.WriteStartElement("system.diagnostics");
                writer.WriteStartElement("sharedListeners");
                writer.WriteStartElement("add");

                writer.WriteAttributeString("name", "console");
                writer.WriteAttributeString("type", "System.Diagnostics.ConsoleTraceListener");
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteStartElement("sources");
                writer.WriteStartElement("source");

                writer.WriteAttributeString("name", "OfficeDevPnP.Core");
                writer.WriteAttributeString("switchValue", "Verbose");

                writer.WriteStartElement("listeners");
                writer.WriteStartElement("add");
                writer.WriteAttributeString("name", "console");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteStartElement("trace");
                writer.WriteAttributeString("indentsize", "0");
                writer.WriteAttributeString("autoflush", "true");
                writer.WriteStartElement("listeners");
                writer.WriteStartElement("add");
                writer.WriteAttributeString("name", "console");
            }
        }
        #endregion

        #region Private methods
        private void WriteProperty(XmlWriter writer, string configuration, string sourcePropertyName, string targetPropertyName)
        {
            writer.WriteStartElement("add");
            writer.WriteAttributeString("key", targetPropertyName);
            writer.WriteAttributeString("value", GetConfigurationElement(configuration, sourcePropertyName));
            writer.WriteEndElement();
        }

        private XmlElement LoadMasterConfigurationFile(string configFile)
        {
            if (File.Exists(configFile))
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(configFile);
                return xDoc.DocumentElement;
            }
            else
            {
                throw new FileNotFoundException(String.Format("Master XML configuration file {0} was not found", configFile));
            }
        }
        #endregion


    }
}
