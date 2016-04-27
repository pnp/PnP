using Microsoft.Online.SharePoint.TenantManagement;
using Microsoft.SharePoint.Client;
using System;
using System.Security;

namespace UserProfile.BatchUpdate.API
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleColor defaultForeground = Console.ForegroundColor;
            // Something like: https://contoso-admin.sharepoint.com
            string tenantAdminUrl = GetInput("Enter the admin URL of your tenant", false, defaultForeground);
            // User name and pwd to login to the tenant
            string userName = GetInput("Enter your user name", false, defaultForeground);
            string pwd = GetInput("Enter your password", true, defaultForeground);
            // File URL to the profile value like: https://contoso.sharepoint.com/Shared%20Documents/sample.txt
            string fileUrl = GetInput("Enter the URL to the file located in your tenant", false, defaultForeground);

            // Get access to source tenant with tenant permissions
            using (var ctx = new ClientContext(tenantAdminUrl))
            {
                //Provide count and pwd for connecting to the source
                var passWord = new SecureString();
                foreach (char c in pwd.ToCharArray()) passWord.AppendChar(c);
                ctx.Credentials = new SharePointOnlineCredentials(userName, passWord);

                // Only to check connection and permission, could be removed
                ctx.Load(ctx.Web);
                ctx.ExecuteQuery();
                string title = ctx.Web.Title;

                // Let's get started on the actual code!!!
                Office365Tenant tenant = new Office365Tenant(ctx);
                ctx.Load(tenant);
                ctx.ExecuteQuery();

                /// /// /// /// /// /// /// /// ///
                /// DO import based on file whcih is already uploaded to tenant
                /// /// /// /// /// /// /// /// ///

                // Type of user identifier ["PrincipleName", "EmailAddress", "CloudId"] 
                // in the User Profile Service.
                // In this case we use email as the identifier at the UPA storage
                ImportProfilePropertiesUserIdType userIdType = 
                            ImportProfilePropertiesUserIdType.Email;

                // Name of user identifier property in the JSON
                var userLookupKey = "IdName";

                var propertyMap = new System.Collections.Generic.Dictionary<string, string>();
                // First one is the file, second is the target at User Profile Service
                // Notice that we have here 2 custom properties in UPA called 'City' and 'Office'
                propertyMap.Add("Property1", "City");
                propertyMap.Add("Property2", "Office");

                // Returns a GUID, which can be used to see the status of the execution and end results
                var workItemId = tenant.QueueImportProfileProperties(
                                        userIdType, userLookupKey, propertyMap, fileUrl
                                        );

                ctx.ExecuteQuery();

                /// /// /// /// /// /// /// /// /// /// 
                // CALL CHECK STATUS in OWN method with the received GUID
                /// /// /// /// /// /// /// /// /// /// 
                CheckStatusOfRequestedProcess(ctx, workItemId.Value);

                // Just to pause and indicate that it's all done
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\n-----------------------------------------");
                Console.WriteLine("We are all done. Press enter to continue.");
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Demonstrates how to check status for specific submission or for all submissions
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="workItemId"></param>
        private static void CheckStatusOfRequestedProcess(ClientContext ctx, Guid workItemId)
        {
            ///
            /// CHECK STATUS of the property job with GUID - notice that there's additional logs in the folder as well
            ///
            
            // Check status of specific request based on received GUID
            Office365Tenant tenant = new Office365Tenant(ctx);
            var job = tenant.GetImportProfilePropertyJob(workItemId);
            ctx.Load(job);
            ctx.ExecuteQuery();
            Console.Write("\n--\n");
            Console.WriteLine(string.Format("ID: {0} - Request status: {1} - Error status: {2}",
                                  job.JobId, job.State.ToString(), job.Error.ToString()));
            Console.Write("\n--\n");

            /// 
            /// Get list of all jobs in the tenant
            /// 
            var jobs = tenant.GetImportProfilePropertyJobs();
            ctx.Load(jobs);
            ctx.ExecuteQuery();
            foreach (var item in jobs)
            {
                Console.WriteLine(string.Format("ID: {0} - Request status: {1} - Error status: {2}",
                                   item.JobId, item.State.ToString(), item.Error.ToString()));
            }
        }

        private static string GetInput(string label, bool isPassword, ConsoleColor defaultForeground)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0} : ", label);
            Console.ForegroundColor = defaultForeground;

            string value = "";

            for (ConsoleKeyInfo keyInfo = Console.ReadKey(true); keyInfo.Key != ConsoleKey.Enter; keyInfo = Console.ReadKey(true))
            {
                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (value.Length > 0)
                    {
                        value = value.Remove(value.Length - 1);
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        Console.Write(" ");
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    }
                }
                else if (keyInfo.Key != ConsoleKey.Enter)
                {
                    if (isPassword)
                    {
                        Console.Write("*");
                    }
                    else
                    {
                        Console.Write(keyInfo.KeyChar);
                    }
                    value += keyInfo.KeyChar;

                }

            }
            Console.WriteLine("");

            return value;
        }
    }
}
