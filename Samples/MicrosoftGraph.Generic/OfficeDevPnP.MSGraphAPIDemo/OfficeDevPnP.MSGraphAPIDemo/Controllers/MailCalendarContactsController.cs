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
                Message = new Models.MailMessage
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

            var singleEvent = CalendarHelper.CreateEvent(calendars[0].Id,
                new Models.Event
                {
                    Attendees = new List<Models.UserInfoContainer>(
                        new Models.UserInfoContainer[]
                        {
                            new Models.UserInfoContainer
                            {
                                Recipient = new Models.UserInfo
                                {
                                    Address = "paolo@pialorsi.com",
                                    Name = "Paolo Pialorsi",
                                }
                            },
                            new Models.UserInfoContainer
                            {
                                Recipient = new Models.UserInfo
                                {
                                    Address = "someone@company.com",
                                    Name = "Someone Else",
                                }
                            },
                        }),
                    Start = new Models.TimeInfo
                    {
                        DateTime = DateTime.Now.AddDays(2).ToUniversalTime(),
                        TimeZone = "UTC"
                    },
                    OriginalStartTimeZone = "UTC",
                    End = new Models.TimeInfo
                    {
                        DateTime = DateTime.Now.AddDays(2).AddHours(1).ToUniversalTime(),
                        TimeZone = "UTC"
                    },
                    OriginalEndTimeZone = "UTC",
                    Importance = Models.MailImportance.High,
                    Subject = "Introducing the Microsoft Graph API",
                    Body = new Models.MessageBody
                    {
                        Content = "<html><body><h2>Let's talk about the Microsoft Graph API!</h2></body></html>",
                        Type = Models.BodyType.Html,
                    },
                    Location  = new Models.EventLocation
                    {
                        Name = "PiaSys.com Head Quarters",
                    },
                    IsAllDay = false,
                    IsOrganizer = true,
                    ShowAs = Models.EventStatus.WorkingElsewhere,
                    Type = Models.EventType.SingleInstance,
                });

            var nextMonday = DateTime.Now.AddDays(((int)DayOfWeek.Monday - (int)DateTime.Now.DayOfWeek + 7) % 7);
            var nextMonday9AM = new DateTime(nextMonday.Year, nextMonday.Month, nextMonday.Day, 9, 0, 0);
            var lastDayOfMonth = new DateTime(nextMonday.AddMonths(1).Year, nextMonday.AddMonths(1).Month, 1).AddDays(-1);
            var eventSeries = CalendarHelper.CreateEvent(calendars[0].Id,
                new Models.Event
                {
                    Start = new Models.TimeInfo
                    {
                        DateTime = nextMonday9AM.ToUniversalTime(),
                        TimeZone = "UTC"
                    },
                    OriginalStartTimeZone = "UTC",
                    End = new Models.TimeInfo
                    {
                        DateTime = nextMonday9AM.AddHours(1).ToUniversalTime(),
                        TimeZone = "UTC"
                    },
                    OriginalEndTimeZone = "UTC",
                    Importance = Models.MailImportance.Normal,
                    Subject = "Recurring Event about Microsoft Graph API",
                    Body = new Models.MessageBody
                    {
                        Content = "<html><body><h2>Let's talk about the Microsoft Graph API!</h2></body></html>",
                        Type = Models.BodyType.Html,
                    },
                    Location = new Models.EventLocation
                    {
                        Name = "Paolo's Office",
                    },
                    IsAllDay = false,
                    IsOrganizer = true,
                    ShowAs = Models.EventStatus.Busy,
                    Type = Models.EventType.SeriesMaster,
                    Recurrence = new Models.EventRecurrence
                    {
                        Pattern = new Models.EventRecurrencePattern
                        {
                            Type = Models.RecurrenceType.Weekly,
                            DaysOfWeek = new DayOfWeek[] { DayOfWeek.Monday },
                            Interval = 1,
                        },
                        Range = new Models.EventRecurrenceRange
                        {
                            StartDate = nextMonday9AM.ToUniversalTime(),
                            Type = Models.RecurrenceRangeType.EndDate,
                            EndDate = lastDayOfMonth.ToUniversalTime(),
                        }
                    }
                });

            var singleEventToUpdate = CalendarHelper.GetEvent(calendar.Id, events[0].Id);

            singleEventToUpdate.Attendees = new List<Models.UserInfoContainer>(
                        new Models.UserInfoContainer[]
                        {
                            new Models.UserInfoContainer
                            {
                                Recipient = new Models.UserInfo
                                {
                                    Address = "paolo@pialorsi.com",
                                    Name = "Paolo Pialorsi",
                                }
                            },
                        });
            var updatedEvent = CalendarHelper.UpdateEvent(calendar.Id, singleEventToUpdate);

            CalendarHelper.DeleteEvent(calendar.Id, singleEvent.Id);
            CalendarHelper.DeleteEvent(calendar.Id, eventSeries.Id);

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