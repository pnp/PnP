using OfficeDevPnP.Core.Framework.Provisioning.Connectors;

namespace OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml
{
    public class XMLAzureStorageTemplateProvider : XMLTemplateProvider
    {
        public XMLAzureStorageTemplateProvider(string connectionString, string container) :
            base(new AzureStorageConnector(connectionString, container))
        {

        }
    }
}
