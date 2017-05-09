using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security;
using System.Threading;

namespace Provisioning.Framework.Extensions
{
    class Program
    {
        /// <summary>
        /// Main routine
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            ConsoleColor defaultForeground = Console.ForegroundColor;

            // Collect information 
            string targetWebUrl = GetInput("Enter the URL of the target site: ", false, defaultForeground);
            string userName = GetInput("Enter your user name:", false, defaultForeground);
            string pwdS = GetInput("Enter your password:", true, defaultForeground);
            SecureString pwd = new SecureString();
            foreach (char c in pwdS.ToCharArray()) pwd.AppendChar(c);

            // Read sitetemplate.xml from current directory
            var provider = new XMLFileSystemTemplateProvider(".\\", "");
            ProvisioningTemplate template = provider.GetTemplate("sitetemplate.xml", XMLPnPSchemaFormatter.LatestFormatter);

            // Apply template to site
            ApplyProvisioningTemplate(defaultForeground, targetWebUrl, userName, pwd, template);

            // Pause
            Console.ReadLine();
        }

        private static void ApplyProvisioningTemplate(ConsoleColor defaultForeground, string webUrl, string userName, SecureString pwd, ProvisioningTemplate template)
        {
            using (var ctx = new ClientContext(webUrl))
            {
                ctx.Credentials = new SharePointOnlineCredentials(userName, pwd);
                ctx.RequestTimeout = Timeout.Infinite;

                Web web = ctx.Web;
                ctx.Load(web, w => w.Title);
                ctx.ExecuteQueryRetry();

                Console.WriteLine("Applying site template to: {0}", web.Title);
                web.ApplyProvisioningTemplate(template);
                Console.WriteLine("Completed successfully");
            }
        }

        private static string GetInput(string label, bool isPassword, ConsoleColor defaultForeground)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0} : ", label);
            Console.ForegroundColor = defaultForeground;

            string value = "";

            for (ConsoleKeyInfo keyInfo = Console.ReadKey(true); keyInfo.Key != ConsoleKey.Enter; keyInfo = Console.ReadKey(true))
            {
                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (value.Length > 0)
                    {
                        value = value.Remove(value.Length - 1);
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        Console.Write(" ");
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    }
                }
                else if (keyInfo.Key != ConsoleKey.Enter)
                {
                    if (isPassword)
                    {
                        Console.Write("*");
                    }
                    else
                    {
                        Console.Write(keyInfo.KeyChar);
                    }
                    value += keyInfo.KeyChar;

                }

            }
            Console.WriteLine("");

            return value;
        }
    }
}

