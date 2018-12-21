using System;
using System.Security;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using SharePointPnP.Modernization.Framework.Transform;


namespace Modernization.PageTransformation
{
    class Program
    {
        static void Main(string[] args)
        {
            string siteUrl = "https://contoso.sharepoint.com/sites/mytestsite";
            string userName = "joe@contoso.onmicrosoft.com";
            AuthenticationManager am = new AuthenticationManager();
            using (var cc = am.GetSharePointOnlineAuthenticatedContextTenant(siteUrl, userName, GetSecureString("Password")))
            {
                var pageTransformator = new PageTransformator(cc);
                
                // Use below override if you want to apply a filter on the pages to transform
                // var pages = cc.Web.GetPages("webparts.aspx");
                var pages = cc.Web.GetPages();

                foreach (var page in pages)
                {
                    PageTransformationInformation pti = new PageTransformationInformation(page)
                    {
                        // If target page exists, then overwrite it
                        Overwrite = true,

                        // Migrated page gets the name of the original page (default = false)
                        //TargetPageTakesSourcePageName = false,

                        // Give the migrated page a specific prefix (default is Migrated_)
                        //TargetPagePrefix = "Yes_",

                        // Configure the page header, empty value means ClientSidePageHeaderType.None (default = null)
                        //PageHeader = new ClientSidePageHeader(cc, ClientSidePageHeaderType.None, null),

                        // If the page is a home page then replace with stock home page (default = false)
                        //ReplaceHomePageWithDefaultHomePage = true,

                        // Replace images and iframes embedded inside a list or table in the wiki text with a placeholder and add respective images and video web parts at the bottom of the page (default = true)
                        //HandleWikiImagesAndVideos = false,
                    };
                    try
                    {
                        Console.WriteLine($"Transforming page {page.FieldValues["FileLeafRef"]}");
                        pageTransformator.Transform(pti);
                    }
                    catch(ArgumentException ex)
                    {
                        Console.WriteLine($"Page {page.FieldValues["FileLeafRef"]} could not be transformed: {ex.Message}");
                    }
                }
            }

            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();

        }

        private static SecureString GetSecureString(string label)
        {
            SecureString sStrPwd = new SecureString();
            try
            {
                Console.Write(String.Format("{0}: ", label));

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

    }
}
