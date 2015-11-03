using Microsoft.SharePoint.Client;
using System.Web.Mvc;
using OfficeDevPnP.Core.Diagnostics;
using System.Threading.Tasks;


namespace Diagnostics.LoggingWeb.Controllers
{
    public class HomeController : Controller
    {
        string source = "I am the Source!";
        [SharePointContextFilter]
        public ActionResult Index(string command)
        {
            setSPUsername();

            switch (command)
            {
                case "Standard":
                    Standard();
                    break;
                case "Bare Minimum":
                    BareMinimum();
                    break;
                case "Monitored Scope":
                    MonitoredScope();
                    break;
            }
            return View();


        }

        private void Standard()
        {
            System.Diagnostics.Trace.TraceInformation("here is an example of what the standard trace output looks like");
            System.Diagnostics.Trace.TraceInformation("here is an another standard trace output");
            ViewBag.StandardMessage = "Ok, I've added a few standard entries to the Trace Log output.";
        }
        private void BareMinimum()
        {

            Log.LogLevel = LogLevel.Debug;
            Log.Error(source, "This is what a standard log entry looks like in PNP");
            Log.Error(source, "Here's another one, notice that the guid on the end is just zeroes.");
            Log.Error(source, "Here's one more with parameters {0}, {1}, {2}", "arg1", "arg2", "arg3");
            ViewBag.BareMinimumMessage = "Ok, I've added some entries to the Trace Log output.";
        }
        private void MonitoredScope()
        {
            Log.LogLevel = LogLevel.Debug;
            var scope = new PnPMonitoredScope("PnP Sleeping Monitored Scope");
            for (int i = 0; i < 3; i++)
            {
                //do a little looping and sleeping to show of the timer
                Task task = Task.Run(() =>
                {
                    SleepAndWake(i);
                });
            }
            ViewBag.MonitoredScopeMessage = "Ok, I'm adding some entries to the Trace Log output.";
        }

        private void SleepAndWake(int taskNumber)
        {
            using (PnPMonitoredScope p = new PnPMonitoredScope(string.Format("PnP Sleeping Monitored Scope {0}", taskNumber)))
            {
                for (int i = 0; i < 10; i++)
                {
                    p.LogInfo(string.Format("Starting sleep for 2 seconds, iteration {0}", i));

                    System.Threading.Thread.Sleep(2000);

                    p.LogInfo("Ending sleep");
                }
            }
        }
        private void setSPUsername()
        {

            User spUser = null;

            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    spUser = clientContext.Web.CurrentUser;

                    clientContext.Load(spUser, user => user.Title);

                    clientContext.ExecuteQuery();

                    ViewBag.UserName = spUser.Title;
                }
            }
        }
    }
}
