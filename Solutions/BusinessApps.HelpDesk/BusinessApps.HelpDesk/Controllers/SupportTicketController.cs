using BusinessApps.HelpDesk.Data;
using BusinessApps.HelpDesk.Helpers;
using BusinessApps.HelpDesk.Models.SupportTicket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BusinessApps.HelpDesk.Controllers
{
    public class SupportTicketController : Controller
    {
        // GET: SupportTicket
        public JsonResult GetSupportTicketsForUser()
        {
            HelpDeskDatabase db = new HelpDeskDatabase();
            IEnumerable<SupportTicket> tickets = db.SupportTickets.Where(s => s.AssignedTo.Equals(User.Identity.Name, StringComparison.InvariantCultureIgnoreCase) && s.Status == "Open");

            return Json(tickets, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult GetSupportTicketDetails(string supportTicketID)
        {
            HelpDeskDatabase db = new HelpDeskDatabase();

            SupportTicketViewModel model = new SupportTicketViewModel();
            model.SupportTicket = db.SupportTickets.Find(Int32.Parse(supportTicketID));

            return PartialView("_SupportTicketDetails", model);
        }

        [HttpPost]
        public async Task AssignSupportTicket(string messageID, string title, string description, string assignedTo)
        {
            SupportTicket supportTicket = new SupportTicket();
            supportTicket.AssignedTo = assignedTo;
            supportTicket.MessageID = messageID;
            supportTicket.Title = Uri.UnescapeDataString(title);
            supportTicket.Description = Uri.UnescapeDataString(description);
            supportTicket.Status = "Open";

            HelpDeskDatabase db = new HelpDeskDatabase();
            db.SupportTickets.Add(supportTicket);
            db.SaveChanges();

            GraphHelper graphHelper = new GraphHelper();
            await graphHelper.MarkEmailMessageAssigned(messageID);
        }

        [HttpPost]
        public async Task CloseSupportTicket(string supportTicketID)
        {
            HelpDeskDatabase db = new HelpDeskDatabase();
            SupportTicket supportTicket = db.SupportTickets.Find(Int32.Parse(supportTicketID));
            supportTicket.Status = "Closed";
            db.SaveChanges();

            GraphHelper graphHelper = new GraphHelper();
            await graphHelper.MarkEmailMessageClosed(supportTicket.MessageID);
        }
    }
}