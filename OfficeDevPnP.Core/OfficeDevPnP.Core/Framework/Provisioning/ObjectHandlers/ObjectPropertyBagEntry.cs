using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.UPAWebService;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public class ObjectPropertyBagEntry : ObjectHandlerBase
    {
        public override void ProvisionObjects(Microsoft.SharePoint.Client.Web web, Model.ProvisioningTemplate template)
        {
           
            foreach (var propbagEntry in template.PropertyBagEntries)
            {
                if (!web.PropertyBagContainsKey(propbagEntry.Key))
                {
                    web.SetPropertyBagValue(propbagEntry.Key,propbagEntry.Value);

                }
            }
        }

        public override Model.ProvisioningTemplate CreateEntities(Microsoft.SharePoint.Client.Web web, Model.ProvisioningTemplate template)
        {
            web.Context.Load(web, w => w.AllProperties.FieldValues);
            web.Context.ExecuteQueryRetry();

            var entries = new List<PropertyBagEntry>();

            foreach (var propbagEntry in web.AllProperties.FieldValues)
            {
                entries.Add(new PropertyBagEntry() {Key = propbagEntry.Key, Value = propbagEntry.Value.ToString()});
            }

            template.PropertyBagEntries = entries;

            return template;
        }

    }
}
