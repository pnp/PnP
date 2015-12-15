using BusinessApps.HelpDesk.Helpers;
using BusinessApps.HelpDesk.Models.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BusinessApps.HelpDesk.Controllers
{
    public class EmailController : Controller
    {
        // GET: Email
        [HttpGet]
        public async Task<JsonResult> Index()
        {
            GraphHelper graphHelper = new GraphHelper();
            IEnumerable<EmailMessage> messages = await graphHelper.GetNewEmailMessages();
            return Json(messages.OrderByDescending(m => m.SentTimestamp), JsonRequestBehavior.AllowGet);
        }

        public async Task<PartialViewResult> EmailDetails(string messageId)
        {
            GraphHelper graphHelper = new GraphHelper();
            EmailMessage message = await graphHelper.GetEmailMessage(messageId);

            EmailDetailsViewModel model = new EmailDetailsViewModel();
            model.EmailMessage = message;

            model.HelpdeskOperators = await graphHelper.GetHelpdeskOperators();

            return PartialView("_EmailDetails", model);
        }

        public async Task DiscardEmail(string messageId)
        {
            GraphHelper graphHelper = new GraphHelper();
            await graphHelper.DeleteEmailMessage(messageId);
        }
    }
}