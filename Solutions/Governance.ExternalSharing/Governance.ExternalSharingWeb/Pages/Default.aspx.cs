using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.Online.SharePoint.TenantManagement;
using Microsoft.SharePoint.Client;
using System.Web.Configuration;
using System.Diagnostics;
using System.IO;
using Microsoft.SharePoint.Client.UserProfiles;
using Contoso.Office365.common;

namespace Governance.ExternalSharingWeb.Pages
{
    public partial class Default : Page
    {
        string siteURL = null;
        string initialSharingSetting = null;
        const string _shared = "Share invitations";
        const string _notshared = "Not Allowed";
        string TenantAdminUrl = WebConfigurationManager.AppSettings.Get("TenantAdminUrl");
        string JavaScriptFile = WebConfigurationManager.AppSettings.Get("JavaScriptFile");
        string source = "ExternalSharing.Banner";
        string log = "Application";

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

            Master.Hdn_Master_PageTitle.Value = "Sharing Outside Contoso";
            Master.Hdn_Master_ShortPageTitle.Value = "External Sharing";

            if (!Page.IsPostBack)
            {
                Log.LogFileSystem(string.Format("Started changing External Sharing Settings for Site Collection - {0}  ", Page.Request["SPHostUrl"]));

                var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    clientContext.Load(clientContext.Site, siteurl => siteurl.Url);
                    clientContext.ExecuteQuery();
                    EventLog.WriteEntry(source, string.Format("Started changing External Sharing Settings for Site Collection {0}", clientContext.Site.Url), EventLogEntryType.Information, 6000);
                    Master.Hdn_Master_CurrentSiteUrl.Value = clientContext.Site.Url;

                    clientContext.Load(clientContext.Web, web => web.Title);
                    clientContext.ExecuteQuery();
                    Master.Hdn_Master_CurrentSiteTitle.Value = clientContext.Web.Title;

                    clientContext.Load(clientContext.Web, user => user.CurrentUser);
                    clientContext.ExecuteQuery();
                    User currentUser = clientContext.Web.CurrentUser;
                    string userName = currentUser.Title;
                    Master.Hdn_Master_CurrentUserName.Value = GetCurrentUserName();
                    Master.Hdn_Master_CurrentUserEmail.Value = GetCurrentUserEmail();

                    clientContext.Load(clientContext.Site);
                    clientContext.ExecuteQuery();
                    siteURL = clientContext.Site.Url;
                    lblSiteURL.Text = "https://contoso.sharepoint.com/teams/NCS";//siteURL;

                    if (clientContext.Site.ShareByEmailEnabled)
                    {
                        //lblSharing.Text = _shared;
                        rdbList.Items[1].Selected = true;
                        initialSharingSetting = _shared;
                        HiddenField_Init_ExternalSharing_Enabled.Value = "true";
                    }
                    else
                    {
                        //lblSharing.Text = _notshared;
                        rdbList.Items[0].Selected = true;
                        initialSharingSetting = _notshared;
                        HiddenField_Init_ExternalSharing_Enabled.Value = "false";
                    }
                }
            }
        }
        static ClientContext GetContext(string tenantURL)
        {
            Uri tenantUri = new Uri(tenantURL);
            string adminRealm = TokenHelper.GetRealmFromTargetUrl(tenantUri);
            var adminToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, tenantUri.Authority, adminRealm).AccessToken;
            var clientContextSC = TokenHelper.GetClientContextWithAccessToken(tenantUri.ToString(), adminToken);
            return clientContextSC;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            siteURL = Page.Request["SPHostUrl"];
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                clientContext.Load(clientContext.Site);
                clientContext.ExecuteQuery();
                if (clientContext.Site.ShareByEmailEnabled)
                    initialSharingSetting = _shared;
                else
                    initialSharingSetting = _notshared;
            }          
                  
            try
            {
                if (rdbList.SelectedValue == "allowed" && initialSharingSetting == _notshared)
                {
                    Log.LogFileSystem(string.Format("Start enabling external sharing..."));
                    Log.LogFileSystem(string.Format("\t" + "Start getting Context..."));

                    var ctx = GetContext(TenantAdminUrl);
                    using (ctx)
                    {
                        Tenant _tenant = new Tenant(ctx);
                        Log.LogFileSystem(string.Format("\t" + "Loading site properties..."));                        
                        SiteProperties _siteProps = _tenant.GetSitePropertiesByUrl(siteURL, false);
                        ctx.Load(_tenant);
                        ctx.Load(_siteProps);
                        ctx.ExecuteQuery();
                        bool _shouldBeUpdated = false;

                        var _tenantSharingCapability = _tenant.SharingCapability;
                        var _siteSharingCapability = _siteProps.SharingCapability;
                        var _targetSharingCapability = SharingCapabilities.Disabled;

                        //if (siteInfo.EnableExternalSharing && _tenantSharingCapability != SharingCapabilities.Disabled)
                        //{
                        _targetSharingCapability = SharingCapabilities.ExternalUserSharingOnly;
                        _shouldBeUpdated = true;
                        //}
                        if (_siteSharingCapability != _targetSharingCapability && _shouldBeUpdated)
                        {
                            Log.LogFileSystem(string.Format("\t" + "Enabling sharing setting..."));
                            _siteProps.SharingCapability = _targetSharingCapability;
                            ctx.Load(_siteProps);
                            SpoOperation op = _siteProps.Update();
                            ctx.Load(op, i => i.IsComplete);
                            ctx.ExecuteQuery();

                            while (!op.IsComplete)
                            {
                                Log.LogFileSystem(string.Format("\t" + "Refreshing update..."));
                                //wait 30seconds and try again
                                System.Threading.Thread.Sleep(3000);
                                op.RefreshLoad();
                                ctx.ExecuteQuery();
                            }

                            Log.LogFileSystem(string.Format("\t" + "Update completed!"));
                        }
                    }

                    try
                    {
                        Log.LogFileSystem(string.Format("\t" + "Start enabling the banner..."));

                        //Enable Banner
                        var clientContextSC = GetContext(siteURL);
                        using (clientContextSC)
                        {
                            Site site = clientContextSC.Site;
                            clientContextSC.Load(site);
                            clientContextSC.ExecuteQuery();

                            var existingActions = site.UserCustomActions;
                            clientContextSC.Load(existingActions);
                            clientContextSC.ExecuteQuery();

                            UserCustomAction targetAction = existingActions.Add();
                            targetAction.Name = "External_Sharing_Banner";
                            targetAction.Description = "External_Sharing_Banner";
                            targetAction.Location = "ScriptLink";
                            
                            targetAction.ScriptBlock = "var headID = document.getElementsByTagName('head')[0]; var externalSharingTag = document.createElement('script'); externalSharingTag.type = 'text/javascript'; externalSharingTag.src = '" + JavaScriptFile + "';headID.appendChild(externalSharingTag);";
                            targetAction.ScriptSrc = "";
                            targetAction.Update();
                            clientContextSC.ExecuteQuery();

                            Log.LogFileSystem(string.Format("\t" + "Banner successfully enabled!"));
                            EventLog.WriteEntry(source, string.Format("Changing External Sharing Settings is completed successfully.  The Site Collection is externally shared and the banner is enabled."), EventLogEntryType.Information, 6000);
                        }
                    }
                    catch (Exception ex)
                    {
                        EventLog.WriteEntry(source, string.Format("Error occurred in changing External Sharing Settings.  The error is {0}", ex.Message), EventLogEntryType.Error, 6001);
                    }
                }
                else if (rdbList.SelectedValue == "notallowed" && initialSharingSetting == _shared)
                {
                    Log.LogFileSystem(string.Format("Start disabling external sharing..."));
                    Log.LogFileSystem(string.Format("\t" + "Start getting Context..."));

                    var ctx = GetContext(TenantAdminUrl);
                    using (ctx)
                    {
                        Tenant _tenant = new Tenant(ctx);
                        Log.LogFileSystem(string.Format("\t" + "Loading site properties..."));
                        SiteProperties _siteProps = _tenant.GetSitePropertiesByUrl(siteURL, false);
                        ctx.Load(_tenant);
                        ctx.Load(_siteProps);
                        ctx.ExecuteQuery();
                        bool _shouldBeUpdated = false;

                        var _tenantSharingCapability = _tenant.SharingCapability;
                        var _siteSharingCapability = _siteProps.SharingCapability;
                        var _targetSharingCapability = SharingCapabilities.Disabled;

                        _targetSharingCapability = SharingCapabilities.Disabled;
                        _shouldBeUpdated = true;

                        if (_siteSharingCapability != _targetSharingCapability && _shouldBeUpdated)
                        {
                            Log.LogFileSystem(string.Format("\t" + "Disabling sharing setting..."));
                            _siteProps.SharingCapability = _targetSharingCapability;
                            ctx.Load(_siteProps);
                            SpoOperation op = _siteProps.Update();
                            ctx.Load(op, i => i.IsComplete);
                            ctx.ExecuteQuery();

                            while (!op.IsComplete)
                            {
                                Log.LogFileSystem(string.Format("\t" + "Refreshing update..."));
                                //wait 30seconds and try again
                                System.Threading.Thread.Sleep(3000);
                                op.RefreshLoad();
                                ctx.ExecuteQuery();
                            }
                        }

                    }

                    try
                    {
                        Log.LogFileSystem(string.Format("\t" + "Start disabling the banner..."));
                        //Disable Banner
                        var clientContextSC = GetContext(siteURL);
                        using (clientContextSC)
                        {
                            Site site = clientContextSC.Site;
                            clientContextSC.Load(site);
                            clientContextSC.ExecuteQuery();

                            var existingActions = site.UserCustomActions;
                            clientContextSC.Load(existingActions);
                            clientContextSC.ExecuteQuery();

                            var actions = existingActions.ToArray();
                            foreach (var action in actions)
                            {
                                if (action.Name == "External_Sharing_Banner" &&
                                    action.Location == "ScriptLink")
                                {
                                    action.DeleteObject();
                                    clientContextSC.ExecuteQuery();
                                    Log.LogFileSystem(string.Format("\t" + "Banner successfully disabled!"));
                                    break;
                                }
                            }
                        }

                        EventLog.WriteEntry(source, string.Format("Changing External Sharing Settings is completed successfully.  The Site Collection is not externally shared and the banner is disabled."), EventLogEntryType.Information, 6000);
                    }
                    catch (Exception ex)
                    {
                        EventLog.WriteEntry(source, string.Format("Error occurred in changing External Sharing Settings.  The error is {0}", ex.Message), EventLogEntryType.Error, 6001);
                    }

                }

                Log.LogFileSystem(string.Format(string.Format("External Sharing is now {0}  ", rdbList.SelectedValue)));
                Log.LogFileSystem(string.Format("External Sharing setting is changed successfully for Site Collection - {0}  ", Page.Request["SPHostUrl"]));
                Log.LogFileSystem(string.Empty);

                //Response.Redirect((string.IsNullOrEmpty(siteURL) ? Page.Request["SPHostUrl"] : siteURL) + "/_layouts/15/settings.aspx");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "successMessageBanner", "alert('External Sharing setting is changed successfully!!'); window.location='" +
                                                               (string.IsNullOrEmpty(siteURL) ? Page.Request["SPHostUrl"] : siteURL) + "/_layouts/15/settings.aspx';", true);
            }
            catch (System.Threading.ThreadAbortException ex)
            {
                Response.Redirect((string.IsNullOrEmpty(siteURL) ? Page.Request["SPHostUrl"] : siteURL) + "/_layouts/15/settings.aspx", false);
            }
            catch (Exception ex)
            {
                Log.LogFileSystem(string.Format("Error occured in changing External Sharing Settings for Site Collection - {0}, error is {1}  ", (string.IsNullOrEmpty(siteURL) ? Page.Request["SPHostUrl"] : siteURL), ex.Message));
                Log.LogFileSystem(string.Empty);
                EventLog.WriteEntry(source, string.Format("Error occured in changing External Sharing Settings.  The error is {0}", ex.Message), EventLogEntryType.Error, 6001);
                ScriptManager.RegisterStartupScript(this, GetType(), "ErrorMessageBanner", "document.getElementById('spanErrorMsg').style.display = 'block';", true);
                ScriptManager.RegisterStartupScript(this, GetType(), "ErrorCursor", "  document.body.style.cursor = 'default';", true);
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

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            siteURL = Page.Request["SPHostUrl"];
            EventLog.WriteEntry(source, string.Format("Changing External Sharing Settings is canceled."), EventLogEntryType.Information, 6000);
            Log.LogFileSystem(string.Format("Changing External Sharing Settings is canceled.  "));
            Log.LogFileSystem(string.Empty);
            Response.Redirect((string.IsNullOrEmpty(siteURL) ? Page.Request["SPHostUrl"] : siteURL) + "/_layouts/15/settings.aspx", false);
        }
    }
}