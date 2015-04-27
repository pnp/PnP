using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.UserProfiles;
using System.Collections.Generic;
using System.Security;

namespace UserProfile.Manipulation.CSOM.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            SetSingleValueProfileProperty();

            SetMultiValueProfileProperty();
        }

        private static void SetSingleValueProfileProperty()
        {
            //Tenant Admin Details
            string tenantAdministrationUrl = "https://yourtenant-admin.sharepoint.com/";
            string tenantAdminLoginName = "admin@yourtenant.onmicrosoft.com";
            string tenantAdminPassword = "Password";

            //AccountName of the user whos property you want to update.
            //If you want to update properties of multiple users, you can fetch the accountnames through search.
            string UserAccountName = "i:0#.f|membership|anotheruser@yourtenant.onmicrosoft.com";

            using (ClientContext clientContext = new ClientContext(tenantAdministrationUrl))
            {
                SecureString passWord = new SecureString();

                foreach (char c in tenantAdminPassword.ToCharArray()) passWord.AppendChar(c);

                clientContext.Credentials = new SharePointOnlineCredentials(tenantAdminLoginName, passWord);

                // Get the people manager instance for tenant context
                PeopleManager peopleManager = new PeopleManager(clientContext);

                // Update the AboutMe property for the user using account name.
                peopleManager.SetSingleValueProfileProperty(UserAccountName, "AboutMe", "Value updated from CSOM");

                clientContext.ExecuteQuery();
            }
        }

        private static void SetMultiValueProfileProperty()
        {
            //Tenant Admin Details
            string tenantAdministrationUrl = "https://yourtenant-admin.sharepoint.com/";
            string tenantAdminLoginName = "admin@yourtenant.onmicrosoft.com";
            string tenantAdminPassword = "Password";

            //AccountName of the user whos property you want to update.
            //If you want to update properties of multiple users, you can fetch the accountnames through search.
            string UserAccountName = "i:0#.f|membership|anotheruser@yourtenant.onmicrosoft.com";

            using (ClientContext clientContext = new ClientContext(tenantAdministrationUrl))
            {
                SecureString passWord = new SecureString();

                foreach (char c in tenantAdminPassword.ToCharArray()) passWord.AppendChar(c);

                clientContext.Credentials = new SharePointOnlineCredentials(tenantAdminLoginName, passWord);

                // List Multiple values
                List<string> skills = new List<string>() { "SharePoint", "Office 365", "C#", "JavaScript" };

                // Get the people manager instance for tenant context
                PeopleManager peopleManager = new PeopleManager(clientContext);

                // Update the SPS-Skills property for the user using account name from profile.
                peopleManager.SetMultiValuedProfileProperty(UserAccountName, "SPS-Skills", skills);

                clientContext.ExecuteQuery();
            }
        }
    }
}
