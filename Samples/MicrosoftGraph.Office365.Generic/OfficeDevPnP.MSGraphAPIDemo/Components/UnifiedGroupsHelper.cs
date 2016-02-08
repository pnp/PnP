using Newtonsoft.Json;
using OfficeDevPnP.MSGraphAPIDemo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;


namespace OfficeDevPnP.MSGraphAPIDemo.Components
{
    public static class UnifiedGroupsHelper
    {
        /// <summary>
        /// This method retrieves the list of threads of an Office 365 Group
        /// </summary>
        /// <param name="groupId">The ID of the group</param>
        /// <returns>The threads of an Office 365 Group</returns>
        public static List<ConversationThread> ListUnifiedGroupThreads(String groupId)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}groups/{1}/threads",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    groupId));

            var conversationThreadsList = JsonConvert.DeserializeObject<ConversationThreadsList>(jsonResponse);
            return (conversationThreadsList.Threads);
        }

        /// <summary>
        /// This method retrieves the list of posts of a thread for an Office 365 Group
        /// </summary>
        /// <param name="groupId">The ID of the thread</param>
        /// <param name="threadId">The ID of the thread</param>
        /// <returns>The posts of a thread for an Office 365 Group</returns>
        public static List<ConversationThreadPost> ListUnifiedGroupThreadPosts(String groupId, String threadId)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}groups/{1}/threads/{2}/posts",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    groupId,
                    threadId));

            var conversationThreadPostsList = JsonConvert.DeserializeObject<ConversationThreadPostsList>(jsonResponse);
            return (conversationThreadPostsList.Posts);
        }

        /// <summary>
        /// This method retrieves a single post of a conversation thread for an Office 365 Group
        /// </summary>
        /// <param name="groupId">The ID of the thread</param>
        /// <param name="threadId">The ID of the thread</param>
        /// <param name="postId">The ID of the post</param>
        /// <returns>The post of the conversation thread for an Office 365 Group</returns>
        public static ConversationThreadPost GetUnifiedGroupThreadPost(String groupId, String threadId, String postId)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}groups/{1}/threads/{2}/posts/{3}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    groupId,
                    threadId,
                    postId));

            var conversationThreadPost = JsonConvert.DeserializeObject<ConversationThreadPost>(jsonResponse);
            return (conversationThreadPost);
        }

        /// <summary>
        /// This method replies to a thread of an Office 365 Group
        /// </summary>
        /// <param name="groupId">The ID of the thread</param>
        /// <param name="threadId">The ID of the thread</param>
        /// <param name="post">The post to send as the reply</param>
        public static void ReplyToUnifiedGroupThread(String groupId, String threadId, ConversationThreadPost post)
        {
            MicrosoftGraphHelper.MakePostRequest(
                String.Format("{0}groups/{1}/threads/{2}/reply",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    groupId,
                    threadId), new { post }, "application/json");
        }

        /// <summary>
        /// This method retrieves the calendar of an Office 365 Group
        /// </summary>
        /// <param name="groupId">The ID of the group</param>
        /// <returns>The calendar of an Office 365 Group</returns>
        public static Calendar GetUnifiedGroupCalendar(String groupId)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}groups/{1}/calendar",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    groupId));

            var calendar = JsonConvert.DeserializeObject<Calendar>(jsonResponse);
            return (calendar);
        }

        /// <summary>
        /// This method retrieves the events of an Office 365 Group calendar
        /// </summary>
        /// <param name="groupId">The ID of the group</param>
        /// <param name="startIndex">The startIndex (0 based) of the items to retrieve, optional</param>
        /// <returns>A page of up to 10 events</returns>
        public static List<Event> ListUnifiedGroupEvents(String groupId, Int32 startIndex = 0)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}groups/{1}/events?$skip={2}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    groupId,
                    startIndex));

            var eventList = JsonConvert.DeserializeObject<EventList>(jsonResponse);
            return (eventList.Events);
        }

        /// <summary>
        /// Retrieves the events of an Office 365 Group calendar within a specific date range
        /// </summary>
        /// <param name="groupId">The ID of the group</param>
        /// <param name="startDate">The start date of the range</param>
        /// <param name="endDate">The end date of the range</param>
        /// <param name="startIndex">The startIndex (0 based) of the items to retrieve, optional</param>
        /// <returns>A page of up to 10 events</returns>
        public static List<Event> ListUnifiedGroupEvents(String groupId, DateTime startDate,
            DateTime endDate, Int32 startIndex = 0)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}groups/{1}/calendarView?startDateTime={2:o}&endDateTime={3:o}&$skip={4}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    groupId,
                    startDate.ToUniversalTime(),
                    endDate.ToUniversalTime(),
                    startIndex));

            var eventList = JsonConvert.DeserializeObject<EventList>(jsonResponse);
            return (eventList.Events);
        }

        /// <summary>
        /// This method retrieves the list of threads of an Office 365 Group
        /// </summary>
        /// <param name="groupId">The ID of the group</param>
        /// <param name="startIndex">The startIndex (0 based) of the items to retrieve, optional</param>
        /// <returns>The threads of an Office 365 Group</returns>
        public static List<Conversation> ListUnifiedGroupConversations(
            String groupId, Int32 startIndex = 0)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}groups/{1}/conversations?$skip={2}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    groupId,
                    startIndex));

            var conversationList = JsonConvert.DeserializeObject<ConversationsList>(jsonResponse);
            return (conversationList.Conversations);
        }

        /// <summary>
        /// This method retrieves the OneDrive for Business of an Office 365 Group
        /// </summary>
        /// <param name="groupId">The ID of the group</param>
        /// <returns>The OneDrive for Business of an Office 365 Group</returns>
        public static Drive GetUnifiedGroupDrive(String groupId)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}groups/{1}/drive",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    groupId));

            var drive = JsonConvert.DeserializeObject<Drive>(jsonResponse);
            return (drive);
        }

        /// <summary>
        /// Creates/Adds a new Office 365 Group
        /// </summary>
        /// <param name="group">The group tp add/create</param>
        /// <returns>The just added group</returns>
        public static Group AddUnifiedGroup(Group group)
        {
            String jsonResponse = MicrosoftGraphHelper.MakePostRequestForString(
                String.Format("{0}groups",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri),
                group, "application/json");

            var addedGroup = JsonConvert.DeserializeObject<Group>(jsonResponse);
            return (addedGroup);
        }

        /// <summary>
        /// Updates the photo of an Office 365 Group
        /// </summary>
        /// <param name="groupId">The ID of the target group</param>
        /// <param name="photo">The byte array of the photo</param>
        public static void UpdateUnifiedGroupPhoto(String groupId, Stream photo)
        {
            MicrosoftGraphHelper.MakePatchRequestForString(
                String.Format("{0}groups/{1}/photo/$value",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    groupId),
                photo, "image/jpeg");
        }

        /// <summary>
        /// Deletes an Office 365 Group
        /// </summary>
        /// <param name="groupId">The ID of the group to delete</param>
        public static void DeleteUnifiedGroup(String groupId)
        {
            MicrosoftGraphHelper.MakeDeleteRequest(
                String.Format("{0}groups/{1}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri, groupId));
        }
    }
}