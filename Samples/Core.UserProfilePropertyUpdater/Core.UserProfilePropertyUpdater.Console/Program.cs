using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Core.UserProfilePropertyUpdater
{
    /// <summary>
    /// Console test program
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            UserProfileManager upm = new UserProfileManager();

            // Office 365 Multi-tenant sample
            upm.User = "bert.jansen@bertonline.onmicrosoft.com";
            upm.Password = GetPassWord();
            upm.TenantAdminUrl = "https://bertonline-admin.sharepoint.com";
            string userLoginName = "i:0#.f|membership|kevinc@set1.bertonline.info";

            // On-premises or Office 365 Dedicated sample
            //string userLoginName = @"SET1\KevinC";
            //upm.User = "administrator";
            //upm.Password = GetPassWord();
            //upm.Domain = "SET1";
            //upm.MySiteHost = "https://sp2013-my.set1.bertonline.info";

            Console.Write(String.Format("Value of {0} for {1} :", "AboutMe", userLoginName));
            Console.WriteLine(upm.GetPropertyForUser<String>("AboutMe", userLoginName));
            Console.WriteLine("-----------------------------------------------------");
            Console.WriteLine("");
            Console.Write(String.Format("Value of {0} for {1} :", "SPS-LastKeywordAdded", userLoginName));
            Console.WriteLine((upm.GetPropertyForUser<DateTime>("SPS-LastKeywordAdded", userLoginName)).ToLongDateString());
            Console.WriteLine("-----------------------------------------------------");
            Console.WriteLine("");
            Console.WriteLine(String.Format("Set value of {0} for {1} to {2}:", "AboutMe", userLoginName, "I love using Office AMS!"));
            upm.SetPropertyForUser<String>("AboutMe", "I love using Office AMS!", userLoginName);
            Console.WriteLine("");
            Console.Write(String.Format("Value of {0} for {1} :", "AboutMe", userLoginName));
            Console.WriteLine(upm.GetPropertyForUser<String>("AboutMe", userLoginName));
            Console.WriteLine("-----------------------------------------------------");            
            Console.WriteLine("");
            //nl-BE,fr-BE,en-US,de-DE
            Console.Write(String.Format("Value of {0} for {1} :", "SPS-MUILanguages", userLoginName));
            UserProfileASMX.PropertyData p = upm.GetPropertyForUser("SPS-MUILanguages", userLoginName);
            Console.WriteLine(p.Values[0].Value.ToString());
            Console.WriteLine("");
            Console.WriteLine(String.Format("Set value of {0} for {1} to {2}:", "SPS-MUILanguages", userLoginName, "nl-BE,en-US"));
            UserProfileASMX.PropertyData[] pMui = new UserProfileASMX.PropertyData[1];
            pMui[0] = new UserProfileASMX.PropertyData();
            pMui[0].Name = "SPS-MUILanguages";
            pMui[0].Values = new UserProfileASMX.ValueData[1];
            pMui[0].Values[0] = new UserProfileASMX.ValueData();
            pMui[0].Values[0].Value = "nl-BE,en-US";
            pMui[0].IsValueChanged = true;
            upm.SetPropertyForUser("SPS-MUILanguages", pMui, userLoginName);
            Console.WriteLine("-----------------------------------------------------");
            Console.WriteLine("");

            Console.ReadLine();
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
