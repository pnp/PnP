using System;
using System.IO;
using System.Text;
using System.Security;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Search.Portability;
using Microsoft.SharePoint.Client.Search.Administration;

namespace Core.Search.SearchSettingsConsole
{
    class Program 
    {
        static int Main(string[] args)
        {
            ConsoleColor defaultForeground = Console.ForegroundColor;
                        
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Select the type of operation you would like to perform:");
            Console.ForegroundColor = defaultForeground;
            Console.WriteLine(" 1 (type 1 for Import and hit enter)");
            Console.WriteLine(" 2 (type 2 for Export and hit enter)");
            string opsTypeSelection = Console.ReadLine();

            if (opsTypeSelection == null || opsTypeSelection != "1" && opsTypeSelection != "2")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Try again. Select the type of operation you would like to perform:");
                Console.ForegroundColor = defaultForeground;
                Console.WriteLine(" 1 (type 1 for Import and hit enter)");
                Console.WriteLine(" 2 (type 2 for Export and hit enter)");
                opsTypeSelection = Console.ReadLine();

                if (opsTypeSelection == null || opsTypeSelection != "1" && opsTypeSelection != "2")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("The correct options were not specified. Exiting application.");
                    Console.WriteLine("Press any key to continue...");
                    Console.ForegroundColor = defaultForeground;
                    Console.ReadLine();
                    return 1;
                }                
            }
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Enter the path and name of the import/export file:");
            Console.ForegroundColor = defaultForeground;
            string settingsFile = Console.ReadLine();

            if (settingsFile == "")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Please try entering the path and name of the import/export file again:");
                Console.ForegroundColor = defaultForeground;
                settingsFile = Console.ReadLine();

                if (settingsFile == "")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("No file name and path were specified. Exiting application.");
                    Console.WriteLine("Press any key to continue...");
                    Console.ForegroundColor = defaultForeground;
                    Console.ReadLine();
                    return 1;
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Enter the URL of the SharePoint Online site:");
            Console.ForegroundColor = defaultForeground;
            string webUrl = Console.ReadLine();

            if (webUrl == "")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Try again. Enter the URL of the SharePoint Online site:");
                Console.ForegroundColor = defaultForeground;
                webUrl = Console.ReadLine();

                if (webUrl == "")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("No SharePoint Online site url was provided. Exiting application.");
                    Console.WriteLine("Press any key to continue...");
                    Console.ForegroundColor = defaultForeground;
                    Console.ReadLine();
                    return 1;
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Enter your user name (ex: yourname@mytenant.microsoftonline.com):");
            Console.ForegroundColor = defaultForeground;
            string userName = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Enter your password:");
            Console.ForegroundColor = defaultForeground;
            SecureString password = GetPasswordFromConsoleInput();
                        
            using (var context = new ClientContext(webUrl))
            {
                try
                {
                    context.Credentials = new SharePointOnlineCredentials(userName, password);
                    context.Load(context.Web, w => w.Title);
                    context.ExecuteQuery();
                }
                catch (IdcrlException idex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("The password you supplied is incorrect. Exiting application. Please try again.");
                    Console.ForegroundColor = defaultForeground;
                    Console.ReadLine();
                    return 1;
                }
                catch(System.Net.WebException e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("There was a problem encountered with the credentials you supplied. Exiting application. Please try again.");
                    Console.ForegroundColor = defaultForeground;
                    Console.ReadLine();
                    return 1;                    
                }
                catch (ArgumentNullException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("You did not specify your username or password. Exiting application. Please try again.");
                    Console.ForegroundColor = defaultForeground;
                    Console.ReadLine();
                    return 1;
                }

                if (opsTypeSelection == "1")
                {
                    ImportSearchSettings(context, settingsFile);
                }
                else if (opsTypeSelection == "2")
                {
                    ExportSearchSettings(context, settingsFile);
                }
            }

            return 0;
            
        }

        private static void ExportSearchSettings(ClientContext context, string settingsFile)
        {
            ConsoleColor defaultForeground = Console.ForegroundColor;
            /*
             * SearchConfigurationPortability Class
             * http://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.search.portability.searchconfigurationportability(v=office.15).aspx
             * 
             * SearchObjectOwner Class
             * http://msdn.microsoft.com/en-us/library/office/microsoft.office.server.search.administration.searchobjectowner(v=office.15).aspx
            */
            SearchConfigurationPortability sconfig = new SearchConfigurationPortability(context);
            SearchObjectOwner owner = new SearchObjectOwner(context, SearchObjectLevel.SPWeb);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Writing out search configuration settings from: " + context.Web.Title);
            Console.ForegroundColor = defaultForeground;

            ClientResult<string> configresults = sconfig.ExportSearchConfiguration(owner);
            context.ExecuteQuery();

            if (configresults.Value != null)
            {
                string results = configresults.Value;
                System.IO.File.WriteAllText(settingsFile, results, Encoding.ASCII);
                
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Search settings have been exported. Press any key to continue");
                Console.ForegroundColor = defaultForeground;
                Console.Read();                
            }
            else
            {                
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No search settings configuration results were returned. Press any key to continue");
                Console.ForegroundColor = defaultForeground;
                Console.Read();
            }            
        }

        private static void ImportSearchSettings(ClientContext context, string settingsFile)
        {
            ConsoleColor defaultForeground = Console.ForegroundColor;

            /*
            * SearchConfigurationPortability Class
            * http://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.search.portability.searchconfigurationportability(v=office.15).aspx
            * 
            * SearchObjectOwner Class
            * http://msdn.microsoft.com/en-us/library/office/microsoft.office.server.search.administration.searchobjectowner(v=office.15).aspx
           */
            SearchConfigurationPortability sconfig = new SearchConfigurationPortability(context);
            SearchObjectOwner owner = new SearchObjectOwner(context, SearchObjectLevel.SPWeb);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Importing search configuration settings to: " + context.Web.Title);
            Console.ForegroundColor = defaultForeground;

            sconfig.ImportSearchConfiguration(owner, System.IO.File.ReadAllText(settingsFile));
            context.ExecuteQuery();            

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Search settings have been imported. Press any key to continue");
            Console.ForegroundColor = defaultForeground;
            Console.Read();            
        }

        private static SecureString GetPasswordFromConsoleInput()
        {
            ConsoleKeyInfo info;

            //Get the user's password as a SecureString
            SecureString securePassword = new SecureString();
            do
            {
                info = Console.ReadKey(true);
                if (info.Key != ConsoleKey.Enter)
                {
                    securePassword.AppendChar(info.KeyChar);
                }
            }
            while (info.Key != ConsoleKey.Enter);
            return securePassword;
        }
    }
}

