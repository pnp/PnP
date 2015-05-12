using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Core.SiteProvisioning.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //set up trace listener to the console output
            ColorTraceListener traceListener = new ColorTraceListener();
            Debug.Listeners.Add(traceListener);

            System.Console.WriteLine(" -- OfficeDev PnP Core.SiteProvisioning.Console -- ");

            WriteLineYellow("Source site URL (ex. https://tenant.sharepoint.com/sites/teamsite): ");
            var sourceWebUrl = System.Console.ReadLine();

            WriteLineYellow("Target site URL (ex. https://tenant.sharepoint.com/sites/newteamsite): ");
            var targetWebUrl = System.Console.ReadLine();

            WriteLineYellow("Please enter a login (ex 'adminuser@tenant.onmicrosoft.com'): ");
            var login = System.Console.ReadLine();

            WriteLineYellow("Please enter the password for the account: ");
            var securePassword = ReadPassword('*');

            System.Console.WriteLine();

            ProcessSiteConfiguration(sourceWebUrl, targetWebUrl, login, securePassword);

            System.Console.WriteLine("Applicaiton is ready.");
            System.Console.ReadKey();
        }

        private static void ProcessSiteConfiguration(string sourceWebUrl, string targetWebUrl, string login, SecureString securePassword)
        {
            ProvisioningTemplate sourceTemplate;
            
            using (ClientContext clientContext = new ClientContext(sourceWebUrl))
            {
                clientContext.Credentials = new SharePointOnlineCredentials(login, securePassword);

                sourceTemplate = clientContext.Web.GetProvisioningTemplate();

                clientContext.ExecuteQuery();

                clientContext.Web.Context.Load(clientContext.Web.Features, fs => fs.Include(f => f.DefinitionId, f => f.DisplayName));
                clientContext.Web.Context.ExecuteQueryRetry();
                var activeFeatures = clientContext.Web.Features.ToList();
                clientContext.ExecuteQuery();
            }

            System.Console.WriteLine();
            WriteLineYellow("Building source template complete. Proceeding with target configuraiton.");

            using (ClientContext clientContext = new ClientContext(targetWebUrl))
            {
                clientContext.Credentials = new SharePointOnlineCredentials(login, securePassword);

                clientContext.Web.ApplyProvisioningTemplate(sourceTemplate);

                clientContext.ExecuteQuery();
            }

            System.Console.WriteLine();
            WriteLineYellow("Target site ready.");

        }

        private static void WriteLineYellow(string message)
        {
            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine(message);
            System.Console.ForegroundColor = ConsoleColor.White;
        }

        private static SecureString ReadPassword(char mask)
        {
            const int ENTER = 13, BACKSP = 8, CTRLBACKSP = 127;
            int[] FILTERED = { 0, 27, 9, 10 /*, 32 space, if you care */ }; // const

            var pass = new Stack<char>();
            char chr = (char)0;

            while ((chr = System.Console.ReadKey(true).KeyChar) != ENTER)
            {
                if (chr == BACKSP)
                {
                    if (pass.Count > 0)
                    {
                        System.Console.Write("\b \b");
                        pass.Pop();
                    }
                }
                else if (chr == CTRLBACKSP)
                {
                    while (pass.Count > 0)
                    {
                        System.Console.Write("\b \b");
                        pass.Pop();
                    }
                }
                else if (FILTERED.Count(x => chr == x) > 0) { }
                else
                {
                    pass.Push((char)chr);
                    System.Console.Write(mask);
                }
            }

            System.Console.WriteLine();

            SecureString securePassword = new SecureString();
            foreach (char c in pass.Reverse().ToArray())
            {
                securePassword.AppendChar(c);
            }

            return securePassword;
        }

    }
}