using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.OneDrive.Reset
{
    class Program
    {

        private const string OneDriveMarkerBagID = "Contoso_OneDriveVersion";
        private const string OneDriveCustomJS = "OneDriveCustomJS";


        static void Main(string[] args)
        {
            string siteUrl = string.Empty;
            string userName = string.Empty;
            SecureString pwd = new SecureString(); 

            if (args.Length > 0)
            {
                siteUrl = args[0];
                userName = args[1];
                foreach (var c in args[2].ToCharArray()) pwd.AppendChar(c);
            }
            else
            {
                // Request Office365 site from the user
                siteUrl = GetSite();

                /* Prompt for Credentials */
                Console.WriteLine("Enter Credentials for {0}", siteUrl);

                userName = GetUserName();
                pwd = GetPassword();
            }


            /* End Program if no Credentials */
            if (string.IsNullOrEmpty(userName) || (pwd == null))
                return;

            ClientContext cc = new ClientContext(siteUrl);
            //For SharePoint Online
            cc.Credentials = new SharePointOnlineCredentials(userName, pwd);

            //For SharePoint Online Dedicated or On-Prem 
            //string domain = GetDomainName();
            //cc.Credentials = new NetworkCredential(userName, pwd, domain);


            try
            {
                Console.WriteLine("Reset the OneDrive customizations.");

                Console.WriteLine("Set propertybag entry {0} equal to {1}.", OneDriveMarkerBagID, 0);
                cc.Web.SetPropertyBagValue(OneDriveMarkerBagID, 0);

                Console.WriteLine("Set the theme back to the default Office theme.");
                cc.Web.SetComposedLookByUrl("Office");

                Console.WriteLine("Remove the {0} JavaScript injection.", OneDriveCustomJS);
                cc.Web.DeleteJsLink(OneDriveCustomJS);

                Console.WriteLine("Press any key to continue.");
                Console.Read();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Format("Exception!"), ex.ToString());
                Console.WriteLine("Press any key to continue.");
                Console.Read();
                throw;
            }

        }


        /// <summary>
        /// Helper to Return a Site Collection URL
        /// </summary>
        /// <returns></returns>
        public static string GetSite()
        {
            string siteUrl = string.Empty;
            try
            {
                Console.Write("Give SharePoint site URL: ");
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
                Console.Write("SharePoint Password: ");

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
                Console.Write("SharePoint Username: ");
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
        /// Helper to return the Domain name.
        /// </summary>
        /// <returns></returns>
        public static string GetDomainName()
        {
            string strDomainName = string.Empty;
            try
            {
                Console.Write("Domain name: ");
                strDomainName = Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                strDomainName = string.Empty;
            }
            return strDomainName;
        }
    }
}
