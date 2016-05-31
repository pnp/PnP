using BusinessApps.HelpDesk.Helpers;
using BusinessApps.HelpDesk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BusinessApps.HelpDesk.Controllers
{
    public class AnnouncementController : Controller
    {
        // GET: Announcement
        public async Task<JsonResult> Index()
        {
            SharePointHelper sharepointHelper = new SharePointHelper();
            IEnumerable<Announcement> announcements = await sharepointHelper.GetAnnouncements();
            return Json(announcements.OrderByDescending(a => a.Timestamp), JsonRequestBehavior.AllowGet);
        }
    }
}