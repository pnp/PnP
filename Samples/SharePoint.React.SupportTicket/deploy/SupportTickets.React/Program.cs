using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using System;
using System.Security;

namespace SupportTickets.React

{
    class Program
    {

        // TODO: update values before running the sample or blank them which 
        // triggers a prompt to ask for the value
        static string siteUrl = "https://michely.sharepoint.com/sites/support";
        static string username = "brian.michely@michely.onmicrosoft.com";
        static SecureString password = null;

        static void Main(string[] args)
        {
            ConsoleColor current = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Provisioning of the SharePoint.React.SupportTickets sample started...");
            Console.ForegroundColor = current;
            Console.WriteLine("");

            #region Get information about the site to deploy to
            // Request Office365 site from the user
            if (String.IsNullOrEmpty(siteUrl))
            {
                siteUrl = GetSite();
            }

            // Prompt for username 
            if (String.IsNullOrEmpty(username))
            {
                username = GetUserName();
            }

            // Prompt for password
            if (password == null || password.Length == 0)
            {
                password = GetPassword();
            }
            #endregion

            #region Deploy assets and lists
            ClientContext ctx = CreateContext();
            // Provision supporting js/jsx files to the Style Library
            ProvisionAssets(ctx);

            // Provision lists and items
            SetupManager.ProvisionLists(ctx);
            
            #endregion

            Console.WriteLine("Provisioning of the React Ticket Creation sample is done.");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Open a browser and navigate to {0} to start the application.", String.Format("{0}/Lists/TicketQueue", siteUrl));
            Console.ForegroundColor = current;
            Console.WriteLine("");
            Console.WriteLine("Press <enter> to continue...");
            Console.ReadLine();
        }

        #region helper methods
        private static void ProvisionSupportTicketComponents(ClientContext ctx)
        {
            Web web = ctx.Web;
            ctx.Load(web, w => w.ServerRelativeUrl);
            ctx.ExecuteQuery();
            Console.WriteLine("");

            string ticketFormPage = ctx.Web.AddWikiPage("Site Pages", "CreateTicket.aspx");
            if (!String.IsNullOrEmpty(ticketFormPage))
            {
                Console.WriteLine("Provisioning CreateTicket.aspx...");
                string newTicketPage = String.Format("{0}/{1}", web.ServerRelativeUrl, ticketFormPage);
                ctx.Web.AddLayoutToWikiPage(OfficeDevPnP.Core.WikiPageLayout.OneColumn, newTicketPage);
                ProvisionWebPart(ctx, newTicketPage, "Create-Ticket-Form-Template.js", isWikiPage: true);
            }
            else
            {
                Console.WriteLine("CreateTicket.aspx was already added");
            }

            if (!SetupManager.IsWebPartOnPage(ctx, String.Format("{0}/Lists/TicketsQueue/newform.aspx", web.ServerRelativeUrl), "Tickets Queue"))
            {
                string newFormUrl = string.Format("{0}/{1}", web.ServerRelativeUrl, "Lists/TicketsQueue/newform.aspx");
                SetupManager.CloseAllWebParts(ctx, newFormUrl);
                ProvisionWebPart(ctx, newFormUrl, "Ticket-NewForm.js");
            }
            else
            {
                Console.WriteLine("The New form page was already customized");
            }

            if (!SetupManager.IsWebPartOnPage(ctx, String.Format("{0}/Lists/TicketsQueue/editform.aspx", web.ServerRelativeUrl), "Tickets Queue"))
            {
                string editFormUrl = string.Format("{0}/{1}", web.ServerRelativeUrl, "Lists/TicketsQueue/editform.aspx");
                SetupManager.CloseAllWebParts(ctx, editFormUrl);
                ProvisionWebPart(ctx, editFormUrl, "Tickets-EditForm.js");
            }
            else
            {
                Console.WriteLine("The Edit form page was already customized");
            }
        }

        private static void ProvisionWebPart(ClientContext ctx, string relativePageUrl, string scriptFile, bool isWikiPage = false)
        {
            Console.WriteLine("Provisioning web part...");

            string webPartXml = System.IO.File.ReadAllText(@"Assets\CreateTicket.dwp");
            //replace tokens
            string scriptUrl = String.Format("~sitecollection/Style Library/OfficeDevPnP/{0}", scriptFile);
            scriptUrl = Utilities.ReplaceTokens(ctx, scriptUrl);
            webPartXml = webPartXml.Replace("%ContentLink%", scriptUrl);

            OfficeDevPnP.Core.Entities.WebPartEntity webPart = new OfficeDevPnP.Core.Entities.WebPartEntity()
            {
                WebPartZone = "Main",
                WebPartIndex = 20,
                WebPartTitle = "Create Ticket",
                WebPartXml = webPartXml
            };

            Console.WriteLine("Adding create ticket web part to " + relativePageUrl);
            if (isWikiPage)
            {
                ctx.Web.AddWebPartToWikiPage(relativePageUrl, webPart, 1, 1, false);
            }
            else
            {
                ctx.Web.AddWebPartToWebPartPage(relativePageUrl, webPart);
            }
            Console.WriteLine("");
        }


        private static void ProvisionAssets(ClientContext ctx)
        {
            Console.WriteLine("Provisioning assets:");

            string[] assetFileNames = {"app.dist.js",
                                  "app.dist.js.map"};

            List styleLibrary = ctx.Web.Lists.GetByTitle("Style Library");
            ctx.Load(styleLibrary, l => l.RootFolder);
            Folder pnpFolder = styleLibrary.RootFolder.EnsureFolder("OfficeDevPnP");
            foreach (string fileName in assetFileNames)
            {
                Console.WriteLine(fileName);

                File assetFile = pnpFolder.GetFile(fileName);
                if (assetFile != null)
                    assetFile.CheckOut();

                string localFilePath = "Assets/" + fileName;
                string newLocalFilePath = Utilities.ReplaceTokensInAssetFile(ctx, localFilePath);

                assetFile = pnpFolder.UploadFile(fileName, newLocalFilePath, true);
                assetFile.CheckIn("Uploaded by provisioning engine.", CheckinType.MajorCheckIn);
                ctx.ExecuteQuery();
                System.IO.File.Delete(newLocalFilePath);
            }
            Console.WriteLine("");

            // Yes, only one for now. More coming
            string[] scriptPartFileNames = {"reactcreatesupportticket.webpart"};

            Site site = ctx.Site;
            ctx.Load(site, s => s.Url);
            ctx.ExecuteQuery();
            String webPartGalleryUrl = site.Url.TrimEnd('/') + "/_catalogs/wp";

            var folder = site.RootWeb.GetList(webPartGalleryUrl).RootFolder;
            
            ctx.Load(folder);
            ctx.ExecuteQuery();

            foreach (string fileName in scriptPartFileNames)
            {
                Console.WriteLine(fileName);

                File assetFile = folder.GetFile(fileName);
                if (assetFile != null)
                    assetFile.CheckOut();

                string localFilePath = "ScriptParts/" + fileName;
                string newLocalFilePath = Utilities.ReplaceTokensInAssetFile(ctx, localFilePath);

                assetFile = folder.UploadFile(fileName, newLocalFilePath, true);              
                

                if (assetFile.CheckOutType == CheckOutType.Online)
                {
                    assetFile.CheckIn("Uploaded by provisioning engine.", CheckinType.MajorCheckIn);
                }
                
                ctx.ExecuteQuery();
                System.IO.File.Delete(newLocalFilePath);

                ListItem webpartItem = assetFile.ListItemAllFields;
                webpartItem["Group"] = "Add-in Script Part";
                webpartItem.Update();
                ctx.ExecuteQuery();
            }
            Console.WriteLine("");


            
        }

        private static ClientContext CreateContext()
        {
            ClientContext ctx = new ClientContext(siteUrl);
            ctx.Credentials = new SharePointOnlineCredentials(username, password);
            ctx.ExecuteQuery();

            Console.WriteLine("Connected to {0}", siteUrl);
            Console.WriteLine("");

            return ctx;
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

        #endregion


    }
}
