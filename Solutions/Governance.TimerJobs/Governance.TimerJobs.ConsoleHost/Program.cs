using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Governance.TimerJobs.Data;
using Governance.TimerJobs.Policy;

namespace Governance.TimerJobs.ConsoleHost
{
    class Program
    {
        #region Program input
        private static string tenantUrl;
        private static string user;
        private static string password;
        private static string clientId;
        private static string clientSecret;

        public static string TenantUrl
        {
            get
            {
                if (String.IsNullOrEmpty(tenantUrl))
                {
                    tenantUrl = ConfigurationManager.AppSettings["tenanturl"];
                }
                if (String.IsNullOrEmpty(tenantUrl))
                {
                    tenantUrl = GetInput("TenantUrl", false);
                }
                return tenantUrl;
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
            // Disbale useTheading when debuging
            bool useThreading = 
#if DEBUG
                false;
#else
                true;
#endif
            // Init site information DB repository
            string connectionString = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
            var dbRepository = new GovernanceDbRepository(connectionString);

            // Step thru each SharePoint site collection,
            // synchronize the latest status to site information DB
            var syncJob = new SynchronizationJob(dbRepository, TenantUrl);
            syncJob.UseThreading = useThreading;
            syncJob.SetEnumerationCredentials(User, Password);
            syncJob.UseAppOnlyAuthentication(ClientId, ClientSecret);
            syncJob.Run();

            // Step thru each site information DB record,
            // delete all out-dated ones, of which the related SharePoint site collection has been deleted
            var cleanupJob = new CleanUpJob(dbRepository, TenantUrl);
            cleanupJob.UseThreading = useThreading;
            cleanupJob.SetEnumerationCredentials(User, Password);
            cleanupJob.UseAppOnlyAuthentication(ClientId, ClientSecret);
            cleanupJob.Run();

            // Detect broadly accessible HBI webs and log large security group information in DB repository
            var hbiBroadAccessJob = new GovernancePreprocessJob(dbRepository, TenantUrl, new HbiBroadAccessPolicy());
            hbiBroadAccessJob.UseThreading = useThreading;
            hbiBroadAccessJob.SetEnumerationCredentials(User, Password);
            hbiBroadAccessJob.UseAppOnlyAuthentication(ClientId, ClientSecret);
            hbiBroadAccessJob.Run();

            // Query all incomlpiant site information records based on the governance policies checking logic (defined in ISitePolicy.NoncompliancePredictor),
            // then run the enforcement logic for each site.
            var governanceJob = new GovernanceJob(dbRepository, TenantUrl);
            governanceJob.UseThreading = useThreading;
            governanceJob.SetEnumerationCredentials(User, Password);
            governanceJob.UseAppOnlyAuthentication(ClientId, ClientSecret);
            governanceJob.Run();
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
