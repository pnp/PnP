using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Provisioning.OneDriveProvisioning
{
    class Program
    {
        static void Main(string[] args)
        {
            string siteUrl = GetAdminURL();
            /* Prompt for Credentials */
            Console.WriteLine("Enter Credentials for {0}", siteUrl);
            string userName = GetUserName();
            SecureString pwd = GetPassword();
            string[] emailIds = GetEmailId();

            /* End Program if no Credentials */
            if (string.IsNullOrEmpty(userName) || (pwd == null) || emailIds == null || string.IsNullOrEmpty(siteUrl))
                return;

            SharePointOnlineCredentials _creds = new SharePointOnlineCredentials(userName, pwd);
            CreatePersonalSiteUsingCSOM(_creds, siteUrl, emailIds);
            Console.WriteLine("Working on it. Press any key to continue");
            Console.Read();

        }


        public static SecureString StringToSecure(string nonSecureString)
        {
            SecureString _secureString = new SecureString();
            foreach (char _c in nonSecureString)
                _secureString.AppendChar(_c);
            return _secureString;
        }

        /// <summary>
        /// Sample Member that provisions personal sites leveraging CSOM
        /// You don't want to do provision more than 200 users during a single request. If you have a large amount of users consider
        /// waiting for the last users site to be provisioned. The reason behind this is not to bombard the service with requests.
        /// </summary>
        /// <param name="tenantAdminUrl">The Tenant Admin URL for your SharePoint Online Subscription</param>
        /// <param name="spoCredentials">The Credentials of the user who has tenant admin permission.</param>
        /// <param name="emailIDs">The email ids for users who's personal site you want to create.</param>
        public static void CreatePersonalSiteUsingCSOM(SharePointOnlineCredentials spoCredentials, string tenantAdminUrl, string[] emailIDs)
        {

            using (ClientContext _context = new ClientContext(tenantAdminUrl))
            {
                try
                {
                    _context.AuthenticationMode = ClientAuthenticationMode.Default;
                    _context.Credentials = spoCredentials;

                    ProfileLoader _profileLoader = ProfileLoader.GetProfileLoader(_context);
                    _profileLoader.CreatePersonalSiteEnqueueBulk(emailIDs);
                    _profileLoader.Context.ExecuteQuery();
                }
                catch (Exception _ex)
                {
                    Console.WriteLine(string.Format("Opps, something went wrong and we need to work on it. The error message is {0}", _ex.Message));
                }
            }
        }

        /// <summary>
        /// Helper to Return a Site Collection URL
        /// </summary>
        /// <returns></returns>
        public static string GetAdminURL()
        {
            string siteUrl = string.Empty;
            try
            {
                Console.Write("Supply your Office365 admin site URL: ");
                siteUrl = Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                siteUrl = string.Empty;
            }
            return siteUrl;
        }

        /// <summary>
        /// Helper to return the password
        /// </summary>
        /// <returns>SecureString representing the password</returns>
        public static SecureString GetPassword()
        {
            SecureString sStrPwd = new SecureString();

            try
            {
                Console.Write("SharePoint Tenant Admin Password: ");

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

        /// <summary>
        /// Helper to return the User name.
        /// </summary>
        /// <returns></returns>
        public static string GetUserName()
        {
            string strUserName = string.Empty;
            try
            {
                Console.Write("SharePoint Tenant Admin Username: ");
                strUserName = Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                strUserName = string.Empty;
            }
            return strUserName;
        }

        /// <summary>
        /// Helper method to return the ids.
        /// 
        /// </summary>
        /// <returns></returns>
        public static string[] GetEmailId()
        {
            string[] emailID;
            try
            {
                ///"User3@MicrosoftACS.onmicrosoft.com" , "User4@MicrosoftACS.onmicrosoft.com"
                Console.Write("Supply the users that you want to provision a OneDrive for. You can supply multiple users  using a comma. Example: test1@contoso.onmicrosoft.com,test1@contoso.onmicrosoft.com: ");
                string emailInput = Console.ReadLine();
                if (!string.IsNullOrEmpty(emailInput))
                {
                    emailID = emailInput.Split(new char[] { ',' });
                    return emailID;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

            }

            return null;
        }
    }
}
