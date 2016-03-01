using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Services;
using System.Web.UI;

using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.UserProfiles;
using System.Web.Configuration;
using System.Diagnostics;
using Contoso.Office365.common;


namespace Governance.ChangeOwnershipWeb.Pages
{
    public partial class Default : Page
    {        
        string OldSiteOwner = null;
        string OldSiteOwnerName = null;
        static string supportedDomains = null;
        static string[] supportedDomain = null;
        string source = "SiteCollection.ChangeOwner";
        string log = "Application";
        string TenantAdminUrl = WebConfigurationManager.AppSettings.Get("TenantAdminUrl");
        string siteURL = null;  

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
            if (!EventLog.SourceExists(source))
                EventLog.CreateEventSource(source, log);

            Master.Hdn_Master_PageTitle.Value = "Change Site Collection Ownership";
            Master.Hdn_Master_ShortPageTitle.Value = "Change Ownership";

            if (!Page.IsPostBack)
            {
                Log.LogFileSystem(string.Format("Started changing ownership for Site Collection - {0}  ", Page.Request["SPHostUrl"]));

                supportedDomains = WebConfigurationManager.AppSettings.Get("SupportedDomains");
                supportedDomain = supportedDomains.ToLower().Split(',');
                              
                var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    clientContext.Load(clientContext.Site, siteurl => siteurl.Url);
                    clientContext.ExecuteQuery();
                    EventLog.WriteEntry(source, string.Format("Started changing ownership for Site Collection {0}", clientContext.Site.Url), EventLogEntryType.Information, 7000);
                    Master.Hdn_Master_CurrentSiteUrl.Value = clientContext.Site.Url;

                    clientContext.Load(clientContext.Web, web => web.Title);
                    clientContext.ExecuteQuery();
                    Master.Hdn_Master_CurrentSiteTitle.Value = clientContext.Web.Title;

                    //Only Site Owner or SCA can change
                    clientContext.Load(clientContext.Web, user => user.CurrentUser);
                    clientContext.ExecuteQuery();
                    User currentUser = clientContext.Web.CurrentUser;
                    if (!currentUser.IsSiteAdmin)
                        btnCreate.Enabled = false;

                    string userName = currentUser.Title;
                    Master.Hdn_Master_CurrentUserName.Value = GetCurrentUserName();
                    Master.Hdn_Master_CurrentUserEmail.Value = GetCurrentUserEmail();

                    clientContext.Load(clientContext.Site, user => user.Owner);
                    clientContext.ExecuteQuery();
                    User siteOwner = clientContext.Site.Owner;

                    //Check if Manager is null
                    var currentUserManager = GetCurrentUserManager();
                    if (currentUserManager == null || currentUserManager == string.Empty || currentUserManager == siteOwner.LoginName)
                    {
                        rdbList.Items[1].Enabled = false;
                        rdbList.Items[1].Attributes.Add("style", "color:grey");
                    }
                    
                    siteURL = Page.Request["SPHostUrl"];
                    lblsitename.Text = siteURL;
                    
                    if (siteOwner.Title != "Company Administrator")
                    {                       
                        hyperlinkCurrentOwner.Text = GetOthersUserName(siteOwner);
                        if (!string.IsNullOrEmpty(siteOwner.Email))
                        {
                            hyperlinkCurrentOwner.Text += " (" + GetOthersUserEmail(siteOwner) + ")";
                            hyperlinkCurrentOwner.NavigateUrl = "mailto:" + siteOwner.Email;
                        }
                    }
                    else
                        lblSiteOwner.Text = "Owner not defined, please choose one from below";

                  
                    if (GetCurrentUserEmail() == GetOthersUserEmail(siteOwner))
                    {
                        rdbList.Items[0].Enabled = false;
                        rdbList.Items[0].Attributes.Add("style", "color:grey");
                    }

                    //SCA Link
                    SCALink.HRef = (string.IsNullOrEmpty(siteURL) ? Page.Request["SPHostUrl"] : siteURL) + "/_layouts/15/mngsiteadmin.aspx";
                }
            }
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            //ScriptManager.RegisterStartupScript(this, GetType(), "NoUser1", "alert('" + rdbList.SelectedItem.Value + "');", true);         
            if (rdbList.SelectedIndex == -1)
            {
                //spanChangeOwnerOption.Style.Add("display", "block");                
            }

            else
            {
                siteURL = Page.Request["SPHostUrl"];

                
                try
                {
                    User owner = null;
                    bool isValidDomainAccount = false;                                 
                    var clientContext = GetContext(siteURL);
                    using (clientContext)
                    {

                        if (rdbList.SelectedItem.Value == "myself" || rdbList.SelectedItem.Value == "manager")
                        {
                            owner = clientContext.Web.EnsureUser(txtboxUser.Text);

                            var userSelected = clientContext.Web.EnsureUser(txtboxUser.Text);
                            clientContext.Load(userSelected);
                            clientContext.ExecuteQuery();
                            if (supportedDomain.Any(userSelected.Email.ToLower().Contains))
                                isValidDomainAccount = true;
                        }

                        else if (rdbList.SelectedItem.Value == "sca")
                        {
                            owner = clientContext.Web.EnsureUser(ddlistSCA.SelectedValue);
                            var userSelectedSCA = clientContext.Web.EnsureUser(ddlistSCA.SelectedValue);
                            clientContext.Load(userSelectedSCA);
                            clientContext.ExecuteQuery();
                            if (supportedDomain.Any(userSelectedSCA.Email.ToLower().Contains))
                                isValidDomainAccount = true;
                        }
                 
                        else
                        {
                            List<PeoplePickerUser> users = JsonHelper.Deserialize<List<PeoplePickerUser>>(hdnAdministrators.Value);
                            foreach (var user in users)
                            {
                                owner = clientContext.Web.EnsureUser(user.Name);

                                var userSelectedOther = clientContext.Web.EnsureUser(user.Name);
                                clientContext.Load(userSelectedOther);
                                clientContext.ExecuteQuery();
                                if (supportedDomain.Any(owner.Email.ToLower().Contains))
                                    isValidDomainAccount = true;
                            }


                        }

                        if (isValidDomainAccount)
                        {
                            clientContext.Load(clientContext.Site, user => user.Owner);
                            clientContext.ExecuteQuery();
                            User siteOwner = clientContext.Site.Owner;
                            OldSiteOwnerName = GetOthersUserName(siteOwner);
                            OldSiteOwner = GetOthersUserEmail(siteOwner);

                            clientContext.Site.Owner = owner;
                            clientContext.Site.Owner.Update();
                            clientContext.Load(clientContext.Site.Owner);
                            clientContext.ExecuteQuery();

                            //Explicitely add OldSiteOwner to SCA
                            var oldSiteOwner = clientContext.Web.EnsureUser(OldSiteOwner);
                            clientContext.Load(oldSiteOwner);
                            clientContext.ExecuteQuery();
                            if (!oldSiteOwner.IsSiteAdmin)
                            {
                                oldSiteOwner.IsSiteAdmin = true;
                                oldSiteOwner.Update();
                                clientContext.ExecuteQuery();
                            }                        

                            clientContext.Load(clientContext.Web);
                            clientContext.ExecuteQuery();

                            //Site Collection Administrators
                            List<PeoplePickerUser> peoplePickerSecondaryUsers = new List<PeoplePickerUser>(10);
                            UserCollection userCollection = clientContext.Web.SiteUsers;
                            clientContext.Load(userCollection);
                            clientContext.ExecuteQuery();
                            foreach (User user in userCollection)
                            {
                                if (user.IsSiteAdmin && !user.Title.Contains("Global Admin") && !GetOthersUserName(user).Contains("Company Administrator") && !GetOthersUserName(user).Contains("Service Account") && !GetOthersUserName(user).Contains("spositeadmins") && !GetOthersUserName(user).Contains("SharePoint Service") && supportedDomain.Any(GetOthersUserEmail(user).ToLower().Contains) && GetOthersUserEmail(user) != OldSiteOwner) // Rare case handle - both primary and secondary?
                                {
                                    peoplePickerSecondaryUsers.Add(new PeoplePickerUser() { Name = GetOthersUserName(user), Email = GetOthersUserEmail(user), Login = user.LoginName });
                                }
                            }

                            try
                            {
                                if (!owner.IsSiteAdmin)
                                {
                                    owner.IsSiteAdmin = true;
                                    owner.Update();
                                    clientContext.ExecuteQuery();
                                }

                                //AccessRequestSettings
                                clientContext.Load(clientContext.Web, w => w.RequestAccessEmail);
                                clientContext.ExecuteQuery();
                                clientContext.Web.RequestAccessEmail = clientContext.Site.Owner.Email;
                                clientContext.Web.Update();
                                clientContext.Load(clientContext.Web, w => w.RequestAccessEmail);
                                clientContext.ExecuteQuery();
                            }
                            catch (Exception ex)
                            {
                                ScriptManager.RegisterStartupScript(this, GetType(), "ExceptionAccessRequestSettings", "alert('" + ex.Message + "');", true);
                            }

                            if (OldSiteOwner != owner.Email)
                                SendEmailNotification(clientContext.Web.Url, OldSiteOwnerName, clientContext.Site.Owner.Title, OldSiteOwner, clientContext.Site.Owner.Email, peoplePickerSecondaryUsers);

                            EventLog.WriteEntry(source, string.Format("Completed changing ownership for Site Collection"), EventLogEntryType.Information, 7000);

                            Log.LogFileSystem(string.Format("Changed the ownership from {0} to {1}  ", OldSiteOwner, clientContext.Site.Owner.Email) + DateTime.Now.ToString());
                            Log.LogFileSystem(string.Format("Site Collection Ownership is changed successfully for Site Collection - {0}  ", Page.Request["SPHostUrl"]) + DateTime.Now.ToString());
                            Log.LogFileSystem(string.Empty);

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "successMessage", "alert('Site Collection Ownership is changed successfully!!'); window.location='" +
                                                                 Page.Request["SPHostUrl"] + "/_layouts/15/settings.aspx';", true);

                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "showMessage", "alert('Please choose a user from a supported domain');", true);
                        }
                    }

                }
                catch (System.Threading.ThreadAbortException ex)
                {
                    Response.Redirect((string.IsNullOrEmpty(siteURL) ? Page.Request["SPHostUrl"] : siteURL) + "/_layouts/15/settings.aspx", false);
                }
                catch (Exception ex)
                {
                    Log.LogFileSystem(string.Format("Error Occurred in changing ownership for Site Collection - {0}, error is {1} - {2}  ", (string.IsNullOrEmpty(siteURL) ? Page.Request["SPHostUrl"] : siteURL), ex.Message, ex.StackTrace));
                    Log.LogFileSystem(string.Empty);

                    EventLog.WriteEntry(source, string.Format("Error Occurred in changing ownership for Site Collection, error is {0}", ex.Message), EventLogEntryType.Error, 7001);
                    ScriptManager.RegisterStartupScript(this, GetType(), "Exception", "document.getElementById('spanErrorMsg').style.display = 'block';", true);
                }
            }
        }

        //This webmethod is called by the csom peoplepicker to retrieve search data
        //In a MVC application you can use a Json Action method
        [WebMethod]
        public static string GetPeoplePickerData()
        {
            //peoplepickerhelper will get the needed values from the querrystring, get data from sharepoint, and return a result in Json format
            return PeoplePickerHelper.GetPeoplePickerSearchData();
        }

        protected void btnGetValueByServer_Click(object sender, EventArgs e)
        {
            
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            siteURL = Page.Request["SPHostUrl"];
            EventLog.WriteEntry(source, string.Format("Canceled changing ownership for Site Collection"), EventLogEntryType.Information, 7000);
            Log.LogFileSystem(string.Format("Canceled changing ownership for Site Collection  "));
            Log.LogFileSystem(string.Empty);
            Response.Redirect((string.IsNullOrEmpty(siteURL) ? Page.Request["SPHostUrl"] : siteURL) + "/_layouts/15/settings.aspx", false);
        }

        protected void rdbmyself_CheckedChanged(object sender, EventArgs e)
        {

        }

        protected void SendEmailNotification(string siteURL, string oldOwnerName, string newOwnerName, string oldSiteOwnerEmail, string newSiteOwnerEmail, List<PeoplePickerUser> usersSecondary)
        {
            try
            {
                StringBuilder _admins = new StringBuilder();
                SuccessEmailMessage _message = new SuccessEmailMessage();
                _message.SiteUrl = siteURL;
                _message.OldSiteOwner = oldOwnerName;
                _message.NewSiteOwner = newOwnerName;
                _message.Subject = "Your SharePoint Online site has a new Site Owner";

                _message.To.Add(oldSiteOwnerEmail);
                _message.To.Add(newSiteOwnerEmail);

                foreach (var admin in usersSecondary)
                {
                    _message.Cc.Add(admin.Email);
                    _admins.Append(admin.Name);
                    _admins.Append(" ");
                }
                //_message.SiteAdmin = _admins.ToString();
                EmailHelper.SendSiteOwnerChangeEmail(_message);
            }
            catch (Exception ex)
            {

            }
        }
        protected void rdbList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!rdbList.Items[1].Enabled)
                rdbList.Items[1].Attributes.Add("style", "color:grey");

            if (!rdbList.Items[0].Enabled)
                rdbList.Items[0].Attributes.Add("style", "color:grey");

            ddlistSCA.Enabled = true;
            btnCreate.Enabled = true;
            ScriptManager.RegisterStartupScript(this, GetType(), "NoSCADefault", "document.getElementById('spanChangeOwnerOption').style.display = 'none';", true);

            siteURL = Page.Request["SPHostUrl"];

            if (rdbList.SelectedItem.Value == "myself")
            {
                ddlistSCA.Visible = false;
                txtboxUser.Visible = true;


                var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                //var clientContext = GetContext(siteURL);
                //using (clientContext)
                {
                    clientContext.Load(clientContext.Web, web => web.Title, user => user.CurrentUser);
                    clientContext.ExecuteQuery();
                    Microsoft.SharePoint.Client.User currentUser = clientContext.Web.CurrentUser;

                    //txtboxUser.Text = currentUser.Title;
                    txtboxUser.Text = GetOthersUserName(currentUser);

                    ddSelectedUser.Text = GetOthersUserEmail(currentUser);
                    ddSelectedUser.Enabled = false;
                }
            }

            if (rdbList.SelectedItem.Value == "manager")
            {

                ddlistSCA.Visible = false;
                txtboxUser.Visible = true;

                var currentUserManager = GetCurrentUserManager();
                var clientContext = GetContext(siteURL);
                using (clientContext)
                {
                    var manager = clientContext.Web.EnsureUser(currentUserManager);
                    clientContext.Load(manager);
                    clientContext.ExecuteQuery();
                    txtboxUser.Text = GetOthersUserName(manager); //manager.Title;
                    //txtboxUser.Text = item.Value;
                    ddSelectedUser.Text = GetOthersUserEmail(manager); //manager.Email;
                    ddSelectedUser.Enabled = false;
                }
            }

            if (rdbList.SelectedItem.Value == "sca")
            {
                try
                {
                    ddlistSCA.Visible = true;
                    txtboxUser.Visible = false;
                    //var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
                    if (ddlistSCA.Items.Count <= 0)
                    {
                        var clientContext = GetContext(siteURL);
                        using (clientContext)
                        {
                            clientContext.Load(clientContext.Web);
                            clientContext.ExecuteQuery();
                            lblsitename.Text = clientContext.Web.Url;

                            clientContext.Load(clientContext.Site, user => user.Owner);
                            clientContext.ExecuteQuery();
                            User siteOwner = clientContext.Site.Owner;

                            List<PeoplePickerUser> peoplePickerSecondaryUsers = new List<PeoplePickerUser>(10);
                            UserCollection userCollection = clientContext.Web.SiteUsers;
                            clientContext.Load(userCollection);
                            clientContext.ExecuteQuery();
                            foreach (User user in userCollection)
                            {
                                if (user.IsSiteAdmin && !user.Title.Contains("Global Admin") && !user.Title.Contains("Company Administrator") && !user.Title.Contains("Service Account") && !user.Title.Contains("spositeadmins") && supportedDomain.Any(user.Email.ToLower().Contains) && !user.Title.Contains("SharePoint Service") && user.Email != siteOwner.Email)
                                {
                                    ddlistSCA.Items.Add(new System.Web.UI.WebControls.ListItem(GetOthersUserName(user), GetOthersUserEmail(user)));
                                }
                            }
                            if (ddlistSCA.Items.Count < 1)
                            {
                                ddlistSCA.Enabled = false;
                                btnCreate.Enabled = false;
                                ScriptManager.RegisterStartupScript(this, GetType(), "btnbg", "$('#btnCreate').css('background-color', '#0096D6');", true);
                                ScriptManager.RegisterStartupScript(this, GetType(), "btnfg", "$('#btnCreate').css('foreground-color', '#0096D6');", true);
                                ScriptManager.RegisterStartupScript(this, GetType(), "NoSCA", "document.getElementById('spanChangeOwnerOption').style.display = 'block';", true);
                            }
                            else
                            {
                                User owner = null;
                                owner = clientContext.Web.EnsureUser(ddlistSCA.SelectedValue);
                                var userSelectedSCA = clientContext.Web.EnsureUser(ddlistSCA.SelectedValue);
                                clientContext.Load(userSelectedSCA);
                                clientContext.ExecuteQuery();
                                ddSelectedUser.Text = userSelectedSCA.Email;
                                ddSelectedUser.Enabled = false;
                            }
                        }
                    }
                    else
                    {
                        var clientContext = GetContext(siteURL);
                        using (clientContext)
                        {
                            User owner = null;
                            owner = clientContext.Web.EnsureUser(ddlistSCA.SelectedValue);
                            var userSelectedSCA = clientContext.Web.EnsureUser(ddlistSCA.SelectedValue);
                            clientContext.Load(userSelectedSCA);
                            clientContext.ExecuteQuery();
                            ddSelectedUser.Text = userSelectedSCA.Email;
                            ddSelectedUser.Enabled = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "exceptionMessage", "alert('" + ex.Message + "');", true);
                }
            }

            if (rdbList.SelectedItem.Value == "other")
            {
                txtboxUser.Visible = false;
                ddlistSCA.Visible = false;
                inputAdministrators.Visible = true;
            }

            // Hide loading gif
            img_ScaLoading.Visible = false;
        }

        protected void ddlistSCA_SelectedIndexChanged(object sender, EventArgs e)
        {
            var clientContext = GetContext(siteURL);
            using (clientContext)
            {
                User owner = null;
                owner = clientContext.Web.EnsureUser(ddlistSCA.SelectedValue);
                var userSelectedSCA = clientContext.Web.EnsureUser(ddlistSCA.SelectedValue);
                clientContext.Load(userSelectedSCA);
                clientContext.ExecuteQuery();
                ddSelectedUser.Text = GetOthersUserEmail(userSelectedSCA);
                ddSelectedUser.Enabled = false;               
            }
        }

        protected string GetCurrentUserName()
        {
            var ownerFirstName = string.Empty;
            var ownerLastName = string.Empty;
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    PeopleManager peopleManager = new PeopleManager(clientContext);
                    PersonProperties personProperties = peopleManager.GetMyProperties();
                    clientContext.Load(personProperties);
                    clientContext.ExecuteQuery();
                    foreach (var item in personProperties.UserProfileProperties)
                    {
                        if (item.Key == "FirstName")
                            ownerFirstName = item.Value;

                        else if (item.Key == "LastName")
                            ownerLastName = item.Value;
                    }
                }
            }
            catch (Exception)
            {
            }
            return ownerLastName + ", " + ownerFirstName;
        }

        protected string GetCurrentUserEmail()
        {
            var ownerEmail = string.Empty;
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    PeopleManager peopleManager = new PeopleManager(clientContext);
                    PersonProperties personProperties = peopleManager.GetMyProperties();
                    clientContext.Load(personProperties);
                    clientContext.ExecuteQuery();
                    foreach (var item in personProperties.UserProfileProperties)
                    {
                        if (item.Key == "WorkEmail")
                        {
                            ownerEmail = item.Value;
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return ownerEmail;
        }

        protected string GetCurrentUserManager()
        {
            var ownerEmail = string.Empty;
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    PeopleManager peopleManager = new PeopleManager(clientContext);
                    PersonProperties personProperties = peopleManager.GetMyProperties();
                    clientContext.Load(personProperties);
                    clientContext.ExecuteQuery();
                    foreach (var item in personProperties.UserProfileProperties)
                    {
                        if (item.Key == "Manager")
                        {
                            ownerEmail = item.Value;
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return ownerEmail;
        }

        protected string GetOthersUserName(User user)
        {
            string fullName = string.Empty;
            try
            {
                ClientContext clientContext = GetContext(TenantAdminUrl);
                PeopleManager peopleManager = new PeopleManager(clientContext);
                string[] profilePropertyNames = new string[] { "LastName", "FirstName" };
                UserProfilePropertiesForUser userForUser = new UserProfilePropertiesForUser(clientContext, user.LoginName, profilePropertyNames);
                IEnumerable<string> userProfileProperties = peopleManager.GetUserProfilePropertiesFor(userForUser);
                clientContext.Load(userForUser);
                clientContext.ExecuteQuery();
                fullName = userProfileProperties.ElementAt(0) + ", " + userProfileProperties.ElementAt(1);
            }
            catch (Exception)
            {
            }
            return fullName;
        }

        protected string GetOthersUserEmail(User user)
        {
            string email = string.Empty;
            try
            {
                ClientContext clientContext = GetContext(TenantAdminUrl);
                PeopleManager peopleManager = new PeopleManager(clientContext);
                string[] profilePropertyNames = new string[] { "WorkEmail" };
                UserProfilePropertiesForUser userForUser = new UserProfilePropertiesForUser(clientContext, user.LoginName, profilePropertyNames);
                IEnumerable<string> userProfileProperties = peopleManager.GetUserProfilePropertiesFor(userForUser);
                clientContext.Load(userForUser);
                clientContext.ExecuteQuery();
                email = userProfileProperties.ElementAt(0);
            }
            catch (Exception)
            {
            }
            return email;
        }


        private string GetSPOUserEmail(User user)
        {
            var returnValue = string.IsNullOrEmpty(user.Email) ? user.LoginName.Substring(user.LoginName.LastIndexOf('|') + 1) : user.Email;

            return returnValue.ToLower();
        }

        static ClientContext GetContext(string tenantURL)
        {
            Uri tenantUri = new Uri(tenantURL);
            string adminRealm = TokenHelper.GetRealmFromTargetUrl(tenantUri);
            var adminToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, tenantUri.Authority, adminRealm).AccessToken;
            var clientContextTenant = TokenHelper.GetClientContextWithAccessToken(tenantUri.ToString(), adminToken);
            return clientContextTenant;
        }
    }
}