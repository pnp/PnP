using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Core.Throttling
{
    class Program
    {
        static void Main(string[] args)
        {
            string serverUrl = "<URL>";
            String login = "<USERNAME>";
            String password = "<PASSWORD>";
            string listUrlName = "Shared%20Documents";

            using (var ctx = new ClientContext(serverUrl))
            {
                //Provide account and pwd for connecting to the source
                var passWord = new SecureString();
                foreach (char c in password.ToCharArray()) passWord.AppendChar(c);
                ctx.Credentials = new SharePointOnlineCredentials(login, passWord);

                try
                {
                    int number = 0;
                    // This loop will be executed 1000 times, which will cause throttling to occur
                    while (number < 1000)
                    {
                        // Let's try to create new folder based on Ticks to the given list as an example process
                        var folder = ctx.Site.RootWeb.GetFolderByServerRelativeUrl(listUrlName);
                        ctx.Load(folder);
                        folder = folder.Folders.Add(DateTime.Now.Ticks.ToString());
                        // Extension method for executing query with throttling checks
                        ctx.ExecuteQueryWithExponentialRetry(5, 30000); //5 retries, with a base delay of 10 secs.
                        // Status indication for execution.
                        Console.WriteLine("CSOM request successful.");
                        // For loop handling.
                        number = number + 1;
                    }
                }
                catch (MaximumRetryAttemptedException mex)
                {
                    // Exception handling for the Maximum Retry Attempted
                    Console.WriteLine(mex.Message);
                }
            }
        }
    }
}
