using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.SharePoint.Client;

namespace Portal.DataAccessLayer
{
    public class ConfigureAdminSite
    {
        public static void DoWork()
        {
            Logger.OpenLog("ConfigureAdminSite");
            Logger.LogInfoMessage("Configure Admin Site Collection", true);
            Logger.LogInfoMessage(String.Format("Operation starting {0}", DateTime.Now.ToString()), true);

            Logger.LogInfoMessage(String.Format("AppSettings:"), true);
            string adminUsername = String.Format("{0}{1}", (String.IsNullOrEmpty(Program.AdminDomain) ? "" : String.Format("{0}\\", Program.AdminDomain)), Program.AdminUsername);
            Logger.LogInfoMessage(String.Format("- Admin Username = {0}", adminUsername), true);

            string siteUrl = Helper.GetSiteUrl("Admin");
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
                    ProcessRootWeb(rootWeb);

                    Logger.LogInfoMessage("");
                    Logger.LogInfoMessage("---------------------------------------");
                    Logger.LogInfoMessage(String.Format(
                        "NOTE: Ensure that the 'PortalAdminSiteAbsoluteUrl' variable is set to [{0}] in the CDN JS file [{1}]",
                        site.Url,
                        Constants.CdnConfigurationFileName),
                        true);
                    Logger.LogInfoMessage("---------------------------------------");
                    Logger.LogInfoMessage("");
                }
            }
            catch (Exception ex)
            {
                Logger.LogErrorMessage(String.Format("ProcessSite() failed for {0}: Error={1}", siteUrl, ex.Message), false);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="web">Web to process</param>
        private static void ProcessRootWeb(Web web)
        {
            try
            {
                Logger.LogInfoMessage(String.Format("Processing Root Web: {0} ...", web.Url), true);

                Helper.EnsureSiteColumns(web);
                Helper.EnsurePortalConfigList(web);
                Helper.EnsureGlobalNavConfigList(web);

                // Force the crawler to re-index the lists of the web
                Logger.LogInfoMessage(String.Format("Scheduling Re-Index of Root Web: {0} ...", web.Url), true);
                web.ReIndexWeb();
            }
            catch (Exception ex)
            {
                Logger.LogErrorMessage(String.Format("ProcessRootWeb() failed for {0}: Error={1}", web.Url, ex.Message), false);
            }
        }
    }
}
