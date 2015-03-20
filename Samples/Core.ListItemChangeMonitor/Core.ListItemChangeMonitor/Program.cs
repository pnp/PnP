using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using System.Threading;
namespace Core.ListItemChangeMonitor
{
    class Program
    {

        static string url;
        static string listName;
        static string userName;
        static string password;
        static bool allItems = false;
        static DateTime lastRunTime;
        const int WaitSeconds = 20;

        static void Main(string[] args)
        {
            Console.WriteLine("Enter the SharePoint site URL:");
            url = Console.ReadLine();
            Console.WriteLine("Enter the name of the list you would like to monitor (case sensitive):");
            listName = Console.ReadLine();
            Console.WriteLine("Enter the user name:");
            userName = Console.ReadLine();
            Console.WriteLine("Enter the password:");
            password = Console.ReadLine();

            DoWork();

        }
        private static void DoWork()
        {
            Console.WriteLine("Ctrl+c to terminate \"r\" to force run without waiting.");
            Console.WriteLine("Url: " + url);
            Console.WriteLine("User Name: " + userName);
            Console.WriteLine("List Name: " + listName);
            try
            {
                ClientContext ctx = new AuthenticationManager().GetSharePointOnlineAuthenticatedContextTenant(url, userName, password);
                List list = ctx.Web.GetListByTitle(listName);
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
                    ctx.Load(coll);
                    ctx.ExecuteQuery();
                    DisplayChanges(coll);

                } while (true);
            }
            catch (Exception)
            {

                throw;
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
    }
}
