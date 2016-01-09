using Newtonsoft.Json;
using OfficeDevPnP.MSGraphAPIDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Components
{
    public static class CalendarHelper
    {
        /// <summary>
        /// This method retrieves the calendars of the current user
        /// </summary>
        /// <param name="startIndex">The startIndex (0 based) of the folders to retrieve, optional</param>
        /// <returns>A page of up to 10 calendars</returns>
        public static List<Calendar> ListCalendars(Int32 startIndex = 0)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}me/calendars?$skip={1}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    startIndex));

            var calendarList = JsonConvert.DeserializeObject<CalendarList>(jsonResponse);
            return (calendarList.Calendars);
        }

        /// <summary>
        /// This method retrieves the calendars of the current user
        /// </summary>
        /// <param name="id">The ID of the calendar</param>
        /// <returns>The calendar</returns>
        public static Calendar GetCalendar(String id)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}me/calendars/{1}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    id));

            var calendar = JsonConvert.DeserializeObject<Calendar>(jsonResponse);
            return (calendar);
        }

        /// <summary>
        /// This method retrieves the events of the current user's calendar
        /// </summary>
        /// <param name="calendarId">The ID of the calendar</param>
        /// <param name="startIndex">The startIndex (0 based) of the folders to retrieve, optional</param>
        /// <returns>A page of up to 10 events</returns>
        public static List<Event> ListEvents(String calendarId, Int32 startIndex = 0)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}me/calendars/{1}/events?$skip={2}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    calendarId,
                    startIndex));

            var eventList = JsonConvert.DeserializeObject<EventList>(jsonResponse);
            return (eventList.Events);
        }

        /// <summary>
        /// This method retrieves the events of the current user's calendar within a specific date range
        /// </summary>
        /// <param name="calendarId">The ID of the calendar</param>
        /// <param name="startDate">The start date of the range</param>
        /// <param name="endDate">The end date of the range</param>
        /// <param name="startIndex">The startIndex (0 based) of the folders to retrieve, optional</param>
        /// <returns>A page of up to 10 events</returns>
        public static List<Event> ListEvents(String calendarId, DateTime startDate, 
            DateTime endDate, Int32 startIndex = 0)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}me/calendars/{1}/calendarView?startDateTime={2:o}&endDateTime={3:o}&$skip={4}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    calendarId,
                    startDate.ToUniversalTime(), 
                    endDate.ToUniversalTime(),
                    startIndex));

            var eventList = JsonConvert.DeserializeObject<EventList>(jsonResponse);
            return (eventList.Events);
        }

        public static Event CreateEvent(String calendarId, Event calendarEvent)
        {
            String jsonResponse = MicrosoftGraphHelper.MakePostRequestForString(
                String.Format("{0}me/calendars/{1}/events",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    calendarId),
                calendarEvent, "application/json");

            var createdEvent = JsonConvert.DeserializeObject<Event>(jsonResponse);
            return (createdEvent);
        }
    }
}