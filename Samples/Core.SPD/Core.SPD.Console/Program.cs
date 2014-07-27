using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Core.SPD
{
    class Program
    {
        /// <summary>
        /// Main entry-point for the application
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Request SharePoint site from the user
            string siteUrl = GetSite();

            /* Prompt for Credentials */
            Console.WriteLine("Enter Credentials for {0}", siteUrl);

            string userName = GetUserName();
            SecureString pwd = GetPassword();

            /* End Program if no Credentials */
            if (string.IsNullOrEmpty(userName) || (pwd == null))
                return;

            ClientContext cc = new ClientContext(siteUrl);
            cc.AuthenticationMode = ClientAuthenticationMode.Default;
            //For SharePoint Online
            cc.Credentials = new SharePointOnlineCredentials(userName, pwd);
            //For SharePoint Online Dedicated or On-Prem 
            //cc.Credentials = new NetworkCredential(userName, pwd);


            try
            {
                // Let's ensure that the connectivity works.
                Web web = cc.Web;
                cc.Load(web);
                cc.ExecuteQuery();

                // Call disable designer
                DisableDesigner(cc);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" - I have disabled some designer settings for you.");


                // Call enhable designer
                EnableDesigner(cc);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" - I have enable some designer settings for you.");

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
        /// This method will disable SharePoint designer you must be a site collection administrator to perform this action
        /// A UnauthorizedAccessException is thrown when attempting to set the property if either the user is not a Site Collection administrator or the setting is disabled at the 
        /// web application level.
        /// Site Collection Administrators will always be able to edit sites. 
        /// </summary>
        /// <param name="ctx"></param>
        public static void DisableDesigner(ClientContext ctx)
        {
            try
            {
                Site _site = ctx.Site;
                ctx.Load(_site);
                //Allow Site Owners and Designers to use SharePoint Designer in this Site Collection 
                _site.AllowDesigner = false;
                //Allow Site Owners and Designers to Customize Master Pages and Page Layouts 
                _site.AllowMasterPageEditing = false;
                //Allow Site Owners and Designers to Detach Pages from the Site Definition 
                _site.AllowRevertFromTemplate = false;
                //Allow Site Owners and Designers to See the Hidden URL structure of their Web Site 
                _site.ShowUrlStructure = false;
                ctx.ExecuteQuery();
            }
            catch 
            {
                throw;
            }
        }
        /// <summary>
        /// This method will Enable SharePoint designer you must be a site collection administrator to perform this action
        /// A UnauthorizedAccessException is thrown when attempting to set the property if either the user is not a Site Collection administrator or the setting is disabled at the 
        /// web application level.
        /// Site Collection Administrators will always be able to edit sites. 
        /// </summary>
        /// <param name="ctx"></param>
        public static void EnableDesigner(ClientContext ctx)
        {
            try
            {
                Site _site = ctx.Site;
                ctx.Load(_site);
                _site.AllowDesigner = true;
                _site.AllowMasterPageEditing = true;
                _site.AllowRevertFromTemplate = true;
                _site.ShowUrlStructure = true;
                ctx.ExecuteQuery();
            }
            catch
            {
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
                Console.Write("Give Office365 site URL: ");
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
