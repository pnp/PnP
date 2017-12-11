using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.UserProfiles;
using System;
using System.Security;

namespace GeoUserDiscovery
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
                    string userPrincipalName = "<user to update>@<your tenant>.onmicrosoft.com";

                    string personalSiteHostUrl = multiGeoManager.GetUserPersonalSiteHostUrlREST(userPrincipalName);
                    //string personalSiteHostUrl = multiGeoManager.GetUserPersonalSiteHostUrlCSOM(userPrincipalName);

                    string tenantAdminSite = null;
                    if (!string.IsNullOrEmpty(personalSiteHostUrl))
                    {
                        // If the user has a personal site then the user's profile lives in the same geo as the user's personal site
                        tenantAdminSite = multiGeoManager.GetTenantAdminSiteForSite(personalSiteHostUrl);
                    }
                    else
                    {
                        throw new Exception($"Account {userPrincipalName} does not have an associated personal site host url, which should't happen");
                    }

                    // Get personal site for the given user
                    string personalSite = multiGeoManager.GetUserPersonalSiteLocation(userPrincipalName);
                    Console.WriteLine($"Personal site Url for user {userPrincipalName} is {personalSite}");

                    // Create client context for the tenant admin site - it's important that the CSOM user profile api's are targetting the geo of the user's profile
                    using (var tenantAdminContext = new ClientContext(tenantAdminSite))
                    {
                        tenantAdminContext.Credentials = ctx.Credentials;

                        // Read user profile properties

                        // Preferred option is to use the Graph API for the OOB properties 
                        // See:  - https://developer.microsoft.com/en-us/graph/docs/api-reference/v1.0/api/user_update (properties settable via graph)
                        // e.g. let's use MS Graph to get the user's department
                        string department = multiGeoManager.GetDepartmentForUser(userPrincipalName);
                        Console.WriteLine($"Department for user {userPrincipalName} is {department}");
                        // Now update the department
                        multiGeoManager.UpdateDepartmentForUser(userPrincipalName, "Bike cleaning");
                        department = multiGeoManager.GetDepartmentForUser(userPrincipalName);
                        Console.WriteLine($"Updated department for user {userPrincipalName} is {department}");

                        // For SPO custom created properties use below approach
                        string userAccountName = $"i:0#.f|membership|{userPrincipalName}";
                        PeopleManager peopleManager = new PeopleManager(tenantAdminContext);
                        var propsToRetrieve = new string[] { "CostCenter", "CustomProperty" };
                        var props = peopleManager.GetUserProfilePropertiesFor(new UserProfilePropertiesForUser(tenantAdminContext, userAccountName, propsToRetrieve));
                        tenantAdminContext.ExecuteQuery();

                        int i = 0;
                        foreach (var prop in props)
                        {
                            Console.WriteLine($"Prop: {propsToRetrieve[i]} Value: {prop}");
                            i++;
                        }

                        // Update user profile properties
                        peopleManager.SetSingleValueProfileProperty(userAccountName, "CustomProperty", "Updated by Multi-Geo geo sample");
                        tenantAdminContext.ExecuteQuery();
                    }
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
