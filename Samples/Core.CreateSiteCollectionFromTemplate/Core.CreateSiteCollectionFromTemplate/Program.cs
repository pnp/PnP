using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client.Publishing;
using System.Security;
using System.IO;

namespace Core.CreateSiteCollection
{
    class Program
    {
        static void Main(string[] args)
        {
            //Method to invoke the process after collecting the details from user
            startSiteProvisioning();
        }

        private static void startSiteProvisioning()
        {
            string tenantUrl = GetSite("Give Office365 admin site URL: ");

            /* Prompt for Credentials */
            Console.WriteLine("Enter Credentials for {0}", tenantUrl);

            string userName = GetUserName();
            SecureString pwd = GetPassword();

            /* End Program if no Credentials */
            if (string.IsNullOrEmpty(userName) || (pwd == null))
                return;
            // Open connection to Office365 tenant
            ClientContext cc = new ClientContext(tenantUrl);
            cc.AuthenticationMode = ClientAuthenticationMode.Default;
            cc.Credentials = new SharePointOnlineCredentials(userName, pwd);

            //get the proposed site url from user
            string newSiteUrl = GetSite("Give new site collection URL: ");

            // Create site collection
            CreateSiteCollectionFromTemplate(cc, tenantUrl, newSiteUrl, userName);

            // Create new context to site collection
            ClientContext newCtx = new ClientContext(newSiteUrl);
            newCtx.Credentials = new SharePointOnlineCredentials(userName, pwd);

            //Upload web template and activate it
            UploadWebTemplate(newCtx);

            // Activate web template
            ApplySiteTemplate(newCtx);

            Console.WriteLine("All done, press enter to continue.");
            Console.ReadLine();
        }

        private static void CreateSiteCollectionFromTemplate(ClientContext cc, string tenantUrl, string newSiteUrl, string userName)
        {
            Tenant tenant = new Tenant(cc);
            SiteCreationProperties newsiteProp = new SiteCreationProperties();
            newsiteProp.Lcid = 1033;
            newsiteProp.Owner = userName;
            newsiteProp.Title = "New Site";
            newsiteProp.Url = newSiteUrl;
            newsiteProp.StorageMaximumLevel = 100; //MB
            newsiteProp.UserCodeMaximumLevel = 10;

            SpoOperation spoO = tenant.CreateSite(newsiteProp);
            cc.Load(spoO, i => i.IsComplete);
            cc.ExecuteQuery();

            while (!spoO.IsComplete)
            {
                //Wait for 30 seconds and then try again
                System.Threading.Thread.Sleep(10000);
                spoO.RefreshLoad();
                cc.ExecuteQuery();
                Console.WriteLine("Site creation status: " + (spoO.IsComplete ? "completed" : "waiting"));
            }

            Console.WriteLine("SiteCollection Created.");
        }

        private static void UploadWebTemplate(ClientContext cc)
        {
            Site newsite = cc.Site;
            Web newWeb = cc.Web;
            cc.Load(newsite);
            cc.Load(newWeb);
            cc.ExecuteQuery();
            List solutionGallery = newWeb.Lists.GetByTitle("Solution Gallery");

            cc.Load(solutionGallery);
            cc.Load(solutionGallery.RootFolder);
            cc.ExecuteQuery();

            string filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Contoso Template.wsp");
            string filename = Path.GetFileName(filepath);
            string filerelativeurl = solutionGallery.RootFolder.ServerRelativeUrl + "/" + filename;

            FileCreationInformation newFile = new FileCreationInformation();
            newFile.Content = System.IO.File.ReadAllBytes(filepath);
            newFile.Overwrite = true;
            newFile.Url = filename;

            Microsoft.SharePoint.Client.File uploadedFile = solutionGallery.RootFolder.Files.Add(newFile);
            cc.Load(uploadedFile);
            cc.ExecuteQuery();

            DesignPackageInfo wsp = new DesignPackageInfo()
            {
                PackageGuid = Guid.Empty,
                PackageName = "Contoso Template"
            };

            DesignPackage.Install(cc, cc.Site, wsp, filerelativeurl);
            cc.ExecuteQuery();
        }

        private static void ApplySiteTemplate(ClientContext cc)
        {
            WebTemplateCollection webTemps = cc.Web.GetAvailableWebTemplates(1033, false);
            cc.Load(webTemps);
            cc.ExecuteQuery();

            foreach (WebTemplate webtemp in webTemps)
            {
                if (webtemp.Title.Equals("Contoso Template"))
                {
                    Console.WriteLine(webtemp.Name + "|" + webtemp.Title + "|" + webtemp.Id);
                    cc.Web.ApplyWebTemplate(webtemp.Name);
                    cc.Web.Update();
                    cc.RequestTimeout = -1;
                    // Can time out... so not necessarely good process
                    cc.ExecuteQuery();
                }
            }
        }

        #region CONNECTIVITY METHODS

        static SecureString GetPassword()
        {
            SecureString sStrPwd = new SecureString();
            try
            {
                Console.Write("SharePoint Password : ");

                for (ConsoleKeyInfo keyInfo = Console.ReadKey(true); keyInfo.Key != ConsoleKey.Enter; keyInfo = Console.ReadKey(true))
                {
                    if (keyInfo.Key == ConsoleKey.Backspace)
                    {
                        if (sStrPwd.Length > 0)
                        {
                            sStrPwd.RemoveAt(sStrPwd.Length - 1);
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                            Console.Write(" ");
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        }
                    }
                    else if (keyInfo.Key != ConsoleKey.Enter)
                    {
                        Console.Write("*");
                        sStrPwd.AppendChar(keyInfo.KeyChar);
                    }

                }
                Console.WriteLine("");
            }
            catch (Exception e)
            {
                sStrPwd = null;
                Console.WriteLine(e.Message);
            }

            return sStrPwd;
        }

        static string GetUserName()
        {
            string strUserName = string.Empty;
            try
            {
                Console.Write("SharePoint Username : ");
                strUserName = Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                strUserName = string.Empty;
            }
            return strUserName;
        }

        static string GetSite(string message)
        {
            string siteUrl = string.Empty;
            try
            {
                Console.Write(message);
                siteUrl = Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                siteUrl = string.Empty;
            }
            return siteUrl;
        }
        #endregion

    }
}
