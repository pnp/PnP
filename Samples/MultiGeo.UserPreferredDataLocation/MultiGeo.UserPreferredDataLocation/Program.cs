using System;
using System.Security;

namespace GeoUserPreferredDataLocation
{
    class Program
    {
        static void Main(string[] args)
        {
            string userPrincipalName = "<user to update>@<your tenant>.onmicrosoft.com";

            // Check the readme file to learn how to register an application in azure ad and replace these values
            MultiGeoManager geo = new MultiGeoManager("<application id>", "<application password>", "<Azure AD domain>");

            Console.WriteLine("Using Graph SDK");
            Console.WriteLine("===============");
            // Shows how to get a user, future Graph SDK's will expose a typed model for getting/setting the preferredDataLocation. 
            // See https://github.com/microsoftgraph/msgraph-sdk-dotnet/blob/dev/docs/overview.md to learn more
            var user = geo.GetUser(userPrincipalName, "id,mysite,mail,department");
            Console.WriteLine($"Mail address for {userPrincipalName} is {user.Mail}");
            Console.WriteLine($"My site url for {userPrincipalName} is {user.MySite}");
            Console.WriteLine($"Department url for {userPrincipalName} is {user.Department}");
            // Shows how to update a user property...demo shows Department for now, will be possible with preferredDataLocation in the future.
            user.Department = "Sales";
            geo.UpdateUser(user);


            Console.WriteLine();
            Console.WriteLine("Using Graph REST calls");
            Console.WriteLine("======================");
            // Until there's an updated SDK version available we need to work with REST queries and the graph beta endpoint
            var userPDL = geo.GetPreferredDataLocationForUser(userPrincipalName);
            Console.WriteLine($"Preferred data location for {userPrincipalName} is {userPDL}");

            // Showing REST model to obtain user personal site
            var userMySite = geo.GetPersonalSiteForUser(userPrincipalName);
            Console.WriteLine($"My site url for {userPrincipalName} is {userMySite}");

            // Showing REST model to update a user's preferredDataLocation
            geo.UpdatePreferredDataLocationForUser(userPrincipalName, "EUR");

            // Showing REST model to update a user's department
            geo.UpdateDepartmentForUser(userPrincipalName, "Bike maintenance");

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
