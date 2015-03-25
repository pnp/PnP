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
        static string listName = string.Empty;
        static string userName;
        static SecureString password;
        static DateTime lastRunTime;
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
            if (string.IsNullOrEmpty(userName) || (password == null) || (listName == string.Empty))
                return;

            DoWork();

        }
        private static void DoWork()
        {
            Console.WriteLine("");
            Console.WriteLine("Url: " + url);
            Console.WriteLine("User Name: " + userName);
            Console.WriteLine("List Name: " + listName);
            Console.WriteLine("");
            Console.WriteLine(string.Format("Ctrl+c to terminate. Press \"r\" key to force run without waiting {0} seconds.",WaitSeconds));
            try
            {

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


                lastRunTime = DateTime.Now.ToUniversalTime();

                ChangeQuery cq = new ChangeQuery(false, false);
                cq.Item = true;
                cq.DeleteObject = true;
                cq.Add = true;
                cq.Update = true;
                do
                {
                    do
                    {
                        if (Console.KeyAvailable && Console.ReadKey(true).KeyChar == 'r') { break; }
                    }
                    while (lastRunTime.AddSeconds(WaitSeconds) > DateTime.Now.ToUniversalTime());

                    cq.ChangeTokenStart = new ChangeToken();
                    cq.ChangeTokenStart.StringValue = string.Format("1;3;{0};{1};-1", list.Id.ToString(), lastRunTime.Ticks.ToString());
                    lastRunTime = lastRunTime.AddSeconds(WaitSeconds).AddMilliseconds(1);
                    ChangeCollection coll = list.GetChanges(cq);
                    cc.Load(coll);
                    cc.ExecuteQuery();
                    DisplayChanges(coll);

                } while (true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();

            }
        }
        private static void DisplayChanges(ChangeCollection coll)
        {
            if (coll.Count == 0)
            {
                Console.WriteLine(string.Format("No changes to {0} since {1} {2} UTC.", listName, lastRunTime.ToShortDateString(), lastRunTime.ToLongTimeString()));
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            foreach (ChangeItem itm in coll)
            {

                Console.WriteLine("");
                Console.WriteLine(string.Format("List {0} had a Change of type \"{1}\" on the item with Id {2}.", listName, itm.ChangeType.ToString(), itm.ItemId));
            }
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
                Console.WriteLine("");
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
