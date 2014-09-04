using Contoso.Patterns.Provisioning;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Enums;
using Patterns.Provisioning.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Patterns.Hosting.IIS.Jobs
{
    class Program
    {

        static void Main(string[] args)
        {
            Provisioning p = new Provisioning();
            p.Provision(GetPassWord());
        }


        /// <summary>
        /// Gets the password input from the console window
        /// </summary>
        /// <returns>the entered password</returns>
        private static string GetPassWord()
        {
            Console.Write("SharePoint Password : ");

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
                    Console.Write("*");
                    strPwd += keyInfo.KeyChar;

                }

            }
            Console.WriteLine("");

            return strPwd;
        }


    }




}
