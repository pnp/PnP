using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Core.DownloadMultipleFilesJS
{
    class Program
    {
        // TODO: update values before running the sample or blank them which 
        // triggers a prompt to ask for the value
        static string siteUrl = "https://<tenant>.sharepoint.com/sites/dev";
        static string username = "<user>@<tenant>.onmicrosoft.com";
        static SecureString password = null;

        private static ClientContext CreateContext()
        {
            ClientContext ctx = new ClientContext(siteUrl);
            ctx.Credentials = new SharePointOnlineCredentials(username, password);
            ctx.ExecuteQuery();

            Console.WriteLine("Connected to {0}", siteUrl);
            Console.WriteLine("");

            return ctx;
        }

        static void ProvisionSiteAssets(ClientContext ctx)
        {
            Console.WriteLine("Provisioning files to SiteAssets library...");

            List listSiteAssets = ctx.Web.GetListByUrl("SiteAssets");
            ctx.Load(listSiteAssets);
            ctx.Load(listSiteAssets.RootFolder);
            ctx.ExecuteQueryRetry();

            string localAssetsFolderPath = @"Assets\SiteAssets\";
            System.IO.DirectoryInfo dirAssets = new System.IO.DirectoryInfo(localAssetsFolderPath);
            UploadFilesToLibrary(ctx, listSiteAssets.RootFolder, dirAssets);

            Console.WriteLine("Completed.");
            Console.WriteLine("");
        }

        static void UploadFilesToLibrary(ClientContext ctx, Folder spFolder, System.IO.DirectoryInfo localFolder)
        {
            foreach (var localFile in localFolder.GetFiles())
            {
                File file = spFolder.GetFile(localFile.Name);
                if (file != null)
                    file.CheckOut();
                Console.WriteLine("> File: " + localFile.Name);
                file = spFolder.UploadFile(localFile.Name, localFile.FullName, true);
                file.PublishFileToLevel(FileLevel.Published);
            }

            foreach (var localSubFolder in localFolder.GetDirectories())
            {
                Console.WriteLine("> Folder: " + localSubFolder.Name);
                Folder spSubFolder = spFolder.EnsureFolder(localSubFolder.Name);
                UploadFilesToLibrary(ctx, spSubFolder, localSubFolder);
            }
        }

        static void ProvisionDocumentLibraryRibbonExtensions(ClientContext ctx)
        {
            Console.WriteLine("Provisioning document library ribbon extensions...");
            Console.WriteLine("> Registering custom actions");
            string ribbonFileName = @"Assets\document-library-ribbon.xml";
            bool isAdded = ctx.Site.AddCustomAction(new OfficeDevPnP.Core.Entities.CustomActionEntity
            {
                Name = "OfficeDevPnP Document Library Ribbon Extensions",
                Description = "OfficeDevPnP Document Library Ribbon Extensions",
                Location = "CommandUI.Ribbon",
                RegistrationId = "101",
                RegistrationType = UserCustomActionRegistrationType.List,
                CommandUIExtension = System.IO.File.ReadAllText(ribbonFileName)
            });
            ctx.ExecuteQueryRetry();

            Console.WriteLine("> Registering JSLink for script loader");
            ctx.Site.AddJsBlock("OfficeDevPnP.Core.ScriptLoader", @"
    Type.registerNamespace('OfficeDevPnP');
    Type.registerNamespace('OfficeDevPnP.Core');
    OfficeDevPnP.Core.loadScript = function (url, callback) {
        var head = document.getElementsByTagName('head')[0];
        var script = document.createElement('script');
        script.src = url;

        // Attach handlers for all browsers 
        var done = false;
        script.onload = script.onreadystatechange = function () {
            if (!done && (!this.readyState
                                            || this.readyState == 'loaded'
                                            || this.readyState == 'complete')) {
                done = true;

                // Continue your code 
                callback();
                // Handle memory leak in IE 
                script.onload = script.onreadystatechange = null;
                head.removeChild(script);
            }
        };
        head.appendChild(script);
    }
", 1);

            Console.WriteLine("> Registering JSLink for ribbonmanager");
            ctx.Load(ctx.Web);
            ctx.ExecuteQueryRetry();
            string jsRibbonManagerUrl = ctx.Web.ServerRelativeUrl + "/SiteAssets/js/ribbonmanager.js";
            ctx.Site.AddJsLink("OfficeDevPnP.Core.RibbonManager", jsRibbonManagerUrl, 10);

            ctx.ExecuteQueryRetry();

            Console.WriteLine("Completed.");
            Console.WriteLine("");
        }

        private static SecureString GetPassword()
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

        private static string GetUserName()
        {
            string strUserName = string.Empty;
            try
            {
                Console.Write("SharePoint User Name : ");
                strUserName = Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                strUserName = string.Empty;
            }
            return strUserName;
        }

        private static string GetSite()
        {
            string siteUrl = string.Empty;
            try
            {
                Console.Write("Give Office365 site URL : ");
                siteUrl = Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                siteUrl = string.Empty;
            }
            return siteUrl;
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Provisioning OfficeDevPnPCore.DownloadMultipleFilesJS sample started...");

            // Request Office365 site from the user
            if (String.IsNullOrEmpty(siteUrl))
            {
                siteUrl = GetSite();
            }

            /* Prompt for Credentials */
            if (String.IsNullOrEmpty(username))
            {
                username = GetUserName();
            }

            if (password == null || password.Length == 0)
            {
                password = GetPassword();
            }

            ClientContext ctx = CreateContext();

            ProvisionSiteAssets(ctx);
            ProvisionDocumentLibraryRibbonExtensions(ctx);

            Console.WriteLine("Provisioning completed.");
            Console.Read();
        }
    }
}
