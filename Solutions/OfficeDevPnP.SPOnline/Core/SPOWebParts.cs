using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OfficeDevPnP.SPOnline.Core
{
    public static class SPOWebParts
    {
        /// <summary>
        /// Adds a webpart to page
        /// </summary>
        /// <param name="webPartXml">XML in either DWP or WEBPART format</param>
        /// <param name="relativePageUrl">Server relative url of the page, e.g. /default.aspx</param>
        /// <param name="zoneId">The name of the Zone, e.g. "Left"</param>
        /// <param name="zoneIndex">The index of the WebPart, starting at 1.</param>
        public static void ImportWebPart(string webPartXml, string relativePageUrl, string zoneId, int zoneIndex, Web web, ClientContext clientContext)
        {
            if (zoneIndex > 0)
            {
                WebPartDefinition definition = null;

                Microsoft.SharePoint.Client.File file = web.GetFileByServerRelativeUrl(relativePageUrl);

                clientContext.Load(file);
                clientContext.ExecuteQuery();

                LimitedWebPartManager limitedWebPartManager = file.GetLimitedWebPartManager(PersonalizationScope.Shared);
                definition = limitedWebPartManager.ImportWebPart(webPartXml);

                limitedWebPartManager.AddWebPart(definition.WebPart, zoneId, zoneIndex);

                clientContext.ExecuteQuery();
            }
        }

        /// <summary>
        /// Adds a webpart to a zone in a page.
        /// </summary>
        /// <param name="webPart">The WebPart to add</param>
        /// <param name="relativePageUrl">The Page to add the webpart to</param>
        /// <param name="zoneId">The Zone to add the webpart to</param>
        /// <param name="zoneIndex">The index of the webpart in the zone</param>
        public static void AddWebPart(WebPart webPart, string relativePageUrl, string zoneId, int zoneIndex, Web web, ClientContext clientContext)
        {
            if (zoneIndex > 0)
            {
                Microsoft.SharePoint.Client.File file = web.GetFileByServerRelativeUrl(relativePageUrl);

                clientContext.Load(file);
                clientContext.ExecuteQuery();

                LimitedWebPartManager limitedWebPartManager = file.GetLimitedWebPartManager(PersonalizationScope.Shared);

                limitedWebPartManager.AddWebPart(webPart, zoneId, zoneIndex);

                clientContext.ExecuteQuery();
            }
        }

        /// <summary>
        /// Returns a WebPart definition from a given page
        /// </summary>
        /// <param name="relativePageUrl">The page to find the webpart on</param>
        /// <param name="title">The title of the webpart to find</param>
        /// <returns></returns>
        public static WebPartDefinition GetWebPartByTitle(string relativePageUrl, string title, Web web, ClientContext clientContext)
        {
            var webparts = GetWebParts(relativePageUrl, web, clientContext);

            var definition = webparts.Where(def => def.WebPart.Title == title).FirstOrDefault();

            return definition;
        }

        public static WebPartDefinition GetWebPartById(string relativePageUrl, Web web, Guid identity, ClientContext clientContext)
        {
            var webparts = GetWebParts(relativePageUrl, web, clientContext);
            var webPart = webparts.Where(w => w.Id == identity).FirstOrDefault();

            return webPart;
        }

        public static List<WebPartDefinition> GetWebParts(string relativePageUrl, Web web, ClientContext clientContext)
        {
            //relativePageUrl = Utils.Urls.CombineUrl(web, relativePageUrl);

            List<WebPartDefinition> webparts = new List<WebPartDefinition>();

            Microsoft.SharePoint.Client.File file = web.GetFileByServerRelativeUrl(relativePageUrl);
            LimitedWebPartManager limitedWebPartManager = file.GetLimitedWebPartManager(PersonalizationScope.Shared);

            clientContext.Load(limitedWebPartManager.WebParts, wps => wps.Include(wp => wp.Id, wp => wp.WebPart, wp => wp.WebPart.Title, wp => wp.WebPart.Properties, wp => wp.WebPart.Hidden));

            clientContext.ExecuteQuery();

            foreach (var definition in limitedWebPartManager.WebParts)
            {
                webparts.Add(definition);
            }

            return webparts;
        }


        /// <summary>
        /// Deletes a web part from a given page
        /// </summary>
        /// <param name="pageUrl">Server relative url of the page</param>
        /// <param name="title">Title of the web part</param>
        public static void RemoveWebPartByTitle(string relativePageUrl, string title, Web web, ClientContext clientContext)
        {
            //relativePageUrl = Utils.Urls.CombineUrl(web, relativePageUrl);
            WebPartDefinition definition = null;

            Microsoft.SharePoint.Client.File file = web.GetFileByServerRelativeUrl(relativePageUrl);
            LimitedWebPartManager limitedWebPartManager = file.GetLimitedWebPartManager(PersonalizationScope.Shared);

            clientContext.Load(limitedWebPartManager.WebParts, wps => wps.Include(wp => wp.WebPart.Title));

            clientContext.ExecuteQuery();

            if (limitedWebPartManager.WebParts.Count == 0)
            {
                throw new Exception(Properties.Resources.NoWebpartsOnThisPage);
            }

            definition = limitedWebPartManager.WebParts.Where(w => w.WebPart.Title == title).FirstOrDefault();

            definition.DeleteWebPart();

            clientContext.ExecuteQuery();
        }

        public static void RemoveWebPartById(string relativePageUrl, Guid id, Web web, ClientContext clientContext)
        {
            //relativePageUrl = Utils.Urls.CombineUrl(web, relativePageUrl);
            WebPartDefinition definition = null;

            Microsoft.SharePoint.Client.File file = web.GetFileByServerRelativeUrl(relativePageUrl);
            LimitedWebPartManager limitedWebPartManager = file.GetLimitedWebPartManager(PersonalizationScope.Shared);

            clientContext.Load(limitedWebPartManager.WebParts, wps => wps.Include(x => x.Id));

            clientContext.ExecuteQuery();

            if (limitedWebPartManager.WebParts.Count == 0)
            {
                throw new Exception(Properties.Resources.NoWebpartsOnThisPage);
            }

            definition = limitedWebPartManager.WebParts.Where(w => w.Id == id).FirstOrDefault();

            definition.DeleteWebPart();

            clientContext.ExecuteQuery();
        }

        public static void AddWebPart(string webPartXml, Web web, string relativePageUrl, ClientContext clientContext, int row = 1, int column = 1)
        {
            //relativePageUrl = Utils.Urls.CombineUrl(web, relativePageUrl);
            webPartXml = ParseWebPartXmlForTokens(webPartXml, web);


            File file = web.GetFileByServerRelativeUrl(relativePageUrl);

            clientContext.Load(file, f => f.ListItemAllFields);
            clientContext.ExecuteQuery();

            ListItem listItem = file.ListItemAllFields;
            LimitedWebPartManager wpm = file.GetLimitedWebPartManager(PersonalizationScope.Shared);


            WebPartDefinition wpd = wpm.ImportWebPart(webPartXml);
            WebPartDefinition wpdNew = wpm.AddWebPart(wpd.WebPart, "wpz", 0);

            clientContext.Load(wpdNew);
            clientContext.Load(listItem);
            clientContext.ExecuteQuery();

            // Create reference to WebPart in HTML
            string wikiField = listItem["WikiField"] as string;

            XmlDocument xd = new XmlDocument();
            xd.PreserveWhitespace = true;

            xd.LoadXml(wikiField);
            XmlElement layoutsTable = xd.SelectSingleNode("div/table") as XmlElement;
            XmlElement layoutsZoneInner = layoutsTable.SelectSingleNode(string.Format("tbody/tr[{0}]/td[{1}]/div/div", row, column)) as XmlElement;
            // - wpBoxDiv
            XmlElement wpBoxDiv = xd.CreateElement("div");
            layoutsZoneInner.AppendChild(wpBoxDiv);
            XmlAttribute attribute = xd.CreateAttribute("class");
            wpBoxDiv.Attributes.Append(attribute);
            attribute.Value = "ms-rtestate-read ms-rte-wpbox";
            attribute = xd.CreateAttribute("contentEditable");
            wpBoxDiv.Attributes.Append(attribute);
            attribute.Value = "false";
            // - div1
            XmlElement div1 = xd.CreateElement("div");
            wpBoxDiv.AppendChild(div1);
            div1.IsEmpty = false;
            attribute = xd.CreateAttribute("class");
            div1.Attributes.Append(attribute);
            attribute.Value = "ms-rtestate-read " + wpdNew.Id.ToString("D");
            attribute = xd.CreateAttribute("id");
            div1.Attributes.Append(attribute);
            attribute.Value = "div_" + wpdNew.Id.ToString("D");
            // - div2
            XmlElement div2 = xd.CreateElement("div");
            wpBoxDiv.AppendChild(div2);
            div2.IsEmpty = false;
            attribute = xd.CreateAttribute("style");
            div2.Attributes.Append(attribute);
            attribute.Value = "display:none";
            attribute = xd.CreateAttribute("id");
            div2.Attributes.Append(attribute);
            attribute.Value = "vid_" + wpdNew.Id.ToString("D");
            // Update
            listItem["WikiField"] = xd.OuterXml;
            listItem.Update();
            clientContext.ExecuteQuery();

        }

        private static string ParseWebPartXmlForTokens(string webPartXml, Web web)
        {
            string xml = webPartXml;
            try
            {
                xml = xml.Replace("{{SITEURL}}", web.ServerRelativeUrl);
                xml = xml.Replace("{{SITEID}}", web.Id.ToString());
                xml = xml.Replace("{{SITETITLE}}", web.Title);
            }
            catch { }
            {
                xml = webPartXml;
            }
            return xml;
        }

        public static void SetWebPartProperty(string Key, string Value, Guid identity, string relativePageUrl, Web web, ClientContext clientContext)
        {
            //relativePageUrl = Utils.Urls.CombineUrl(web, relativePageUrl);

            ClientContext context = web.Context as ClientContext;

            File file = web.GetFileByServerRelativeUrl(relativePageUrl);

            context.Load(file, f => f.ListItemAllFields);
            context.ExecuteQuery();

            ListItem listItem = file.ListItemAllFields;
            LimitedWebPartManager wpm = file.GetLimitedWebPartManager(PersonalizationScope.Shared);

            context.Load(wpm.WebParts);

            WebPartDefinition def = wpm.WebParts.GetById(identity);

            switch (Key.ToLower())
            {
                case "title":
                    {
                        def.WebPart.Title = Value;
                        break;
                    }
                case "titleurl":
                    {
                        def.WebPart.TitleUrl = Value;
                        break;
                    }
                default:
                    {
                        def.WebPart.Properties[Key] = Value;
                        break;
                    }
            }
            def.SaveWebPartChanges();

            context.ExecuteQuery();

        }
    }
}
