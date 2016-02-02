using Office365Api.Graph.Simple.MailAndFiles.Helpers;
using Office365Api.Graph.Simple.MailAndFiles.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace Office365Api.Graph.Simple.MailAndFiles.Controllers
{
    /// <summary>
    /// Used to get email information in JSON format for the caller
    /// </summary>
    public class EmailController : Controller
    {
        // GET: Email
        [HttpGet]
        public async Task<JsonResult> Index()
        {
            string accessToken = (Session[SessionKeys.Login.AccessToken] as string);
            IEnumerable<EmailMessage> messages = await GraphHelper.GetEmails(accessToken);
            return Json(messages.OrderByDescending(m => m.SentTimestamp), JsonRequestBehavior.AllowGet);
        }
    }
}