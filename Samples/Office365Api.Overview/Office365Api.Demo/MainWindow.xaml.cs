using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Office365Api.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double scrollViewerHeight = 0.0d;

        //TODO: update these values to make them relevant for your environment
        private string uploadFile = @"C:\temp\bulkadusers.xlsx";
        private string serviceResourceId = "https://bertonline.sharepoint.com";
        //https://bertonline.sharepoint.com/sites/20140050 should work due to the user having read access
        //https://bertonline.sharepoint.com/sites/20140052 should not work due to the user not having access
        //https://bertonline.sharepoint.com/sites/20140053 should not work due to the user being site collection admin
        private string siteUrl = "https://bertonline.sharepoint.com/sites/20140053";
        private string sendMailTo = "bjansen@microsoft.com";

        public MainWindow()
        {
            InitializeComponent();
            txtOutput.Background = Brushes.Black;
        }
        
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PrintHeader("Discovery API demo");
                var t = await DiscoveryAPISample.DiscoverMyFiles();
                PrintSubHeader("Current user information");
                PrintAttribute("OneDrive URL", t.ServiceEndpointUri);

                var t3 = await DiscoveryAPISample.DiscoverMail();
                PrintAttribute("Mail URL", t3.ServiceEndpointUri);

                PrintHeader("Files API demo");
                // Read all files on your onedrive
                PrintSubHeader("List all files and folders in the OneDrive");

                // Pass along the discovery context object
                MyFilesApiSample._discoveryContext = DiscoveryAPISample._discoveryContext;
                MailApiSample._discoveryContext = DiscoveryAPISample._discoveryContext;
                SitesApiSample._discoveryContext = DiscoveryAPISample._discoveryContext;
                ActiveDirectoryApiSample._discoveryContext = DiscoveryAPISample._discoveryContext;

                var allMyFiles = await MyFilesApiSample.GetMyFiles();
                foreach (var item in allMyFiles)
                {
                    PrintAttribute("URL", item.Url);
                }

                // upload a file to the "Shared with everyone" folder
                PrintSubHeader("Upload a file to OneDrive");
                await MyFilesApiSample.UploadFile(uploadFile, "Shared with everyone");

                // iterate over the "Shared with everyone" folder
                PrintSubHeader("List all files and folders in the Shared with everyone folder");
                var myFiles = await MyFilesApiSample.GetMyFiles("Shared with everyone");
                foreach (var item in myFiles)
                {
                    PrintAttribute("URL", item.Url);
                }

                PrintHeader("Sites API demo");
                //set the SharePointResourceId
                SitesApiSample.ServiceResourceId = serviceResourceId;
                var mySharePointFiles = await SitesApiSample.GetDefaultDocumentFiles(siteUrl);
                foreach (var item in mySharePointFiles)
                {
                    PrintAttribute("URL", item.Url);
                }

                PrintHeader("Mail API demo");
                //Get mail stats
                PrintSubHeader("List mail statistics");
                var mailStats = await MailApiSample.GetMailStats();
                PrintAttribute("Total number of emails", mailStats);

                //Get mails
                PrintSubHeader("Retrieve all mails, print first 10");
                var mails = await MailApiSample.GetMessages();
                int i = 0;
                foreach (var item in mails)
                {
                    PrintAttribute("From", String.Format("{0} / {1}", item.From != null ? item.From.Address : "", item.Subject));
                    i++;
                    if (i == 10) break;
                }

                //Send mail
                PrintSubHeader("Send a mail");
                await MailApiSample.SendMail(sendMailTo, "Let's Hack-A-Thon", "This will be <B>fun...</B>");

                //Create message in drafts folder
                PrintSubHeader("Store a mail in the drafts folder");
                await MailApiSample.DraftMail(sendMailTo, "Let's Hack-A-Thon", "This will be fun (in draft folder)...");

                PrintHeader("Active Directory API demo");
                PrintSubHeader("Get all users, print first 10");
                var allADUsers = await ActiveDirectoryApiSample.GetUsers();
                i = 0;
                foreach (var user in allADUsers)
                {
                    PrintAttribute("User", user.UserPrincipalName);
                    i++;
                    if (i == 10) break;
                }

                PrintHeader("All done...");
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

                PrintException(message);
            }
        }

        private void PrintException(string exception)
        {
            txtOutput.Inlines.Add(new Run(string.Format("{0}\r", exception)) { Foreground = Brushes.Red });
        }

        private void PrintHeader(string header)
        {
            txtOutput.Inlines.Add(new Run("\r") { Foreground = Brushes.Gray });
            txtOutput.Inlines.Add(new Run(string.Format("{0}\r", new string('*', header.Length + 4))) { Foreground = Brushes.Green });
            txtOutput.Inlines.Add(new Run(string.Format("{0}", "*")) { Foreground = Brushes.Green });
            txtOutput.Inlines.Add(new Run(string.Format("{0}", " " + header + " ")) { Foreground = Brushes.Gray });
            txtOutput.Inlines.Add(new Run(string.Format("{0}\r", "*")) { Foreground = Brushes.Green });
            txtOutput.Inlines.Add(new Run(string.Format("{0}\r", new string('*', header.Length + 4))) { Foreground = Brushes.Green });
        }

        private void PrintSubHeader(string header)
        {
            txtOutput.Inlines.Add(new Run(string.Format("{0}\r", header)) { Foreground = Brushes.Yellow });
        }

        private void PrintAttribute(string attribute, object attributeValue)
        {
            txtOutput.Inlines.Add(new Run(string.Format("{0}:", attribute)) { Foreground = Brushes.White });
            if (attributeValue != null)
            {
                txtOutput.Inlines.Add(new Run(string.Format("{0}\r", attributeValue)) { Foreground = Brushes.Gray });
            }
            else
            {
                txtOutput.Inlines.Add(new Run("\r") { Foreground = Brushes.Gray });
            }
        }

        private void PrintAttribute(string attribute)
        {
            PrintAttribute(attribute, null);
        }

        private void ScrollViewer_LayoutUpdated(object sender, EventArgs e)
        {
            if (this.scrollViewerOutput.ExtentHeight != scrollViewerHeight)
            {
                this.scrollViewerOutput.ScrollToVerticalOffset(this.scrollViewerOutput.ExtentHeight);
                this.scrollViewerHeight = this.scrollViewerOutput.ExtentHeight;
            }
        }
    }
}
