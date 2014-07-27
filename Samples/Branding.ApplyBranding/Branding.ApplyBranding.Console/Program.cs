using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Contoso.Branding.ApplyBranding
{
    class Program
    {
        internal static char[] trimChars = new char[] { '/' };

        static void Main(string[] args)
        {
            var branding = XDocument.Load("settings.xml").Element("branding");
            var url = branding.Attribute("url").Value;
            
            var username = GetUserName();
            var password = GetPassword();
            var credentials = new SharePointOnlineCredentials(username, password);

            foreach (var site in branding.Element("sites").Descendants("site"))
            {
                var siteUrl = url.TrimEnd(trimChars) + "/" + site.Attribute("url").Value.TrimEnd(trimChars);
                using (ClientContext clientContext = new ClientContext(siteUrl))
                {
                    clientContext.Credentials = credentials;
                    clientContext.Load(clientContext.Web);
                    clientContext.ExecuteQuery();

                    UploadFiles(clientContext, branding);
                    UploadMasterPages(clientContext, branding);
                    UploadPageLayouts(clientContext, branding);       
                }
            }            
            
            Console.WriteLine("Done!");
            Console.ReadLine();
        }

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
        #endregion
    }
}
