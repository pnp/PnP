using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.SharePoint.Client;
using System.Security;
using System.Xml.Linq;

namespace Core.ListItemChangeMonitor
{
    class Program
    {

        static string url;
        static string listName;
        static string userName;
        static SecureString password;
        //static DateTime lastRunTime;
        static DateTime nextRunTime;
        const int WaitSeconds = 30;

        static void Main(string[] args)
        {

            // Request Office365 site from the user
            url = GetSite();
            listName = GetListName();

            /* Prompt for Credentials */
            Console.WriteLine("Enter Credentials for {0}", url);
            userName = GetUserName();
            password = GetPassword();

            /* End Program if no Credentials */
            if (string.IsNullOrEmpty(userName) || (password == null) || string.IsNullOrEmpty(listName))
                return;

            DoWork();

        }
        private static void DoWork()
        {
            Console.WriteLine();
            Console.WriteLine("Url: " + url);
            Console.WriteLine("User Name: " + userName);
            Console.WriteLine("List Name: " + listName);
            Console.WriteLine();
            try
            {

                Console.WriteLine(string.Format("Connecting to {0}", url));
                Console.WriteLine();
                ClientContext cc = new ClientContext(url);
                cc.AuthenticationMode = ClientAuthenticationMode.Default;
                cc.Credentials = new SharePointOnlineCredentials(userName, password);

                ListCollection lists = cc.Web.Lists;
                IEnumerable<List> results = cc.LoadQuery<List>(lists.Where(lst => lst.Title == listName));
                cc.ExecuteQuery();
                List list = results.FirstOrDefault();
                if (list == null)
                {

                    Console.WriteLine("A list named \"{0}\" does not exist. Press any key to exit...", listName);
                    Console.ReadKey();
                    return;
                }

                nextRunTime = DateTime.Now;

                ChangeQuery cq = new ChangeQuery(false, false);
                cq.Item = true;
                cq.DeleteObject = true;
                cq.Add = true;
                cq.Update = true;
                
                // Initially set the ChangeTokenStart to 2 days ago so we don't go off and grab every item from the list since the day it was created.
                // The format of the string is semicolon delimited with the following pieces of information in order
                // Version number 
                // A number indicating the change scope: 0 – Content Database, 1 – site collection, 2 – site, 3 – list. 
                // GUID representing the scope ID of the change token
                // Time (in UTC) when the change occurred
                // Number of the change relative to other changes
                cq.ChangeTokenStart = new ChangeToken();
                cq.ChangeTokenStart.StringValue = string.Format("1;3;{0};{1};-1", list.Id.ToString(), DateTime.Now.AddDays(-2).ToUniversalTime().Ticks.ToString());

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Format("Ctrl+c to terminate. Press \"r\" key to force run without waiting {0} seconds.", WaitSeconds));
                Console.WriteLine();
                Console.ResetColor();
                do
                {
                    do
                    {
                        if (Console.KeyAvailable && Console.ReadKey(true).KeyChar == 'r') { break; }
                    }
                    while (nextRunTime > DateTime.Now);

                    Console.WriteLine(string.Format("Looking for items modified after {0} UTC", GetDateStringFromChangeToken(cq.ChangeTokenStart)));

                    
                    ChangeCollection coll = list.GetChanges(cq);
                    cc.Load(coll);
                    cc.ExecuteQuery();


                    DisplayChanges(coll, cq.ChangeTokenStart);
                    // if we find any changes to the list take the last change and use the ChangeToken as the start time for our next query.
                    // The ChangeToken will contain the Date/time of the last change to any item in the list.
                    cq.ChangeTokenStart = coll.Count > 0 ? coll.Last().ChangeToken : cq.ChangeTokenStart;

                    nextRunTime = DateTime.Now.AddSeconds(WaitSeconds);

                } while (true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();

            }
        }

        private static String GetDateStringFromChangeToken(ChangeToken ct)
        {
            string ticks = ct.StringValue.Split(';')[3];
            DateTime dt = new DateTime(Convert.ToInt64(ticks));

            return string.Format("{0} {1}", dt.ToShortDateString(), dt.ToLongTimeString());
        }

        private static void DisplayChanges(ChangeCollection coll, ChangeToken ct)
        {
            if (coll.Count == 0)
            {
                Console.WriteLine(string.Format("No changes to {0} since {1} UTC.", listName, GetDateStringFromChangeToken(ct)));
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            foreach (ChangeItem itm in coll)
            {

                Console.WriteLine();
                Console.WriteLine(string.Format("List {0} had a Change of type \"{1}\" on the item with Id {2}.", listName, itm.ChangeType.ToString(), itm.ItemId));
            }
            Console.WriteLine();
            Console.ResetColor();
        }

        static SecureString GetPassword()
        {
            SecureString sStrPwd = new SecureString();
            try
            {
                Console.Write("SharePoint Password : ");

                for (ConsoleKeyInfo keyInfo = Console.ReadKey(true); keyInfo.Key != ConsoleKey.Enter; keyInfo = Console.ReadKey(true))
                {
                    if (keyInfo.Key == ConsoleKey.Backspace)
                    {
                        if (sStrPwd.Length > 0)
                        {
                            sStrPwd.RemoveAt(sStrPwd.Length - 1);
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                            Console.Write(" ");
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        }
                    }
                    else if (keyInfo.Key != ConsoleKey.Enter)
                    {
                        Console.Write("*");
                        sStrPwd.AppendChar(keyInfo.KeyChar);
                    }

                }
                Console.WriteLine();
            }
            catch (Exception e)
            {
                sStrPwd = null;
                Console.WriteLine(e.Message);
            }

            return sStrPwd;
        }

        static string GetListName()
        {
            string strListName = string.Empty;
            try
            {
                Console.Write("SharePoint List Name : ");
                strListName = Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                strListName = string.Empty;
            }
            return strListName;
        }

        static string GetUserName()
        {
            string strUserName = string.Empty;
            try
            {
                Console.Write("SharePoint Username : ");
                strUserName = Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                strUserName = string.Empty;
            }
            return strUserName;
        }

        static string GetSite()
        {
            string siteUrl = string.Empty;
            try
            {
                Console.Write("Give Office365 site URL: ");
                siteUrl = Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                siteUrl = string.Empty;
            }
            return siteUrl;
        }
    }
}
