using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPPublishing = Microsoft.SharePoint.Client.Publishing;

namespace Provisioning.Extensibility.Providers.Helpers
{
    public static class PageHelper
    {
        private static void RemovePublishingPage(SPPublishing.PublishingPage publishingPage, PublishingPage page, ClientContext ctx, Web web)
        {
            if (publishingPage != null && publishingPage.ServerObjectIsNull.Value == false)
            {
                if (!web.IsPropertyAvailable("RootFolder"))
                {
                    web.Context.Load(web.RootFolder);
                    web.Context.ExecuteQueryRetry();
                }

                if (page.Overwrite)
                {
                    if (page.WelcomePage && web.RootFolder.WelcomePage.Contains(page.FileName + ".aspx"))
                    {
                        //set the welcome page to a Temp page to allow remove the page
                        web.RootFolder.WelcomePage = "home.aspx";
                        web.RootFolder.Update();
                        web.Update();

                        ctx.Load(publishingPage);
                        ctx.ExecuteQuery();
                    }

                    publishingPage.ListItem.DeleteObject();
                    ctx.ExecuteQuery();
                }
                else
                {
                    return;
                }
            }
        }

        public static void AddPublishingPage(PublishingPage page, ClientContext ctx, Web web)
        {
            SPPublishing.PublishingPage publishingPage = web.GetPublishingPage(page.FileName + ".aspx");

            RemovePublishingPage(publishingPage, page, ctx, web);

            web.AddPublishingPage(page.FileName, page.Layout, page.Title, false); //DO NOT Publish here or it will fail if library doesn't enable Minor versions (PnP bug)

            publishingPage = web.GetPublishingPage(page.FileName + ".aspx");

            Microsoft.SharePoint.Client.File pageFile = publishingPage.ListItem.File;
            pageFile.CheckOut();

            if (page.Properties != null && page.Properties.Count > 0)
            {
                ctx.Load(pageFile, p => p.Name, p => p.CheckOutType); //need these values in SetFileProperties
                ctx.ExecuteQuery();
                pageFile.SetFileProperties(page.Properties, false);
            }

            if (page.WebParts != null && page.WebParts.Count > 0)
            {
                Microsoft.SharePoint.Client.WebParts.LimitedWebPartManager mgr = pageFile.GetLimitedWebPartManager(Microsoft.SharePoint.Client.WebParts.PersonalizationScope.Shared);

                ctx.Load(mgr);
                ctx.ExecuteQuery();

                AddWebpartsToPublishingPage(page, ctx, mgr);
            }

            List pagesLibrary = publishingPage.ListItem.ParentList;
            ctx.Load(pagesLibrary);
            ctx.ExecuteQueryRetry();

            ListItem pageItem = publishingPage.ListItem;
            web.Context.Load(pageItem, p => p.File.CheckOutType);
            web.Context.ExecuteQueryRetry();            

            if (pageItem.File.CheckOutType != CheckOutType.None)
            {
                pageItem.File.CheckIn(String.Empty, CheckinType.MajorCheckIn);
            }

            if (page.Publish && pagesLibrary.EnableMinorVersions)
            {
                pageItem.File.Publish(String.Empty);
                if (pagesLibrary.EnableModeration)
                {
                    pageItem.File.Approve(String.Empty);
                }
            }


            if (page.WelcomePage)
            {
                SetWelcomePage(web, pageFile);
            }

            ctx.ExecuteQuery();
        }

        private static void SetWelcomePage(Web web, Microsoft.SharePoint.Client.File pageFile)
        {
            if (!web.IsPropertyAvailable("RootFolder"))
            {
                web.Context.Load(web.RootFolder);
                web.Context.ExecuteQueryRetry();
            }

            if (!pageFile.IsPropertyAvailable("ServerRelativeUrl"))
            {
                web.Context.Load(pageFile, p => p.ServerRelativeUrl);
                web.Context.ExecuteQueryRetry();
            }

            var rootFolderRelativeUrl = pageFile.ServerRelativeUrl.Substring(web.RootFolder.ServerRelativeUrl.Length);

            web.SetHomePage(rootFolderRelativeUrl);
        }

        private static void AddWebpartsToPublishingPage(PublishingPage page, ClientContext ctx, Microsoft.SharePoint.Client.WebParts.LimitedWebPartManager mgr)
        {
            foreach (var wp in page.WebParts)
            {
                string wpContentsTokenResolved = wp.Contents;
                Microsoft.SharePoint.Client.WebParts.WebPart webPart = mgr.ImportWebPart(wpContentsTokenResolved).WebPart;
                Microsoft.SharePoint.Client.WebParts.WebPartDefinition definition = mgr.AddWebPart(
                                                                                            webPart,
                                                                                            wp.Zone,
                                                                                            (int)wp.Order
                                                                                        );
                var webPartProperties = definition.WebPart.Properties;
                ctx.Load(definition.WebPart);
                ctx.Load(webPartProperties);
                ctx.ExecuteQuery();

                if (wp.IsListViewWebPart)
                {
                    AddListViewWebpart(ctx, wp, definition, webPartProperties);
                }
            }
        }

        private static void AddListViewWebpart(
            ClientContext ctx, 
            PublishingPageWebPart wp, 
            Microsoft.SharePoint.Client.WebParts.WebPartDefinition definition, 
            PropertyValues webPartProperties)
        {
            string defaultViewDisplayName = wp.DefaultViewDisplayName;

            if (!String.IsNullOrEmpty(defaultViewDisplayName))
            {
                string listUrl = webPartProperties.FieldValues["ListUrl"].ToString();

                ctx.Load(definition, d => d.Id); // Id of the hidden view which gets automatically created
                ctx.ExecuteQuery();

                Guid viewId = definition.Id;
                List list = ctx.Web.GetListByUrl(listUrl);

                Microsoft.SharePoint.Client.View viewCreatedFromWebpart = list.Views.GetById(viewId);
                ctx.Load(viewCreatedFromWebpart);

                Microsoft.SharePoint.Client.View viewCreatedFromList = list.Views.GetByTitle(defaultViewDisplayName);
                ctx.Load(
                    viewCreatedFromList,
                    v => v.ViewFields,
                    v => v.ListViewXml,
                    v => v.ViewQuery,
                    v => v.ViewData,
                    v => v.ViewJoins,
                    v => v.ViewProjectedFields);

                ctx.ExecuteQuery();

                //need to copy the same View definition to the new View added by the Webpart manager
                viewCreatedFromWebpart.ViewQuery = viewCreatedFromList.ViewQuery;
                viewCreatedFromWebpart.ViewData = viewCreatedFromList.ViewData;
                viewCreatedFromWebpart.ViewJoins = viewCreatedFromList.ViewJoins;
                viewCreatedFromWebpart.ViewProjectedFields = viewCreatedFromList.ViewProjectedFields;
                viewCreatedFromWebpart.ViewFields.RemoveAll();

                foreach (var field in viewCreatedFromList.ViewFields)
                {
                    viewCreatedFromWebpart.ViewFields.Add(field);
                }

                //need to set the JSLink to the new View added by the Webpart manager.
                //This is because there's no way to change the BaseViewID property of the new View,
                //and we needed to do that because the custom JSLink was bound to a specific BaseViewID (overrideCtx.BaseViewID = 3;)
                //The work around to this is to add the JSLink to the specific new View created when you add the xsltViewWebpart to the page
                //and remove the "overrideCtx.BaseViewID = 3;" from the JSLink file
                //that way, the JSLink will be executed only for this View, that is only used in the xsltViewWebpart,
                //so the effect is the same that bind the JSLink to the BaseViewID
                if (webPartProperties.FieldValues.ContainsKey("JSLink") && webPartProperties.FieldValues["JSLink"] != null)
                {
                    viewCreatedFromWebpart.JSLink = webPartProperties.FieldValues["JSLink"].ToString();
                }

                viewCreatedFromWebpart.Update();

                ctx.ExecuteQuery();
            }
        }
    }
}
