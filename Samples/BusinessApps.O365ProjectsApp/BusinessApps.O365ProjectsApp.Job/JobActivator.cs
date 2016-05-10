using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using BusinessApps.O365ProjectsApp.Infrastructure;
using System.IO;
using BusinessApps.O365ProjectsApp.Infrastructure.MicrosoftGraph;

namespace BusinessApps.O365ProjectsApp.Job
{
    public class JobActivator
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage(
            [QueueTrigger(O365ProjectsAppConstants.Blob_Storage_Queue_Name)] GroupCreationInformation groupCreation,
            TextWriter log)
        {
            log.WriteLine(String.Format("Starting Job: {0} - Creating Group: {1}",
                groupCreation.JobId, groupCreation.Name));

            // Convert photo into a MemoryStream
            MemoryStream photoStream = new MemoryStream();
            photoStream.Write(groupCreation.Photo, 0, groupCreation.Photo.Length);
            photoStream.Position = 0;

            // Create the Office 365 Group
            var group = GraphRemoteActions.CreateOffice365Group(
                new Group
                {
                    DisplayName = groupCreation.Name,
                    MailEnabled = true,
                    SecurityEnabled = true,
                    GroupTypes = new List<String>(new String[] { "Unified" }),
                    MailNickname = groupCreation.Name,
                },
                groupCreation.Members,
                photoStream,
                groupCreation.AccessToken);

            // Send the welcome message into the group's conversation
            GraphRemoteActions.SendMessageToGroupConversation(group.Id,
                new Conversation
                {
                    Topic = $"Let's manage the Project {groupCreation.Name}!",
                    Threads = new List<ConversationThread>(
                        new ConversationThread[] {
                                new ConversationThread
                                {
                                    Topic = "We've just created this Business Project",
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
                },
                groupCreation.AccessToken);

            log.WriteLine("Completed Job execution");
        }
    }
}
