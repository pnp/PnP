using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Azure.ActiveDirectory.GraphClient.Extensions;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AzureAD.GroupMembership
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //An example of building & authenticating the ActiveDirectoryClient 
                ActiveDirectoryClient azureClient = AzureAdAuthentication.GetActiveDirectoryClientAsApplication();

                //a sample of getting app groups
                GetAllGroups(azureClient);

                //a sample showing how to get users, first 5 in this case
                var users = GetUsers(azureClient);

                //this sample lists all groups for a given user
                foreach (var user in users.Result)
                {
                    GetAllGroupsForUser(azureClient, user);                    
                }

                //this sample checks if a user is in the "All Employees" group
                foreach (var user in users.Result)
                {
                    var member = IsUserMemberOfGroup(azureClient, user, "All Employees");
                    if (!member) Console.WriteLine("User is not in group");
                }
                
                Console.WriteLine("Application finished. Press any key to continue...");
                Console.ReadKey();
            }
            catch (AuthenticationException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Acquiring a token failed with the following error: {0}", ex.Message);
                if (ex.InnerException != null)
                {
                    //You should implement retry and back-off logic per the guidance given here:http://msdn.microsoft.com/en-us/library/dn168916.aspx
                    //InnerException Message will contain the HTTP error status codes mentioned in the link above
                    Console.WriteLine("Error detail: {0}", ex.InnerException.Message);
                }
                Console.ResetColor();
                Console.ReadKey();
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
            }
        }


        /// <summary>
        /// Gets all groups for a given user
        /// </summary>
        /// <param name="azureClient">An authenticated ActiveDirectoryClient</param>
        /// <param name="user">A resolved User object</param>
        private static void GetAllGroupsForUser(ActiveDirectoryClient azureClient, IUser user)
        {
            Console.WriteLine("");
            Console.WriteLine("Listing groups for " + user.DisplayName);
            IUserFetcher retrievedUserFetcher = (User)user;

            //access through the MemberOf collection
            IPagedCollection<IDirectoryObject> pagedCollection = retrievedUserFetcher.MemberOf.ExecuteAsync().Result; 
            do 
            { 
                List<IDirectoryObject> directoryObjects = pagedCollection.CurrentPage.ToList(); 

                foreach (IDirectoryObject directoryObject in directoryObjects) 
                { 
                    if (directoryObject is Group) 
                    { 
                        Group group = directoryObject as Group; 
                        Console.WriteLine(" Group: {0}", group.DisplayName); 
                        //add to parent collection if you need to extract them
                    } 

                    //removed to simplify 
                    //if (directoryObject is DirectoryRole) 
                    //{ 
                    //    DirectoryRole role = directoryObject as DirectoryRole; 
                    //    Console.WriteLine(" Role: {0}  Description: {1}", role.DisplayName, role.Description); 
                    //} 
                } 
                pagedCollection = pagedCollection.GetNextPageAsync().Result; 

            } while (pagedCollection != null && pagedCollection.MorePagesAvailable); 

        }

        /// <summary>
        /// A simple method that resolves the Id of a passed group display name
        /// and checks if the passed user belongs to the group
        /// </summary>
        /// <param name="azureClient">ActiveDirectoryClient object</param>
        /// <param name="user">Azure AD User object</param>
        /// <param name="groupName">The display name of the group</param>
        /// <returns>Yes if the user is a member of the group</returns>
        private static bool IsUserMemberOfGroup(ActiveDirectoryClient azureClient, IUser user, string groupName)
        {
            Console.WriteLine("");
            Console.WriteLine(String.Format("Checking if user {0} is member of '{1}' ", user.DisplayName, groupName));

            //get group id
            var groupId = azureClient.Groups.Where(g=>g.DisplayName == groupName).ExecuteSingleAsync().Result;

            //check if group id is in Users groups
            IUserFetcher retrievedUserFetcher = (User)user;
            var groups = retrievedUserFetcher.CheckMemberGroupsAsync
                (new List<string> { groupId.ObjectId }).Result.ToList();

            if (groups.Count() > 0)
            {
                Console.WriteLine("User is in group " + groupName + " " + groups.FirstOrDefault());
                return true;
            }
            else return false;

        }

        /// <summary>
        /// This method demonstrates how to get all groups for a user with the pager logic
        /// </summary>
        /// <param name="azureClient">ActiveDirectoryClient object</param>
        private static void GetAllGroups(ActiveDirectoryClient azureClient)
        {
            Console.WriteLine("Listing all groups...");

            List<IGroup> groups = new List<IGroup>();
            IPagedCollection<IGroup> pagedCollection = azureClient.Groups.ExecuteAsync().Result;

            if (pagedCollection != null)
            {
                do //append pages to the list
                {
                    groups.AddRange(pagedCollection.CurrentPage.ToList());
                    pagedCollection = pagedCollection.GetNextPageAsync().Result;
                } while (pagedCollection != null && pagedCollection.MorePagesAvailable);
            }

            foreach (var group in groups)
            {
                Console.WriteLine("Group: " + group.DisplayName);
            }
        }

        /// <summary>
        /// This method illustrates how to get a list of users from AD
        /// </summary>
        /// <param name="azureClient">ActiveDirectoryClient object</param>
        /// <returns></returns>
        private static async Task<List<IUser>> GetUsers(ActiveDirectoryClient azureClient)
        {
            Console.WriteLine("");
            Console.WriteLine("Listing top 5 users...");

            List<IUser> users = new List<IUser>();
            IPagedCollection<IUser> pagedCollection = await azureClient.Users.Take(5).ExecuteAsync();

            if (pagedCollection != null)
            {
                do //append pages to the list
                {
                    users.AddRange(pagedCollection.CurrentPage.ToList());
                    pagedCollection = await pagedCollection.GetNextPageAsync();
                } while (pagedCollection != null && pagedCollection.MorePagesAvailable);
            }

            foreach (var user in users)
            {
                Console.WriteLine("User: " + user.DisplayName);
            }

            return users;
        }
    }
}
