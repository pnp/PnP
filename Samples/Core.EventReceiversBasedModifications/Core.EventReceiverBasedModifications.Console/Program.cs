using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeDevPnP.Core;
using Microsoft.SharePoint.Client;

namespace Contoso.Core.EventReceiverBasedModifications
{
    class Program
    {
        static void Main(string[] args)
        {

            AuthenticationManager am = new AuthenticationManager();
            ClientContext cc = am.GetSharePointOnlineAuthenticatedContextTenant("https://bertonline.sharepoint.com/sites/dev", "bert.jansen@bertonline.onmicrosoft.com", GetPassWord());

            cc.Load(cc.Web.EventReceivers);
            cc.ExecuteQuery();

            List<EventReceiverDefinition> rerToDelete = new List<EventReceiverDefinition>();

            foreach (EventReceiverDefinition rer in cc.Web.EventReceivers)
            {
                Console.WriteLine(string.Format("Type:{0}, Url:{1}, Assembly:{2}, Class:{3}", rer.EventType, rer.ReceiverUrl, rer.ReceiverAssembly, rer.ReceiverClass));
                if (rer.EventType == EventReceiverType.ListAdded && !String.IsNullOrEmpty(rer.ReceiverUrl))
                {
                    rerToDelete.Add(rer);
                }
            }

            Console.WriteLine("Cleanup old ListAdded event receivers");
            foreach (EventReceiverDefinition rer in rerToDelete)
            {
                // this might fail for side loaded apps
                rer.DeleteObject();
            }

            cc.ExecuteQuery();

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
