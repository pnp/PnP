using OfficeDevPnP.MSGraphAPIDemo.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OfficeDevPnP.MSGraphAPIDemo.Controllers
{
    public class MailCalendarContactsController : Controller
    {
        // GET: MailCalendarContacts
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListFolders()
        {
            var folders = MailHelper.ListFolders();
            var messages = MailHelper.ListMessages(folders.FirstOrDefault(f => f.Name == "Posta in arrivo").Id);
            var message = MailHelper.GetMessage(messages[0].Id, true);

            return View("Index");
        }

        public ActionResult ListMessages()
        {
            var folders = MailHelper.ListFolders();
            var messages = MailHelper.ListMessages(folders.FirstOrDefault(f => f.Name == "Posta in arrivo").Id);
            var message = MailHelper.GetMessage(messages[0].Id, true);

            return View("Index");
        }

        public ActionResult SendMessage()
        {
            return View();
        }

        public ActionResult ListCalendarEvents()
        {
            return View();
        }

        public ActionResult SendMeetingRequest()
        {
            return View();
        }

        public ActionResult ListContacts()
        {
            return View();
        }

        public ActionResult AddContact()
        {
            return View();
        }
    }
}