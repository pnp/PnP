using OfficeDevPnP.Core.Framework.TimerJobs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.TimerJobs.Samples.TenantAPIJob
{
    class Program
    {
        #region Program input
        private static string user;
        private static string password;
        private static string domain;
        private static string clientId;
        private static string realm;
        private static string clientSecret;

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

        public static string Domain
        {
            get
            {
                if (String.IsNullOrEmpty(domain))
                {
                    domain = ConfigurationManager.AppSettings["domain"];
                }
                if (String.IsNullOrEmpty(domain))
                {
                    domain = GetInput("Domain", false);
                }
                return domain;
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

        static void Main(string[] args)
        {
            // Instantiate the timer job class
            TenantAPIJob tenantAPIJob = new TenantAPIJob();

            // The provided credentials need access to the site collections you want to use
            tenantAPIJob.UseAppOnlyAuthentication(ClientId, ClientSecret);

            // set enumeration credentials to allow using search API to find the OD4B sites
            //tenantAPIJob.SetEnumerationCredentials(User, Password);

            // In case of SharePoint on-premises use
            //tenantAPIJob.UseNetworkCredentialsAuthentication(User, Password, Domain);

            // Add one or more sites to operate on
            tenantAPIJob.AddSite("https://bertonline.sharepoint.com/sites/dev");

            // Print timer job information and then call Run() on the job
            PrintJobSettingsAndRunJob(tenantAPIJob);
        }

        #region Helper methods
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
        #endregion


    }
}
