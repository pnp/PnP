using Microsoft.Azure.ActiveDirectory.GraphClient;
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
                // get OAuth token using Client Credentials
                string authString = "https://login.windows.net/" + ConfigurationManager.AppSettings["TenantName"];
                AuthenticationContext authenticationContext = new AuthenticationContext(authString, false);

                // Config for OAuth client credentials 
                ClientCredential clientCred = new ClientCredential(ConfigurationManager.AppSettings["AzureADClientId"], ConfigurationManager.AppSettings["AzureADClientSecret"]);
                string resource = "https://graph.windows.net";
                string token = String.Empty;

                // Authenticate
                AuthenticationResult authenticationResult = authenticationContext.AcquireToken(resource, clientCred);
                token = authenticationResult.AccessToken;

                // setup Graph connection
                GraphConnection graphConnection = SetupGraphConnection(token);

                // Check group memberships. Pass along UPN of user and displayname of 
                // the group to be checked. API support checking multiple groups at once
                Test(graphConnection, "kevinc@set1.bertonline.info", "executives");
                Test(graphConnection, "frankm@set1.bertonline.info", "executives");
                Test(graphConnection, "frankm@set1.bertonline.info", "employees");

                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
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
        }

        private static void Test(GraphConnection graphConnection, string userUPN, string groupDisplayName)
        {
            Console.WriteLine("Is user {0} member of group {1}: {2}", userUPN, groupDisplayName, UserIsMemberOfGroup(graphConnection, userUPN, groupDisplayName));
        }

        private static bool UserIsMemberOfGroup(GraphConnection graphConnection, string userUPN, string groupDisplayName)
        {
            // Get the group for which we want to check membership
            FilterGenerator filter = new FilterGenerator();
            Expression filterExpression = ExpressionHelper.CreateEqualsExpression(typeof(Group), GraphProperty.DisplayName, groupDisplayName);
            filter.QueryFilter = filterExpression;
            PagedResults<Group> groupToCheckResults = graphConnection.List<Group>(null, filter);

            if (groupToCheckResults.Results.Count == 1)
            {
                // Add group to our groups to check list
                Group groupToCheck = groupToCheckResults.Results[0] as Group;
                IList<String> groupsList = new List<string>();
                groupsList.Add(groupToCheck.ObjectId);

                // Get the user for which we want to check the group membership
                FilterGenerator userFilter = new FilterGenerator();
                Expression userFilterExpression = ExpressionHelper.CreateEqualsExpression(typeof(User), GraphProperty.UserPrincipalName, userUPN);
                userFilter.QueryFilter = userFilterExpression;
                User retrievedUser = new User();
                PagedResults<User> pagedUserResults = graphConnection.List<User>(null, userFilter);
                if (pagedUserResults.Results.Count == 1)
                {
                    retrievedUser = pagedUserResults.Results[0] as User;

                    // Check if the user belongs to any of the passed groups
                    IList<String> memberships = graphConnection.CheckMemberGroups(retrievedUser, groupsList);

                    // If the passed group is returned back then the user is a member
                    if (memberships.Contains(groupToCheck.ObjectId))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    throw new ArgumentException(String.Format("Group {0} does not exist", groupDisplayName));
                }
            }
            else
            {
                throw new ArgumentException(String.Format("User {0} does not exist", userUPN));
            }
        }

        private static GraphConnection SetupGraphConnection(string accessToken)
        {
            Guid ClientRequestId = Guid.NewGuid();
            GraphSettings graphSettings = new GraphSettings();
            graphSettings.ApiVersion = "2013-11-08";
            graphSettings.GraphDomainName = "graph.windows.net";
            return new GraphConnection(accessToken, ClientRequestId, graphSettings);
        }


    }
}
