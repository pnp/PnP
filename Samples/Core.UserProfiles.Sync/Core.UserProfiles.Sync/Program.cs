using Core.UserProfiles.Sync.XMLObjects;
using GraphApi = Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.UserProfiles;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using System.Linq.Expressions;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using Microsoft.IdentityModel.Claims;
using Newtonsoft.Json;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.Azure.ActiveDirectory.GraphClient.Extensions;
using System.Xml.Linq;
using Core.UserProfiles.Sync.Extensions;

namespace Core.UserProfiles.Sync
{
    class Program
    {
        private const string UserProfilePrefix = "i:0#.f|membership|";

        static void Main(string[] args)
        {
            try
            {
                //read the configuration data and setup tenant sp admin url
                SyncConfiguration configuration = GetSyncConfiguration();
                Uri sharePointAdminUri = new Uri(ConfigurationManager.AppSettings["TenantSharePointAdminUrl"]);

                //query Azure AD for users
                var users = GetUsersFromActiveDirectory(sharePointAdminUri).Result; //run query to AD
                Console.WriteLine("Found " + users.Count + " users...");

                //pass users and populate data
                SetUserProfileDataWithUserContext(sharePointAdminUri, configuration, users);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("Application finished...");

            bool notClose = true;
            bool.TryParse(ConfigurationManager.AppSettings["DoNotCloseConsole"], out notClose);

            if (notClose)
            {
                Console.ReadKey();
            }
            
        }

        /// <summary>
        /// This method constructs an AD Client and gets all users with paging
        /// </summary>
        /// <returns>A list of IUser objects</returns>
        private static async Task<List<GraphApi.IUser>> GetUsersFromActiveDirectory(Uri sharePointAdminUri)
        {
            //get the Active Directory client
            var activeDirectoryClient = AuthenticationHelper.GetActiveDirectoryClientAsApplication(sharePointAdminUri);

            List<GraphApi.IUser> usersList = new List<GraphApi.IUser>();

            //get all users from AD with paging
            IPagedCollection<GraphApi.IUser> pagedCollection = await activeDirectoryClient.Users.ExecuteAsync();

            if (pagedCollection != null)
            {
                do //append pages to the list
                {
                    usersList.AddRange(pagedCollection.CurrentPage.ToList());
                    pagedCollection = await pagedCollection.GetNextPageAsync();
                    //Below condition checks for the last batch of users and adds it to the list. 
                    //Otherwise, the pagedCollection.MorePagesAvailable fails in the while condition and the loop misses the last batch of users.
                    if (!pagedCollection.MorePagesAvailable)
                    {
                        usersList.AddRange(pagedCollection.CurrentPage.ToList());
                    }
                } while (pagedCollection != null && pagedCollection.MorePagesAvailable);
            }
            
            return usersList;
        }

        private static void SetUserProfileDataWithUserContext(Uri sharePointAdminUri, SyncConfiguration configuration, List<GraphApi.IUser> users)
        {
            string tenantAdminLoginName = ConfigurationManager.AppSettings["TenantAdminLogin"];
            string tenantAdminPassword = ConfigurationManager.AppSettings["TenantAdminPassword"];

            using (ClientContext clientContext = new ClientContext(sharePointAdminUri.ToString()))
            {
                //authenticate with SPOCredentials
                SecureString password = new SecureString();
                foreach (char c in tenantAdminPassword.ToCharArray()) password.AppendChar(c);
                clientContext.Credentials = new SharePointOnlineCredentials(tenantAdminLoginName, password);
                clientContext.ExecuteQuery();

                // Get the people manager instance for tenant context
                PeopleManager peopleManager = new PeopleManager(clientContext);

                foreach (GraphApi.User user in users)
                {
                    foreach (Property prop in configuration.Properties)
                    {
                        try
                        {
                            UpdateProperty(peopleManager, user, prop);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }

                    clientContext.ExecuteQueryWithExponentialRetry(10, 30000); //implemented with throttling
                }
            }
        }

        private static void UpdateProperty(PeopleManager peopleManager, GraphApi.User user, Property prop)
        {
            var propertyNewValue = typeof(GraphApi.User).GetProperty(prop.ADAttributeName).GetValue(user);

            if (propertyNewValue != null || prop.WriteIfBlank)
            {
                if (prop.IsMulti)
                {
                    peopleManager.SetMultiValuedProfileProperty(UserProfilePrefix + user.UserPrincipalName,
                        prop.UserProfileAttributeName, new List<string>() { });
                }
                else
                {
                    peopleManager.SetSingleValueProfileProperty(UserProfilePrefix + user.UserPrincipalName,
                        prop.UserProfileAttributeName,
                        propertyNewValue == null ? string.Empty : propertyNewValue.ToString());
                }

                Console.WriteLine("Updated User: {0} Property: {1} New Value: {2}",
                    user.DisplayName, prop.UserProfileAttributeName, propertyNewValue);
            }

            //logic to write only if different
        }

        private static SyncConfiguration GetSyncConfiguration()
        {
            SyncConfiguration configuration = null;
            string path = "PropertyConfiguration.xml";

            XElement xml = XElement.Load(path);

            var xmlSerializer = new XmlSerializer(typeof(SyncConfiguration));
            configuration = (SyncConfiguration)xmlSerializer.Deserialize(xml.CreateReader());

            return configuration;
        }
    }
}
