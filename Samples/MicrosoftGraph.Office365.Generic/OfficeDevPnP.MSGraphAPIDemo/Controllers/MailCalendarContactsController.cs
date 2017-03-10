using OfficeDevPnP.MSGraphAPIDemo.Components;
using OfficeDevPnP.MSGraphAPIDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace OfficeDevPnP.MSGraphAPIDemo.Controllers
{
    public class MailCalendarContactsController : Controller
    {
        #region Actions to be implemented

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListFolders()
        {
            return View();
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

        #endregion

        [HttpPost]
        public ActionResult PlayWithMail(PlayWithMailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            AntiForgery.Validate();

            var folders = MailHelper.ListFolders();

            // Here you can use whatever mailbox name that you like, instead of Inbox
            var messages = MailHelper.ListMessages(folders.FirstOrDefault(f => f.Name == "Posta in arrivo" || f.Name == "Inbox").Id);
            if (messages != null && messages.Count > 0)
            {
                var message = MailHelper.GetMessage(messages[0].Id, true);

                foreach (var attachment in message.Attachments)
                {
                    // Download content only for attachments smaller than 100K
                    if (attachment.Size < 100 * 1024)
                    {
                        attachment.EnsureContent();
                    }
                }
            }

            MailHelper.SendMessage(new Models.MailMessageToSend
            {
                Message = new Models.MailMessage
                {
                    Subject = "Test message",
                    Body = new Models.ItemBody
                    {
                        Content = "<html><body><h1>Hello from ASP.NET MVC calling Microsoft Graph API!</h1></body></html>",
                        Type = Models.BodyType.Html,
                    },
                    To = new List<Models.UserInfoContainer>(
                        new Models.UserInfoContainer[] {
                            new Models.UserInfoContainer
                            {
                                Recipient = new Models.UserInfo
                                {
                                    Name = model.MailSendToDescription,
                                    Address = model.MailSendTo,
                                }
                            }
                    }),
                },
                SaveToSentItems = true,
            });

            if (messages != null && messages.Count > 0)
            {
                MailHelper.Reply(messages[0].Id, "This a direct reply!");
                MailHelper.ReplyAll(messages[0].Id, "This a reply all!");
                MailHelper.Forward(messages[0].Id,
                    new List<Models.UserInfoContainer>(
                        new Models.UserInfoContainer[]
                        {
                        new Models.UserInfoContainer
                        {
                            Recipient = new Models.UserInfo
                            {
                                Name = model.MailSendToDescription,
                                Address = model.MailSendTo,
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
                    "Hey! Look at this!");
            }

            return View("Index");
        }

        [HttpPost]
        public ActionResult PlayWithCalendars(PlayWithMailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            AntiForgery.Validate();

            var calendars = CalendarHelper.ListCalendars();
            var calendar = CalendarHelper.GetCalendar(calendars[0].Id);
            var events = CalendarHelper.ListEvents(calendar.Id, 0);
            var eventsCalendarView = CalendarHelper.ListEvents(calendar.Id, DateTime.Now, DateTime.Now.AddDays(10), 0);

            if (events[0].ResponseStatus != null && events[0].ResponseStatus.Response == Models.ResponseType.NotResponded)
            {
                CalendarHelper.SendFeedbackForMeetingRequest(
                    calendar.Id, events[0].Id, MeetingRequestFeedback.Accept,
                    "I'm looking forward to meet you!");
            }

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
                                    Name = model.MailSendToDescription,
                                    Address = model.MailSendTo,
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
                    Importance = Models.ItemImportance.High,
                    Subject = "Introducing the Microsoft Graph API",
                    Body = new Models.ItemBody
                    {
                        Content = "<html><body><h2>Let's talk about the Microsoft Graph API!</h2></body></html>",
                        Type = Models.BodyType.Html,
                    },
                    Location = new Models.EventLocation
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
                    Importance = Models.ItemImportance.Normal,
                    Subject = "Recurring Event about Microsoft Graph API",
                    Body = new Models.ItemBody
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

            var seriesInstances = CalendarHelper.ListSeriesInstances(
                calendar.Id, eventSeries.Id, DateTime.Now, DateTime.Now.AddMonths(2));

            var singleEventToUpdate = CalendarHelper.GetEvent(calendar.Id, events[0].Id);

            singleEventToUpdate.Attendees = new List<Models.UserInfoContainer>(
                        new Models.UserInfoContainer[]
                        {
                            new Models.UserInfoContainer
                            {
                                Recipient = new Models.UserInfo
                                {
                                    Name = model.MailSendToDescription,
                                    Address = model.MailSendTo,
                                }
                            },
                        });
            var updatedEvent = CalendarHelper.UpdateEvent(calendar.Id, singleEventToUpdate);

            CalendarHelper.DeleteEvent(calendar.Id, singleEvent.Id);
            CalendarHelper.DeleteEvent(calendar.Id, eventSeries.Id);

            return View("Index");
        }

        public ActionResult PlayWithContacts()
        {
            var contacts = ContactsHelper.ListContacts();
            try
            {
                var photo = ContactsHelper.GetContactPhoto(contacts[0].Id);
            }
            catch (Exception)
            {
                // Something wrong while getting the thumbnail,
                // We will have to handle it properly ...
            }

            contacts[0].PersonalNotes += String.Format("Modified on {0}", DateTime.Now);
            var updatedContact = ContactsHelper.UpdateContact(contacts[0]);

            var addedContact = ContactsHelper.AddContact(new Models.Contact
            {
                GivenName = "Michael",
                DisplayName = "Michael Red",
                EmailAddresses = new List<Models.UserInfo>(
                    new Models.UserInfo[]
                    {
                        new Models.UserInfo
                        {
                            Address = "michael@company.com",
                            Name  = "Michael Red",
                        }
                    }),
                CompanyName = "Sample Company",
            });

            ContactsHelper.DeleteContact(addedContact.Id);

            return View("Index");
        }
    }
}
