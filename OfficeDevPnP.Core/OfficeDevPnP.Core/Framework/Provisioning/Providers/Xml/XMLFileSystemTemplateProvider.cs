using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml
{
    public class XMLFileSystemTemplateProvider : XMLTemplateProvider
    {
        public XMLFileSystemTemplateProvider(string connectionString, string container) :
            base(new FileSystemConnector(connectionString, container))
        {

        }
    }
}
