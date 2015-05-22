using OfficeDevPnP.Core.Framework.Provisioning.Connectors;

namespace OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml
{
    public class XMLAzureStorageTemplateProvider : XMLTemplateProvider
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public XMLAzureStorageTemplateProvider() : base()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="container"></param>
        public XMLAzureStorageTemplateProvider(string connectionString, string container) :
            base(new AzureStorageConnector(connectionString, container))
        {
        }
    }
}
