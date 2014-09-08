using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Core.SiteEnumeration
{
    class Program
    {
        static void Main(string[] args)
        {

            // Office 365 Multi-tenant sample
            ClientContext cc = new AuthenticationManager().GetSharePointOnlineAuthenticatedContextTenant("https://yourtenant-my.sharepoint.com", "keyzersoze@yourtenant.com", GetPassWord());

            // Office 365 Dedicated sample - On-Premises sample
            //ClientContext cc = new AuthenticationManager().GetNetworkCredentialAuthenticatedContext("https://my.contoso.com", "keyzersoze", GetPassWord(), "contoso");
            
            Console.WriteLine("----------------------------------------------------------------------");

            // Only lists the my sites
            List<SiteEntity> sites = cc.Web.MySiteSearch();

            // List all site collections
            //List<SiteEntity> sites = cc.Web.SiteSearch();

            // Lists site collections scoped to an URL
            //List<SiteEntity> sites = cc.Web.SiteSearchScopedByUrl("https://yourtenant.sharepoint.com");
            // List site collections scoped by title
            //List<SiteEntity> sites = cc.Web.SiteSearchScopedByTitle("test");

            // if needed furhter refine the returned set of site collections
            var filteredSites = from p in sites
                                where p.Url.Contains("my")
                                select p;

            foreach (var site in filteredSites)
            {
                Console.WriteLine("Title: {0}", site.Title);
                Console.WriteLine("Path: {0}", site.Url);
                Console.WriteLine("Description: {0}", site.Description);
                Console.WriteLine("Template: {0}", site.Template);
                Console.WriteLine("----------------------------------------------------------------------");
            }

            Console.WriteLine();
            Console.WriteLine("Press a key to continue...");
            Console.ReadKey();
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
