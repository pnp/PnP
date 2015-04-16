using Contoso.Provisioning.Hybrid.Contract;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Utilities;
using Microsoft.SharePoint.Client;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Contoso.Provisioning.Hybrid.Core;

namespace Contoso.Provisioning.Hybrid.Web.Pages
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            Uri redirectUrl;
            switch (SharePointContextProvider.CheckRedirectionStatus(Context, out redirectUrl))
            {
                case RedirectionStatus.Ok:
                    return;
                case RedirectionStatus.ShouldRedirect:
                    Response.Redirect(redirectUrl.AbsoluteUri, endResponse: true);
                    break;
                case RedirectionStatus.CanNotRedirect:
                    Response.Write("An error occurred while processing your request.");
                    Response.End();
                    break;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // define initial script, needed to render the chrome control
            string script = @"
            function chromeLoaded() {
                $('body').show();
            }

            //function callback to render chrome after SP.UI.Controls.js loads
            function renderSPChrome() {
                //Set the chrome options for launching Help, Account, and Contact pages
                var options = {
                    'appTitle': document.title,
                    'onCssLoaded': 'chromeLoaded()'
                };

                //Load the Chrome Control in the divSPChrome element of the page
                var chromeNavigation = new SP.UI.Controls.Navigation('divSPChrome', options);
                chromeNavigation.setVisible(true);
            }";

            //register script in page
            Page.ClientScript.RegisterClientScriptBlock(typeof(Default), "BasePageScript", script, true);

            if (!this.IsPostBack)
            {
                // The following code gets the client context and Title property by using TokenHelper.
                // To access other properties, the app may need to request permissions on the host web.
                var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    clientContext.Load(clientContext.Web, web => web.Title
                                                        , web => web.CurrentUser);
                    clientContext.ExecuteQuery();

                    Microsoft.SharePoint.Client.User currentUser = clientContext.Web.CurrentUser;
                    List<SharePointUser> peoplePickerUsers = new List<SharePointUser>(1);
                    peoplePickerUsers.Add(new SharePointUser() { Name = currentUser.Title, Email = currentUser.Email, Login = currentUser.LoginName });
                    hdnAdministrators.Value = JsonUtility.Serialize<List<SharePointUser>>(peoplePickerUsers);
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(Page.Request["SPHostUrl"]);
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            ProcessSiteRequest();
        }

        public string GetNextTempSiteCollectionUrl(string baseSiteUrl)
        {
            return String.Format("{0}{1}", baseSiteUrl, Guid.NewGuid().ToString());
        }

        private void ProcessSiteRequest()
        {
            try
            {
                string generalSiteDirectoryUrl = RoleEnvironment.GetConfigurationSettingValue("General.SiteDirectoryUrl");
                string generalSiteDirectoryListName = RoleEnvironment.GetConfigurationSettingValue("General.SiteDirectoryListName");
                string generalSiteDirectoryProvisioningPage = RoleEnvironment.GetConfigurationSettingValue("General.SiteDirectoryProvisioningPage");
                string generalSiteCollectionUrl = RoleEnvironment.GetConfigurationSettingValue("General.SiteCollectionUrl");
                string generalMailSMTPServer = RoleEnvironment.GetConfigurationSettingValue("General.MailSMTPServer");
                string generalMailUser = RoleEnvironment.GetConfigurationSettingValue("General.MailUser");
                string generalMailUserPassword = RoleEnvironment.GetConfigurationSettingValue("General.MailUserPassword");
                string generalMailSiteRequested = RoleEnvironment.GetConfigurationSettingValue("General.MailSiteRequested");
                string generalEncryptionThumbPrint = RoleEnvironment.GetConfigurationSettingValue("General.EncryptionThumbPrint");

                //Manager initiation
                SiteDirectoryManager siteDirectoryManager = new SiteDirectoryManager();

                //Decrypt mail password
                generalMailUserPassword = EncryptionUtility.Decrypt(generalMailUserPassword, generalEncryptionThumbPrint);

                // SharePoint context for the host web
                var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
                ClientContext hostWebClientContext = spContext.CreateAppOnlyClientContextForSPHost();                

                // Object that contains data about the site collection we're gonna provision
                SharePointProvisioningData siteData = new SharePointProvisioningData();

                siteData.Url = String.Format("{0}{1}", generalSiteCollectionUrl, Guid.NewGuid().ToString());

                // Deal with the Title
                siteData.Title = txtTitle.Text;

                // Deal with the template
                siteData.Template = drlTemplate.SelectedItem.Value;

                // Deal with the data classification
                siteData.DataClassification = drlClassification.SelectedItem.Value;

                // Deal with the site name (empty for root)
                siteData.Name = "";

                // Deal with the site owners
                List<SharePointUser> ownersList = JsonUtility.Deserialize<List<SharePointUser>>(hdnAdministrators.Value);
                SharePointUser[] owners = new SharePointUser[ownersList.Count];
                List<String> mailTo = new List<string>(ownersList.Count);
                string ownerNames = "";
                string ownerAccounts = "";

                int i = 0;
                foreach (SharePointUser owner in ownersList)
                {
                    owner.Login = StripUPN(owner.Login);
                    owners[i] = owner;

                    mailTo.Add(owner.Email);

                    if (ownerNames.Length > 0)
                    {
                        ownerNames = ownerNames + ", ";
                        ownerAccounts = ownerAccounts + ", ";
                    }
                    ownerNames = ownerNames + owner.Name;
                    ownerAccounts = ownerAccounts + owner.Login;

                    i++;
                }
                siteData.Owners = owners;

#if (DEBUG)
                //In debug mode have the WCF call ignore certificate errors
                System.Net.ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) =>
                {
                    return true;
                };
#endif
                // Provision site collection on the 
                using (SharePointProvisioning.SharePointProvisioningServiceClient service = new SharePointProvisioning.SharePointProvisioningServiceClient())
                {
                    if (service.ProvisionSiteCollection(siteData))
                    {
                        string[] ownerLogins = new string[owners.Length];
                        int j = 0;
                        foreach (SharePointUser owner in owners)
                        {
                            ownerLogins[j] = owner.Login;
                            j++;
                        }

                        siteDirectoryManager.AddSiteDirectoryEntry(hostWebClientContext, hostWebClientContext.Web, generalSiteDirectoryUrl, generalSiteDirectoryProvisioningPage, generalSiteDirectoryListName, siteData.Title, siteData.Url, siteData.Template, ownerLogins);

                        string mailBody = String.Format(generalMailSiteRequested, siteData.Title, ownerNames, ownerAccounts);
                        MailUtility.SendEmail(generalMailSMTPServer, generalMailUser, generalMailUserPassword, mailTo, null, "Your SharePoint site request has been registered", mailBody);
                    }
                }

                if (Page.Request["IsDlg"].Equals("0", StringComparison.InvariantCultureIgnoreCase))
                {
                    // redirect to host web home page
                    Response.Redirect(Page.Request["SPHostUrl"]);
                }
                else
                {
                    // refresh the page from which the dialog was opened. Normally this is always the SPHostUrl
                    ClientScript.RegisterStartupScript(typeof(Default), "RedirectToSite", "navigateParent('" + Page.Request["SPHostUrl"] + "');", true);
                }

            }
            catch (Exception ex)
            {
                lblErrors.Text = String.Format("Error: {0} \n\r Stacktrace: {1}", ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// Extracts the user's UPN from the loginid
        /// </summary>
        /// <param name="userid">)Login ID (i:0#.f|membership|kevinc@set1.bertonline.info</param>
        /// <returns>UPN kevinc@set1.bertonline.info</returns>
        private string StripUPN(string userid)
        {
            //input format: i:0#.f|membership|kevinc@set1.bertonline.info
            string[] words = userid.Split('|');
            if (words.Length == 3)
            {
                return words[2];
            }
            else
            {
                return "";
            }
        }
    }
}