using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Modern.Console.RESTAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get context and user information used for operations
            string siteUrl = GetInput("URL to site to connect to (any site collection in your tenant)", false);
            string userId = GetInput("User Id", false);
            string pwd = GetInput("Password", true);

            // Get group details from console user
            string groupDisplayName = GetInput("Modern Site / Group Display Name", false);
            string groupAlias = GetInput("Modern Site / Group Alias (url)", false);
            string description = GetInput("Description for the modern site / group", false);
            string additionalOwners = GetInput("Comma seperated list of optional additional owners (empty = none)", false);

            // Just to indicate that we start the process
            System.Console.WriteLine("-- -- --");
            System.Console.WriteLine("Working on it...");
            System.Console.WriteLine("-- -- --");

            string newSiteUrl = string.Empty;

            // Let's get moving on the operations
            using (var ctx = new ClientContext(siteUrl))
            {
                //Provide count and pwd for connecting to the source
                var passWord = new SecureString();
                foreach (char c in pwd.ToCharArray()) passWord.AppendChar(c);
                ctx.Credentials = new SharePointOnlineCredentials(userId, passWord);

                // Get valid Site URL from Alias
                if (new ModernSiteCreator(ctx).CanAliasBeUsed(groupAlias))
                {
                    System.Console.WriteLine("- Alias is good - we can provision site with given values");
                    // If additional owners were provided, let's use them
                    List<string> owners = null;
                    if (additionalOwners.Trim().Length > 0)
                    {
                        owners = additionalOwners.Split(',').ToList();
                    }

                    // Create new modern site
                    newSiteUrl = new ModernSiteCreator(ctx).CreateGroup(groupDisplayName, groupAlias, true, description, owners);

                    // Output URL of created site
                    System.Console.WriteLine(string.Format("New modern site created at URL: {0}", newSiteUrl));
                }
                else
                {
                    // We can't use given alias
                    System.Console.WriteLine(string.Format("Alias '{0}' cannot be used - it's either taken in SP, AAD or in Exchange.", groupAlias));
                }
            }

            // Pause to see the end result
            System.Console.ReadLine();
        }

        /// <summary>
        /// Generic helper for getting input in the console
        /// </summary>
        /// <param name="label">String to show for user</param>
        /// <param name="isPassword">Hide written value or not</param>
        /// <param name="defaultForeground">Can be used to change color</param>
        /// <returns></returns>
         private static string GetInput(string label, bool isPassword, ConsoleColor defaultForeground = ConsoleColor.White)
        {
            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("{0} : ", label);
            System.Console.ForegroundColor = defaultForeground;

            string value = "";

            for (ConsoleKeyInfo keyInfo = System.Console.ReadKey(true); keyInfo.Key != ConsoleKey.Enter; keyInfo = System.Console.ReadKey(true))
            {
                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (value.Length > 0)
                    {
                        value = value.Remove(value.Length - 1);
                        System.Console.SetCursorPosition(System.Console.CursorLeft - 1, System.Console.CursorTop);
                        System.Console.Write(" ");
                        System.Console.SetCursorPosition(System.Console.CursorLeft - 1, System.Console.CursorTop);
                    }
                }
                else if (keyInfo.Key != ConsoleKey.Enter)
                {
                    if (isPassword)
                    {
                        System.Console.Write("*");
                    }
                    else
                    {
                        System.Console.Write(keyInfo.KeyChar);
                    }
                    value += keyInfo.KeyChar;

                }

            }
            System.Console.WriteLine("");

            return value;
        }
    }
}
