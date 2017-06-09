using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OfficeDevPnP.Core;

using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Publishing;

namespace Portal.DataAccessLayer
{
    public class ConfigureDemoSite
    {
        public static void DoWork()
        {
            Logger.OpenLog("ConfigureDemoSite");
            Logger.LogInfoMessage("Configure Demo Site Collection", true);
            Logger.LogInfoMessage(String.Format("Operation starting {0}", DateTime.Now.ToString()), true);

            Logger.LogInfoMessage(String.Format("AppSettings:"), true);
            string adminUsername = String.Format("{0}{1}", (String.IsNullOrEmpty(Program.AdminDomain) ? "" : String.Format("{0}\\", Program.AdminDomain)), Program.AdminUsername);
            Logger.LogInfoMessage(String.Format("- Admin Username = {0}", adminUsername), true);

            string siteUrl = Helper.GetSiteUrl("Demo");
            ProcessSite(siteUrl);

            Logger.LogInfoMessage(String.Format("Operation completed {0}", DateTime.Now.ToString()), true);
            Logger.CloseLog();
        }

        /// <summary>
        /// </summary>
        /// <param name="siteUrl">URL of the site collection to process</param>
        private static void ProcessSite(string siteUrl)
        {
            try
            {
                Logger.LogInfoMessage(String.Format("Processing Site: {0} ...", siteUrl), true);

                using (ClientContext userContext = Helper.CreateAuthenticatedUserContext(Program.AdminDomain, Program.AdminUsername, Program.AdminPassword, siteUrl))
                {
                    Site site = userContext.Site;
                    Web rootWeb = userContext.Site.RootWeb;
                    userContext.Load(site);
                    userContext.Load(rootWeb);
                    userContext.ExecuteQueryRetry();

                    // Configure the root web
                    string mpPath = ProcessRootWeb(rootWeb);

                    // Create/Configure the demo subweb
                    ProcessDemoWeb(rootWeb, mpPath);
                }
            }
            catch (Exception ex)
            {
                Logger.LogErrorMessage(String.Format("ProcessSite() failed for {0}: Error={1}", siteUrl, ex.Message), false);
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="webUrl">Url of web to process</param>
        private static string ProcessRootWeb(Web web)
        {
            string mpServerRelativeUrl = String.Empty;
            try
            {
                Logger.LogInfoMessage(String.Format("Processing Root Web: {0} ...", web.Url), true);

                Helper.EnsureSiteColumns(web);
                Helper.EnsureCompanyLinksConfigList(web);
                mpServerRelativeUrl = Helper.EnsureMasterPage(web, Constants.PortalMasterPageFileName);
                Helper.SetMasterPages(web, mpServerRelativeUrl, false, Helper.MasterPageOptions.SiteMasterPageOnly);
                Helper.EnsureLocalNavConfigList(web);
                EnsureWelcomePage(web);

                // Force the crawler to re-index the lists of the web
                Logger.LogInfoMessage(String.Format("Scheduling Re-Index of Root Web: {0} ...", web.Url), true);
                web.ReIndexWeb();
            }
            catch (Exception ex)
            {
                Logger.LogErrorMessage(String.Format("ProcessRootWeb() failed for {0}: Error={1}", web.Url, ex.Message), false);
            }
            return mpServerRelativeUrl;
        }

        /// <summary>
        /// </summary>
        /// <param name="parentWeb">parent web to process</param>
        private static void ProcessDemoWeb(Web parentWeb, string mpServerRelativeUrl)
        {
            try
            {
                Logger.LogInfoMessage(String.Format("Processing DAL Demo Web: {0}/{1} ...", parentWeb.Url, Constants.DalDemoWebLeafUrl), true);

                // Create and configure the demo subweb
                if (!parentWeb.WebExists(Constants.DalDemoWebLeafUrl))
                {
                    Logger.LogInfoMessage(String.Format("Creating SubWeb: {0} ...", Constants.DalDemoWebTitle), true);
                    parentWeb.CreateWeb(Constants.DalDemoWebTitle, Constants.DalDemoWebLeafUrl, Constants.DalDemoWebDescription, "BLANKINTERNET#0", 1033, true, true);
                }

                Web dalWeb = parentWeb.GetWeb(Constants.DalDemoWebLeafUrl);
                parentWeb.Context.Load(dalWeb);
                parentWeb.Context.ExecuteQueryRetry();

                Helper.SetMasterPages(dalWeb, mpServerRelativeUrl, true, Helper.MasterPageOptions.SiteMasterPageOnly);
                Helper.EnsureLocalNavConfigList(dalWeb);
                EnsureWelcomePage(dalWeb);

                // Force the crawler to re-index the lists of the web
                Logger.LogInfoMessage(String.Format("Scheduling Re-Index of DAL Demo Web: {0} ...", dalWeb.Url), true);
                dalWeb.ReIndexWeb();
            }
            catch (Exception ex)
            {
                Logger.LogErrorMessage(String.Format("ProcessDemoWeb() failed for {0}: Error={1}", parentWeb.Url, ex.Message), false);
            }
        }

        private static void EnsureWelcomePage(Web web)
        {
            try
            {
                Logger.LogInfoMessage(String.Format("Ensuring Welcome Page for {0} ...", web.Url), true);

                PublishingPage page = web.GetPublishingPage(Constants.WelcomePageName);
                if (page == null)
                {
                    Logger.LogInfoMessage("- Creating Welcome Page...", false);
                    web.AddPublishingPage(Constants.WelcomePageName, "PageFromDocLayout", title: "Welcome", publish: false);

                    page = web.GetPublishingPage(Constants.WelcomePageName);
                    if (page == null)
                    {
                        Logger.LogErrorMessage(String.Format("EnsureWelcomePage() failed for {0}: Error=Could not retrieve newly-created Welcome Page.", web.Url), false);
                        return;
                    }

                    Logger.LogInfoMessage("- Editing Welcome Page...", false);

                    // Get parent list of item, this way we can handle all languages
                    var pagesLibrary = page.ListItem.ParentList;
                    web.Context.Load(pagesLibrary);
                    web.Context.ExecuteQueryRetry();

                    var pageItem = page.ListItem;

                    // Check out the page
                    web.Context.Load(pageItem, p => p.File.CheckOutType);
                    web.Context.ExecuteQueryRetry();
                    if (pagesLibrary.ForceCheckout || pagesLibrary.EnableVersioning)
                    {
                        if (pageItem.File.CheckOutType == CheckOutType.None)
                        {
                            pageItem.File.CheckOut();
                        }
                    }

                    // Set the page content
                    string localFilePath = Environment.CurrentDirectory + "\\Pages\\" + Constants.WelcomePageContentFileName;
                    string pageContent = System.IO.File.ReadAllText(localFilePath);
                    pageItem["PublishingPageContent"] = pageContent;
                    pageItem.Update();

                    Logger.LogInfoMessage("- Publishing Welcome Page...", false);

                    // Check in the page
                    web.Context.Load(pageItem, p => p.File.CheckOutType);
                    web.Context.ExecuteQueryRetry();
                    if (pageItem.File.CheckOutType != CheckOutType.None)
                    {
                        pageItem.File.CheckIn(string.Empty, CheckinType.MajorCheckIn);
                    }

                    // Publish the page
                    pageItem.File.Publish(string.Empty);
                    if (pagesLibrary.EnableModeration)
                    {
                        pageItem.File.Approve(string.Empty);
                    }

                    web.Context.ExecuteQueryRetry();
                }

                Logger.LogInfoMessage("- Establishing Welcome Page...", false);
                web.SetHomePage("Pages/" + Constants.WelcomePageName);

                Logger.LogInfoMessage(String.Format("Ensured welcome page for {0} ...", web.Url), false);
            }
            catch (Exception ex)
            {
                Logger.LogErrorMessage(String.Format("EnsureWelcomePage() failed for {0}: Error={1}", web.Url, ex.Message), false);
            }
        }

    }
}
