using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    class ObjectListInstance : ObjectHandlerBase
    {
        public override void ProvisionObjects(Microsoft.SharePoint.Client.Web web, ProvisioningTemplate template)
        {

            foreach (var list in template.Lists)
            {
                var listCreate = new ListCreationInformation();
                listCreate.Description = list.Description;
                listCreate.TemplateType = list.TemplateType;
                listCreate.Title = list.Title;

                
            }
        }

        public override ProvisioningTemplate CreateEntities(Microsoft.SharePoint.Client.Web web, ProvisioningTemplate template)
        {
            // For each list in the site
            ListCollection lists = web.Lists;
            web.Context.Load(lists);
            web.Context.ExecuteQuery();
            foreach (var item in lists)
            {
                if (!item.Hidden)
                {
                    ListInstance list = new ListInstance();
                    list.Description = item.Description;
                    list.EnableVersioning = item.EnableVersioning;
                    list.TemplateType = item.BaseTemplate;
                    list.Title = item.Title;
                    template.Lists.Add(list);
                }
            }

            return template;
        }
    }
}
