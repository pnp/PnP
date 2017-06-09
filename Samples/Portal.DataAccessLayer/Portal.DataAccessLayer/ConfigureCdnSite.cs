using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.SharePoint.Client;

namespace Portal.DataAccessLayer
{
    public class ConfigureCdnSite
    {
        public static void DoWork()
        {
            Logger.OpenLog("ConfigureCdnSite");
            Logger.LogInfoMessage("Configure CDN", true);
            Logger.LogInfoMessage(String.Format("Operation starting {0}", DateTime.Now.ToString()), true);

            Logger.LogInfoMessage(String.Format("AppSettings:"), true);
            string adminUsername = String.Format("{0}{1}", (String.IsNullOrEmpty(Program.AdminDomain) ? "" : String.Format("{0}\\", Program.AdminDomain)), Program.AdminUsername);
            Logger.LogInfoMessage(String.Format("- Admin Username = {0}", adminUsername), true);

            string siteUrl = Helper.GetSiteUrl("CDN");
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

                EnsureCdn(web);
            }
            catch (Exception ex)
            {
                Logger.LogErrorMessage(String.Format("ProcessRootWeb() failed for {0}: Error={1}", web.Url, ex.Message), false);
            }
        }

        private static void EnsureCdn(Web web)
        {
            try
            {
                Logger.LogInfoMessage(String.Format("Ensuring CDN for {0} ...", web.Url), true);

                Folder cdn = web.EnsureFolderPath(Constants.CdnWebRelativeUrl);
                Folder images = cdn.EnsureFolder("images");
                Folder js = cdn.EnsureFolder("js");
                Folder styles = cdn.EnsureFolder("styles");

                EnsureCdnFiles(web, images, "images");
                EnsureCdnFiles(web, js, "js");
                EnsureCdnFiles(web, styles, "styles");

                Logger.LogSuccessMessage("Ensured CDN", false);

                Logger.LogInfoMessage("");
                Logger.LogInfoMessage("---------------------------------------");
                Logger.LogInfoMessage(String.Format(
                    "NOTE: Ensure that the 'src' attribute is set to [{0}] for all CDN file <script> tags in the custom masterpage file [{1}]", 
                    ((web.ServerRelativeUrl == "/") ? "" : web.ServerRelativeUrl) + Constants.CdnWebRelativeUrl, 
                    Constants.PortalMasterPageFileName), 
                    true);
                Logger.LogInfoMessage(String.Format(
                    "NOTE: Ensure that the 'PortalCdnUrl' variable is set to [{0}] in the CDN JS file [{1}]",
                    ((web.ServerRelativeUrl == "/") ? "" : web.ServerRelativeUrl) + Constants.CdnWebRelativeUrl,
                    Constants.CdnConfigurationFileName),
                    true);
                Logger.LogInfoMessage("---------------------------------------");
                Logger.LogInfoMessage("");
            }
            catch (Exception ex)
            {
                Logger.LogErrorMessage(String.Format("EnsureCdn() failed for {0}: Error={1}", web.Url, ex.Message), false);
            }
        }

        private static void EnsureCdnFiles(Web web, Folder folder, string localFolderPath)
        {
            Logger.LogInfoMessage(String.Format("Ensuring Files for CDN folder [{0}] ...", folder.ServerRelativeUrl), true);

            string localFilePath = Environment.CurrentDirectory + "\\" + localFolderPath;

            try
            {
                System.IO.DirectoryInfo sourceDir = new System.IO.DirectoryInfo(localFilePath);
                System.IO.FileInfo[] sourceFiles = null;

                Logger.LogInfoMessage(String.Format("Enumerating local file path [{0}]...", localFilePath), false);
                sourceFiles = sourceDir.GetFiles("*.*");

                if (sourceFiles != null)
                {
                    Logger.LogInfoMessage(String.Format("{1} files found in local file path [{0}]...", localFilePath, sourceFiles.Length), false);

                    foreach (System.IO.FileInfo fileInfo in sourceFiles)
                    {
                        string fileName = fileInfo.Name;
                        string filePath = fileInfo.FullName;

                        EnsureCdnFile(folder, fileName, filePath);
                    }
                }
                else
                {
                    Logger.LogInfoMessage(String.Format("No files found in local file path [{0}]...", localFilePath), false);
                }
                Logger.LogInfoMessage(String.Format("Ensured Files for CDN folder [{0}]", folder.ServerRelativeUrl), false);
            }
            catch (Exception ex)
            {
                Logger.LogErrorMessage(String.Format("EnsureCdnFiles() GetFile() failed for local file path [{0}]: Error={1}", localFilePath, ex.Message), false);
                return;
            }
        }

        private static void EnsureCdnFile(Folder folder, string fileName, string filePath)
        {
            Logger.LogInfoMessage(String.Format("Ensuring CDN File {0} ...", fileName), false);

            try
            {
                File tryFile = null;
                try
                {
                    tryFile = folder.GetFile(fileName);
                }
                catch { }

                if (tryFile == null)
                {
                    Logger.LogInfoMessage(String.Format("Uploading CDN File {0}...", filePath), false);
                    File uploadedFile = folder.UploadFile(fileName, filePath, false);

                    Logger.LogInfoMessage(String.Format("Publishing CDN File {0}...", fileName), false);
                    uploadedFile.PublishFileToLevel(FileLevel.Published);
                }
                Logger.LogInfoMessage(String.Format("Ensured CDN File {0}", fileName), false);
            }
            catch (Exception ex)
            {
                Logger.LogErrorMessage(String.Format("EnsureCdnFile() failed for {0}: Error={1}", fileName, ex.Message), false);
            }
        }
    }
}
