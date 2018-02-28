using Microsoft.SharePoint.Client;
using System;
using System.Security;

namespace GeoTenantInformationCollection
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string tenantAdminUrl = "https://contoso-admin.sharepoint.com";
                string userName = "admin@contoso.onmicrosoft.com";

                using (var clientContext = new ClientContext(tenantAdminUrl))
                {
                    clientContext.Credentials = new SharePointOnlineCredentials(userName, GetSecureString($"Password for {userName}"));

                    // Check the readme file to learn how to register an application in azure ad and replace these values
                    MultiGeoManager multiGeoManager = new MultiGeoManager(clientContext);
                    var geos = multiGeoManager.GetTenantGeoLocations();
                    foreach (var geo in geos)
                    {
                        Console.WriteLine($"{geo.GeoLocation} - {geo.RootSiteUrl} - {geo.TenantAdminUrl}");
                    }

                    Console.WriteLine("Press a key to continue...");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Multi-geo exception: ${ex.ToString()}");
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
        #endregion

    }
}
