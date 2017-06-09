using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Portal.DataAccessLayer
{

    class Program
    {

        /// <summary>
        /// Gets or sets the domain provided by user
        /// </summary>
        public static string AdminDomain
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the username provided by user
        /// </summary>
        public static string AdminUsername
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the password provided by user
        /// </summary>
        public static SecureString AdminPassword
        {
            get;
            set;
        }

        private static void ShowUsage()
        {
            System.Console.WriteLine();
            System.Console.WriteLine();
            System.Console.ForegroundColor = System.ConsoleColor.White;
            System.Console.WriteLine("#### PnP Data Access Layer Console ####");
            System.Console.ResetColor();
            System.Console.ForegroundColor = System.ConsoleColor.Yellow;
            System.Console.WriteLine("Please type an operation number and press [Enter] to execute the specified operation:");
            System.Console.WriteLine("1. Configure CDN - ensures CDN folders and resource files");
            System.Console.WriteLine("2. Configure Admin Site Collection - ensures Site Columns and Custom Lists");
            System.Console.WriteLine("3. Configure Demo Site Collection - ensures Site Columns, Custom Lists, Master Page, and Demo web");
            System.Console.WriteLine();
            System.Console.WriteLine("NOTE: These operations will not overwrite existing PnP DAL assets (sites, webs, files, lists, list items, or settings)");
            System.Console.WriteLine();
            System.Console.WriteLine("Q. Quit/Exit");
            System.Console.ResetColor();
            System.Console.WriteLine();
        }

        static void Main(string[] args)
        {
            string input = String.Empty;

            GetCredentials();

            do
            {
                ShowUsage();

                input = System.Console.ReadLine().Trim().ToUpper(System.Globalization.CultureInfo.CurrentCulture);

                switch (input)
                {
                    case "1":
                        ConfigureCdnSite.DoWork();
                        break;

                    case "2":
                        ConfigureAdminSite.DoWork();
                        break;

                    case "3":
                        ConfigureDemoSite.DoWork();
                        break;

                    default:
                        break;
                }
            }
            while (input.ToUpper(System.Globalization.CultureInfo.CurrentCulture) != "Q");
        }

        /// <summary>
        /// get credentials
        /// </summary>
        public static void GetCredentials()
        {
            ConsoleKeyInfo key;
            bool retryUserNameInput = false;
            string account = String.Empty;
            string password = String.Empty;

            do
            {
                System.Console.ForegroundColor = System.ConsoleColor.Cyan;
                System.Console.WriteLine(@"Please enter the Admin account: ");
                System.Console.ForegroundColor = System.ConsoleColor.Yellow;
              //System.Console.WriteLine(@"- Use [domain\alias] format for On-Prem farms");
                System.Console.WriteLine(@"- Use [alias@domain.com] format for SPO-MT farms");
                System.Console.ResetColor();

                account = System.Console.ReadLine();

                if (account.Contains('\\'))
                {
                    string[] segments = account.Split('\\');
                    AdminDomain = segments[0];
                    AdminUsername = segments[1];
                    break;
                }
                if (account.Contains("@"))
                {
                    AdminUsername = account;
                    break;
                }
            }
            while (retryUserNameInput);

            System.Console.ForegroundColor = System.ConsoleColor.Cyan;
            System.Console.WriteLine("Please enter the Admin password: ");
            System.Console.ResetColor();

            do
            {
                key = System.Console.ReadKey(true);

                if (key.Key != ConsoleKey.Backspace)
                {
                    password += key.KeyChar;
                    System.Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password = password.Substring(0, password.Length - 1);
                        System.Console.CursorLeft--;
                        System.Console.Write('\0');
                        System.Console.CursorLeft--;
                    }
                }
            }
            while (key.Key != ConsoleKey.Enter);

            System.Console.WriteLine("");

            AdminPassword = Helper.CreateSecureString(password.TrimEnd('\r'));
        }
    }
}
