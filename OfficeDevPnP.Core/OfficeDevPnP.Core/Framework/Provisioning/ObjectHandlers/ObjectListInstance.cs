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
                listCreate.QuickLaunchOption = list.OnQuickLaunch ? QuickLaunchOptions.On : QuickLaunchOptions.Off;
                listCreate.Url = list.Url;
              
                var createdList = web.Lists.Add(listCreate);

                createdList.EnableVersioning = list.EnableVersioning;
                createdList.DocumentTemplateUrl = list.DocumentTemplate;
                createdList.Hidden = list.Hidden;
                createdList.ContentTypesEnabled = list.AllowContentTypes;

                createdList.Update();


                web.Context.ExecuteQueryRetry();

                // TODO: handle 'removedefaultcontenttype'

                foreach (var ctBinding in list.ContentTypeBindings)
                {
                    createdList.AddContentTypeToListById(ctBinding.ContentTypeID);
                }

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

                ListInstance list = new ListInstance();
                list.Description = item.Description;
                list.EnableVersioning = item.EnableVersioning;
                list.TemplateType = item.BaseTemplate;
                list.Title = item.Title;
                list.Hidden = item.Hidden;
                list.DocumentTemplate = item.DocumentTemplateUrl;
                list.AllowContentTypes = item.AllowContentTypes;

                int count = 0;
                foreach (var ct in item.ContentTypes)
                {
                    list.ContentTypeBindings.Add(new ContentTypeBinding() {ContentTypeID = ct.StringId, Default = count == 0 ? true : false  });
                    count++;
                }

                template.Lists.Add(list);
            }

            return template;
        }
    }
}
