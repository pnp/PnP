using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeDevPnP.Core.Utilities;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    class ObjectListInstance : ObjectHandlerBase
    {
        public override void ProvisionObjects(Microsoft.SharePoint.Client.Web web, ProvisioningTemplate template)
        {
            if (!web.IsPropertyAvailable("ServerRelativeUrl"))
            {
                web.Context.Load(web, w => w.ServerRelativeUrl);
            }
            
            web.Context.Load(web.Lists, lc => lc.IncludeWithDefaultProperties(l => l.RootFolder.ServerRelativeUrl));
            web.Context.ExecuteQueryRetry();

            var serverRelativeUrl = web.ServerRelativeUrl;
            
            var existingLists = web.Lists;
            foreach (var list in template.Lists)
            {

                var existingList = existingLists.FirstOrDefault(l => l.RootFolder.ServerRelativeUrl == UrlUtility.Combine(serverRelativeUrl, list.Url));
                if (existingList == null)
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
                    createdList.ContentTypesEnabled = list.ContentTypesEnabled;

                    createdList.Update();


                    web.Context.ExecuteQueryRetry();

                    
                    // TODO: handle 'removedefaultcontenttype'

                    foreach (var ctBinding in list.ContentTypeBindings)
                    {
                        createdList.AddContentTypeToListById(ctBinding.ContentTypeID);
                    }

                    foreach (var view in list.Views)
                    {
                        createdList.CreateViewsFromXMLString(view.SchemaXml);
                    }
                }

            }
        }

        public override ProvisioningTemplate CreateEntities(Microsoft.SharePoint.Client.Web web, ProvisioningTemplate template)
        {
            if (!web.IsPropertyAvailable("ServerRelativeUrl"))
            {
                web.Context.Load(web, w => w.ServerRelativeUrl);
                web.Context.ExecuteQueryRetry();
            }

            var serverRelativeUrl = web.ServerRelativeUrl;


            // For each list in the site
            ListCollection lists = web.Lists;
            web.Context.Load(lists, lc => lc.IncludeWithDefaultProperties(l => l.ContentTypes, l => l.Views, l => l.RootFolder.ServerRelativeUrl));
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
                list.ContentTypesEnabled = item.ContentTypesEnabled;
                list.Url = item.RootFolder.ServerRelativeUrl.Substring(serverRelativeUrl.Length);
               
                int count = 0;
                foreach (var ct in item.ContentTypes)
                {
                    list.ContentTypeBindings.Add(new ContentTypeBinding() { ContentTypeID = ct.StringId, Default = count == 0 ? true : false });
                    count++;
                }

                foreach (var view in item.Views)
                {
                    list.Views.Add(new Model.View() { SchemaXml = view.ListViewXml });
                }

                template.Lists.Add(list);
            }

            return template;
        }
    }
}
