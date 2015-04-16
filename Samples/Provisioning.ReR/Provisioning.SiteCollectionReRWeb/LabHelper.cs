using Contoso.Provisioning.SiteCollectionCreationWeb.Models;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contoso.Provisioning.SiteCollectionCreationWeb
{
    public class LabHelper
    {
        private const string CAML_NEWREQUESTS = "<View><Query><Where><Eq><FieldRef Name='Status'/><Value Type='Text'>New</Value></Eq></Where></Query><RowLimit>100</RowLimit></View>";

        public void AddRequest(SiteRequestInformation siteRequest, ClientContext ctx)
        {
            Web _web = ctx.Web;
            List _list = _web.Lists.GetByTitle(Lists.SiteRepositoryTitle);
            ListItemCreationInformation _listItemCreation = new ListItemCreationInformation();
            ListItem _record = _list.AddItem(_listItemCreation);

            _record[SiteRequestFields.Title] = siteRequest.Title;
            _record[SiteRequestFields.Description] = siteRequest.Description;
            _record[SiteRequestFields.Template] = siteRequest.Template;
            _record[SiteRequestFields.State] = siteRequest.RequestStatus;
            _record[SiteRequestFields.Url] = siteRequest.Url;
            _record[SiteRequestFields.TimeZone] = siteRequest.TimeZoneId;
            FieldUserValue _siteOwner = FieldUserValue.FromUser(siteRequest.SiteOwner.Email);
            _record[SiteRequestFields.Owner] = _siteOwner;

            _record.Update();
            ctx.ExecuteQuery();
        }

        /// <summary>
        /// Used to create a User Object to SharePointUser
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static SharePointUser BaseSetUser(ClientContext ctx, ListItem item, string field)
        {
            SharePointUser _owner = new SharePointUser();
            var _fieldUser = ((FieldUserValue)(item[field]));
            User _user = ctx.Web.EnsureUser(_fieldUser.LookupValue);
            ctx.Load(_user, u => u.LoginName, u => u.Email, u => u.PrincipalType, u => u.Title);
            ctx.ExecuteQuery();

            _owner.Email = _user.Email;
            _owner.Login = _user.LoginName;
            _owner.Name = _user.Title;

            return _owner;
        }
          
        public void SetThemeBasedOnName(ClientContext ctx, Web web, Web rootWeb, string themeName)
        {
            // Let's get instance to the composite look gallery
            List themeList = rootWeb.GetCatalog(124);
            ctx.Load(themeList);
            ctx.ExecuteQuery();

            CamlQuery query = new CamlQuery();
            string camlString = @"
                <View>
                    <Query>                
                        <Where>
                            <Eq>
                                <FieldRef Name='Name' />
                                <Value Type='Text'>{0}</Value>
                            </Eq>
                        </Where>
                     </Query>
                </View>";
            // Let's update the theme name accordingly
            camlString = string.Format(camlString, themeName);
            query.ViewXml = camlString;
            var found = themeList.GetItems(query);
            ctx.Load(found);
            ctx.ExecuteQuery();
            if (found.Count > 0)
            {
                Microsoft.SharePoint.Client.ListItem themeEntry = found[0];
                //Set the properties for applying custom theme which was jus uplaoded
                string spColorURL = null;
                if (themeEntry["ThemeUrl"] != null && themeEntry["ThemeUrl"].ToString().Length > 0)
                {
                    spColorURL = MakeAsRelativeUrl((themeEntry["ThemeUrl"] as FieldUrlValue).Url);
                }
                string spFontURL = null;
                if (themeEntry["FontSchemeUrl"] != null && themeEntry["FontSchemeUrl"].ToString().Length > 0)
                {
                    spFontURL = MakeAsRelativeUrl((themeEntry["FontSchemeUrl"] as FieldUrlValue).Url);
                }
                string backGroundImage = null;
                if (themeEntry["ImageUrl"] != null && themeEntry["ImageUrl"].ToString().Length > 0)
                {
                    backGroundImage = MakeAsRelativeUrl((themeEntry["ImageUrl"] as FieldUrlValue).Url);
                }

                // Set theme for demonstration
                web.ApplyTheme(spColorURL,
                                    spFontURL,
                                    backGroundImage,
                                    false);

                // Let's also update master page, if needed
                if (themeEntry["MasterPageUrl"] != null && themeEntry["MasterPageUrl"].ToString().Length > 0)
                {
                    web.MasterUrl = MakeAsRelativeUrl((themeEntry["MasterPageUrl"] as FieldUrlValue).Url); ;
                }

                ctx.ExecuteQuery();
            }
        }

        private string MakeAsRelativeUrl(string urlToProcess)
        {
            Uri uri = new Uri(urlToProcess);
            return uri.AbsolutePath;
        }


        public static string CreateSiteCollection(ClientContext ctx, string hostWebUrl, string template, string title, string description, string userEmail)
        {
            //get the base tenant admin urls
            var tenantStr = hostWebUrl.ToLower().Replace("-my", "").Substring(8);
            tenantStr = tenantStr.Substring(0, tenantStr.IndexOf("."));

            //create site collection using the Tenant object
            var webUrl = String.Format("https://{0}.sharepoint.com/{1}/{2}", tenantStr, "sites", title);
            var tenantAdminUri = new Uri(String.Format("https://{0}-admin.sharepoint.com", tenantStr));
            string realm = TokenHelper.GetRealmFromTargetUrl(tenantAdminUri);
            var token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, tenantAdminUri.Authority, realm).AccessToken;
            using (var adminContext = TokenHelper.GetClientContextWithAccessToken(tenantAdminUri.ToString(), token))
            {
                var tenant = new Tenant(adminContext);
                var properties = new SiteCreationProperties()
                {
                    Url = webUrl,
                    Owner = userEmail,
                    Title = title,
                    Template = template,
                    StorageMaximumLevel = 100,
                    UserCodeMaximumLevel = 100
                };

                //start the SPO operation to create the site
                SpoOperation op = tenant.CreateSite(properties);
                adminContext.Load(tenant);
                adminContext.Load(op, i => i.IsComplete);
                adminContext.ExecuteQuery();

                //check if site creation operation is complete
                while (!op.IsComplete)
                {
                    //wait 30seconds and try again
                    System.Threading.Thread.Sleep(30000);
                    op.RefreshLoad();
                    adminContext.ExecuteQuery();
                }
            }

            //get the new site collection
            var siteUri = new Uri(webUrl);
            token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, siteUri.Authority, realm).AccessToken;
            using (var newWebContext = TokenHelper.GetClientContextWithAccessToken(siteUri.ToString(), token))
            {
                var newWeb = newWebContext.Web;
                newWebContext.Load(newWeb);
                newWebContext.ExecuteQuery();

                new LabHelper().SetThemeBasedOnName(newWebContext, newWeb, newWeb, "Orange");

                // All done, let's return the newly created site
                return newWeb.Url;
            }
        }
    }
}