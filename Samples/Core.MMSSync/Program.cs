using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Core.MMSSync
{
    class Program
    {
        static int Main(string[] args)
        {
            ConsoleColor defaultForeground = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Enter the URL of the Source MMS.");
            Console.ForegroundColor = defaultForeground;
            string _sourceUrl = Console.ReadLine();

            if (_sourceUrl == "")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Enter the URL of the Source MMS.");
                Console.ForegroundColor = defaultForeground;
                _sourceUrl = Console.ReadLine();

                if (_sourceUrl == "")
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
            string _sourceUserName = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Enter your password:");
            Console.ForegroundColor = defaultForeground;
            SecureString _sourcePassword = GetPasswordFromConsole();


            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Enter the URL of the target MMS:");
            Console.ForegroundColor = defaultForeground;
            string _targetUrl = Console.ReadLine();

            if (_targetUrl == "")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Enter the URL of the Source MMS SharePoint Online site");
                Console.ForegroundColor = defaultForeground;
                _sourceUrl = Console.ReadLine();

                if (_targetUrl == "")
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
            string _targetUserName = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Enter your password:");
            Console.ForegroundColor = defaultForeground;
            SecureString _targetPassword = GetPasswordFromConsole();

            ClientContext _ctxSource = new ClientContext(_sourceUrl);
            _ctxSource.Credentials = new SharePointOnlineCredentials(_sourceUserName, _sourcePassword);
            _ctxSource.ExecuteQuery();

            ClientContext _ctxTarget = new ClientContext(_targetUrl);
            _ctxTarget.Credentials = new SharePointOnlineCredentials(_targetUserName, _targetPassword);
            _ctxTarget.ExecuteQuery();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Select the type of operation you would like to perform:");
            Console.ForegroundColor = defaultForeground;
            Console.WriteLine(" 1 (Enter 1 for Move Term Group)");
            Console.WriteLine(" 2 (Enter 2 for Process Changes)");
            string opsTypeSelection = Console.ReadLine();

            if (opsTypeSelection == null || opsTypeSelection != "1" && opsTypeSelection != "2")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Try again. Select the type of operation you would like to perform:");
                Console.ForegroundColor = defaultForeground;
                Console.WriteLine(" 1 (Enter 1 for Move Term Group)");
                Console.WriteLine(" 2 (Enter 2 for Process Changes)");
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

            

            if (opsTypeSelection == "1")
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Enter the TermGroup Name you wish to move.");
                Console.ForegroundColor = defaultForeground;
                string _termGroup = Console.ReadLine();
                if(string.IsNullOrEmpty(_termGroup))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Term Group Name is empty. I can not work on it.");
                    Console.WriteLine("Press any key to continue...");
                    Console.ForegroundColor = defaultForeground;
                    Console.ReadLine();
                    return 1;
                }
                MMSSyncManager _manager = new MMSSyncManager();
                _manager.MoveTermGroup(_ctxSource, _ctxTarget, _termGroup);

            }
            else if (opsTypeSelection == "2")
            {
                string[] _termsetInput;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Enter the Termset to Sync, (ex: Fruit, Colors)");
                Console.ForegroundColor = defaultForeground;
                _termsetInput = Console.ReadLine().Split(new char[]{','});


                List<string> _termSets = new List<string>(_termsetInput);

                MMSSyncManager _manager = new MMSSyncManager();
                _manager.ProcessChanges(_ctxSource, _ctxTarget, _termSets);
            }

            Console.WriteLine("I worked on it for you.");
            return 0;   
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
    }
}
