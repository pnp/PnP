using Microsoft.SharePoint.Client;
using System;
using System.Security;

namespace RestrictSiteToGeo
{
    class Program
    {
        static void Main(string[] args)
        {
            string siteUrl = "https://<your tenant>.sharepoint.com";
            string userName = "<your user>@<your tenant>.onmicrosoft.com";

            SecureString password = GetSecureString("Password");

            using (var ctx = new ClientContext(siteUrl))
            {
                ctx.Credentials = new SharePointOnlineCredentials(userName, password);

                // Check the readme file to learn how to register an application in azure ad and replace these values
                MultiGeoManager multiGeoManager = new MultiGeoManager(ctx, "<application id>", "<application password>", "<Azure AD domain>");

                try
                {
                    // Get geo for loaded site
                    ctx.Load(ctx.Site, p => p.GeoLocation);
                    ctx.ExecuteQuery();
                    Console.WriteLine($"Site {siteUrl} is hosted in geo: {ctx.Site.GeoLocation}");

                    // get current "geo restriction" setting
                    var restriction = multiGeoManager.GetSiteGeoRestriction(siteUrl);
                    Console.WriteLine($"Site {siteUrl} has following geo restriction set: {restriction}");

                    // set "geo restriction"
                    Console.WriteLine("Let's set to RestrictedToRegion.BlockFull");
                    multiGeoManager.SetSiteGeoRestriction(siteUrl, Microsoft.Online.SharePoint.TenantAdministration.RestrictedToRegion.BlockFull);

                    // load the newly set value
                    restriction = multiGeoManager.GetSiteGeoRestriction(siteUrl);
                    Console.WriteLine($"Site {siteUrl} has following geo restriction set: {restriction}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                Console.WriteLine("Press <enter> to continue...");
                Console.ReadLine();
            }
        }

        #region Helper methods
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

        private static String GetString(string label)
        {
            String sStrPwd = "";
            try
            {
                Console.Write(String.Format("{0}: ", label));

                for (ConsoleKeyInfo keyInfo = Console.ReadKey(true); keyInfo.Key != ConsoleKey.Enter; keyInfo = Console.ReadKey(true))
                {
                    if (keyInfo.Key == ConsoleKey.Backspace)
                    {
                        if (sStrPwd.Length > 0)
                        {
                            //sStrPwd.RemoveAt(sStrPwd.Length - 1);
                            sStrPwd.Remove(sStrPwd.Length - 1);
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                            Console.Write(" ");
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        }
                    }
                    else if (keyInfo.Key != ConsoleKey.Enter)
                    {
                        Console.Write("*");
                        //sStrPwd.AppendChar(keyInfo.KeyChar);
                        sStrPwd = sStrPwd + keyInfo.KeyChar;
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
        #endregion


    }
}
