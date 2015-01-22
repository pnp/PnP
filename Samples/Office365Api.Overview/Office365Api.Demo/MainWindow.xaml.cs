using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Office365Api.Helpers;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Office365Api.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double scrollViewerHeight = 0.0d;

        public MainWindow()
        {
            InitializeComponent();
            txtOutput.Background = Brushes.Black;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Uri authorityUri;
            Uri sharePointTenantUri;
            Uri siteCollectionUri;

            Regex mailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            // Input parameters sanity check 
            if ((String.IsNullOrEmpty(this.Authority.Text) || !Uri.TryCreate(this.Authority.Text, UriKind.Absolute, out authorityUri)) ||
                (String.IsNullOrEmpty(this.SharePointTenantUri.Text) || !Uri.TryCreate(this.SharePointTenantUri.Text, UriKind.Absolute, out sharePointTenantUri)) ||
                (String.IsNullOrEmpty(this.SiteCollectionUri.Text) || !Uri.TryCreate(this.SiteCollectionUri.Text, UriKind.Absolute, out siteCollectionUri)) ||
                (String.IsNullOrEmpty(this.MailAddressTo.Text) || !mailRegex.IsMatch(this.MailAddressTo.Text)) ||
                (String.IsNullOrEmpty(this.FileToUploadPath.Text) || !File.Exists(this.FileToUploadPath.Text)))
            {
                MessageBoxResult msgBoxResult = MessageBox.Show("Please fill all the input parameters!");
                return;
            }

            try
            {
                PrintHeader("Authentication Phase");
                AuthenticationHelper authenticationHelper = new AuthenticationHelper();
                authenticationHelper.EnsureAuthenticationContext(this.Authority.Text);

                PrintHeader("Discovery API demo");
                DiscoveryHelper discoveryHelper = new DiscoveryHelper(authenticationHelper);
                var t = await discoveryHelper.DiscoverMyFiles();
                PrintSubHeader("Current user information");
                PrintAttribute("OneDrive URL", t.ServiceEndpointUri);

                var t3 = await discoveryHelper.DiscoverMail();
                PrintAttribute("Mail URL", t3.ServiceEndpointUri);

                PrintHeader("Files API demo");
                // Read all files on your onedrive
                PrintSubHeader("List TOP 20 files and folders in the OneDrive");

                MyFilesHelper myFilesHelper = new MyFilesHelper(authenticationHelper);
                var allMyFolders = await myFilesHelper.GetMyFolders();

                var allMyFiles = await myFilesHelper.GetMyFiles();
                foreach (var item in allMyFiles.Take(20))
                {
                    PrintAttribute("URL", item.WebUrl);
                }

                // Upload a file to the "Shared with everyone" folder
                PrintSubHeader("Upload a file to OneDrive");
                if (allMyFolders.Any())
                {
                    await myFilesHelper.UploadFile(this.FileToUploadPath.Text, allMyFolders.First().Id);
                }
                else
                {
                    await myFilesHelper.UploadFile(this.FileToUploadPath.Text);
                }
                // Shared with everyone

                // Iterate over the "Shared with everyone" folder
                PrintSubHeader("List all files and folders in the Shared with everyone folder");
                var myFiles = await myFilesHelper.GetMyFiles(allMyFolders.First().Id);
                foreach (var item in myFiles)
                {
                    PrintAttribute("URL", item.WebUrl);
                }

                PrintHeader("Mail API demo");

                //Get mails
                PrintSubHeader("Retrieve mails from INBOX");
                MailHelper mailHelper = new MailHelper(authenticationHelper);
                var mails = await mailHelper.GetMessages();
                PrintSubHeader(String.Format("Printing TOP 10 mails of {0}", mails.Count()));
                foreach (var item in mails.Take(10))
                {
                    PrintAttribute("From ", String.Format("{0} / {1}", item.From != null ? item.From.EmailAddress.Address : "", item.Subject));
                }

                //Send mail
                PrintSubHeader("Send a mail");
                await mailHelper.SendMail(this.MailAddressTo.Text, "Let's Hack-A-Thon - Office365Api.Demo", "This will be <B>fun...</B>");

                //Create message in drafts folder
                PrintSubHeader("Store a mail in the drafts folder");
                await mailHelper.DraftMail(this.MailAddressTo.Text, "Let's Hack-A-Thon - Office365Api.Demo", "This will be fun (in draft folder)...");

                PrintHeader("Active Directory API demo");
                ActiveDirectoryHelper activeDirectoryHelper = new ActiveDirectoryHelper(authenticationHelper);
                var allADUsers = await activeDirectoryHelper.GetUsers();
                PrintSubHeader(String.Format("Printing TOP 10 users of {0}", allADUsers.Count()));
                foreach (var user in allADUsers.Take(10))
                {
                    PrintAttribute("User", user.UserPrincipalName);
                }

                PrintHeader("All done...");
            }
            catch (Exception ex)
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
            txtOutput.Inlines.Add(new Run(string.Format("{0}: ", attribute)) { Foreground = Brushes.White });
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

        private void BrowseFileToUploadPath_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            Nullable<Boolean> fileDialogResult = fileDialog.ShowDialog();


            // Get the file if any
            if (fileDialogResult == true)
            {
                this.FileToUploadPath.Text = fileDialog.FileName;
            }
        }
    }
}
