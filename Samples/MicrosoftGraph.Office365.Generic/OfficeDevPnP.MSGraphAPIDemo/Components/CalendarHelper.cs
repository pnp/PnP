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
        /// <param name="startIndex">The startIndex (0 based) of the items to retrieve, optional</param>
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
        /// <param name="startIndex">The startIndex (0 based) of the items to retrieve, optional</param>
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
        /// <param name="startIndex">The startIndex (0 based) of the items to retrieve, optional</param>
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

        /// <summary>
        /// This method retrieves the events of a series within a specific date range
        /// </summary>
        /// <param name="calendarId">The ID of the calendar</param>
        /// <param name="masterSeriesId">The ID of the master event of the series</param>
        /// <param name="startDate">The start date of the range</param>
        /// <param name="endDate">The end date of the range</param>
        /// <param name="startIndex">The startIndex (0 based) of the items to retrieve, optional</param>
        /// <returns>A page of up to 10 events</returns>
        public static List<Event> ListSeriesInstances(String calendarId, 
            String masterSeriesId,
            DateTime startDate,
            DateTime endDate, 
            Int32 startIndex = 0)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}me/calendars/{1}/events/{2}/instances?startDateTime={3:o}&endDateTime={4:o}&$skip={5}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    calendarId,
                    masterSeriesId,
                    startDate.ToUniversalTime(),
                    endDate.ToUniversalTime(),
                    startIndex));

            var eventList = JsonConvert.DeserializeObject<EventList>(jsonResponse);
            return (eventList.Events);
        }

        /// <summary>
        /// This method creates an event in a target calendar
        /// </summary>
        /// <param name="calendarId">The ID of the target calendar</param>
        /// <param name="calendarEvent">The event to add</param>
        /// <returns>The added event</returns>
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

        /// <summary>
        /// This method retrieves an event from a calendar
        /// </summary>
        /// <param name="calendarId">The ID of the calendar</param>
        /// <param name="eventId">The ID of the event</param>
        /// <returns>The retrieved event</returns>
        public static Event GetEvent(String calendarId, String eventId)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}me/calendars/{1}/events/{2}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    calendarId, eventId));

            var calendarEvent = JsonConvert.DeserializeObject<Event>(jsonResponse);
            return (calendarEvent);
        }

        /// <summary>
        /// This method updates an event in a calendar
        /// </summary>
        /// <param name="calendarId">The ID of the calendar</param>
        /// <param name="eventId">The event to update</param>
        /// <returns>The updated event</returns>
        public static Event UpdateEvent(String calendarId, Event eventToUpdate)
        {
            String jsonResponse = MicrosoftGraphHelper.MakePatchRequestForString(
                String.Format("{0}me/calendars/{1}/events/{2}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    calendarId, eventToUpdate.Id), 
                eventToUpdate, "application/json");

            var updatedEvent = JsonConvert.DeserializeObject<Event>(jsonResponse);
            return (updatedEvent);
        }

        /// <summary>
        /// This method deletes an event from a calendar
        /// </summary>
        /// <param name="calendarId">The ID of the calendar</param>
        /// <param name="eventId">The ID of the event to delete</param>
        public static void DeleteEvent(String calendarId, String eventId)
        {
            MicrosoftGraphHelper.MakeDeleteRequest(
                String.Format("{0}me/calendars/{1}/events/{2}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    calendarId, eventId));
        }

        /// <summary>
        /// This method provides a feedback for a received meeting request 
        /// </summary>
        /// <param name="calendarId">The ID of the calendar</param>
        /// <param name="eventId">The ID of the meeting request</param>
        /// <param name="feedback">The feedback for the meeting request</param>
        /// <param name="comment">Any comment to include in the feedback, optional</param>
        public static void SendFeedbackForMeetingRequest(String calendarId,
            String eventId,
            MeetingRequestFeedback feedback,
            String comment = null)
        {
            MicrosoftGraphHelper.MakePostRequest(
                String.Format("{0}me/calendars/{1}/events/{2}/{3}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    calendarId, eventId, feedback),
                    content: !String.IsNullOrEmpty(comment) ? new { Comment = comment } : null,
                    contentType: "application/json");
        }
    }

    /// <summary>
    /// Defines the possible feedbacks for a meeting request
    /// </summary>
    public enum MeetingRequestFeedback
    {
        Accept,
        Decline,
        TentativelyAccept,
    }
}