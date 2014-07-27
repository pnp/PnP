using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Core.SitePermissions
{
    class Program
    {
        const string SAMPLE_USER_ACCOUNTNAME = "i:0#.f|membership|user@domain";

        static void Main(string[] args)
        {
            // Office 365 Multi-tenant sample
            // For a console application you need to create a context: this can be done by creating a context based on a user and password. 
            // Using this approach you can add administrators to site collections for which the user used to create the context is already an 
            // admin.
            Console.Write("Tenant name (e.g. ams if your SharePoint url is https://");
            ConsoleWriteColor("ams", ConsoleColor.Green);
            Console.WriteLine(".sharepoint.com) :");
            string tenantName = Console.ReadLine();

            Console.Write("Name of site collection to target permissions (siteName)\r\n(https://" + tenantName + ".sharepoint.com/sites/");
            ConsoleWriteColor("<siteName>", ConsoleColor.Green);
            Console.WriteLine(") :");
            string siteName = Console.ReadLine();
            
            Console.Write("Tenant admin name (");
            ConsoleWriteColor("<tenantAdminName>", ConsoleColor.Green);
            Console.WriteLine("@" + tenantName + ".onmicrosoft.com) :");
            string tenantAdminUserName = Console.ReadLine();

            Console.WriteLine("Create SharePoint ClientContext object for the web");
            string tenantAdminPassword = GetPassword();
            AuthenticationManager authManager = new AuthenticationManager();

            string targetSiteUrl = String.Format("https://{0}.sharepoint.com/sites/{1}", tenantName, siteName);
            string tenantAdmin = String.Format("{0}@{1}.onmicrosoft.com", tenantAdminUserName, tenantName);
            ClientContext cc = authManager.GetSharePointOnlineAuthenticatedContextTenant(targetSiteUrl, tenantAdmin, tenantAdminPassword);
            
            // Alternative approach is via an AppOnly app that has been registered via AppRegNew/AppInv. Good thing with this approach is
            // that you registered app can have tenant level permissions which makes that you can use below code to for example set the 
            // additional site collection administrators to site collections where you today are not listed as site collection admin. A 
            // typical example would be adding additional admins to OneDrive site collections to enable eDiscovery
            //ClientContext cc = authManager.GetAppOnlyAuthenticatedContext("https://tenantname-my.sharepoint.com/personal/user2", "<your tenant realm>", "<appID>", "<appsecret>");

            // Tenant admin site context
            Console.WriteLine("Create SharePoint ClientContext object for the tenant administration web");
            string tenantAdminUrl = String.Format("https://{0}-admin.sharepoint.com/", tenantName);
            ClientContext ccTenant = authManager.GetSharePointOnlineAuthenticatedContextTenant(tenantAdminUrl, tenantAdmin, tenantAdminPassword);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Admins for site collection {0}:", targetSiteUrl);
            // Get a list of current admins
            List<UserEntity> admins = cc.Web.GetAdministrators();
            foreach (var admin in admins)
            {
                Console.WriteLine("{0} ({1})", admin.Title, admin.LoginName);
            }

            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Add administrators to the current site collection:");

            // Prepare a list of admins to add: below sample shows how to do this for Office 365 Multi-Tenant
            // NOTE: This is a sample of the code that must be implemented and WILL NOT EXECUTE since the user

            List<UserEntity> adminsToAdd = new List<UserEntity>();
            adminsToAdd.Add(new UserEntity() { LoginName = SAMPLE_USER_ACCOUNTNAME });

            cc.Web.AddAdministrators(adminsToAdd);
            foreach (var admin in adminsToAdd)
            {
                Console.WriteLine("Add: {0}", admin.LoginName);
            }

            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Remove administrators from the current site collection:");
            UserEntity adminToRemove = new UserEntity() { LoginName = SAMPLE_USER_ACCOUNTNAME };
            cc.Web.RemoveAdministrator(adminToRemove);
            Console.WriteLine("Removed: {0}", adminToRemove.LoginName);

            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("External sharing settings for current site collection:");
            Console.WriteLine(ccTenant.Web.GetSharingCapabilitiesTenant(new Uri(targetSiteUrl)));

            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("External users for current site collection:");
            List<ExternalUserEntity> externalUsers = ccTenant.Web.GetExternalUsersForSiteTenant(new Uri(targetSiteUrl));
            //List<ExternalUserEntity> externalUsers = ccTenant.Web.GetExternalUsersTenant();"
            foreach (var externalUser in externalUsers)
            {
                Console.WriteLine("{0} ({1})", externalUser.DisplayName, externalUser.AcceptedAs);
            }

            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
        }

        /// <summary>
        /// Gets the password input from the console window
        /// </summary>
        /// <returns>the entered password</returns>
        private static string GetPassword()
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

        static void ConsoleWriteColor(string message, ConsoleColor color) {
            var lastForegroundColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ForegroundColor = lastForegroundColor;
        }
    }
}
