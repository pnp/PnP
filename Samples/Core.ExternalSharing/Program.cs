using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.Online.SharePoint.TenantManagement;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Core.ExternalSharing
{
    class Program
    {
        static void Main(string[] args)
        {
           
            /* Prompt for you Admin Tenant*/
            Console.WriteLine("Enter your Office 365 admin center URL:");
            string tenantAdminURL = GetSite();

            /* End Program if no tenantAdmin */
            if (string.IsNullOrEmpty(tenantAdminURL))
            {
                Console.WriteLine("Hmm, i tried to work on it but you didn't supply your Office 365 admin center URL:");
                return;
            }
               
            // Request Office365 site from the user
            Console.WriteLine("Enter your Office 365 site collection URL that you want to share:");
            string siteUrl = GetSite();

            /* Prompt for Credentials */
            Console.WriteLine("Enter credentials for your Office 365 site collection {0}:", siteUrl);

            string userName = GetUserName();
            SecureString pwd = GetPassword();

            /* End Program if no Credentials */
            if (string.IsNullOrEmpty(userName) || (pwd == null))
            {
                Console.WriteLine("Hmm, I tried to work on it but you didn't supply your credentials:");
                return;
            }

            try 
            {
                SharingCapabilities _sharingSettingToApply = GetInputSharing(siteUrl);
                using (ClientContext cc = new ClientContext(tenantAdminURL))
                { 
                    cc.AuthenticationMode = ClientAuthenticationMode.Default;
                    cc.Credentials = new SharePointOnlineCredentials(userName, pwd);
                    SetSiteSharing(cc, siteUrl, _sharingSettingToApply);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Oops, mistakes can happen to anyone. An error occured: {0}", ex.Message);
               
            }

            Console.WriteLine("Press Enter to exit.");
            Console.Read();

        
        }
        /// <summary>
        /// Helper to Return user input for the sharing capability for a site
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static SharingCapabilities GetInputSharing(string url)
        {
            Console.WriteLine("*********************************************************************");
            Console.WriteLine("Please select the Sharing Capabilities for the site {0}", url);
            Console.WriteLine("1: for {0}", SharingCapabilities.Disabled);
            Console.WriteLine("2: for {0}", SharingCapabilities.ExternalUserAndGuestSharing);
            Console.WriteLine("3: for {0}", SharingCapabilities.ExternalUserSharingOnly);


            string userInputSharing = Console.ReadLine();
            switch (userInputSharing)
            {
                case "1":
                    return SharingCapabilities.Disabled;                 
                case "2":
                    return SharingCapabilities.ExternalUserAndGuestSharing;                 
                case "3":
                    return SharingCapabilities.ExternalUserSharingOnly;               
                default:
                    throw new Exception("Hmm, I did not understand your selection.");
            }
            
        }
        /// <summary>
        /// Sets the Site Collection External Sharing Setting using the SharePoint Tenant API
        /// </summary>
        /// <param name="adminCC"></param>
        /// <param name="siteCollectionURl"></param>
        /// <param name="shareSettings"></param>
        public static void SetSiteSharing(ClientContext adminCC, string siteCollectionURl, SharingCapabilities shareSettings)
        {
            var _tenantAdmin = new Tenant(adminCC);
            SiteProperties _siteprops = _tenantAdmin.GetSitePropertiesByUrl(siteCollectionURl, true);
            adminCC.Load(_tenantAdmin);
            adminCC.Load(_siteprops);
            adminCC.ExecuteQuery();

            SharingCapabilities _tenantSharing = _tenantAdmin.SharingCapability;
            var _currentShareSettings = _siteprops.SharingCapability;
            bool _isUpdatable = false;

            if(_tenantSharing == SharingCapabilities.Disabled)
            {
                Console.WriteLine("Sharing is currently disabled in your Office 365 subscription. I am unable to work on it.");
            }
            else
            {  
                if(shareSettings == SharingCapabilities.Disabled)
                { _isUpdatable = true; }
                else if(shareSettings == SharingCapabilities.ExternalUserSharingOnly)
                {
                    _isUpdatable = true;   
                }
                else if (shareSettings == SharingCapabilities.ExternalUserAndGuestSharing)
                {
                    if (_tenantSharing == SharingCapabilities.ExternalUserAndGuestSharing)
                    {
                        _isUpdatable = true;
                    }
                    else
                    {
                        Console.WriteLine("ExternalUserAndGuestSharing is currently disabled in your Office 365 subscription. I am unable to work on it.");
                    }
                }
            }
            if (_currentShareSettings != shareSettings && _isUpdatable)
            {
                _siteprops.SharingCapability = shareSettings;
                _siteprops.Update();
                adminCC.ExecuteQuery();
                Console.WriteLine("Set Sharing on site {0} to {1}.", siteCollectionURl, shareSettings);
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
    }
}
