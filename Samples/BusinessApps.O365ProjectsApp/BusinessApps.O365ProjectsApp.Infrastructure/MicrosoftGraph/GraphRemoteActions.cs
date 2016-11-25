using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BusinessApps.O365ProjectsApp.Infrastructure.MicrosoftGraph
{
    public static class GraphRemoteActions
    {
        /// <summary>
        /// Creates a new Office 365 Group for a target Project
        /// </summary>
        /// <param name="AccessToken">The AccessToken to use for creating the Office 365 Group</param>
        /// <param name="group">The group to create</param>
        /// <param name="membersUPN">An array of users' UPN that will become members of the group</param>
        /// <param name="photo">The photo of the group</param>
        /// <returns>The Office 365 Group created</returns>
        public static Group CreateOffice365Group(
            Group group,
            String[] membersUPN,
            Stream photo = null, 
            String accessToken = null)
        {
            // Create the Office 365 Group
            String jsonResponse = MicrosoftGraphHelper.MakePostRequestForString(
                String.Format("{0}groups",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri),
                group, "application/json", accessToken: accessToken);

            var addedGroup = JsonConvert.DeserializeObject<Group>(jsonResponse);

            // Set users' membership
            foreach (var upn in membersUPN)
            {
                MicrosoftGraphHelper.MakePostRequest(
                    String.Format("{0}groups/{1}/members/$ref",
                        MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                        addedGroup.Id),
                    new GroupMemberToAdd
                    {
                        ObjectId = String.Format("{0}users/{1}/id",
                        MicrosoftGraphHelper.MicrosoftGraphV1BaseUri, upn)
                    },
                    "application/json", accessToken: accessToken);
            }

            // Update the group's picture, if any
            if (photo != null)
            {
                // Retry up to 10 times within 5 seconds, because the 
                // Office 365 Group sometime takes long to be ready
                Int32 retryCount = 0;
                while (true)
                {
                    retryCount++;

                    try
                    {
                        if (retryCount > 10) break;
                        System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(500));

                        photo.Position = 0;
                        MemoryStream photoCopy = new MemoryStream();
                        photo.CopyTo(photoCopy);
                        photoCopy.Position = 0;

                        MicrosoftGraphHelper.MakePatchRequestForString(
                            String.Format("{0}groups/{1}/photo/$value",
                                MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                                addedGroup.Id),
                            photoCopy, "image/jpeg", accessToken: accessToken);

                        break;
                    }
                    catch
                    {
                        // Ignore any exception, just wait for a while and retry
                    }
                }
            }

            return (addedGroup);
        }

        /// <summary>
        /// Checks whether an Office 365 Group exists or not
        /// </summary>
        /// <param name="groupName">The name of the group</param>
        /// <returns>Whether the group exists or not</returns>
        public static Boolean Office365GroupExists(String groupName)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}groups?$select=id,displayName" +
                    "&$filter=groupTypes/any(gt:%20gt%20eq%20'Unified')%20" +
                    "and%20displayName%20eq%20'{1}'",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    HttpUtility.UrlEncode(groupName).Replace("%27", "''")));

            var foundGroups = JsonConvert.DeserializeObject<GroupsList>(jsonResponse);

            return (foundGroups != null && foundGroups.Groups.Count > 0);
        }

        public static void SendMessageToGroupConversation(String groupId, Conversation conversation, String accessToken = null)
        {
            String jsonResponse = MicrosoftGraphHelper.MakePostRequestForString(
                String.Format("{0}groups/{1}/conversations",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri, groupId),
                    conversation, "application/json", accessToken);
        }

        /// <summary>
        /// Creates a new thread in the conversation flow of a target Office 365 Group
        /// </summary>
        /// <param name="groupId">The ID of the target Office 365 Group</param>
        public static void SendMessageToGroupConversation(String groupId, String accessToken = null)
        {
            var conversation = new Conversation
            {
                Topic = "Let's manage this Business Project!",
                Threads = new List<ConversationThread>(
                    new ConversationThread[] {
                                new ConversationThread
                                {
                                    Topic = "I've just created this Business Project",
                                    Posts = new List<ConversationThreadPost>(
                                        new ConversationThreadPost[]
                                        {
                                            new ConversationThreadPost
                                            {
                                                Body = new ItemBody
                                                {
                                                    Content = "<h1>Welcome to this Business Project</h1>",
                                                    Type = BodyType.Html,
                                                },
                                            }
                                        })
                                }
                    })
            };

            MicrosoftGraphHelper.MakePostRequest(
                String.Format("{0}groups/{1}/conversations",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri, groupId),
                    conversation, "application/json", accessToken);
        }
    }
}