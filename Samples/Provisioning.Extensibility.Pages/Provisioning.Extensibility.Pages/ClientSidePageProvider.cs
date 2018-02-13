using OfficeDevPnP.Core.Framework.Provisioning.Extensibility;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Diagnostics;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers.TokenDefinitions;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers.Utilities;
using System.Text.RegularExpressions;

namespace Provisioning.Extensibility.Pages
{
    public class ClientSidePageProvider : IProvisioningExtensibilityHandler
    {
        private const string CAMLQueryByExtension = @"
                <View Scope='Recursive'>
                  <Query>
                    <Where>
                      <Contains>
                        <FieldRef Name='File_x0020_Type'/>
                        <Value Type='text'>aspx</Value>
                      </Contains>
                    </Where>
                  </Query>
                </View>";
        private const string FileRefField = "FileRef";
        private const string FileLeafRefField = "FileLeafRef";
        private const string ClientSideApplicationId = "ClientSideApplicationId";
        private static readonly Guid FeatureId_Web_ModernPage = new Guid("B6917CB1-93A0-4B97-A84D-7CF49975D4EC");


        #region Extensions for template creation
        public ProvisioningTemplate Extract(ClientContext ctx, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInformation, PnPMonitoredScope scope, string configurationData)
        {
            var web = ctx.Web;

            #region Extract all client side pages
            var clientSidePageContentsHelper = new ClientSidePageContentsHelper();

            // Extract the Home Page
            web.EnsureProperties(w => w.RootFolder.WelcomePage, w => w.ServerRelativeUrl, w => w.Url);
            var homePageUrl = web.RootFolder.WelcomePage;

            // Get pages library
            ListCollection listCollection = web.Lists;
            listCollection.EnsureProperties(coll => coll.Include(li => li.BaseTemplate, li => li.RootFolder));
            var sitePagesLibrary = listCollection.Where(p => p.BaseTemplate == (int)ListTemplateType.WebPageLibrary).FirstOrDefault();
            if (sitePagesLibrary != null)
            {
                CamlQuery query = new CamlQuery
                {
                    ViewXml = CAMLQueryByExtension
                };
                var pages = sitePagesLibrary.GetItems(query);
                web.Context.Load(pages);
                web.Context.ExecuteQueryRetry();
                if (pages.FirstOrDefault() != null)
                {
                    foreach (var page in pages)
                    {
                        string pageUrl = null;
                        string pageName = "";
                        if (page.FieldValues.ContainsKey(FileRefField) && !String.IsNullOrEmpty(page[FileRefField].ToString()))
                        {
                            pageUrl = page[FileRefField].ToString();
                            pageName = page[FileLeafRefField].ToString();
                        }
                        else
                        {
                            //skip page
                            continue;
                        }

                        // Is this page the web's home page?
                        bool isHomePage = false;
                        if (pageUrl.EndsWith(homePageUrl, StringComparison.InvariantCultureIgnoreCase))
                        {
                            isHomePage = true;
                        }

                        // Is this a client side page?
                        if (FieldExistsAndUsed(page, ClientSideApplicationId) && page[ClientSideApplicationId].ToString().Equals(FeatureId_Web_ModernPage.ToString(), StringComparison.InvariantCultureIgnoreCase))
                        {
                            // extract the page using the OOB logic
                            clientSidePageContentsHelper.ExtractClientSidePage(web, template, creationInformation, scope, pageUrl, pageName, isHomePage);
                        }
                    }
                }
            }
            #endregion

            #region Cleanup template
            // Mark all pages as overwrite
            foreach (var page in template.ClientSidePages)
            {
                page.Overwrite = true;
            }

            // Drop all lists except FaqList and Site Assets
            foreach (var list in template.Lists.ToList())
            {
                if (!(list.Url.Equals("Lists/FAQList", StringComparison.CurrentCultureIgnoreCase) || list.Url.Equals("SiteAssets", StringComparison.CurrentCultureIgnoreCase)))
                {
                    template.Lists.Remove(list);
                }
            }

            // Mark all files to be published in target
            foreach (var file in template.Files)
            {
                file.Level = OfficeDevPnP.Core.Framework.Provisioning.Model.FileLevel.Published;
            }

            // Export the FAQ list items
            try
            {
                var faqTemplateList = template.Lists.Where(p => p.Url.Equals("Lists/FAQList", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                if (faqTemplateList != null)
                {
                    var faqList = web.GetListByUrl("Lists/FAQList");
                    ListItemCollection faqListItems = faqList.GetItems(CamlQuery.CreateAllItemsQuery());
                    web.Context.Load(faqListItems, f => f.Include(item => item["Title"],
                                                                  item => item["Product"],
                                                                  item => item["Section_x0020_Header"],
                                                                  item => item["Answer"],
                                                                  item => item["Visible"],
                                                                  item => item["SortOrder"]));
                    web.Context.ExecuteQueryRetry();
                    foreach (ListItem faq in faqListItems)
                    {
                        var faqValues = new Dictionary<string, string>();
                        faqValues.Add("Title", FieldExistsAndUsed(faq, "Title") ? faq["Title"].ToString() : "");
                        faqValues.Add("Product", FieldExistsAndUsed(faq, "Product") ? faq["Product"].ToString() : "");
                        faqValues.Add("Section_x0020_Header", FieldExistsAndUsed(faq, "Section_x0020_Header") ? faq["Section_x0020_Header"].ToString() : "");
                        faqValues.Add("Answer", FieldExistsAndUsed(faq, "Answer") ? TokenizeField(web, faq["Answer"].ToString()) : "");
                        faqValues.Add("Visible", FieldExistsAndUsed(faq, "Visible") ? faq["Visible"].ToString() : "");
                        faqValues.Add("SortOrder", FieldExistsAndUsed(faq, "SortOrder") ? faq["SortOrder"].ToString() : "");
                        faqTemplateList.DataRows.Add(new DataRow(faqValues, faq["Title"].ToString()));
                    }

                    // Configure data rows
                    faqTemplateList.DataRows.KeyColumn = "Title";
                    faqTemplateList.DataRows.UpdateBehavior = UpdateBehavior.Overwrite;

                }
            }
            catch (Exception ex)
            {
                scope.LogError("Something went wrong with the extraction of the list items. Error {0}", ex.Message);
            }
            #endregion

            return template;
        }

        private string TokenizeField(Web web, string json)
        {
            web.EnsureProperties(w => w.ServerRelativeUrl, w => w.Url);

            // HostUrl token replacement
            var uri = new Uri(web.Url);
            json = Regex.Replace(json, $"{uri.Scheme}://{uri.DnsSafeHost}:{uri.Port}", "{hosturl}", RegexOptions.IgnoreCase);
            json = Regex.Replace(json, $"{uri.Scheme}://{uri.DnsSafeHost}", "{hosturl}", RegexOptions.IgnoreCase);

            // Site token replacement
            json = Regex.Replace(json, "(\"" + web.ServerRelativeUrl + ")(?!&)", "\"{site}", RegexOptions.IgnoreCase);
            json = Regex.Replace(json, "'" + web.ServerRelativeUrl, "'{site}", RegexOptions.IgnoreCase);
            json = Regex.Replace(json, ">" + web.ServerRelativeUrl, ">{site}", RegexOptions.IgnoreCase);
            json = Regex.Replace(json, web.ServerRelativeUrl, "{site}", RegexOptions.IgnoreCase);

            return json;
        }

        private static bool FieldExistsAndUsed(ListItem item, string fieldName)
        {
            return (item.FieldValues.ContainsKey(fieldName) && item[fieldName] != null);
        }
        #endregion

        #region Extensions for template "applying"
        public IEnumerable<TokenDefinition> GetTokens(ClientContext ctx, ProvisioningTemplate template, string configurationData)
        {
            throw new NotImplementedException();
        }

        public void Provision(ClientContext ctx, ProvisioningTemplate template, ProvisioningTemplateApplyingInformation applyingInformation, TokenParser tokenParser, PnPMonitoredScope scope, string configurationData)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}