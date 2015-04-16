using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Contoso.Branding.SetThemeToSite
{
    public class ThemeManager
    {
        /// <summary>
        /// Deploy new theme to site collection. Resolves root web, if sub site is given as paraneter
        /// </summary>
        /// <param name="cc">Client Context with connection information</param>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="themeName">Name for the new theme</param>
        /// <param name="colorFilePath">Color file for the theme</param>
        /// <param name="fontFilePath">Font file for the theme</param>
        /// <param name="backgroundImagePath">Background image for the team</param>
        /// <param name="masterPageName">Master page name for the theme. Only name of the master page needed, no full path to catalog</param>
        public void DeployContosoThemeToWeb(ClientContext cc, Web web, string themeName, string colorFilePath, string fontFilePath, string backgroundImagePath, string masterPageName)
        {
            Web rootWeb;

            if (EnsureWeb(cc, web, "ServerRelativeUrl").ServerRelativeUrl.ToLowerInvariant() != 
                    EnsureSite(cc, cc.Site, "ServerRelativeUrl").ServerRelativeUrl.ToLowerInvariant())
            {
                // get instances to root web, since we are processign currently sub site 
                rootWeb = cc.Site.RootWeb;
                cc.Load(rootWeb);
                cc.ExecuteQuery();
            }
            else
            {
                // Let's double check that the web is available
                rootWeb = EnsureWeb(cc, web, "Title");
            }

            // Deploy files one by one to proper location
            if (!string.IsNullOrEmpty(colorFilePath))
            {
                DeployFileToThemeFolderSite(cc, rootWeb, colorFilePath);
            }
            if (!string.IsNullOrEmpty(fontFilePath))
            {
                DeployFileToThemeFolderSite(cc, rootWeb, fontFilePath);
            }
            if (!string.IsNullOrEmpty(backgroundImagePath))
            {
                DeployFileToThemeFolderSite(cc, rootWeb, backgroundImagePath);
            }

            // Let's also add entry to the Theme catalog. This is not actually required, but provides visibility for the theme option, if manually changed
            AddNewThemeOptionToSite(cc, rootWeb, themeName, colorFilePath, fontFilePath, backgroundImagePath, masterPageName);
        }

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

        private void DeployFileToThemeFolderSite(ClientContext clientContext, Web web, string sourceAddress)
        {
            // Get the path to the file which we are about to deploy
            string file = sourceAddress;

            List themesList = web.GetCatalog(123);
            // get the theme list
            clientContext.Load(themesList);
            clientContext.ExecuteQuery();
            Folder rootfolder = themesList.RootFolder;
            clientContext.Load(rootfolder);
            clientContext.Load(rootfolder.Folders);
            clientContext.ExecuteQuery();
            Folder folder15 = rootfolder;
            foreach (Folder folder in rootfolder.Folders)
            {
                if (folder.Name == "15")
                {
                    folder15 = folder;
                    break;
                }
            }

            // Use CSOM to uplaod the file in
            FileCreationInformation newFile = new FileCreationInformation();
            newFile.Content = System.IO.File.ReadAllBytes(file);
            newFile.Url = folder15.ServerRelativeUrl + "/" + Path.GetFileName(sourceAddress);
            newFile.Overwrite = true;
            Microsoft.SharePoint.Client.File uploadFile = folder15.Files.Add(newFile);
            clientContext.Load(uploadFile);
            clientContext.ExecuteQuery();
        }

        private void AddNewThemeOptionToSite(ClientContext clientContext, Web web, string themeName, string colorFilePath, string fontFilePath, string backGroundPath, string masterPageName)
        {
            // Let's get instance to the composite look gallery
            List themesOverviewList = web.GetCatalog(124);
            clientContext.Load(themesOverviewList);
            clientContext.ExecuteQuery();
            // Is the item already in the list?
            if (!ThemeEntryExists(clientContext, web, themesOverviewList, themeName))
            {
                // if web information is nto used, load it
                if (!web.IsObjectPropertyInstantiated("ServerRelativeUrl"))
                {
                    clientContext.Load(web);
                    clientContext.ExecuteQuery();
                }
                // Let's create new theme entry. Notice that theme selection is not available from UI in personal sites, so this is just for consistency sake
                ListItemCreationInformation itemInfo = new ListItemCreationInformation();
                Microsoft.SharePoint.Client.ListItem item = themesOverviewList.AddItem(itemInfo);
                item["Name"] = themeName;
                item["Title"] = themeName;
                if (!string.IsNullOrEmpty(colorFilePath))
                {
                    item["ThemeUrl"] = URLCombine(web.ServerRelativeUrl, string.Format("/_catalogs/theme/15/{0}", Path.GetFileName(colorFilePath)));
                }
                if (!string.IsNullOrEmpty(fontFilePath))
                {
                    item["FontSchemeUrl"] = URLCombine(web.ServerRelativeUrl, string.Format("/_catalogs/theme/15/{0}", Path.GetFileName(fontFilePath)));
                }
                if (!string.IsNullOrEmpty(backGroundPath))
                {
                    item["ImageUrl"] = URLCombine(web.ServerRelativeUrl, string.Format("/_catalogs/theme/15/{0}", Path.GetFileName(backGroundPath)));
                }
                // we use seattle master if anythign else is not set
                if (string.IsNullOrEmpty(masterPageName))
                { 
                    item["MasterPageUrl"] = URLCombine(web.ServerRelativeUrl, "/_catalogs/masterpage/seattle.master");
                }
                else
                {
                    item["MasterPageUrl"] = URLCombine(web.ServerRelativeUrl, string.Format("/_catalogs/masterpage/{0}", Path.GetFileName(masterPageName)));
                }
                
                item["DisplayOrder"] = 11;
                item.Update();
                clientContext.ExecuteQuery();
            }

        }

        private bool ThemeEntryExists(ClientContext clientContext, Web web, List themeList, string themeName)
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
            clientContext.Load(found);
            clientContext.ExecuteQuery();
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


        public void SetThemeBasedOnName(ClientContext cc, Web web, string themeName)
        {
            Web rootWeb; 
            if (EnsureWeb(cc, web, "ServerRelativeUrl").ServerRelativeUrl.ToLowerInvariant() !=
                    EnsureSite(cc, cc.Site, "ServerRelativeUrl").ServerRelativeUrl.ToLowerInvariant())
            {
                // get instances to root web, since we are processign currently sub site 
                rootWeb = cc.Site.RootWeb;
                cc.Load(rootWeb);
                cc.ExecuteQuery();
            }
            else
            {
                rootWeb = web;
            }

            // Let's get instance to the composite look gallery
            List themeList = rootWeb.GetCatalog(124);
            cc.Load(themeList);
            cc.ExecuteQuery();

            // Double checking that theme exists
            if (ThemeEntryExists(cc, rootWeb, themeList, themeName))
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
                cc.Load(found);
                cc.ExecuteQuery();
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

                    cc.ExecuteQuery();
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