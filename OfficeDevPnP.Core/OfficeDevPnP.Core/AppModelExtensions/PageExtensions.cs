using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.SharePoint.Client
{
    /// <summary>
    /// Class that handles all page and web part related operations
    /// </summary>
    public static class PageExtensions
    {
        private const string WikiPage_OneColumn = @"<div class=""ExternalClassC1FD57BEDB8942DC99A06C02F9A98241""><table id=""layoutsTable"" style=""width&#58;100%;""><tbody><tr style=""vertical-align&#58;top;""><td style=""width&#58;100%;""><div class=""ms-rte-layoutszone-outer"" style=""width&#58;100%;""><div class=""ms-rte-layoutszone-inner"" role=""textbox"" aria-haspopup=""true"" aria-autocomplete=""both"" aria-multiline=""true""></div>&#160;</div></td></tr></tbody></table><span id=""layoutsData"" style=""display&#58;none;"">false,false,1</span></div>";
        private const string WikiPage_OneColumnSideBar = @"<div class=""ExternalClass47565ACDF7974263AA4A556DA974B687""><table id=""layoutsTable"" style=""width&#58;100%;""><tbody><tr style=""vertical-align&#58;top;""><td style=""width&#58;66.6%;""><div class=""ms-rte-layoutszone-outer"" style=""width&#58;100%;""><div class=""ms-rte-layoutszone-inner"" role=""textbox"" aria-haspopup=""true"" aria-autocomplete=""both"" aria-multiline=""true""></div>&#160;</div></td><td class=""ms-wiki-columnSpacing"" style=""width&#58;33.3%;""><div class=""ms-rte-layoutszone-outer"" style=""width&#58;100%;""><div class=""ms-rte-layoutszone-inner"" role=""textbox"" aria-haspopup=""true"" aria-autocomplete=""both"" aria-multiline=""true""></div>&#160;</div></td></tr></tbody></table><span id=""layoutsData"" style=""display&#58;none;"">false,false,2</span></div>";
        private const string WikiPage_TwoColumns = @"<div class=""ExternalClass3811C839E5984CCEA4C8CF738462AFD8""><table id=""layoutsTable"" style=""width&#58;100%;""><tbody><tr style=""vertical-align&#58;top;""><td style=""width&#58;49.95%;""><div class=""ms-rte-layoutszone-outer"" style=""width&#58;100%;""><div class=""ms-rte-layoutszone-inner"" role=""textbox"" aria-haspopup=""true"" aria-autocomplete=""both"" aria-multiline=""true""></div>&#160;</div></td><td class=""ms-wiki-columnSpacing"" style=""width&#58;49.95%;""><div class=""ms-rte-layoutszone-outer"" style=""width&#58;100%;""><div class=""ms-rte-layoutszone-inner"" role=""textbox"" aria-haspopup=""true"" aria-autocomplete=""both"" aria-multiline=""true""></div>&#160;</div></td></tr></tbody></table><span id=""layoutsData"" style=""display&#58;none;"">false,false,2</span></div>";
        private const string WikiPage_TwoColumnsHeader = @"<div class=""ExternalClass850251EB51394304A07A64A05C0BB0F1""><table id=""layoutsTable"" style=""width&#58;100%;""><tbody><tr style=""vertical-align&#58;top;""><td colspan=""2""><div class=""ms-rte-layoutszone-outer"" style=""width&#58;100%;""><div class=""ms-rte-layoutszone-inner"" role=""textbox"" aria-haspopup=""true"" aria-autocomplete=""both"" aria-multiline=""true""></div>&#160;</div></td></tr><tr style=""vertical-align&#58;top;""><td style=""width&#58;49.95%;""><div class=""ms-rte-layoutszone-outer"" style=""width&#58;100%;""><div class=""ms-rte-layoutszone-inner"" role=""textbox"" aria-haspopup=""true"" aria-autocomplete=""both"" aria-multiline=""true""></div>&#160;</div></td><td class=""ms-wiki-columnSpacing"" style=""width&#58;49.95%;""><div class=""ms-rte-layoutszone-outer"" style=""width&#58;100%;""><div class=""ms-rte-layoutszone-inner"" role=""textbox"" aria-haspopup=""true"" aria-autocomplete=""both"" aria-multiline=""true""></div>&#160;</div></td></tr></tbody></table><span id=""layoutsData"" style=""display&#58;none;"">true,false,2</span></div>";
        private const string WikiPage_TwoColumnsHeaderFooter = @"<div class=""ExternalClass71C5527252AD45859FA774445D4909A2""><table id=""layoutsTable"" style=""width&#58;100%;""><tbody><tr style=""vertical-align&#58;top;""><td colspan=""2""><div class=""ms-rte-layoutszone-outer"" style=""width&#58;100%;""><div class=""ms-rte-layoutszone-inner"" role=""textbox"" aria-haspopup=""true"" aria-autocomplete=""both"" aria-multiline=""true""></div>&#160;</div></td></tr><tr style=""vertical-align&#58;top;""><td style=""width&#58;49.95%;""><div class=""ms-rte-layoutszone-outer"" style=""width&#58;100%;""><div class=""ms-rte-layoutszone-inner"" role=""textbox"" aria-haspopup=""true"" aria-autocomplete=""both"" aria-multiline=""true""></div>&#160;</div></td><td class=""ms-wiki-columnSpacing"" style=""width&#58;49.95%;""><div class=""ms-rte-layoutszone-outer"" style=""width&#58;100%;""><div class=""ms-rte-layoutszone-inner"" role=""textbox"" aria-haspopup=""true"" aria-autocomplete=""both"" aria-multiline=""true""></div>&#160;</div></td></tr><tr style=""vertical-align&#58;top;""><td colspan=""2""><div class=""ms-rte-layoutszone-outer"" style=""width&#58;100%;""><div class=""ms-rte-layoutszone-inner"" role=""textbox"" aria-haspopup=""true"" aria-autocomplete=""both"" aria-multiline=""true""></div>&#160;</div></td></tr></tbody></table><span id=""layoutsData"" style=""display&#58;none;"">true,true,2</span></div>";
        private const string WikiPage_ThreeColumns = @"<div class=""ExternalClass833D1FA704C94892A26C4069C3FE5FE9""><table id=""layoutsTable"" style=""width&#58;100%;""><tbody><tr style=""vertical-align&#58;top;""><td style=""width&#58;33.3%;""><div class=""ms-rte-layoutszone-outer"" style=""width&#58;100%;""><div class=""ms-rte-layoutszone-inner"" role=""textbox"" aria-haspopup=""true"" aria-autocomplete=""both"" aria-multiline=""true""></div>&#160;</div></td><td class=""ms-wiki-columnSpacing"" style=""width&#58;33.3%;""><div class=""ms-rte-layoutszone-outer"" style=""width&#58;100%;""><div class=""ms-rte-layoutszone-inner"" role=""textbox"" aria-haspopup=""true"" aria-autocomplete=""both"" aria-multiline=""true""></div>&#160;</div></td><td class=""ms-wiki-columnSpacing"" style=""width&#58;33.3%;""><div class=""ms-rte-layoutszone-outer"" style=""width&#58;100%;""><div class=""ms-rte-layoutszone-inner"" role=""textbox"" aria-haspopup=""true"" aria-autocomplete=""both"" aria-multiline=""true""></div>&#160;</div></td></tr></tbody></table><span id=""layoutsData"" style=""display&#58;none;"">false,false,3</span></div>";
        private const string WikiPage_ThreeColumnsHeader = @"<div class=""ExternalClassD1A150D6187F449B8A6C4BEA2D4913BB""><table id=""layoutsTable"" style=""width&#58;100%;""><tbody><tr style=""vertical-align&#58;top;""><td colspan=""3""><div class=""ms-rte-layoutszone-outer"" style=""width&#58;100%;""><div class=""ms-rte-layoutszone-inner"" role=""textbox"" aria-haspopup=""true"" aria-autocomplete=""both"" aria-multiline=""true""></div>&#160;</div></td></tr><tr style=""vertical-align&#58;top;""><td style=""width&#58;33.3%;""><div class=""ms-rte-layoutszone-outer"" style=""width&#58;100%;""><div class=""ms-rte-layoutszone-inner"" role=""textbox"" aria-haspopup=""true"" aria-autocomplete=""both"" aria-multiline=""true""></div>&#160;</div></td><td class=""ms-wiki-columnSpacing"" style=""width&#58;33.3%;""><div class=""ms-rte-layoutszone-outer"" style=""width&#58;100%;""><div class=""ms-rte-layoutszone-inner"" role=""textbox"" aria-haspopup=""true"" aria-autocomplete=""both"" aria-multiline=""true""></div>&#160;</div></td><td class=""ms-wiki-columnSpacing"" style=""width&#58;33.3%;""><div class=""ms-rte-layoutszone-outer"" style=""width&#58;100%;""><div class=""ms-rte-layoutszone-inner"" role=""textbox"" aria-haspopup=""true"" aria-autocomplete=""both"" aria-multiline=""true""></div>&#160;</div></td></tr></tbody></table><span id=""layoutsData"" style=""display&#58;none;"">true,false,3</span></div>";
        private const string WikiPage_ThreeColumnsHeaderFooter = @"<div class=""ExternalClass5849C2C61FEC44E9B249C60F7B0ACA38""><table id=""layoutsTable"" style=""width&#58;100%;""><tbody><tr style=""vertical-align&#58;top;""><td colspan=""3""><div class=""ms-rte-layoutszone-outer"" style=""width&#58;100%;""><div class=""ms-rte-layoutszone-inner"" role=""textbox"" aria-haspopup=""true"" aria-autocomplete=""both"" aria-multiline=""true""></div>&#160;</div></td></tr><tr style=""vertical-align&#58;top;""><td style=""width&#58;33.3%;""><div class=""ms-rte-layoutszone-outer"" style=""width&#58;100%;""><div class=""ms-rte-layoutszone-inner"" role=""textbox"" aria-haspopup=""true"" aria-autocomplete=""both"" aria-multiline=""true""></div>&#160;</div></td><td class=""ms-wiki-columnSpacing"" style=""width&#58;33.3%;""><div class=""ms-rte-layoutszone-outer"" style=""width&#58;100%;""><div class=""ms-rte-layoutszone-inner"" role=""textbox"" aria-haspopup=""true"" aria-autocomplete=""both"" aria-multiline=""true""></div>&#160;</div></td><td class=""ms-wiki-columnSpacing"" style=""width&#58;33.3%;""><div class=""ms-rte-layoutszone-outer"" style=""width&#58;100%;""><div class=""ms-rte-layoutszone-inner"" role=""textbox"" aria-haspopup=""true"" aria-autocomplete=""both"" aria-multiline=""true""></div>&#160;</div></td></tr><tr style=""vertical-align&#58;top;""><td colspan=""3""><div class=""ms-rte-layoutszone-outer"" style=""width&#58;100%;""><div class=""ms-rte-layoutszone-inner"" role=""textbox"" aria-haspopup=""true"" aria-autocomplete=""both"" aria-multiline=""true""></div>&#160;</div></td></tr></tbody></table><span id=""layoutsData"" style=""display&#58;none;"">true,true,3</span></div>";


        /// <summary>
        /// Returns the HTML contents of a wiki page
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="serverRelativePageUrl">Server relative url of the page, e.g. /sites/demo/SitePages/Test.aspx</param>
        /// <returns></returns>
        public static string GetWikiPageContent(this Web web, string serverRelativePageUrl)
        {
            File file = web.GetFileByServerRelativeUrl(serverRelativePageUrl);

            web.Context.Load(file, f => f.ListItemAllFields);

            web.Context.ExecuteQuery();

            return file.ListItemAllFields["WikiField"] as string;
        }

        /// <summary>
        /// List the web parts on a page
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="serverRelativePageUrl">Server relative url of the page containing the webparts</param>
        public static IEnumerable<WebPartDefinition> GetWebParts(this Web web, string serverRelativePageUrl)
        {
            Microsoft.SharePoint.Client.File file = web.GetFileByServerRelativeUrl(serverRelativePageUrl);
            LimitedWebPartManager limitedWebPartManager = file.GetLimitedWebPartManager(PersonalizationScope.Shared);

            var query = web.Context.LoadQuery(limitedWebPartManager.WebParts.IncludeWithDefaultProperties(wp => wp.Id, wp => wp.WebPart, wp => wp.WebPart.Title, wp => wp.WebPart.Properties, wp => wp.WebPart.Hidden));

            web.Context.ExecuteQuery();

            return query;
        }

        /// <summary>
        /// Inserts a web part on a web part page
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="webPart">Information about the web part to insert</param>
        /// <param name="page">Page to add the web part on</param>
        public static void AddWebPartToWebPartPage(this Web web, WebPartEntity webPart, string page)
        {
            if (!web.IsObjectPropertyInstantiated("ServerRelativeUrl"))
            {
                web.Context.Load(web, w => w.ServerRelativeUrl);
                web.Context.ExecuteQuery();
            }
            var serverRelativeUrl = UrlUtility.Combine(web.ServerRelativeUrl, page);

            AddWebPartToWebPartPage(web, serverRelativeUrl, webPart);
        }

        /// <summary>
        /// Inserts a web part on a web part page
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="serverRelativePageUrl">Page to add the web part on</param>
        /// <param name="webPart">Information about the web part to insert</param>
        public static void AddWebPartToWebPartPage(this Web web, string serverRelativePageUrl, WebPartEntity webPart)
        {
            var webPartPage = web.GetFileByServerRelativeUrl(serverRelativePageUrl);

            if (webPartPage == null)
            {
                return;
            }

            web.Context.Load(webPartPage);
            web.Context.ExecuteQuery();

            LimitedWebPartManager limitedWebPartManager = webPartPage.GetLimitedWebPartManager(PersonalizationScope.Shared);
            WebPartDefinition oWebPartDefinition = limitedWebPartManager.ImportWebPart(webPart.WebPartXml);

            limitedWebPartManager.AddWebPart(oWebPartDefinition.WebPart, webPart.WebPartZone, webPart.WebPartIndex);
            web.Context.ExecuteQuery();
        }

        /// <summary>
        /// Add web part to a wiki style page
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="folder">System name of the wiki page library - typically sitepages</param>
        /// <param name="webPart">Information about the web part to insert</param>
        /// <param name="page">Page to add the web part on</param>
        /// <param name="row">Row of the wiki table that should hold the inserted web part</param>
        /// <param name="col">Column of the wiki table that should hold the inserted web part</param>
        /// <param name="addSpace">Does a blank line need to be added after the web part (to space web parts)</param>
        public static void AddWebPartToWikiPage(this Web web, string folder, WebPartEntity webPart, string page, int row, int col, bool addSpace)
        {
            if (!web.IsObjectPropertyInstantiated("ServerRelativeUrl"))
            {
                web.Context.Load(web, w => w.ServerRelativeUrl);
                web.Context.ExecuteQuery();
            }

            var webServerRelativeUrl = UrlUtility.EnsureTrailingSlash(web.ServerRelativeUrl);
            var serverRelativeUrl = UrlUtility.Combine(folder, page);
            AddWebPartToWikiPage(web, webServerRelativeUrl + serverRelativeUrl, webPart, row, col, addSpace);
        }

        /// <summary>
        /// Add web part to a wiki style page
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="serverRelativePageUrl">Server relative url of the page to add the webpart to</param>
        /// <param name="webPart">Information about the web part to insert</param>
        /// <param name="row">Row of the wiki table that should hold the inserted web part</param>
        /// <param name="col">Column of the wiki table that should hold the inserted web part</param>
        /// <param name="addSpace">Does a blank line need to be added after the web part (to space web parts)</param>
        public static void AddWebPartToWikiPage(this Web web, string serverRelativePageUrl, WebPartEntity webPart, int row, int col, bool addSpace)
        {
            File webPartPage = web.GetFileByServerRelativeUrl(serverRelativePageUrl);

            if (webPartPage == null)
            {
                return;
            }

            web.Context.Load(webPartPage);
            web.Context.Load(webPartPage.ListItemAllFields);
            web.Context.ExecuteQuery();

            string wikiField = (string)webPartPage.ListItemAllFields["WikiField"];

            LimitedWebPartManager limitedWebPartManager = webPartPage.GetLimitedWebPartManager(PersonalizationScope.Shared);
            WebPartDefinition oWebPartDefinition = limitedWebPartManager.ImportWebPart(webPart.WebPartXml);
            WebPartDefinition wpdNew = limitedWebPartManager.AddWebPart(oWebPartDefinition.WebPart, "wpz", 0);
            web.Context.Load(wpdNew);
            web.Context.ExecuteQuery();

            //HTML structure in default team site home page (W16)
            //<div class="ExternalClass284FC748CB4242F6808DE69314A7C981">
            //  <div class="ExternalClass5B1565E02FCA4F22A89640AC10DB16F3">
            //    <table id="layoutsTable" style="width&#58;100%;">
            //      <tbody>
            //        <tr style="vertical-align&#58;top;">
            //          <td colspan="2">
            //            <div class="ms-rte-layoutszone-outer" style="width&#58;100%;">
            //              <div class="ms-rte-layoutszone-inner" style="word-wrap&#58;break-word;margin&#58;0px;border&#58;0px;">
            //                <div><span><span><div class="ms-rtestate-read ms-rte-wpbox"><div class="ms-rtestate-read 9ed0c0ac-54d0-4460-9f1c-7e98655b0847" id="div_9ed0c0ac-54d0-4460-9f1c-7e98655b0847"></div><div class="ms-rtestate-read" id="vid_9ed0c0ac-54d0-4460-9f1c-7e98655b0847" style="display&#58;none;"></div></div></span></span><p> </p></div>
            //                <div class="ms-rtestate-read ms-rte-wpbox">
            //                  <div class="ms-rtestate-read c7a1f9a9-4e27-4aa3-878b-c8c6c87961c0" id="div_c7a1f9a9-4e27-4aa3-878b-c8c6c87961c0"></div>
            //                  <div class="ms-rtestate-read" id="vid_c7a1f9a9-4e27-4aa3-878b-c8c6c87961c0" style="display&#58;none;"></div>
            //                </div>
            //              </div>
            //            </div>
            //          </td>
            //        </tr>
            //        <tr style="vertical-align&#58;top;">
            //          <td style="width&#58;49.95%;">
            //            <div class="ms-rte-layoutszone-outer" style="width&#58;100%;">
            //              <div class="ms-rte-layoutszone-inner" style="word-wrap&#58;break-word;margin&#58;0px;border&#58;0px;">
            //                <div class="ms-rtestate-read ms-rte-wpbox">
            //                  <div class="ms-rtestate-read b55b18a3-8a3b-453f-a714-7e8d803f4d30" id="div_b55b18a3-8a3b-453f-a714-7e8d803f4d30"></div>
            //                  <div class="ms-rtestate-read" id="vid_b55b18a3-8a3b-453f-a714-7e8d803f4d30" style="display&#58;none;"></div>
            //                </div>
            //              </div>
            //            </div>
            //          </td>
            //          <td class="ms-wiki-columnSpacing" style="width&#58;49.95%;">
            //            <div class="ms-rte-layoutszone-outer" style="width&#58;100%;">
            //              <div class="ms-rte-layoutszone-inner" style="word-wrap&#58;break-word;margin&#58;0px;border&#58;0px;">
            //                <div class="ms-rtestate-read ms-rte-wpbox">
            //                  <div class="ms-rtestate-read 0b2f12a4-3ab5-4a59-b2eb-275bbc617f95" id="div_0b2f12a4-3ab5-4a59-b2eb-275bbc617f95"></div>
            //                  <div class="ms-rtestate-read" id="vid_0b2f12a4-3ab5-4a59-b2eb-275bbc617f95" style="display&#58;none;"></div>
            //                </div>
            //              </div>
            //            </div>
            //          </td>
            //        </tr>
            //      </tbody>
            //    </table>
            //    <span id="layoutsData" style="display&#58;none;">true,false,2</span>
            //  </div>
            //</div>

            XmlDocument xd = new XmlDocument();
            xd.PreserveWhitespace = true;
            xd.LoadXml(wikiField);

            // Sometimes the wikifield content seems to be surrounded by an additional div? 
            XmlElement layoutsTable = xd.SelectSingleNode("div/div/table") as XmlElement;
            if (layoutsTable == null)
            {
                layoutsTable = xd.SelectSingleNode("div/table") as XmlElement;
            }

            XmlElement layoutsZoneInner = layoutsTable.SelectSingleNode(string.Format("tbody/tr[{0}]/td[{1}]/div/div", row, col)) as XmlElement;
            // - space element
            XmlElement space = xd.CreateElement("p");
            XmlText text = xd.CreateTextNode(" ");
            space.AppendChild(text);

            // - wpBoxDiv
            XmlElement wpBoxDiv = xd.CreateElement("div");
            layoutsZoneInner.AppendChild(wpBoxDiv);

            if (addSpace)
            {
                layoutsZoneInner.AppendChild(space);
            }

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

            ListItem listItem = webPartPage.ListItemAllFields;
            listItem["WikiField"] = xd.OuterXml;
            listItem.Update();
            web.Context.ExecuteQuery();

        }

        public static void AddLayoutToWikiPage(this Web web, WikiPageLayout layout, string serverRelativePageUrl)
        {

            string html = "";
            switch (layout)
            {
                case WikiPageLayout.OneColumn:
                    html = WikiPage_OneColumn;
                    break;
                case WikiPageLayout.OneColumnSideBar:
                    html = WikiPage_OneColumnSideBar;
                    break;
                case WikiPageLayout.TwoColumns:
                    html = WikiPage_TwoColumns;
                    break;
                case WikiPageLayout.TwoColumnsHeader:
                    html = WikiPage_TwoColumnsHeader;
                    break;
                case WikiPageLayout.TwoColumnsHeaderFooter:
                    html = WikiPage_TwoColumnsHeaderFooter;
                    break;
                case WikiPageLayout.ThreeColumns:
                    html = WikiPage_ThreeColumns;
                    break;
                case WikiPageLayout.ThreeColumnsHeader:
                    html = WikiPage_ThreeColumnsHeader;
                    break;
                case WikiPageLayout.ThreeColumnsHeaderFooter:
                    html = WikiPage_ThreeColumnsHeaderFooter;
                    break;
                default:
                    break;
            }

            web.AddHtmlToWikiPage(serverRelativePageUrl, html);
        }

        /// <summary>
        /// Applies a layout to a wiki page
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="folder">System name of the wiki page library - typically sitepages</param>
        /// <param name="layout">Wiki page layout to be applied</param>
        /// <param name="page">Name of the page that will get a new wiki page layout</param>
        public static void AddLayoutToWikiPage(this Web web, string folder, WikiPageLayout layout, string page)
        {
            string html = "";
            switch (layout)
            {
                case WikiPageLayout.OneColumn:
                    html = WikiPage_OneColumn;
                    break;
                case WikiPageLayout.OneColumnSideBar:
                    html = WikiPage_OneColumnSideBar;
                    break;
                case WikiPageLayout.TwoColumns:
                    html = WikiPage_TwoColumns;
                    break;
                case WikiPageLayout.TwoColumnsHeader:
                    html = WikiPage_TwoColumnsHeader;
                    break;
                case WikiPageLayout.TwoColumnsHeaderFooter:
                    html = WikiPage_TwoColumnsHeaderFooter;
                    break;
                case WikiPageLayout.ThreeColumns:
                    html = WikiPage_ThreeColumns;
                    break;
                case WikiPageLayout.ThreeColumnsHeader:
                    html = WikiPage_ThreeColumnsHeader;
                    break;
                case WikiPageLayout.ThreeColumnsHeaderFooter:
                    html = WikiPage_ThreeColumnsHeaderFooter;
                    break;
                default:
                    break;
            }

            web.AddHtmlToWikiPage(folder, html, page);
        }

        /// <summary>
        /// Add html to a wiki style page
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="folder">System name of the wiki page library - typically sitepages</param>
        /// <param name="html">The html to insert</param>
        /// <param name="page">Page to add the html on</param>
        public static void AddHtmlToWikiPage(this Web web, string folder, string html, string page)
        {
            if (!web.IsObjectPropertyInstantiated("ServerRelativeUrl"))
            {
                web.Context.Load(web, w => w.ServerRelativeUrl);
                web.Context.ExecuteQuery();
            }

            var webServerRelativeUrl = UrlUtility.EnsureTrailingSlash(web.ServerRelativeUrl);

            var serverRelativeUrl = UrlUtility.Combine(webServerRelativeUrl, folder, page);

            AddHtmlToWikiPage(web, serverRelativeUrl, html);
        }

        /// <summary>
        /// Add HTML to a wiki page
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="serverRelativePageUrl"></param>
        /// <param name="html"></param>
        public static void AddHtmlToWikiPage(this Web web, string serverRelativePageUrl, string html)
        {
            File file = web.GetFileByServerRelativeUrl(serverRelativePageUrl);

            web.Context.Load(file, f => f.ListItemAllFields);
            web.Context.ExecuteQuery();

            ListItem item = file.ListItemAllFields;

            item["WikiField"] = html;

            item.Update();

            web.Context.ExecuteQuery();
        }

        /// <summary>
        /// Add a HTML fragment to a location on a wiki style page
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="folder">System name of the wiki page library - typically sitepages</param>
        /// <param name="html">html to be inserted</param>
        /// <param name="page">Page to add the web part on</param>
        /// <param name="row">Row of the wiki table that should hold the inserted web part</param>
        /// <param name="col">Column of the wiki table that should hold the inserted web part</param>
        public static void AddHtmlToWikiPage(this Web web, string folder, string html, string page, int row, int col)
        {
            if (!web.IsObjectPropertyInstantiated("ServerRelativeUrl"))
            {
                web.Context.Load(web, w => w.ServerRelativeUrl);
                web.Context.ExecuteQuery();
            }

            var webServerRelativeUrl = UrlUtility.EnsureTrailingSlash(web.ServerRelativeUrl);

            var serverRelativeUrl = UrlUtility.Combine(webServerRelativeUrl, folder, page);

            AddHtmlToWikiPage(web, serverRelativeUrl, html, row, col);
        }

        /// <summary>
        /// Add a HTML fragment to a location on a wiki style page
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="serverRelativePageUrl">server relative Url of the page to add the fragment to</param>
        /// <param name="html">html to be inserted</param>
        /// <param name="row">Row of the wiki table that should hold the inserted web part</param>
        /// <param name="col">Column of the wiki table that should hold the inserted web part</param>
        public static void AddHtmlToWikiPage(this Web web, string serverRelativePageUrl, string html, int row, int col)
        {
            if (!web.IsObjectPropertyInstantiated("ServerRelativeUrl"))
            {
                web.Context.Load(web, w => w.ServerRelativeUrl);
                web.Context.ExecuteQuery();
            }

            var serverRelativeUrl = UrlUtility.Combine(UrlUtility.EnsureTrailingSlash(web.ServerRelativeUrl) + serverRelativePageUrl);
	    
            File file = web.GetFileByServerRelativeUrl(serverRelativeUrl);

            web.Context.Load(file, f => f.ListItemAllFields);
            web.Context.ExecuteQuery();

            ListItem item = file.ListItemAllFields;

            string wikiField = (string)item["WikiField"];

            XmlDocument xd = new XmlDocument();
            xd.PreserveWhitespace = true;
            xd.LoadXml(wikiField);

            // Sometimes the wikifield content seems to be surrounded by an additional div? 
            XmlElement layoutsTable = xd.SelectSingleNode("div/div/table") as XmlElement;
            if (layoutsTable == null)
            {
                layoutsTable = xd.SelectSingleNode("div/table") as XmlElement;
            }

            // Add the html content
            XmlElement layoutsZoneInner = layoutsTable.SelectSingleNode(string.Format("tbody/tr[{0}]/td[{1}]/div/div", row, col)) as XmlElement;
            XmlText text = xd.CreateTextNode("!!123456789!!");
            layoutsZoneInner.AppendChild(text);

            item["WikiField"] = xd.OuterXml.Replace("!!123456789!!", html);
            item.Update();
            web.Context.ExecuteQuery();
        }

        /// <summary>
        /// Deletes a web part from a page
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="folder">System name of the wiki page library - typically sitepages</param>
        /// <param name="title">Title of the web part that needs to be deleted</param>
        /// <param name="page">Page to remove the web part from</param>
        public static void DeleteWebPart(this Web web, string folder, string title, string page)
        {
            if (!web.IsObjectPropertyInstantiated("ServerRelativeUrl"))
            {
                web.Context.Load(web, w => w.ServerRelativeUrl);
                web.Context.ExecuteQuery();
            }

            var webServerRelativeUrl = UrlUtility.EnsureTrailingSlash(web.ServerRelativeUrl);

            var serverRelativeUrl = UrlUtility.Combine(folder, page);

            DeleteWebPart(web, webServerRelativeUrl + serverRelativeUrl, title);
        }

        /// <summary>
        /// Deletes a web part from a page
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="serverRelativePageUrl">Server relative URL of the page to remove</param>
        /// <param name="title">Title of the web part that needs to be deleted</param>
        public static void DeleteWebPart(this Web web, string serverRelativePageUrl, string title)
        {
            var webPartPage = web.GetFileByServerRelativeUrl(serverRelativePageUrl);

            if (webPartPage == null)
            {
                return;
            }

            web.Context.Load(webPartPage);
            web.Context.ExecuteQuery();

            LimitedWebPartManager limitedWebPartManager = webPartPage.GetLimitedWebPartManager(PersonalizationScope.Shared);
            web.Context.Load(limitedWebPartManager.WebParts, wps => wps.Include(wp => wp.WebPart.Title));
            web.Context.ExecuteQuery();

            if (limitedWebPartManager.WebParts.Count >= 0)
            {
                for (int i = 0; i < limitedWebPartManager.WebParts.Count; i++)
                {
                    WebPart oWebPart = limitedWebPartManager.WebParts[i].WebPart;
                    if (oWebPart.Title.Equals(title, StringComparison.InvariantCultureIgnoreCase))
                    {
                        limitedWebPartManager.WebParts[i].DeleteWebPart();
                        web.Context.ExecuteQuery();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Adds a blank Wiki page to the site pages library
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="wikiPageLibraryName">Name of the wiki page library</param>
        /// <param name="wikiPageName">Wiki page to operate on</param>
        /// <returns>The relative URL of the added wiki page</returns>
        public static string AddWikiPage(this Web web, string wikiPageLibraryName, string wikiPageName)
        {
            string wikiPageUrl = "";


            var pageLibrary = web.Lists.GetByTitle(wikiPageLibraryName);

            web.Context.Load(pageLibrary.RootFolder, f => f.ServerRelativeUrl);
            web.Context.ExecuteQuery();

            var pageLibraryUrl = pageLibrary.RootFolder.ServerRelativeUrl;
            var newWikiPageUrl = pageLibraryUrl + "/" + wikiPageName;

            var currentPageFile = web.GetFileByServerRelativeUrl(newWikiPageUrl);

            web.Context.Load(currentPageFile, f => f.Exists);
            web.Context.ExecuteQuery();

            if (!currentPageFile.Exists)
            {
                var newpage = pageLibrary.RootFolder.Files.AddTemplateFile(newWikiPageUrl, TemplateFileType.WikiPage);

                web.Context.Load(newpage);
                web.Context.ExecuteQuery();

                wikiPageUrl = UrlUtility.Combine("sitepages", wikiPageName);
            }

            return wikiPageUrl;
        }

        public static void AddWikiPageByUrl(this Web web, string serverRelativePageUrl, string html = null)
        {
            string folderName = serverRelativePageUrl.Substring(0, serverRelativePageUrl.LastIndexOf("/"));
            Folder folder = web.GetFolderByServerRelativeUrl(folderName);
            File file = folder.Files.AddTemplateFile(serverRelativePageUrl, TemplateFileType.WikiPage);

            web.Context.ExecuteQuery();
            if (html != null)
            {
                web.AddHtmlToWikiPage(serverRelativePageUrl, html);
            }
        }

        /// <summary>
        /// Sets a web part property
        /// </summary>
        /// <param name="web">The web to process</param>
        /// <param name="key">The key to update</param>
        /// <param name="value">The value to set</param>
        /// <param name="id">The id of the webpart</param>
        /// <param name="serverRelativePageUrl"></param>
        public static void SetWebPartProperty(this Web web, string key, string value, Guid id, string serverRelativePageUrl)
        {
            ClientContext context = web.Context as ClientContext;

            File file = web.GetFileByServerRelativeUrl(serverRelativePageUrl);

            context.Load(file, f => f.ListItemAllFields);
            context.ExecuteQuery();

            ListItem listItem = file.ListItemAllFields;
            LimitedWebPartManager wpm = file.GetLimitedWebPartManager(PersonalizationScope.Shared);

            context.Load(wpm.WebParts);

            WebPartDefinition def = wpm.WebParts.GetById(id);

            switch (key.ToLower())
            {
                case "title":
                    {
                        def.WebPart.Title = value;
                        break;
                    }
                case "titleurl":
                    {
                        def.WebPart.TitleUrl = value;
                        break;
                    }
                default:
                    {
                        def.WebPart.Properties[key] = value;
                        break;
                    }
            }
            def.SaveWebPartChanges();

            context.ExecuteQuery();

        }
    }
}
