using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Branding.Refresh
{
    class Program
    {
        const string BRANDING_VERSION = "Contoso.Branding.Version";
        const string BRANDING_THEME = "Contoso.Branding.Theme";

        /// <summary>
        /// returns the directory holding the executing program
        /// </summary>
        public static string AppRootPath
        {
            get
            {
                return Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            }
        }

        static void Main(string[] args)
        {
            // Create a context to work with
            // Office 365 Multi-tenant sample - TODO Change your URL and username
            ClientContext cc = new AuthenticationManager().GetSharePointOnlineAuthenticatedContextTenant("https://bertonline.sharepoint.com", "bert.jansen@bertonline.onmicrosoft.com", GetPassWord());

            // Office 365 Dedicated sample - On-Premises sample
            //ClientContext cc = new AuthenticationManager().GetNetworkCredentialAuthenticatedContext("https://sp2013.bertonline.info", "administrator", GetPassWord(), "set1");

            // Get a list of sites: search is one way to obtain this list, alternative can be a site directory 
            List<SiteEntity> sites = cc.Web.SiteSearchScopedByUrl("https://bertonline.sharepoint.com");

            // Generic settings (apply changes on all webs or just root web
            bool applyChangesToAllWebs = true;

            // Settings for branding specific changes
            // Version of the new theme, if the site has a lower version it will be upgraded
            int currentBrandingVersion = 2;
            // Name of the theme that will be applied
            string currentThemeName = "SPCTheme";
            // Turn forceBranding to true to apply branding in case the site was not branded before
            bool forceBranding = true;

            // Optionally further refine the list of returned site collections by inspecting the url here we are looking for a specific value that is contained in the url

            var filteredSites = from p in sites
                                where p.Url.Contains("130042")
                                select p;

            List<SiteEntity> sitesAndSubSites = new List<SiteEntity>();
            if (applyChangesToAllWebs)
            {
                // we want to update all webs, so the list of sites is extended with all sub sites
                foreach (SiteEntity site in filteredSites)
                {
                    sitesAndSubSites.Add(new SiteEntity() { Url = site.Url, Title = site.Title, Template = site.Template });
                    GetSubSites(cc, site.Url, ref sitesAndSubSites);
                }
                sites = sitesAndSubSites;
            }

            // iterate the list of sites and perform the wanted updates
            foreach (SiteEntity site in sites)
            {
                //Create a clientcontext for the site we're inspecting
                using (ClientContext siteContext = new ClientContext(site.Url))
                {
                    siteContext.Credentials = cc.Credentials;
                    //Process the branding updates for the site
                    ProcessBrandingUpdate(siteContext, site, currentThemeName, currentBrandingVersion, forceBranding);
                }
            }

            Console.WriteLine("----------------------------------------------------------------------");
            Console.ReadLine();
        }

        /// <summary>
        /// Recursively gets the sub sites of the passed sites
        /// </summary>
        /// <param name="cc">Client context of the calling method</param>
        /// <param name="siteUrl">Url of the passed parent site</param>
        /// <param name="sites">List of SiteEntity objects</param>
        private static void GetSubSites(ClientContext cc, string siteUrl, ref List<SiteEntity> sites)
        {
            using (ClientContext ccParent = new ClientContext(siteUrl))
            {
                ccParent.Credentials = cc.Credentials;

                Web parentWeb = ccParent.Web;
                ccParent.Load(parentWeb, website => website.Webs,
                                         website => website.WebTemplate,
                                         website => website.Title);
                ccParent.ExecuteQuery();

                foreach (Web subWeb in parentWeb.Webs)
                {
                    string newUrl = GetDomain(siteUrl) + subWeb.ServerRelativeUrl;
                    sites.Add(new SiteEntity() { Url = newUrl, Title = subWeb.Title, Template = subWeb.WebTemplate });
                    GetSubSites(ccParent, newUrl, ref sites);
                }
            }
        }

        /// <summary>
        /// Processess the branding updates for the passed site
        /// </summary>
        /// <param name="cc">clientcontext of the site to operate on</param>
        /// <param name="site">Information about the site that will be processed</param>
        /// <param name="currentThemeName">Theme to be applied</param>
        /// <param name="currentBrandingVersion">Version of the theme that should be applied</param>
        /// <param name="forceBranding">Enforce branding when the branding was not yet set</param>
        private static void ProcessBrandingUpdate(ClientContext cc, SiteEntity site, string currentThemeName, int currentBrandingVersion, bool forceBranding)
        {
            // Check if we've a property bag entry 
            string themeName = cc.Web.GetPropertyBagValueString(BRANDING_THEME, "");

            if (!String.IsNullOrEmpty(themeName))
            {
                // No theme property bag entry, assume no theme has been applied
                if (themeName.Equals(currentThemeName, StringComparison.InvariantCultureIgnoreCase))
                {
                    Console.WriteLine("Theme {0} is set for site {1}", themeName, site.Url);
                    // the used theme matches to the theme we want to update
                    int? brandingVersion = cc.Web.GetPropertyBagValueInt(BRANDING_VERSION, 0);
                    if (brandingVersion < currentBrandingVersion || forceBranding)
                    {
                        Console.WriteLine("Theme {0} has version {1} while version {2} is needed. Theme will be updated", themeName, brandingVersion, currentBrandingVersion);
                        // The used theme is having an older version or the branding version was property bag entry was removed
                        DeployTheme(cc, currentThemeName);
                        // Set the web propertybag entries
                        cc.Web.SetPropertyBagValue(BRANDING_THEME, currentThemeName);
                        cc.Web.SetPropertyBagValue(BRANDING_VERSION, currentBrandingVersion);
                    }
                    else
                    {
                        // We're good, no action needed
                        Console.WriteLine("Theme {0} has latest version {1}. Theme will not be updated", themeName, brandingVersion);
                    }
                }
                else
                {
                    // Theme does not match what we've expected, don't change it
                    Console.WriteLine("Theme {0} was applied. No update required", themeName);
                }
            }
            else
            {
                // No theme property bag entry, assume no theme has been applied, so this site should not be updated
                if (forceBranding)
                {
                    Console.WriteLine("Web property bag {0} is not set for site {1}, but force branding is configured, so set theme {1} for site {2}", BRANDING_THEME, themeName, site.Url);

                    // The used theme is having an older version or the branding version was property bag entry was removed
                    DeployTheme(cc, currentThemeName);
                    // Set the web propertybag entries
                    cc.Web.SetPropertyBagValue(BRANDING_THEME, currentThemeName);
                    cc.Web.SetPropertyBagValue(BRANDING_VERSION, currentBrandingVersion);
                }
                else
                {
                    Console.WriteLine("Web property bag {0} is not set for site {1}...skip this site ", BRANDING_THEME, site.Url);
                }
            }
        }

        /// <summary>
        /// Update the theme information for the passed site which can be a sub site or root site
        /// </summary>
        /// <param name="cc">CLient context of the site to operate on</param>
        /// <param name="themeName">Theme to apply</param>
        private static void DeployTheme(ClientContext cc, string themeName)
        {
            string themeRoot = Path.Combine(AppRootPath, String.Format(@"Themes\{0}", themeName));
            string spColorFile = Path.Combine(themeRoot, string.Format("{0}.spcolor", themeName));
            if (!System.IO.File.Exists(spColorFile))
            {
                spColorFile = null;
            }
            string spFontFile = Path.Combine(themeRoot, string.Format("{0}.spfont", themeName));
            if (!System.IO.File.Exists(spFontFile))
            {
                spFontFile = null;
            }
            string spBackgroundFile = Path.Combine(themeRoot, string.Format("{0}bg.jpg", themeName));
            if (!System.IO.File.Exists(spBackgroundFile))
            {
                spBackgroundFile = null;
            }
            string logoFile = Path.Combine(themeRoot, string.Format("{0}logo.png", themeName));

            if (IsThisASubSite(cc))
            {
                // Retrieve the context of the root site of the site collection
                using (ClientContext ccParent = cc.Clone(GetRootSite(cc)))
                {

                    // Show the approach that uses the relative paths to the theme files. Works for sub site composed look setting as well as for root site composed look settings
                    string colorFileRelativePath = "";
                    string fontFileRelativePath = "";
                    string backgroundFileRelativePath = "";
                    if (!String.IsNullOrEmpty(spColorFile))
                    {
                        colorFileRelativePath = ccParent.Web.UploadThemeFile(spColorFile).ServerRelativeUrl;
                    }
                    if (!String.IsNullOrEmpty(spFontFile))
                    {
                        fontFileRelativePath = ccParent.Web.UploadThemeFile(spFontFile).ServerRelativeUrl;
                    }
                    if (!String.IsNullOrEmpty(spBackgroundFile))
                    {
                        backgroundFileRelativePath = ccParent.Web.UploadThemeFile(spBackgroundFile).ServerRelativeUrl;
                    }

                    cc.Web.CreateComposedLookByUrl(themeName, colorFileRelativePath, fontFileRelativePath, backgroundFileRelativePath, null, replaceContent: true);
                    cc.Web.SetComposedLookByUrl(themeName);
                }
            }
            else
            {
                // Use the absolute paths to the theme files, works for the root site only
                if (!String.IsNullOrEmpty(spColorFile))
                {
                    cc.Web.UploadThemeFile(spColorFile);
                }
                if (!String.IsNullOrEmpty(spFontFile))
                {
                    cc.Web.UploadThemeFile(spFontFile);
                }
                if (!String.IsNullOrEmpty(spBackgroundFile))
                {
                    cc.Web.UploadThemeFile(spBackgroundFile);
                }

                cc.Web.CreateComposedLookByName(themeName, spColorFile, spFontFile, spBackgroundFile, null, replaceContent: true);
                cc.Web.SetComposedLookByUrl(themeName);
            }
        }

        /// <summary>
        /// Checks if we're processing a sub site or not
        /// </summary>
        /// <param name="ctx">Context of the site to check</param>
        /// <returns>true if sub site, false otherwise</returns>
        private static bool IsThisASubSite(ClientContext ctx)
        {
            //refractored to look at the root web url and compare
            Site _site = ctx.Site;
            ctx.Load(_site,
                site => site.RootWeb);
            ctx.ExecuteQuery();

            if (string.Compare(ctx.Url.TrimEnd('/'), _site.RootWeb.Url) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }

            //var url = new Uri(siteUrl);
            //var urlDomain = string.Format("{0}://{1}", url.Scheme, url.Host);
            //int idx = url.PathAndQuery.Substring(1).IndexOf("/") + 2;
            //var urlPath = url.PathAndQuery.Substring(0, idx);
            //var name = url.PathAndQuery.Substring(idx);
            //var index = name.IndexOf('/');

            //if (index == -1)
            //{
            //    return false;
            //}
            //else
            //{
            //    return true;
            //}
        }

        /// <summary>
        /// Get's the root domain of the site url
        /// </summary>
        /// <param name="siteUrl">site url to check</param>
        /// <returns>The root domain of the passed site url</returns>
        private static string GetDomain(string siteUrl)
        {
            var url = new Uri(siteUrl);
            var urlDomain = string.Format("{0}://{1}", url.Scheme, url.Host);

            return urlDomain;
        }

        /// <summary>
        /// Get's the root site collection url from a given site url
        /// </summary>
        /// <param name="ctx">ClientContext to Check/param>
        /// <returns>root site collection url of the passed site url</returns>
        private static string GetRootSite(ClientContext ctx)
        {
            //refractored to the get Root web. 
            Site _site = ctx.Site;
            ctx.Load(_site,
                site => site.RootWeb);
            ctx.ExecuteQuery();

            return _site.RootWeb.Url;

            //    var url = new Uri(siteUrl);
            //    var urlDomain = string.Format("{0}://{1}", url.Scheme, url.Host);
            //    int idx = url.PathAndQuery.Substring(1).IndexOf("/") + 2;
            //    var urlPath = url.PathAndQuery.Substring(0, idx);
            //    var name = url.PathAndQuery.Substring(idx);
            //    var index = name.IndexOf('/');

            //    return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0}{1}{2}", urlDomain, urlPath, name.Split("/".ToCharArray())[0]);
        }


        /// <summary>
        /// Gets the password input from the console window
        /// </summary>
        /// <returns>the entered password</returns>
        private static string GetPassWord()
        {
            Console.Write("SharePoint Password : ");

            string strPwd = "";

            for (ConsoleKeyInfo keyInfo = Console.ReadKey(true); keyInfo.Key != ConsoleKey.Enter; keyInfo = Console.ReadKey(true))
            {
                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (strPwd.Length > 0)
                    {
                        strPwd = strPwd.Remove(strPwd.Length - 1);
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        Console.Write(" ");
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    }
                }
                else if (keyInfo.Key != ConsoleKey.Enter)
                {
                    Console.Write("*");
                    strPwd += keyInfo.KeyChar;

                }

            }
            Console.WriteLine("");

            return strPwd;
        }
    }
}
