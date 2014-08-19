using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECM.RecordsManagement
{
    /// <summary>
    /// This program has been used to dump web and list properties + list eventhandler settings. 
    /// </summary>
    class Program
    {
        static bool toConsole = false;

        static void Main(string[] args)
        {
            // Office 365 Multi-tenant sample
            ClientContext cc = new AuthenticationManager().GetSharePointOnlineAuthenticatedContextTenant("https://bertonline.sharepoint.com/sites/130020", "bert.jansen@bertonline.onmicrosoft.com", GetPassWord());
            
            //if (!cc.Site.IsInPlaceRecordsManagementActive())
            //{
            //    cc.Site.EnableSiteForInPlaceRecordsManagement();
            //}
            
            FileStream ostrm;
            StreamWriter writer = null;
            TextWriter oldOut = Console.Out;
            string fileName = @"c:\temp\recordsmanagement.txt";

            //Redirect console to file if needed
            if (!toConsole)
            {
                try
                {
                    if (System.IO.File.Exists(fileName))
                    {
                        System.IO.File.Delete(fileName);
                    }
                    ostrm = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
                    writer = new StreamWriter(ostrm);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Cannot open recordsmanagement.txt for writing");
                    Console.WriteLine(e.Message);
                    return;
                }
                Console.SetOut(writer);
            }

            List ecm = cc.Web.GetListByTitle("Documents");

            //List ecm = cc.Web.GetListByTitle("ECMTest");
            cc.Load(ecm.RootFolder, p => p.Properties);
            cc.Load(ecm.EventReceivers);
            cc.Load(cc.Web, t => t.AllProperties);
            cc.ExecuteQuery();

            Console.WriteLine("Web properties:");
            foreach(var prop in cc.Web.AllProperties.FieldValues)
            {
                Console.WriteLine(String.Format("{0} : {1}", prop.Key, prop.Value != null ? prop.Value.ToString() : ""));
            }

            Console.WriteLine("=======================================================");
            Console.WriteLine("Rootfolder props = list props:");
            foreach(var prop in ecm.RootFolder.Properties.FieldValues)
            {
                Console.WriteLine(String.Format("{0} : {1}", prop.Key, prop.Value != null ? prop.Value.ToString() : ""));
            }
            Console.WriteLine("=======================================================");
            Console.WriteLine("List event receivers:");
            foreach (var eventReceiver in ecm.EventReceivers)
            {
                Console.WriteLine(String.Format("Name: {0}", eventReceiver.ReceiverName));
                Console.WriteLine(String.Format("Type: {0}", eventReceiver.EventType));
                Console.WriteLine(String.Format("Assembly: {0}", eventReceiver.ReceiverAssembly));
                Console.WriteLine(String.Format("Class: {0}", eventReceiver.ReceiverClass));
                Console.WriteLine(String.Format("Url: {0}", eventReceiver.ReceiverUrl));
                Console.WriteLine(String.Format("Sequence: {0}", eventReceiver.SequenceNumber));
                Console.WriteLine(String.Format("Synchronisation: {0}", eventReceiver.Synchronization));
            }

            Console.WriteLine("=======================================================");
            Console.WriteLine("List items:");
            CamlQuery query = CamlQuery.CreateAllItemsQuery(100);
            ListItemCollection items = ecm.GetItems(query);

            cc.Load(items);
            cc.ExecuteQuery();
            foreach (ListItem listItem in items)
            {
                foreach (var field in listItem.FieldValues)
                {
                    Console.WriteLine("{0} : {1}", field.Key, field.Value);
                }
                Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++");
            }

            if (!toConsole)
            {
                writer.Flush();
                Console.Out.Close();
                Console.SetOut(oldOut);
            }
            Console.WriteLine("Press enter to continue...");
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
