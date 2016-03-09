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
        string source = "SiteCollection.ChangeOwner";
        string log = "Application";
        string TenantAdminUrl = WebConfigurationManager.AppSettings.Get("TenantAdminUrl");

        string _oldSiteOwnerName = null;
        string _oldSiteOwnerEmail = null;
        string siteURL = null;
        static string _unLicensedOwner = "Company Administrator";

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
                Log.LogFileSystem(string.Format("Started changing ownership for Site Collection - {0}  ", GetCurrentSiteCollectionURL()));

                // prefill people pickers with current user
                var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    clientContext.Load(clientContext.Site, siteurl => siteurl.Url);
                    clientContext.ExecuteQuery();

                    EventLog.WriteEntry(source, string.Format("Started changing ownership for Site Collection {0}", GetCurrentSiteCollectionURL()), EventLogEntryType.Information, 7000);
                    Master.Hdn_Master_CurrentSiteUrl.Value = GetCurrentSiteCollectionURL();

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

                    //Site Owner
                    clientContext.Load(clientContext.Web);
                    clientContext.ExecuteQuery();
                    siteURL = GetCurrentSiteCollectionURL();
                    lblsitename.Text = siteURL;

                    clientContext.Load(clientContext.Site, user => user.Owner);
                    clientContext.ExecuteQuery();
                    User siteOwner = clientContext.Site.Owner;

                    _oldSiteOwnerName = GetOthersUserName(siteOwner);
                    _oldSiteOwnerEmail = GetOthersUserEmail(siteOwner);

                    if (siteOwner.Title != _unLicensedOwner)
                    {
                        lblSiteOwner.Text = _oldSiteOwnerName;
                        var email = _oldSiteOwnerEmail;
                        if (!string.IsNullOrEmpty(email))
                        {
                            hyperlinkSiteOwnerEmail.Text = email;
                            hyperlinkSiteOwnerEmail.NavigateUrl = "mailto:" + _oldSiteOwnerEmail;
                        }
                    }
                    else
                    {
                        lblSiteOwner.Text = "Owner not defined, please choose one from below";
                    }

                    if (GetCurrentUserEmail() == _oldSiteOwnerEmail)
                    {
                        rdbList.Items[0].Enabled = false;
                        rdbList.Items[0].Attributes.Add("style", "color:grey");
                    }

                    //Check if Manager is null
                    var currentUserManager = GetCurrentUserManager();
                    if (currentUserManager == null || currentUserManager == string.Empty || currentUserManager == siteOwner.LoginName)
                    {
                        rdbList.Items[1].Enabled = false;
                        rdbList.Items[1].Attributes.Add("style", "color:grey");
                    }

                    //SCA Link
                    SCALink.HRef = (string.IsNullOrEmpty(siteURL) ? GetCurrentSiteCollectionURL() : siteURL) + "/_layouts/15/mngsiteadmin.aspx";
                }
            }
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            if (rdbList.SelectedIndex == -1)
            {

            }

            else
            {
                siteURL = GetCurrentSiteCollectionURL();
                try
                {
                    User owner = null;
                    bool isValidDomainAccount = false;
                    using (var clientContext = GetContext(siteURL))
                    {
                        if (rdbList.SelectedItem.Value == "myself" || rdbList.SelectedItem.Value == "manager")
                        {
                            owner = clientContext.Web.EnsureUser(ddSelectedUser.Text);
                            var userSelected = clientContext.Web.EnsureUser(ddSelectedUser.Text);

                            clientContext.Load(userSelected);
                            clientContext.ExecuteQuery();
                            if (IsUserInSupportedDomain(userSelected.Email))
                                isValidDomainAccount = true;
                        }

                        else if (rdbList.SelectedItem.Value == "sca")
                        {
                            owner = clientContext.Web.EnsureUser(ddlistSCA.SelectedValue);
                            var userSelectedSCA = clientContext.Web.EnsureUser(ddlistSCA.SelectedValue);
                            clientContext.Load(userSelectedSCA);
                            clientContext.ExecuteQuery();
                            if (IsUserInSupportedDomain(userSelectedSCA.Email))
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
                                if (IsUserInSupportedDomain(owner.Email))
                                    isValidDomainAccount = true;
                            }


                        }

                        if (isValidDomainAccount)
                        {
                            clientContext.Load(clientContext.Site, user => user.Owner);
                            clientContext.ExecuteQuery();
                            User siteOwner = clientContext.Site.Owner;
                            _oldSiteOwnerName = GetOthersUserName(siteOwner);
                            _oldSiteOwnerEmail = GetOthersUserEmail(siteOwner);

                            clientContext.Site.Owner = owner;
                            clientContext.Site.Owner.Update();
                            clientContext.Load(clientContext.Site.Owner);
                            clientContext.ExecuteQuery();

                            //Explicitely add OldSiteOwner to SCA
                            var oldSiteOwner = clientContext.Web.EnsureUser(_oldSiteOwnerEmail);
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
                            List<PeoplePickerUser> peoplePickerSecondaryUsers = new List<PeoplePickerUser>();
                            UserCollection userCollection = clientContext.Web.SiteUsers;
                            clientContext.Load(userCollection);
                            clientContext.ExecuteQuery();

                            ClientContext ctxTenant = GetContext(WebConfigurationManager.AppSettings.Get("TenantAdminUrl"));
                            bool siteOwnerFound = false;
                            foreach (User user in userCollection)
                            {
                                if (IsRealUser(user, ctxTenant))
                                {
                                    if (IsUserSiteAdmin(user))
                                    {
                                        if (!siteOwnerFound)
                                        {
                                            if (IsUserTheSiteOwner(user, _oldSiteOwnerEmail))
                                            {
                                                siteOwnerFound = true;
                                            }
                                            else
                                                peoplePickerSecondaryUsers.Add(new PeoplePickerUser
                                                {
                                                    Name = GetOthersUserName(user),
                                                    Email = GetOthersUserEmail(user),
                                                    Login = user.LoginName
                                                });
                                        }
                                        else
                                            peoplePickerSecondaryUsers.Add(new PeoplePickerUser
                                            {
                                                Name = GetOthersUserName(user),
                                                Email = GetOthersUserEmail(user),
                                                Login = user.LoginName
                                            });

                                        //if (!IsUserTheSiteOwner(user, _oldSiteOwnerEmail))
                                        //{
                                        //    peoplePickerSecondaryUsers.Add(new PeoplePickerUser
                                        //    {
                                        //        Name = GetOthersUserName(user),
                                        //        Email = GetOthersUserEmail(user),
                                        //        Login = user.LoginName
                                        //    });
                                        //}
                                    }
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
                                Log.LogFileSystem(string.Format("Error happened in setting access request for Site Collection - {0}  ", (string.IsNullOrEmpty(siteURL) ? GetCurrentSiteCollectionURL() : siteURL)));
                            }

                            if (_oldSiteOwnerEmail != owner.Email)
                                SendEmailNotification(GetCurrentSiteCollectionURL(), _oldSiteOwnerName, clientContext.Site.Owner.Title, _oldSiteOwnerEmail, clientContext.Site.Owner.Email, peoplePickerSecondaryUsers);

                            EventLog.WriteEntry(source, string.Format("Completed changing ownership for Site Collection"), EventLogEntryType.Information, 7000);

                            Log.LogFileSystem(string.Format("Changed the ownership from {0} to {1}  ", _oldSiteOwnerEmail, clientContext.Site.Owner.Email));
                            Log.LogFileSystem(string.Format("Site Collection Ownership is changed successfully for Site Collection - {0}  ", (string.IsNullOrEmpty(siteURL) ? GetCurrentSiteCollectionURL() : siteURL)));
                            Log.LogFileSystem(string.Empty);

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "successMessage", "alert('Site Collection Ownership is changed successfully!!'); window.location='" +
                                                                 (string.IsNullOrEmpty(siteURL) ? GetCurrentSiteCollectionURL() : siteURL) + "/_layouts/15/settings.aspx';", true);

                        }
                    }

                }
                catch (System.Threading.ThreadAbortException ex)
                {
                    Response.Redirect((string.IsNullOrEmpty(siteURL) ? GetCurrentSiteCollectionURL() : siteURL) + "/_layouts/15/settings.aspx", false);
                }
                catch (Exception ex)
                {
                    Log.LogFileSystem(string.Format("Error Occurred in changing ownership for Site Collection - {0}, error is {1} - {2}  ", (string.IsNullOrEmpty(siteURL) ? GetCurrentSiteCollectionURL() : siteURL), ex.Message, ex.StackTrace));
                    Log.LogFileSystem(string.Empty);

                    EventLog.WriteEntry(source, string.Format("Error Occurred in changing ownership for Site Collection, error is {0}", ex.Message), EventLogEntryType.Error, 7001);
                    ScriptManager.RegisterStartupScript(this, GetType(), "Exception", "document.getElementById('spanErrorMsg').style.display = 'block';", true);
                }
            }
        }

        //This webmethod is called by the csom peoplepicker to retrieve search data
        [WebMethod]
        public static string GetPeoplePickerData()
        {
            //peoplepickerhelper will get the needed values from the querrystring, get data from sharepoint, and return a result in Json format
            return PeoplePickerHelper.GetPeoplePickerSearchData();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            siteURL = GetCurrentSiteCollectionURL();
            EventLog.WriteEntry(source, string.Format("Canceled changing ownership for Site Collection"), EventLogEntryType.Information, 7000);
            Log.LogFileSystem(string.Format("Canceled changing ownership for Site Collection  "));
            Log.LogFileSystem(string.Empty);
            Response.Redirect((string.IsNullOrEmpty(siteURL) ? GetCurrentSiteCollectionURL() : siteURL) + "/_layouts/15/settings.aspx", false);
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
            //var spanId = spanChangeOwnerOption.ID.ToString();
            ScriptManager.RegisterStartupScript(this, GetType(), "NoSCADefault", "document.getElementById('spanChangeOwnerOption').style.display = 'none';", true);

            siteURL = GetCurrentSiteCollectionURL();

            if (rdbList.SelectedItem.Value == "myself")
            {
                ddlistSCA.Visible = false;
                txtboxUser.Visible = true;

                var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    //clientContext.Load(clientContext.Web, web => web.Title, user => user.CurrentUser);
                    //clientContext.ExecuteQuery();
                    //User currentUser = clientContext.Web.CurrentUser;
                    //txtboxUser.Text = GetOthersUserName(currentUser);              
                    //ddSelectedUser.Text = GetOthersUserEmail(currentUser);
                    txtboxUser.Text = GetCurrentUserName();
                    ddSelectedUser.Text = GetCurrentUserEmail();
                    ddSelectedUser.Enabled = false;
                }
            }

            if (rdbList.SelectedItem.Value == "manager")
            {
                ddlistSCA.Visible = false;
                txtboxUser.Visible = true;

                var currentUserManager = GetCurrentUserManager();
                using (var clientContext = GetContext(siteURL))
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
                    if (ddlistSCA.Items.Count <= 0)
                    {
                        using (var clientContext = GetContext(siteURL))
                        {
                            clientContext.Load(clientContext.Web);
                            clientContext.ExecuteQuery();
                            lblsitename.Text = clientContext.Web.Url;

                            clientContext.Load(clientContext.Site, user => user.Owner);
                            clientContext.ExecuteQuery();
                            User siteOwner = clientContext.Site.Owner;

                            _oldSiteOwnerEmail = GetOthersUserEmail(siteOwner);

                            List<PeoplePickerUser> peoplePickerSecondaryUsers = new List<PeoplePickerUser>();
                            UserCollection userCollection = clientContext.Web.SiteUsers;
                            clientContext.Load(userCollection);
                            clientContext.ExecuteQuery();

                            ClientContext ctxTenant = GetContext(WebConfigurationManager.AppSettings.Get("TenantAdminUrl"));
                            bool siteOwnerFound = false;
                            foreach (User user in userCollection)
                            {
                                if (IsRealUser(user, ctxTenant))
                                {
                                    if (IsUserSiteAdmin(user))
                                    {
                                        if (!siteOwnerFound)
                                        {
                                            if (IsUserTheSiteOwner(user, _oldSiteOwnerEmail))
                                            {
                                                siteOwnerFound = true;
                                            }
                                            else
                                                ddlistSCA.Items.Add(new System.Web.UI.WebControls.ListItem(GetOthersUserName(user), GetOthersUserEmail(user)));
                                        }
                                        else
                                            ddlistSCA.Items.Add(new System.Web.UI.WebControls.ListItem(GetOthersUserName(user), GetOthersUserEmail(user)));

                                        //if (!IsUserTheSiteOwner(user, _oldSiteOwnerEmail))
                                        //{
                                        //    ddlistSCA.Items.Add(new System.Web.UI.WebControls.ListItem(GetOthersUserName(user), GetOthersUserEmail(user)));
                                        //}
                                    }
                                }
                            }
                            if (ddlistSCA.Items.Count < 1)
                            {
                                ddlistSCA.Enabled = false;
                                btnCreate.Enabled = false;
                                ScriptManager.RegisterStartupScript(this, GetType(), "NoSCA", "document.getElementById('spanChangeOwnerOption').style.display = 'block';", true);
                                ddSelectedUser.Text = "";
                                ddSelectedUser.Enabled = false;
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
                        using (var clientContext = GetContext(siteURL))
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
                    Log.LogFileSystem(string.Format("Error occurred for Site Collection {0} in getting site collection administrators.  Error is {1} ", (string.IsNullOrEmpty(siteURL) ? GetCurrentSiteCollectionURL() : siteURL), ex.Message));
                }
            }

            if (rdbList.SelectedItem.Value == "other")
            {
                //divAdministrators.Style.Add("display", "visible");
                txtboxUser.Visible = false;
                ddlistSCA.Visible = false;
                inputAdministrators.Visible = true;
            }

            // Hide loading gif
            img_ScaLoading.Visible = false;
        }

        protected void ddlistSCA_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                siteURL = GetCurrentSiteCollectionURL();
                using (var clientContext = GetContext(siteURL))
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
            catch (Exception ex)
            {
                Log.LogFileSystem(string.Format("Error occurred for Site Collection {0} in choosing a site collection administrator.  Error is {1} ", (string.IsNullOrEmpty(siteURL) ? GetCurrentSiteCollectionURL() : siteURL), ex.Message));
            }
        }

        private static ClientContext GetContext(string URL)
        {
            Uri tenantUri = new Uri(URL);
            string adminRealm = TokenHelper.GetRealmFromTargetUrl(tenantUri);
            var adminToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, tenantUri.Authority, adminRealm).AccessToken;
            var clientContextTenant = TokenHelper.GetClientContextWithAccessToken(tenantUri.ToString(), adminToken);
            return clientContextTenant;
        }

        private string GetCurrentSiteCollectionURL()
        {
            return Page.Request["SPHostUrl"];
        }

        /// <summary>
        /// Get email address - Check if the user is not a service account, search, or any admin account - These account won't have valid email address
        /// </summary>
        /// <param name="user">user</param>
        /// <returns></returns>
        private bool IsRealUser(User user, ClientContext ctxTenant)
        {
            bool isRealUser = false;
            try
            {
                //ClientContext ctxTenant = GetContext(WebConfigurationManager.AppSettings.Get("TenantAdminUrl"));
                using (ctxTenant)
                {
                    PeopleManager peopleManager = new PeopleManager(ctxTenant);
                    string[] profilePropertyNames = new string[] { "WorkEmail" };
                    UserProfilePropertiesForUser userForUser = new UserProfilePropertiesForUser(ctxTenant, user.LoginName, profilePropertyNames);
                    IEnumerable<string> userProfileProperties = peopleManager.GetUserProfilePropertiesFor(userForUser);
                    ctxTenant.Load(userForUser);
                    ctxTenant.ExecuteQuery();
                    if (userProfileProperties.Count() == 1)
                    {
                        if (userProfileProperties.ElementAt(0) != "") //app@sharepoint, search crawl account..
                        {
                            if (IsUserInSupportedDomain(userProfileProperties.ElementAt(0)))
                                isRealUser = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogFileSystem(string.Format("Error happened in validating if the user is real - {0}", ex.Message) + DateTime.Now.ToString());
            }
            return isRealUser;
        }

        /// <summary>
        /// Check if the given user is a site collection administrator
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private bool IsUserSiteAdmin(User user)
        {
            bool isUserSiteAdmin = false;
            try
            {
                if (user.IsSiteAdmin)
                    isUserSiteAdmin = true;
            }
            catch (Exception ex)
            {
                Log.LogFileSystem(string.Format("Error happened in checking if the user is a site admin - {0}", ex.Message) + DateTime.Now.ToString());
            }
            return isUserSiteAdmin;
        }

        /// <summary>
        /// Check if the given user is current site owner
        /// </summary>
        /// <param name="user"></param>
        /// <param name="oldSiteOwner"></param>
        /// <returns></returns>
        private bool IsUserTheSiteOwner(User user, string oldSiteOwner)
        {
            bool isUserTheSiteOwner = false;
            try
            {
                if (GetOthersUserEmail(user) == oldSiteOwner)
                    isUserTheSiteOwner = true;
            }
            catch (Exception ex)
            {
                Log.LogFileSystem(string.Format("Error happened in checking if the user is same as site owner - {0}", ex.Message) + DateTime.Now.ToString());
            }
            return isUserTheSiteOwner;
        }

        /// <summary>
        /// Check if the user's email belongs to supported domain defined in config file
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        private bool IsUserInSupportedDomain(string userEmail)
        {
            bool isUserInSupportedDomain = false;
            string supportedDomains = WebConfigurationManager.AppSettings.Get("SupportedDomains");
            string[] supportedDomain = supportedDomains.ToLower().Split(',');
            try
            {
                if (supportedDomain.Any(userEmail.ToLower().Contains))
                    isUserInSupportedDomain = true;
            }
            catch (Exception ex)
            {
                Log.LogFileSystem(string.Format("Error happened in checking if the user is in supported domain - {0}", ex.Message) + DateTime.Now.ToString());
            }
            return isUserInSupportedDomain;
        }

        /// <summary>
        /// Get current user's name.  User.Title may not have the correct values in all the cases hence making a call to user profile to get accurate values
        /// </summary>
        /// <returns></returns>

        private string GetCurrentUserName()
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
            catch (Exception ex)
            {
                Log.LogFileSystem(string.Format("Error happened in getting logged in User's Name - {0}", ex.Message) + DateTime.Now.ToString());
            }
            return ownerLastName + ", " + ownerFirstName;
        }

        /// <summary>
        /// Get current user's Email.  User.Email may not have the correct values in all the cases hence making a call to user profile to get accurate values
        /// </summary>
        /// <returns></returns>
        private string GetCurrentUserEmail()
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
            catch (Exception ex)
            {
                Log.LogFileSystem(string.Format("Error happened in getting logged in User's Email - {0}", ex.Message) + DateTime.Now.ToString());
            }
            return ownerEmail;
        }

        /// <summary>
        /// Get other user's name.  User.Title may not have the correct values in all the cases hence making a call to user profile to get accurate values
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private string GetOthersUserName(User user)
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
            catch (Exception ex)
            {
                Log.LogFileSystem(string.Format("Error happened in getting User's Name - {0}", ex.Message) + DateTime.Now.ToString());
            }
            return fullName;
        }

        /// <summary>
        /// Get current user's Email.  User.Email may not have the correct values in all the cases hence making a call to user profile to get accurate values
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>

        private string GetOthersUserEmail(User user)
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
            catch (Exception ex)
            {
                Log.LogFileSystem(string.Format("Error happened in getting User's Email - {0}", ex.Message) + DateTime.Now.ToString());
            }
            return email;
        }

        /// <summary>
        /// Get current user's manager  
        /// </summary>
        /// <returns></returns>
        private string GetCurrentUserManager()
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
            catch (Exception ex)
            {
                Log.LogFileSystem(string.Format("Error happened in getting Current User Manager - {0}", ex.Message) + DateTime.Now.ToString());
            }
            return ownerEmail;
        }

    }
}