using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using OfficeDevPnP.Core.Diagnostics;
using Microsoft.SharePoint.Client;
using System.Configuration;
using System.Security;

namespace Diagnostics.LoggingWebJob
{
    public class Functions
    {
        // This function will enqueue a message on an Azure Queue called queue
        //public static void ManualTrigger(TextWriter log, int value, [Queue("queue")] out string message)
        //{
        //    log.WriteLine("Function is invoked with value={0}", value);
        //    message = value.ToString();
        //    log.WriteLine("Following message will be written on the Queue={0}", message);
        //}

        [NoAutomaticTrigger]
        // This function will be triggered based on the schedule you have set for this WebJob
        public static void VerifyDocLib()
        {
            var url = ConfigurationManager.AppSettings["SpUrl"].ToString();
            var user = ConfigurationManager.AppSettings["SpUserName"].ToString();
            var pw = ConfigurationManager.AppSettings["SpPassword"].ToString();
            using (PnPMonitoredScope p = new PnPMonitoredScope())
            {
                p.LogInfo("Gathering connect info from web.config");
                p.LogInfo("SharePoint url is " + url);
                p.LogInfo("SharePoint user is " + user);
                try
                {
                    ClientContext cc = new ClientContext(url);
                    cc.AuthenticationMode = ClientAuthenticationMode.Default;
                    cc.Credentials = new SharePointOnlineCredentials(user, GetPassword(pw));
                    using (cc)
                    {
                        if (cc != null)
                        {
                            string listName = "Site Pages";
                            p.LogInfo("This is what a Monitored Scope log entry looks like in PNP, more to come as we try to validate a lists existence.");
                            p.LogInfo("Connect to Sharepoint");
                            ListCollection lists = cc.Web.Lists;
                            IEnumerable<List> results = cc.LoadQuery<List>(lists.Where(lst => lst.Title == listName));
                            p.LogInfo("Executing query to find the Site Pages library");
                            cc.ExecuteQuery();
                            p.LogInfo("Query Executed successfully.");
                            List list = results.FirstOrDefault();
                            if (list == null)
                            {
                                p.LogError("Site Pages library not found.");
                            }
                            else
                            {
                                p.LogInfo("Site Pages library found.");

                                ListItemCollection items = list.GetItems(new CamlQuery());
                                cc.Load(items, itms => itms.Include(item => item.DisplayName));
                                cc.ExecuteQueryRetry();
                                string pgs = "These pages were found, " + string.Join(", ", items.Select(x => x.DisplayName));
                                p.LogInfo(pgs);
                            }
                        }
                    }

                }
                catch (System.Exception ex)
                {
                    p.LogInfo("We had a problem: " + ex.Message);
                }
            }
        }
        static SecureString GetPassword(string password)
        {
            SecureString spassword = new SecureString();
            foreach (char c in password)
                spassword.AppendChar(c);
            spassword.MakeReadOnly();
            return spassword;
        }

    }
}
