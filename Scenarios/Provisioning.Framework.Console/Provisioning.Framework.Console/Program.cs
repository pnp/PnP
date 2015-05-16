using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.Providers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Framework
{
    class Program
    {

        static string m_SourceWebUrl = string.Empty;
        static string m_TargetWebUrl = string.Empty;
        static string m_Login = string.Empty;
        static SecureString m_SecurePassword;

        static void Main(string[] args)
        {

            //set up trace listener to the console output
            ColorTraceListener traceListener = new ColorTraceListener();
            System.Diagnostics.Debug.Listeners.Add(traceListener);

            System.Console.WriteLine(" -- OfficeDev PnP Provisioning.Framework.Console Demo -- ");

            //gather URL's and password for access
            GetInfoFromUser();

            // Template 
            ProvisioningTemplate sourceTemplate;
            sourceTemplate = GetWebTemplateFromSite();

            System.Console.WriteLine();
            WriteLineYellow("Building source template complete. Proceeding with saving to the current folder.");

            // Save template using XML provider
            string templateFileName = SaveTemplateToFileSystem(sourceTemplate);
            
            // Load the saved model again
            ProvisioningTemplate targetTemplate = LoadTemplateFromFileSystem(templateFileName);

            ApplyWebTemplateToSite(targetTemplate);

            System.Console.WriteLine();
            WriteLineYellow("Application finished.");
            Console.ReadKey();
        }

        private static string SaveTemplateToFileSystem(ProvisioningTemplate sourceTemplate)
        {
            XMLFileSystemTemplateProvider provider = new XMLFileSystemTemplateProvider(Environment.CurrentDirectory, string.Empty);
            string templateName = "SiteTemplate_" + DateTime.Now.ToString("yyyyddMHHmmss") +".xml";
            provider.SaveAs(sourceTemplate, templateName);

            System.Console.WriteLine();
            WriteLineYellow("File saved: " + templateName);

            // console log
            return templateName;
        }

        private static ProvisioningTemplate LoadTemplateFromFileSystem(string filename)
        {
            XMLFileSystemTemplateProvider provider = new XMLFileSystemTemplateProvider
                (Environment.CurrentDirectory, string.Empty);

            System.Console.WriteLine();
            WriteLineYellow("Will now load the template from: " + filename);

            // Get the available, valid templates
            var templates = provider.GetTemplates();
            foreach (var template in templates)
            {
                WriteLineYellow("Found template with ID: " + template.Id);
            }

            ProvisioningTemplate loadedTemplate = provider.GetTemplate(filename);
            WriteLineYellow("Template loaded:" + filename);

            // console log
            return loadedTemplate;
        }

        private static ProvisioningTemplate GetWebTemplateFromSite()
        {
            ProvisioningTemplate template;
            // Establish context and get access to source site
            using (ClientContext clientContext = new ClientContext(m_SourceWebUrl))
            {
                clientContext.Credentials = new SharePointOnlineCredentials(m_Login, m_SecurePassword);

                // Get template from existing site
                template = clientContext.Web.GetProvisioningTemplate();

                clientContext.ExecuteQuery();
            }

            return template;
        }

        private static void ApplyWebTemplateToSite(ProvisioningTemplate sourceTemplate)
        {
            System.Console.WriteLine();
            WriteLineYellow("Applying template to site: " + m_TargetWebUrl);

            using (ClientContext clientContext = new ClientContext(m_TargetWebUrl))
            {
                clientContext.Credentials = new SharePointOnlineCredentials(m_Login, m_SecurePassword);

                // Get template from existing site
                clientContext.Web.ApplyProvisioningTemplate(sourceTemplate);

                clientContext.ExecuteQuery();
            }

            System.Console.WriteLine();
            WriteLineYellow("Target site ready.");
        }

        private static void GetInfoFromUser()
        {
            WriteLineYellow("Source site URL (ex. https://tenant.sharepoint.com/sites/teamsite): ");
            m_SourceWebUrl = System.Console.ReadLine();

            WriteLineYellow("Target site URL (ex. https://tenant.sharepoint.com/sites/newteamsite): ");
            m_TargetWebUrl = System.Console.ReadLine();

            WriteLineYellow("Please enter a login (ex 'adminuser@tenant.onmicrosoft.com'): ");
            m_Login = System.Console.ReadLine();

            WriteLineYellow("Please enter the password for the account: ");
            m_SecurePassword = ReadPassword('*');

            System.Console.WriteLine();
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

        [Obsolete("Not used anymore")]
        private static string GetInput(string label, bool isPassword)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("{0} : ", label);
            Console.ForegroundColor = ConsoleColor.Gray;

            string strPwd = "";

            for (ConsoleKeyInfo keyInfo = Console.ReadKey(true); keyInfo.Key != ConsoleKey.Enter; keyInfo = Console.ReadKey(true))
            {
                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (strPwd.Length > 0)
                    {
                        strPwd = strPwd.Remove(strPwd.Length - 1);
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
                    strPwd += keyInfo.KeyChar;

                }

            }
            Console.WriteLine("");

            return strPwd;
        }
    }
}

