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
            var message = MailHelper.GetMessage(messages[1].Id, true);

            foreach (var attachment in message.Attachments)
            {
                // Download content only for attachments smaller than 100K
                if (attachment.Size < 100 * 1024)
                {
                    attachment.EnsureContent();
                }
            }

            MailHelper.SendMessage(new Models.MailMessageToSend {
                Message = new Models.MailMessageToSendContent
                {
                    Subject = "Test message",
                    Body = new Models.MessageBody
                    {
                        Content = "<html><body><h1>Hello from ASP.NET MVC calling Microsoft Graph API!</h1></body></html>",
                        Type = Models.BodyType.Html,
                    },
                    To = new List<Models.UserInfoContainer>(new Models.UserInfoContainer[] {
                    new Models.UserInfoContainer
                    {
                        Recipient = new Models.UserInfo
                        {
                            Name = "Paolo Pialorsi",
                            Address = "paolo@pialorsi.com",
                        }
                    }
                }),
                },
                SaveToSentItems = true,
            });

            return View("Index");
        }

        public ActionResult ListMessages()
        {
            return View();
        }

        public ActionResult SendMessage()
        {
            return View();
        }

        public ActionResult ListCalendarEvents()
        {
            var calendars = CalendarHelper.ListCalendars();
            var calendar = CalendarHelper.GetCalendar(calendars[0].Id);
            var events = CalendarHelper.ListEvents(calendar.Id, 0);
            var eventsCalendarView = CalendarHelper.ListEvents(calendar.Id, DateTime.Now, DateTime.Now.AddDays(10),  0);
            return View("Index");
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