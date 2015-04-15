using System;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.Core.Framework.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Model;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public class ObjectPages : ObjectHandlerBase
    {
        public override void ProvisionObjects(Web web, ProvisioningTemplate template, TokenParser parser)
        {
            var context = web.Context as ClientContext;

            if (!web.IsPropertyAvailable("ServerRelativeUrl"))
            {
                context.Load(web, w => w.ServerRelativeUrl);
                context.ExecuteQueryRetry();
            }

            foreach (var page in template.Pages)
            {
                var url = parser.Parse(page.Url);


                if (!url.ToLower().StartsWith(web.ServerRelativeUrl.ToLower()))
                {
                    url = UrlUtility.Combine(web.ServerRelativeUrl, url);
                }


                var exists = true;
                Microsoft.SharePoint.Client.File file = null;
                try
                {
                    file = web.GetFileByServerRelativeUrl(url);
                    web.Context.Load(file);
                    web.Context.ExecuteQuery();
                }
                catch (ServerException ex)
                {
                    if (ex.ServerErrorTypeName == "System.IO.FileNotFoundException")
                    {
                        exists = false;
                    }
                }
                if (exists && page.Overwrite)
                {
                    file.DeleteObject();
                    web.Context.ExecuteQueryRetry();
                    web.AddWikiPageByUrl(url);
                    web.AddLayoutToWikiPage(page.Layout, url);
                }
                else
                {
                    web.AddWikiPageByUrl(url);
                    web.AddLayoutToWikiPage(page.Layout, url);
                }

                foreach (var webpart in page.WebParts)
                {
                    WebPartEntity wpEntity = new WebPartEntity();
                    wpEntity.WebPartTitle = webpart.Title;
                    wpEntity.WebPartXml = parser.Parse(webpart.Contents);
                    web.AddWebPartToWikiPage(url, wpEntity, (int)webpart.Row, (int)webpart.Column, false);
                }

            }

        }


        public override ProvisioningTemplate CreateEntities(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            // Impossible to return all files in the site currently

            // If a base template is specified then use that one to "cleanup" the generated template model
            if (creationInfo.BaseTemplate != null)
            {
                template = CleanupEntities(template, creationInfo.BaseTemplate);
            }

            return template;
        }

        private ProvisioningTemplate CleanupEntities(ProvisioningTemplate template, ProvisioningTemplate baseTemplate)
        {

            return template;
        }
    }
}
