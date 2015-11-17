using System;
using System.Security;
using Microsoft.SharePoint.Client;

namespace Core.ListRatingSettings
{
    class Program
    {
        static void Main(string[] args)
        {
            //  Read the Office365 Url from Console
            var siteUrl = GetSite();

            // Prompt for Credentials
            Console.WriteLine("Enter Credentials for {0}", siteUrl);

            string userName = GetUserName();
            SecureString pwd = GetPassword();

            /* End Program if no Credentials */
            if (string.IsNullOrEmpty(userName) || (pwd == null))
            {
                Console.WriteLine("Please provide credentials...");
                return;
            }

            Console.WriteLine();

            // Open connection to Office365 tenant
            var clientContext = new ClientContext(siteUrl)
            {
                AuthenticationMode = ClientAuthenticationMode.Default,
                Credentials = new SharePointOnlineCredentials(userName, pwd)
            };

            /*  provide the clientcontext of target web */
            var ratingEnabler = new RatingsEnabler(clientContext);

            /*  Set 
             *  1. Library name as per locale
             *  2. Experience Ratings/Likes
            */  
            ratingEnabler.Enable("listtoconfigure",VotingExperience.Likes);



        }

        static SecureString GetPassword()
        {
            SecureString sStrPwd = new SecureString();
            try
            {
                Console.Write("Password: ");

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
                Console.Write("Username: ");
                strUserName = Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                strUserName = string.Empty;
            }
            return strUserName;
        }

        static string GetSite()
        {
            string siteUrl = string.Empty;
            try
            {
                Console.Write("Enter your Office365 site collection URL: ");
                siteUrl = Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                siteUrl = string.Empty;
            }
            return siteUrl;
        }
    }
}
