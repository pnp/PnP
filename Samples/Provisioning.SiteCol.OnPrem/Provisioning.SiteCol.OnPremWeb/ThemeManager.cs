using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contoso.Provisioning.SiteCol.OnPremWeb
{
    public class ThemeManager
    {
        private Site EnsureSite(ClientContext cc, Site site, string propertyToCheck)
        {
            if (!site.IsObjectPropertyInstantiated(propertyToCheck))
            {
                // get instances to root web, since we are processign currently sub site 
                cc.Load(site);
                cc.ExecuteQuery();
            }
            return site;
        }

        private Web EnsureWeb(ClientContext cc, Web web, string propertyToCheck)
        {
            if (!web.IsObjectPropertyInstantiated(propertyToCheck))
            {
                // get instances to root web, since we are processign currently sub site 
                cc.Load(web);
                cc.ExecuteQuery();
            }
            return web;
        }
        private bool ThemeEntryExists(Web web, List themeList, string themeName)
        {

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
            web.Context.Load(found);
            web.Context.ExecuteQuery();
            if (found.Count > 0)
            {
                return true;
            }
            return false;
        }

        private string URLCombine(string baseUrl, string relativeUrl)
        {
            if (baseUrl.Length == 0)
                return relativeUrl;
            if (relativeUrl.Length == 0)
                return baseUrl;
            return string.Format("{0}/{1}", baseUrl.TrimEnd(new char[] { '/', '\\' }), relativeUrl.TrimStart(new char[] { '/', '\\' }));
        }


        /// <summary>
        /// Apply theme to the web. 
        /// Note. Only works with root site web, since does not resolve the root site if web is actually a sub site.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="web"></param>
        /// <param name="themeName"></param>
        public void SetThemeBasedOnName(Web web, string themeName)
        {

            // Let's get instance to the composite look gallery
            List themeList = web.GetCatalog(124);
            web.Context.Load(themeList);
            web.Context.ExecuteQuery();

            // Double checking that theme exists
            if (ThemeEntryExists(web, themeList, themeName))
            {
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
                web.Context.Load(found);
                web.Context.ExecuteQuery();
                if (found.Count > 0)
                {
                    ListItem themeEntry = found[0];
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

                    web.Context.ExecuteQuery();
                }
            }
        }

        private string MakeAsRelativeUrl(string urlToProcess)
        {
            Uri uri = new Uri(urlToProcess);
            return uri.AbsolutePath;
        }
    }
}