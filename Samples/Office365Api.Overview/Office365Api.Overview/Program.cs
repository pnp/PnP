using System;
using System.Threading.Tasks;

namespace Office365Api.Overview
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {

                //IMPORTANT: using these API's from a console app is not the intended model and as of version 0.1.1.243 this is
                //           not possible anymore (adding connected service to a console app)
                //

                //UPDATE: after update to version 0.1.1.243 of the O365 API this sample always prompts for creds.
                //Use this code to signout. Is needed when the app permissions have changed. 
                //Sometimes the API's throw a random error...preview soft :-) Anyway, calling signout generally
                //fixes the problem
                //var signout = MailApiSample.SignOut();

                PrintHeader("Discovery API demo");
                var t = DiscoveryAPISample.DiscoverMyFiles();
                Task.WaitAll(t);
                PrintSubHeader("Current user information");
                
                //Not returned anymore
                //PrintAttribute("Name", String.Format("{0} {1}", t.Result.UserId IdToken.GivenName, t.Result.IdToken.FamilyName));
                //PrintAttribute("Email", t.Result.IdToken.Email);
                //PrintAttribute("UPN", t.Result.IdToken.UPN);
                //PrintAttribute("TenantID", t.Result.IdToken.TenantId);
                
                PrintAttribute("OneDrive URL", t.Result.ServiceEndpointUri);
                var t3 = DiscoveryAPISample.DiscoverMail();
                Task.WaitAll(t3);
                PrintAttribute("Mail URL", t3.Result.ServiceEndpointUri);

                PrintHeader("Files API demo");

                // Read all files on your onedrive
                PrintSubHeader("List all files and folders in the OneDrive");

                // Pass along the discovery context object
                MyFilesApiSample._discoveryContext = DiscoveryAPISample._discoveryContext;
                MailApiSample._discoveryContext = DiscoveryAPISample._discoveryContext;
                SitesApiSample._discoveryContext = DiscoveryAPISample._discoveryContext;
                ActiveDirectoryApiSample._discoveryContext = DiscoveryAPISample._discoveryContext;

                var allMyFiles = MyFilesApiSample.GetMyFiles();
                Task.WaitAll(allMyFiles);
                foreach (var item in allMyFiles.Result)
                {
                    PrintAttribute("URL", item.Url);
                }

                // upload a file to the "Shared with everyone" folder
                PrintSubHeader("Upload a file to OneDrive");
                var t1 = MyFilesApiSample.UploadFile(@"C:\temp\bulkadusers.xlsx", "Shared with everyone");
                Task.WaitAll(t1);

                // iterate over the "Shared with everyone" folder
                PrintSubHeader("List all files and folders in the Shared with everyone folder");
                var myFiles = MyFilesApiSample.GetMyFiles("Shared with everyone");
                Task.WaitAll(myFiles);
                foreach (var item in myFiles.Result)
                {
                    PrintAttribute("URL", item.Url);
                }

                PrintHeader("Sites API demo");
                //set the SharePointResourceId
                SitesApiSample.ServiceResourceId = "https://bertonline.sharepoint.com";

                //https://bertonline.sharepoint.com/sites/20140050 should work due to the user having read access
                //https://bertonline.sharepoint.com/sites/20140052 should not work due to the user not having access
                //https://bertonline.sharepoint.com/sites/20140053 should not work due to the user being site collection admin
                var mySharePointFiles = SitesApiSample.GetDefaultDocumentFiles("https://bertonline.sharepoint.com/sites/20140053");
                Task.WaitAll(mySharePointFiles);
                foreach (var item in mySharePointFiles.Result)
                {
                    PrintAttribute("URL", item.Url);
                }

                PrintHeader("Mail API demo");
                //Get mail stats
                PrintSubHeader("List mail statistics");
                var mailStats = MailApiSample.GetMailStats();
                Task.WaitAll(mailStats);
                PrintAttribute("Total number of emails", mailStats.Result);

                //Get mails
                PrintSubHeader("Retrieve all mails, print first 10");
                var mails = MailApiSample.GetMessages();
                Task.WaitAll(mails);
                int i = 0;
                foreach (var item in mails.Result)
                {
                    PrintAttribute("From", String.Format("{0} / {1}", item.From != null ? item.From.Address : "", item.Subject));
                    i++;
                    if (i == 10) break;
                }

                //Send mail
                PrintSubHeader("Send a mail");
                var sendMail = MailApiSample.SendMail("bjansen@microsoft.com", "Let's Hack-A-Thon", "This will be <B>fun...</B>");
                Task.WaitAll(sendMail);

                //Create message in drafts folder
                PrintSubHeader("Store a mail in the drafts folder");
                var draftMail = MailApiSample.DraftMail("bjansen@microsoft.com", "Let's Hack-A-Thon", "This will be fun (in draft folder)...");
                Task.WaitAll(sendMail);
                
                PrintHeader("Active Directory API demo");
                PrintSubHeader("Get all users, print first 10");
                var allADUsers = ActiveDirectoryApiSample.GetUsers();
                Task.WaitAll(allADUsers);
                i = 0;
                foreach(var user in allADUsers.Result)
                {
                    PrintAttribute("User", user.UserPrincipalName);
                    i++;
                    if (i == 10) break;
                }

            }
            catch(Exception ex)
            {
                string message = "";
                if (ex is AggregateException)
                {
                    message = ex.InnerException.Message;
                }
                else
                {
                    message = ex.Message;
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(message);
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            Console.WriteLine();
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();

        }

        private static void PrintHeader(string header)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            string line = new string('*', header.Length + 4);
            Console.WriteLine();
            Console.WriteLine(line);
            Console.Write("*");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" "+ header + " ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("*");
            Console.WriteLine(line);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static void PrintSubHeader(string header)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(header);
            Console.ForegroundColor = ConsoleColor.Gray;
        }


        private static void PrintAttribute(string attribute, object attributeValue)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(String.Format("{0}: ", attribute));
            Console.ForegroundColor = ConsoleColor.Gray;
            if (attributeValue != null)
            {
                Console.WriteLine(attributeValue);
            }
            else
            {
                Console.WriteLine();
            }
        }

        private static void PrintAttribute(string attribute)
        {
            PrintAttribute(attribute, null);
        }

    }
}
