using OfficeDevPnP.Framework.TimerJob.Samples.Jobs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Framework.TimerJob.Samples
{
    public class Program
    {
        private static string user;
        private static string password;
        private static string tenant;
        private static string clientId;
        private static string realm;
        private static string clientSecret;

        #region Program input
        public static string Tenant
        {
            get
            {
                if (String.IsNullOrEmpty(tenant))
                {
                    tenant = ConfigurationManager.AppSettings["tenant"];
                }
                if (String.IsNullOrEmpty(tenant))
                {
                    tenant = GetInput("Tenant (short name)", false);
                }
                return tenant;
            }
        }

        public static string User
        {
            get
            {
                if (String.IsNullOrEmpty(user))
                {
                    user = ConfigurationManager.AppSettings["user"];
                }
                if (String.IsNullOrEmpty(user))
                {
                    user = GetInput("User", false);
                }
                return user;
            }
        }

        public static string Password
        {
            get
            {
                if (String.IsNullOrEmpty(password))
                {
                    password = ConfigurationManager.AppSettings["password"];
                }
                if (String.IsNullOrEmpty(password))
                {
                    password = GetInput("Password", true);
                }
                return password;
            }
        }

        public static string Realm
        {
            get
            {
                if (String.IsNullOrEmpty(realm))
                {
                    realm = ConfigurationManager.AppSettings["realm"];
                }
                if (String.IsNullOrEmpty(realm))
                {
                    realm = GetInput("Realm", false);
                }
                return realm;
            }
        }

        public static string ClientId
        {
            get
            {
                if (String.IsNullOrEmpty(clientId))
                {
                    clientId = ConfigurationManager.AppSettings["clientid"];
                }
                if (String.IsNullOrEmpty(clientId))
                {
                    clientId = GetInput("ClientId", false);
                }
                return clientId;
            }
        }

        public static string ClientSecret
        {
            get
            {
                if (String.IsNullOrEmpty(clientSecret))
                {
                    clientSecret = ConfigurationManager.AppSettings["clientsecret"];
                }
                if (String.IsNullOrEmpty(clientSecret))
                {
                    clientSecret = GetInput("ClientSecret", true);
                }
                return clientSecret;
            }
        }
        #endregion

        public static void Main(string[] args)
        {
            
            // Demo1: most simple timer job
            SimpleJob simpleJob = new SimpleJob();
            // The provided credentials need access to the site collections you want to use
            simpleJob.UseOffice365Authentication(Tenant, User, Password);
            simpleJob.AddSite("https://bertonline.sharepoint.com/sites/dev");
            //PrintJobSettingsAndRunJob(simpleJob);
            
            // Demo2: use wildcard site urls and have sub sites expanded
            ExpandJob expandJob = new ExpandJob();
            // The provided credentials need access to the site collections you want to use
            expandJob.UseOffice365Authentication(Tenant, User, Password);
            expandJob.AddSite("https://bertonline.sharepoint.com/sites/d*");
            //PrintJobSettingsAndRunJob(expandJob);
            
            // Demo3: let's use an app-only token
            ExpandJob expandJobAppOnly = new ExpandJob();
            expandJobAppOnly.UseAppOnlyAuthentication(Tenant, Realm, ClientId, ClientSecret);
            // set enumeration credentials to allow using search API to find the OD4B sites
            expandJobAppOnly.SetEnumerationCredentials(User, Password);
            expandJobAppOnly.AddSite("https://bertonline.sharepoint.com/sites/2014*");
            expandJobAppOnly.AddSite("https://bertonline-my.sharepoint.com/personal/*");
            //PrintJobSettingsAndRunJob(expandJobAppOnly);
            
            // Demo4: Let's use the framework state management capabilities to optimize performance 
            SiteGovernanceJob governanceJob = new SiteGovernanceJob();
            governanceJob.UseAppOnlyAuthentication(Tenant, Realm, ClientId, ClientSecret);
            // set enumeration credentials to allow using search API to find the OD4B sites
            governanceJob.SetEnumerationCredentials(User, Password);
            governanceJob.AddSite("https://bertonline.sharepoint.com/sites/dev");
            governanceJob.UseThreading = false;
            //PrintJobSettingsAndRunJob(governanceJob);
            
            // Demo5: Let's override the default site adding and resolving mechanisms and roll our own implementations
            OverrideJob overrideJob = new OverrideJob();
            // The provided credentials need access to the site collections you want to use
            overrideJob.UseOffice365Authentication(Tenant, User, Password);
            overrideJob.AddSite("https://bertonline.sharepoint.com/sites/dev");
            //PrintJobSettingsAndRunJob(overrideJob);
            
            // Demo6: Let's not do multi-threading
            NoThreadingJob noThreadingJob = new NoThreadingJob();
            noThreadingJob.UseOffice365Authentication(Tenant, User, Password);
            noThreadingJob.AddSite("https://bertonline.sharepoint.com/sites/d*");
            //PrintJobSettingsAndRunJob(noThreadingJob);

            // Demo7: subsites processing within same thread, but having multiple threads at site collection level
            SiteCollectionScopedJob siteCollectionScopedJob = new SiteCollectionScopedJob();
            siteCollectionScopedJob.UseOffice365Authentication(Tenant, User, Password);
            siteCollectionScopedJob.AddSite("https://bertonline.sharepoint.com/sites/dev*");
            //PrintJobSettingsAndRunJob(siteCollectionScopedJob);

            // Demo8: Timer jobs can chain other timer jobs in their execution chaining 
            ChainingJob chainingJob = new ChainingJob();
            chainingJob.UseOffice365Authentication(Tenant, User, Password);
            chainingJob.AddSite("https://bertonline.sharepoint.com/sites/dev");
            PrintJobSettingsAndRunJob(chainingJob);


            // on-premises


            // logging
            // docu only

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Press <enter> to continue");
            Console.ReadLine();
        }


        private static void PrintJobSettingsAndRunJob(TimerJob job)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("************************************************");
            Console.WriteLine("Job name: {0}", job.Name);
            Console.WriteLine("Job version: {0}", job.Version);
            Console.WriteLine("Use threading: {0}", job.UseThreading);
            Console.WriteLine("Maximum threads: {0}", job.MaximumThreads);
            Console.WriteLine("Expand sub sites: {0}", job.ExpandSubSites);
            Console.WriteLine("Authentication type: {0}", job.AuthenticationType);
            Console.WriteLine("Manage state: {0}", job.ManageState);
            Console.WriteLine("SharePoint version: {0}", job.SharePointVersion);
            Console.WriteLine("************************************************");
            Console.ForegroundColor = ConsoleColor.Gray;

            //Run job
            job.Run();
        }

        /// <summary>
        /// Gets the password input from the console window
        /// </summary>
        /// <returns>the entered string</returns>
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
