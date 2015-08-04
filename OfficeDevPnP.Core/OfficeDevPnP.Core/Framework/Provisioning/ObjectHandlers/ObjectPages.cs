using System;
using System.Linq;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.Core.Framework.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Utilities;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    internal class ObjectPages : ObjectHandlerBase
    {
        public override string Name
        {
            get { return "Pages"; }
        }


        public override void ProvisionObjects(Web web, ProvisioningTemplate template, ProvisioningTemplateApplyingInformation applyingInformation)
        {
            Log.Info(Constants.LOGGING_SOURCE_FRAMEWORK_PROVISIONING, CoreResources.Provisioning_ObjectHandlers_Pages);

            var context = web.Context as ClientContext;

            if (!web.IsPropertyAvailable("ServerRelativeUrl"))
            {
                context.Load(web, w => w.ServerRelativeUrl);
                context.ExecuteQueryRetry();
            }

            foreach (var page in template.Pages)
            {
                var url = page.Url.ToParsedString();


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
                if (exists)
                {
                    if (page.Overwrite)
                    {
                        file.DeleteObject();
                        web.Context.ExecuteQueryRetry();
                        web.AddWikiPageByUrl(url);
                        web.AddLayoutToWikiPage(page.Layout, url);
                    }
                }
                else
                {
                    web.AddWikiPageByUrl(url);
                    web.AddLayoutToWikiPage(page.Layout, url);
                }

                if (page.WelcomePage)
                {
                    if (!web.IsPropertyAvailable("RootFolder"))
                    {
                        web.Context.Load(web.RootFolder);
                        web.Context.ExecuteQueryRetry();
                    }

                    var rootFolderRelativeUrl = url.Substring(web.RootFolder.ServerRelativeUrl.Length);
                    web.SetHomePage(rootFolderRelativeUrl);
                }

                if (page.WebParts != null & page.WebParts.Any())
                {
                    var existingWebParts = web.GetWebParts(url);

                    foreach (var webpart in page.WebParts)
                    {
                        if (existingWebParts.FirstOrDefault(w => w.WebPart.Title == webpart.Title) == null)
                        {
                            WebPartEntity wpEntity = new WebPartEntity();
                            wpEntity.WebPartTitle = webpart.Title;
                            wpEntity.WebPartXml = webpart.Contents.ToParsedString().Trim(new[] {'\n', ' '});
                            web.AddWebPartToWikiPage(url, wpEntity, (int)webpart.Row, (int)webpart.Column, false);
                        }
                    }
                }
            }
        }


        public override ProvisioningTemplate ExtractObjects(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
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


        public override bool WillProvision(Web web, ProvisioningTemplate template)
        {
            if (!_willProvision.HasValue)
            {
                _willProvision = template.Pages.Any();
            }
            return _willProvision.Value;
        }

        public override bool WillExtract(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            if (!_willExtract.HasValue)
            {
                _willExtract = false;
            }
            return _willExtract.Value;
        }
    }
}
