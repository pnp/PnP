using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;

using OfficeDevPnP.Core.Entities;
using Microsoft.SharePoint.Client.Utilities;
using Core.SiteClassification.Common;



namespace Core.SiteActions.ConsoleSample
{
    class Program
    {
        const string PROVIDERHOSTED_URL = "https://spmanaged.azurewebsites.net/pages/index.aspx?SPHostUrl={0}";
        static int Main(string[] args)
        {
            try { 
                ConsoleColor defaultForeground = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Enter the URL of your SharePoint Site");
                Console.ForegroundColor = defaultForeground;
                string _sourceUrl = Console.ReadLine();

                if (_sourceUrl == "")
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Enter the URL of your SharePoint Site");
                    Console.ForegroundColor = defaultForeground;
                    _sourceUrl = Console.ReadLine();

                    if (_sourceUrl == "")
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("No SharePoint site url was provided. Exiting application.");
                        Console.WriteLine("Press any key to continue...");
                        Console.ForegroundColor = defaultForeground;
                        Console.ReadLine();
                        return 1;
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Enter your user name (ex: yourname@mytenant.microsoftonline.com):");
                Console.ForegroundColor = defaultForeground;
                string _sourceUserName = Console.ReadLine();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Enter your password:");
                Console.ForegroundColor = defaultForeground;
                SecureString _sourcePassword = GetPasswordFromConsole();

                using (var _ctx = new ClientContext(_sourceUrl))
                {
                    _ctx.AuthenticationMode = ClientAuthenticationMode.Default;
                    _ctx.ApplicationName = "OFFICEAMS_CUSTOMACTION";
                    _ctx.Credentials = new SharePointOnlineCredentials(_sourceUserName, _sourcePassword);
                
                    AddCustomAction(_ctx, PROVIDERHOSTED_URL);
                }

                Console.WriteLine("I worked on it for you.");
                return 0; 
           }
            catch(Exception _ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Format("I encountered an error when working on it. {0}", _ex.Message));
                return 1;
            }
        }

        static SecureString GetPasswordFromConsole()
        {
            SecureString _secureString = new SecureString();
            
            try
            {
                for (ConsoleKeyInfo keyInfo = Console.ReadKey(true); keyInfo.Key != ConsoleKey.Enter; keyInfo = Console.ReadKey(true))
                {
                    if (keyInfo.Key == ConsoleKey.Backspace)
                    {
                        if (_secureString.Length > 0)
                        {
                            _secureString.RemoveAt(_secureString.Length - 1);
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                            Console.Write(" ");
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        }
                    }
                    else if (keyInfo.Key != ConsoleKey.Enter)
                    {
                        Console.Write("*");
                        _secureString.AppendChar(keyInfo.KeyChar);
                    }

                }
                Console.WriteLine("");
            }
            catch (Exception e)
            {
                _secureString = null;
                Console.WriteLine(e.Message);
            }

            return _secureString;
        }

        /// <summary>
        /// Adds a custom Action to a Site Collection
        /// </summary>
        /// <param name="ctx">The Authenticaed client context.</param>
        /// <param name="hostUrl">The Provider hosted URL for the Application</param>
        static void AddCustomAction(ClientContext ctx, string hostUrl)
        {
            var _web = ctx.Web;
            ctx.Load(_web);
            ctx.ExecuteQuery();

            //we only want the action to show up if you have manage web permissions
            BasePermissions _manageWebPermission = new BasePermissions();
            _manageWebPermission.Set(PermissionKind.ManageWeb);


            CustomActionEntity _entity = new CustomActionEntity()
            {
                Name = "CA_SITE_SETTINGS_SITECLASSIFICATION",
                Group = "SiteTasks",
                Location = "Microsoft.SharePoint.SiteSettings",
                Title = "Site Classification",
                Sequence = 1000,
                Url = string.Format(hostUrl, ctx.Url),
                Rights = _manageWebPermission,
            };

            CustomActionEntity _siteActionSC = new CustomActionEntity()
            {
                Name = "CA_STDMENU_SITECLASSIFICATION",
                Group = "SiteActions",
                Location = "Microsoft.SharePoint.StandardMenu",
                Title = "Site Classification",
                Sequence = 1000,
                Url = string.Format(hostUrl, ctx.Url),
                Rights = _manageWebPermission
            };


            _web.AddCustomAction(_entity);
            _web.AddCustomAction(_siteActionSC);
        }
    }
}
