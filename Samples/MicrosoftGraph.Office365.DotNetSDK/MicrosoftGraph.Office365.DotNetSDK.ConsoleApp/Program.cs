using Microsoft.Graph;
using MSAL = Microsoft.Identity.Client;
using ADAL = Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

namespace MicrosoftGraph.Office365.DotNetSDK.ConsoleApp
{
    class Program
    {
        static String MSAL_ClientID = ConfigurationManager.AppSettings["MSAL:ClientId"];
        static String MSAL_AccessToken = null;
        static MSAL.PublicClientApplication MSAL_clientApplication =
            new MSAL.PublicClientApplication(MSAL_ClientID);

        static void Main(string[] args)
        {
            UseMSGraphSDKWithMSAL().Wait();
            Console.ReadKey();
        }

        static async Task UseMSGraphSDKWithMSAL()
        {
            GraphServiceClient graphClient = new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    async (requestMessage) =>
                    {
                        // Configure the permissions
                        String[] scopes = {
                        "User.Read",
                        "User.ReadBasic.All",
                        "Mail.Send",
                        "Mail.Read",
                        "Group.ReadWrite.All",
                        "Sites.Read.All",
                        "Directory.AccessAsUser.All",
                        "Files.ReadWrite",
                        };

                        // Acquire an access token for the given scope.
                        MSAL_clientApplication.RedirectUri = "urn:ietf:wg:oauth:2.0:oob";
                        var authenticationResult = await MSAL_clientApplication.AcquireTokenAsync(scopes);

                        // Get back the access token.
                        MSAL_AccessToken = authenticationResult.Token;

                        // Configure the HTTP bearer Authorization Header
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", MSAL_AccessToken);
                    }));

            await ShowMyDisplayName(graphClient);

            await SelectUsers(graphClient);

            await FilterUsers(graphClient);

            await FilterAndOrderUsers(graphClient);

            await PartitionInboxMessages(graphClient);

            await ExpandFiles(graphClient);

            await BrowseUsersPages(graphClient);

            await CreateUnifiedGroup(graphClient);

            await SendMail(graphClient, "paolo@pialorsi.com", "Paolo Pialorsi");

            await ListUnifiedGroups(graphClient);

            await GetGroupFiles(graphClient);

            await SearchGroupFiles(graphClient, "sample");

            await GetGroupConversations(graphClient);

            await GetGroupEvents(graphClient);

            await AddGroupConversationThread(graphClient);

            await AddGroupEvent(graphClient);

            await ManageCurrentUserPicture(graphClient);

            await RetrieveCurrentUserManagerAndReports(graphClient);

            await UploadFileToOneDriveForBusiness(graphClient);

            await SearchForFilesInOneDriveForBusiness(graphClient, "contract");
        }

        private static async Task ShowMyDisplayName(GraphServiceClient graphClient)
        {
            var me = await graphClient.Me.Request().Select("DisplayName").GetAsync();
            Console.WriteLine("Your ID is: {0}", me.DisplayName);
        }

        private static async Task SelectUsers(GraphServiceClient graphClient)
        {
            var users = await graphClient.Users.Request().Select("DisplayName,UserPrincipalName,Mail").GetAsync();
        }

        private static async Task FilterUsers(GraphServiceClient graphClient)
        {
            var filteredUsers = await graphClient.Users.Request()
                .Select("DisplayName,UserPrincipalName,Mail")
                .Filter("department eq 'IT'")
                .GetAsync();
        }

        private static async Task FilterAndOrderUsers(GraphServiceClient graphClient)
        {
            var filteredAndOrderedUsers = await graphClient.Users.Request()
                .Select("displayName,userPrincipalName,mail")
                .OrderBy("displayName")
                .GetAsync();
        }

        private static async Task PartitionInboxMessages(GraphServiceClient graphClient)
        {
            var partitionedMails = await graphClient.Me.MailFolders.Inbox.Messages.Request()
                .Select("subject,from,receivedDateTime")
                .OrderBy("ReceivedDateTime desc")
                .Skip(10)
                .Top(5)
                .GetAsync();
        }

        private static async Task ExpandFiles(GraphServiceClient graphClient)
        {
            var expandedFiles = await graphClient.Me.Drive.Root.Request()
                .Expand("children($select=id,name,createdBy,lastModifiedBy)")
                .Select("id,name,webUrl")
                .GetAsync();
        }

        private static async Task BrowseUsersPages(GraphServiceClient graphClient)
        {
            var pagedUsers = await graphClient.Users
                .Request()
                .Select("id,DisplayName,Mail")
                .GetAsync();
            Int32 pageCount = 0;

            while (true)
            {
                pageCount++;
                Console.WriteLine("Page: {0}", pageCount);
                foreach (var user in pagedUsers)
                {
                    Console.WriteLine("{0} - {1} - {2}", user.Id, user.DisplayName, user.Mail);
                }

                if (pagedUsers.NextPageRequest != null)
                {
                    pagedUsers = await pagedUsers.NextPageRequest.GetAsync();
                }
                else
                {
                    break;
                }
            }
        }

        private static async Task CreateUnifiedGroup(GraphServiceClient graphClient)
        {
            String randomSuffix = Guid.NewGuid().ToString("N");

            // Prepare the group resource object
            Group newGroup = new Group
            {
                DisplayName = "SDK Group " + randomSuffix,
                Description = "This has been created via Graph SDK",
                MailNickname = "sdk-" + randomSuffix,
                MailEnabled = true,
                SecurityEnabled = false,
                GroupTypes = new List<string> { "Unified" },
            };

            Group addedGroup = null;

            try
            {
                // Add the group to the collection of groups
                addedGroup = await graphClient.Groups.Request().AddAsync(newGroup);

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

                        // Upload a new photo for the Group (with retry logic)
                        using (FileStream fs = new FileStream(@"..\..\AppIcon.png", FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            await graphClient.Groups[addedGroup.Id].Photo.Content.Request().PutAsync(fs);
                            break;
                        }
                    }
                    catch
                    {
                        // Ignore any exception, just wait for a while and retry
                    }
                }

            }
            catch (ServiceException ex)
            {
                Console.WriteLine(ex.Error);
                if (ex.IsMatch(GraphErrorCode.AccessDenied.ToString()))
                {
                    Console.WriteLine("Access Denied! Fix permission scopes ...");
                }
                else if (ex.IsMatch(GraphErrorCode.ThrottledRequest.ToString()))
                {
                    Console.WriteLine("Please retry ...");
                }
            }

            // Add owners to the group
            var ownerQuery = await graphClient.Users
                .Request()
                .Filter("userPrincipalName eq 'paolo.pialorsi@sharepoint-camp.com'")
                .GetAsync();
            var owner = ownerQuery.FirstOrDefault();

            if (owner != null)
            {
                try
                {
                    await graphClient.Groups[addedGroup.Id].Owners.References.Request().AddAsync(owner);
                }
                catch (ServiceException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            // Add members to the group
            var memberOneQuery = await graphClient.Users
                .Request()
                .Filter("userPrincipalName eq 'paolo.pialorsi@sharepoint-camp.com'")
                .GetAsync();
            var memberTwoQuery = await graphClient.Users
                .Request()
                .Filter("userPrincipalName eq 'cristian.civera@sharepoint-camp.com'")
                .GetAsync();
            var memberOne = memberOneQuery.FirstOrDefault();
            var memberTwo = memberTwoQuery.FirstOrDefault();

            if (memberOne != null)
            {
                try
                {
                    await graphClient.Groups[addedGroup.Id].Members.References.Request().AddAsync(memberOne);
                }
                catch (ServiceException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            if (memberTwo != null)
            {
                try
                {
                    await graphClient.Groups[addedGroup.Id].Members.References.Request().AddAsync(memberTwo);
                }
                catch (ServiceException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            await UpdateUnifiedGroup(graphClient, addedGroup.Id);

            Console.WriteLine("Press ENTER to delete the just created group!");
            Console.ReadLine();

            await DeleteUnifiedGroup(graphClient, addedGroup.Id);
        }

        private static async Task UpdateUnifiedGroup(GraphServiceClient graphClient, String groupId)
        {
            var groupToUpdate = await graphClient.Groups[groupId]
                .Request()
                .GetAsync();

            groupToUpdate.DisplayName = "SDK Group - Updated!";
            groupToUpdate.Description += " - Updated!";

            try
            {
                var updatedGroup = await graphClient.Groups[groupId]
                    .Request()
                    .UpdateAsync(groupToUpdate);
            }
            catch (ServiceException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static async Task DeleteUnifiedGroup(GraphServiceClient graphClient, String groupId)
        {
            try
            {
                await graphClient.Groups[groupId]
                    .Request()
                    .DeleteAsync();
            }
            catch (ServiceException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static async Task SendMail(GraphServiceClient graphClient,
            String recipientMail, String recipientName)
        {
            try
            {
                await graphClient.Me.SendMail(new Message
                {
                    Subject = "Sent from Graph SDK",
                    Body = new ItemBody
                    {
                        Content = "<h1>Hello from Graph SDK!</h1>",
                        ContentType = BodyType.Html,
                    },
                    ToRecipients = new Recipient[] {
                        new Recipient
                        {
                            EmailAddress = new EmailAddress
                            {
                                Address = recipientMail,
                                Name = recipientName,
                            }
                        }
                    },
                    Importance = Importance.High,
                },
                true).Request().PostAsync();
            }
            catch (ServiceException ex)
            {
                Console.WriteLine(ex.Error);
            }
        }

        private static async Task ListUnifiedGroups(GraphServiceClient graphClient)
        {
            var pagedGroups = await graphClient.Groups
                .Request()
                .Filter("groupTypes/any(grp: grp eq 'Unified')")
                .GetAsync();
            Int32 pageCount = 0;

            while (true)
            {
                pageCount++;
                Console.WriteLine("Page: {0}", pageCount);
                foreach (var group in pagedGroups)
                {
                    Console.WriteLine("{0} - {1} - {2}", group.Id, group.DisplayName, group.Description);
                }

                if (pagedGroups.NextPageRequest != null)
                {
                    pagedGroups = await pagedGroups.NextPageRequest.GetAsync();
                }
                else
                {
                    break;
                }
            }
        }

        private static async Task GetGroupFiles(GraphServiceClient graphClient)
        {
            var unifiedGroups = await graphClient.Groups
                .Request()
                .Filter("groupTypes/any(grp: grp eq 'Unified')")
                .GetAsync();

            var groupDriveItems = await graphClient
                .Groups[unifiedGroups.FirstOrDefault().Id].Drive.Root.Children
                .Request()
                .GetAsync();
            Int32 pageCount = 0;

            while (true)
            {
                pageCount++;
                Console.WriteLine("Page: {0}", pageCount);
                foreach (var driveItem in groupDriveItems)
                {
                    Console.WriteLine("{0} - {1}", driveItem.Id, driveItem.Name);
                }

                if (groupDriveItems.NextPageRequest != null)
                {
                    groupDriveItems = await groupDriveItems.NextPageRequest.GetAsync();
                }
                else
                {
                    break;
                }
            }
        }

        private static async Task SearchGroupFiles(GraphServiceClient graphClient, String searchText)
        {
            var unifiedGroups = await graphClient.Groups
                .Request()
                .Filter("groupTypes/any(grp: grp eq 'Unified')")
                .GetAsync();

            var groupDriveItems = await graphClient
                .Groups[unifiedGroups.FirstOrDefault().Id].Drive.Root.Search(searchText)
                .Request()
                .GetAsync();
            Int32 pageCount = 0;

            while (true)
            {
                pageCount++;
                Console.WriteLine("Page: {0}", pageCount);
                foreach (var driveItem in groupDriveItems)
                {
                    Console.WriteLine("{0} - {1}", driveItem.Id, driveItem.Name);
                }

                if (groupDriveItems.NextPageRequest != null)
                {
                    groupDriveItems = await groupDriveItems.NextPageRequest.GetAsync();
                }
                else
                {
                    break;
                }
            }
        }

        private static async Task GetGroupConversations(GraphServiceClient graphClient)
        {
            var unifiedGroups = await graphClient.Groups
                .Request()
                .Filter("groupTypes/any(grp: grp eq 'Unified')")
                .GetAsync();

            var groupConversations = await graphClient
                .Groups[unifiedGroups.FirstOrDefault().Id].Conversations
                .Request()
                .GetAsync();
            Int32 pageCount = 0;

            while (true)
            {
                pageCount++;
                Console.WriteLine("Page: {0}", pageCount);
                foreach (var c in groupConversations)
                {
                    Console.WriteLine("{0} - {1}", c.Id, c.Topic);
                }

                if (groupConversations.NextPageRequest != null)
                {
                    groupConversations = await groupConversations.NextPageRequest.GetAsync();
                }
                else
                {
                    break;
                }
            }
        }

        private static async Task GetGroupEvents(GraphServiceClient graphClient)
        {
            var unifiedGroups = await graphClient.Groups
                .Request()
                .Filter("groupTypes/any(grp: grp eq 'Unified')")
                .GetAsync();

            var groupEvents = await graphClient
                .Groups[unifiedGroups.FirstOrDefault().Id].Events
                .Request()
                .GetAsync();
            Int32 pageCount = 0;

            while (true)
            {
                pageCount++;
                Console.WriteLine("Page: {0}", pageCount);
                foreach (var e in groupEvents)
                {
                    Console.WriteLine("{0} - {1}", e.Id, e.Subject);
                }

                if (groupEvents.NextPageRequest != null)
                {
                    groupEvents = await groupEvents.NextPageRequest.GetAsync();
                }
                else
                {
                    break;
                }
            }
        }

        private static async Task AddGroupConversationThread(GraphServiceClient graphClient)
        {
            var posts = new ConversationThreadPostsCollectionPage();
            posts.Add(new Post
            {
                Body = new ItemBody
                {
                    Content = "Welcome to this group!",
                    ContentType = BodyType.Text,
                }
            });

            var ct = new ConversationThread
            {
                Topic = "The Microsoft Graph SDK!",
                Posts = posts
            };

            var unifiedGroups = await graphClient.Groups
                .Request()
                .Filter("groupTypes/any(grp: grp eq 'Unified')")
                .GetAsync();

            var groupEvents = await graphClient
                .Groups[unifiedGroups.FirstOrDefault().Id].Threads
                .Request()
                .AddAsync(ct);
        }

        private static async Task AddGroupEvent(GraphServiceClient graphClient)
        {
            var unifiedGroups = await graphClient.Groups
                .Request()
                .Filter("groupTypes/any(grp: grp eq 'Unified')")
                .GetAsync();

            Event evt = new Event
            {
                Subject = "Created with Graph SDK",
                Body = new ItemBody
                {
                    Content = "<h1>Office 365 Party!</h1>",
                    ContentType = BodyType.Html,
                },
                Start = new DateTimeTimeZone
                {
                    DateTime = DateTime.Now.AddDays(1).ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ss"),
                    TimeZone = "UTC",
                },
                End = new DateTimeTimeZone
                {
                    DateTime = DateTime.Now.AddDays(2).ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ss"),
                    TimeZone = "UTC",
                },
                Location = new Location
                {
                    Address = new PhysicalAddress
                    {
                        City = "Redmond",
                        CountryOrRegion = "USA",
                        State = "WA",
                        Street = "Microsft Way",
                        PostalCode = "98052",
                    },
                    DisplayName = "Microsoft Corp. HQ",
                },
                Type = EventType.SingleInstance,
                ShowAs = FreeBusyStatus.Busy,
            };

            var groupEvents = await graphClient
                .Groups[unifiedGroups.FirstOrDefault().Id].Events
                .Request()
                .AddAsync(evt);
        }

        private static async Task ManageCurrentUserPicture(GraphServiceClient graphClient)
        {
            // Get the photo of the current user
            var userPhotoStream = await graphClient.Me.Photo.Content.Request().GetAsync();

            using (FileStream fs = new FileStream(@"..\..\user-photo-original.png", FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                userPhotoStream.CopyTo(fs);
            }

            // Upload a new photo for the current user
            using (FileStream fs = new FileStream(@"..\..\user-photo-two.png", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                try
                {
                    await graphClient.Me.Photo.Content.Request().PutAsync(fs);
                }
                catch (ServiceException ex)
                {
                    Console.WriteLine(ex.Error);
                    if (ex.IsMatch(GraphErrorCode.AccessDenied.ToString()))
                    {
                        Console.WriteLine("Access Denied! Fix permission scopes ...");
                    }
                    else if (ex.IsMatch(GraphErrorCode.ThrottledRequest.ToString()))
                    {
                        Console.WriteLine("Please retry ...");
                    }
                }
            }
        }

        private static async Task RetrieveCurrentUserManagerAndReports(GraphServiceClient graphClient)
        {
            var managerPointer = await graphClient.Me.Manager.Request().GetAsync();
            var manager = await graphClient.Users[managerPointer.Id].Request().Select("DisplayName").GetAsync();
            if (manager != null)
            {
                Console.WriteLine("Your manager is: {0}", manager.DisplayName);
            }

            var reports = await graphClient.Me.DirectReports.Request().GetAsync();

            if (reports.Count > 0)
            {
                Console.WriteLine("Here are your direct reports:");
                foreach (var r in reports)
                {
                    var report = await graphClient.Users[r.Id].Request().Select("DisplayName").GetAsync();
                    Console.WriteLine(report.DisplayName);
                }
            }
            else
            {
                Console.WriteLine("You don't have direct reports!");
            }
        }

        private static async Task UploadFileToOneDriveForBusiness(GraphServiceClient graphClient)
        {
            var newFile = new Microsoft.Graph.DriveItem
            {
                File = new Microsoft.Graph.File(),
                Name = "user-photo-two.png",
            };

            newFile = await graphClient.Me.Drive.Root.Children.Request().AddAsync(newFile);
            using (FileStream fs = new FileStream(@"..\..\user-photo-two.png", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var newFileContent = await graphClient.Me.Drive.Items[newFile.Id].Content.Request().PutAsync<DriveItem>(fs);
            }

            Console.WriteLine("Uploaded file with ID: {0}", newFile.Id);
        }

        private static async Task SearchForFilesInOneDriveForBusiness(GraphServiceClient graphClient, String queryText)
        {
            var searchResults = await graphClient.Me.Drive.Root.Search(queryText).Request().GetAsync();
            Int32 pageCount = 0;

            while (true)
            {
                pageCount++;
                Console.WriteLine("Page: {0}", pageCount);
                foreach (var result in searchResults)
                {
                    Console.WriteLine("{0} - {1}\n{2}\n", result.Id, result.Name, result.WebUrl);
                    await DownloadFileFromOneDriveForBusiness(graphClient, result.Id);
                }

                if (searchResults.NextPageRequest != null)
                {
                    searchResults = await searchResults.NextPageRequest.GetAsync();
                }
                else
                {
                    break;
                }
            }
        }
        private static async Task DownloadFileFromOneDriveForBusiness(GraphServiceClient graphClient, String driveItemId)
        {
            var file = await graphClient.Me.Drive.Items[driveItemId].Request().Select("id,Name").GetAsync();
            var fileContent = await graphClient.Me.Drive.Items[driveItemId].Content.Request().GetAsync();

            using (FileStream fs = new FileStream(@"..\..\" + file.Name, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                fileContent.CopyTo(fs);
            }
        }
    }
}
