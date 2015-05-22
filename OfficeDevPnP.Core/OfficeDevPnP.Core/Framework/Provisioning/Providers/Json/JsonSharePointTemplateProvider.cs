using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OfficeDevPnP.Core.Framework.Provisioning.Providers.Json
{
    public class JsonSharePointTemplateProvider : JsonTemplateProvider
    {
        public JsonSharePointTemplateProvider(ClientRuntimeContext cc, string connectionString, string container) :
            base(new SharePointConnector(cc, connectionString, container))
        {
        }
    }
}
