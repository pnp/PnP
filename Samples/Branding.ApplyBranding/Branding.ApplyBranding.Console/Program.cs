using Microsoft.SharePoint.Client;
using System;
using System.Security;
using System.Xml.Linq;

namespace Contoso.Branding.ApplyBranding
{
    class Program
    {
        internal static char[] trimChars = new char[] { '/' };

        static void Main(string[] args)
        {
            var isOnline = false;
            SharePointOnlineCredentials credentials = null;

            //check to ensure there's at least one argument
            if (args.Length < 1 || args.Length > 2)
            {
                DisplayUsage();
                return;
            }
            else if (args.Length > 1)
            {
                //assuming online
                if (args[1] == "online")
                {
                    isOnline = true;
                    //only relevant if SharePoint Online
                    var username = GetUserName();
                    var password = GetPassword();
                    credentials = new SharePointOnlineCredentials(username, password);
                }
            }
            
            //activate or deactivate the branding
            if (args[0].ToLower() == "activate" || args[0].ToLower() == "deactivate")
            {
                var branding = XDocument.Load("settings.xml").Element("branding");
                var url = branding.Attribute("url").Value;

                foreach (var site in branding.Element("sites").Descendants("site"))
                {
                    var siteUrl = url.TrimEnd(trimChars) + "/" + site.Attribute("url").Value.TrimEnd(trimChars);
                    using (ClientContext clientContext = new ClientContext(siteUrl))
                    {
                        if (isOnline)
                        {
                            clientContext.Credentials = credentials;
                        }

                        clientContext.Load(clientContext.Web);
                        clientContext.ExecuteQuery();
                        switch (args[0].ToLower())
                        {
                            case "activate":
                                UploadFiles(clientContext, branding);
                                UploadMasterPages(clientContext, branding);
                                UploadPageLayouts(clientContext, branding);
                                break;
                            case "deactivate":
                                RemoveFiles(clientContext, branding);
                                RemoveMasterPages(clientContext, branding);
                                RemovePageLayouts(clientContext, branding);
                                break;
                        }
                    }
                }
                Console.WriteLine("Done!");
                Console.ReadLine();
            }                
            //invalid parameter(s)
            else
            {
                DisplayUsage();
                return;
            }
        }

        #region "activate branding functions"

        private static void UploadFiles(ClientContext clientContext, XElement branding)
        {
            foreach (var file in branding.Element("files").Descendants("file"))
            {
                var name = file.Attribute("name").Value;
                var folder = file.Attribute("folder").Value.TrimEnd(trimChars);
                var path = file.Attribute("path").Value.TrimEnd(trimChars);

                BrandingHelper.UploadFile(clientContext, name, folder, path);
            }
        }
        
        private static void UploadMasterPages(ClientContext clientContext, XElement branding)
        {
            foreach (var masterpage in branding.Element("masterpages").Descendants("masterpage"))
            {
                var name = masterpage.Attribute("name").Value;
                var folder = masterpage.Attribute("folder").Value.TrimEnd(new char[] { '/' });

                BrandingHelper.UploadMasterPage(clientContext, name, folder);
            }
        }

        private static void UploadPageLayouts(ClientContext clientContext, XElement branding)
        {
            foreach (var pagelayout in branding.Element("pagelayouts").Descendants("pagelayout"))
            {
                var name = pagelayout.Attribute("name").Value;
                var folder = pagelayout.Attribute("folder").Value.TrimEnd(trimChars);
                var publishingAssociatedContentType = pagelayout.Attribute("publishingAssociatedContentType").Value;
                var title = pagelayout.Attribute("title").Value;

                BrandingHelper.UploadPageLayout(clientContext, name, folder, title, publishingAssociatedContentType);
            }
        }

        #endregion

        #region "deactivate branding functions"

        private static void RemoveFiles(ClientContext clientContext, XElement branding)
        {
            var name = "";
            var folder = "";
            var path = "";            
            foreach (var file in branding.Element("files").Descendants("file"))
            {
                name = file.Attribute("name").Value;
                folder = file.Attribute("folder").Value.TrimEnd(trimChars);
                path = file.Attribute("path").Value.TrimEnd(trimChars);

                BrandingHelper.RemoveFile(clientContext, name, folder, path);
            }
            BrandingHelper.RemoveFolder(clientContext, folder, path);
        }

        private static void RemoveMasterPages(ClientContext clientContext, XElement branding)
        {
            var name = "";
            var folder = "";
            foreach (var masterpage in branding.Element("masterpages").Descendants("masterpage"))
            {
                name = masterpage.Attribute("name").Value;
                folder = masterpage.Attribute("folder").Value.TrimEnd(new char[] { '/' });

                BrandingHelper.RemoveMasterPage(clientContext, name, folder);
            }
            BrandingHelper.RemoveFolder(clientContext, folder, "_catalogs/masterpage");
        }

        private static void RemovePageLayouts(ClientContext clientContext, XElement branding)
        {
            foreach (var pagelayout in branding.Element("pagelayouts").Descendants("pagelayout"))
            {
                var name = pagelayout.Attribute("name").Value;
                var folder = pagelayout.Attribute("folder").Value.TrimEnd(trimChars);
                var publishingAssociatedContentType = pagelayout.Attribute("publishingAssociatedContentType").Value;
                var title = pagelayout.Attribute("title").Value;

                BrandingHelper.RemovePageLayout(clientContext, name, folder);
            }
        }

        #endregion

        #region "helper functions"

        static SecureString GetPassword()
        {
            SecureString sStrPwd = new SecureString();
            
            try
            {
                Console.Write("SharePoint Password: ");

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
                Console.Write("SharePoint Username: ");
                strUserName = Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                strUserName = string.Empty;
            }
            return strUserName;
        }

        static void DisplayUsage()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Please specify 'activate' or 'deactivate' and optionally 'online'");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Example 1 (SharePoint Online): \n Contoso.Branding.ApplyBranding.Console.exe activate online");
            Console.WriteLine("Example 2 (SharePoint Online):  \n Contoso.Branding.ApplyBranding.Console.exe deactivate online");
            Console.WriteLine("Example 3 (SharePoint On-premises):  \n Contoso.Branding.ApplyBranding.Console.exe activate");
            Console.WriteLine("Example 4 (SharePoint On-premises):  \n Contoso.Branding.ApplyBranding.Console.exe deactivate");
            Console.ResetColor();
        }
        
        #endregion
    }
}
