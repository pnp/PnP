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
        static void Main(string[] args)
        {
            bool interactiveLogin = true;
            string templateSiteUrl = "https://bertonline.sharepoint.com/sites/130049";
            string targetSiteUrl = "https://bertonline.sharepoint.com/sites/pr1";
            string loginId = "bert.jansen@bertonline.onmicrosoft.com";

            // Get pwd from environment variable, so that we do to need to show that.
            string pwd = "";
            if (interactiveLogin)
            {
                pwd = GetInput("Password", true);
            }
            else
            {
                pwd = System.Environment.GetEnvironmentVariable("MSOPWD", EnvironmentVariableTarget.User);
            }

            if (string.IsNullOrEmpty(pwd))
            {
                System.Console.WriteLine("MSOPWD user environment variable empty or no password was specified, cannot continue. Press any key to abort.");
                System.Console.ReadKey();
                return;
            }

            // Template 
            ProvisioningTemplate template;

            // Get access to source site
            using (var ctx = new ClientContext(templateSiteUrl))
            {
                //Provide count and pwd for connecting to the source
                var passWord = new SecureString();
                foreach (char c in pwd.ToCharArray()) passWord.AppendChar(c);
                ctx.Credentials = new SharePointOnlineCredentials(loginId, passWord);

                // Get template from existing site
                template = ctx.Web.GetProvisioningTemplate();
            }

            // Save template using XML provider
            XMLFileSystemTemplateProvider provider = new XMLFileSystemTemplateProvider(@"c:\temp\pnpprovisioningdemo", "");
            string templateName = "bert.xml";
            provider.SaveAs(template, templateName);
            
            // Load the saved model again
            ProvisioningTemplate p2 = provider.GetTemplate(templateName);

            // Get the available, valid templates
            var templates = provider.GetTemplates();
            foreach(var template1 in templates)
            {
                Console.WriteLine("Found template with ID {0}", template1.ID);
            }

            // Get access to target site and apply template
            using (var ctx = new ClientContext(targetSiteUrl))
            {
                //Provide count and pwd for connecting to the source
                var passWord = new SecureString();
                foreach (char c in pwd.ToCharArray()) passWord.AppendChar(c);
                ctx.Credentials = new SharePointOnlineCredentials(loginId, passWord);

                // Apply template to existing site
                ctx.Web.ApplyProvisioningTemplate(template);
            }
        }

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

