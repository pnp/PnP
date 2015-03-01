using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using Office365Api.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Office365Api.WebFormsDemo.Office365API
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.Current.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties { RedirectUri = "/" },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
        }

        protected async void ListFilesCommand_Click(object sender, EventArgs e)
        {
            var signInUserId = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            AuthenticationHelper authenticationHelper = new AuthenticationHelper();
            authenticationHelper.EnsureAuthenticationContext(new ADALTokenCache(signInUserId));

            MyFilesHelper myFilesHelper = new MyFilesHelper(authenticationHelper);
            var myFiles = await myFilesHelper.GetMyFiles();

            List<String> results = new List<String>();

            commandResult.Text = String.Format("Found {0} my files! Showing first 10, if any.", myFiles.Count());

            foreach (var item in myFiles.Take(10))
            {
                results.Add(String.Format(
                    "URL: {0}",
                    !String.IsNullOrEmpty(item.WebUrl) ? item.WebUrl : String.Empty));
            }

            resultsList.DataSource = results;
            resultsList.DataBind();
        }

        protected async void ListContactsCommand_Click(object sender, EventArgs e)
        {
            var signInUserId = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            AuthenticationHelper authenticationHelper = new AuthenticationHelper();
            authenticationHelper.EnsureAuthenticationContext(new ADALTokenCache(signInUserId));

            ContactsHelper contactsHelper = new ContactsHelper(authenticationHelper);
            var contacts = await contactsHelper.GetContacts();

            List<String> results = new List<String>();

            commandResult.Text = String.Format("Found {0} contacts! Showing first 10, if any.", contacts.Count());

            foreach (var item in contacts.Take(10))
            {
                results.Add(String.Format(
                    "Name: {0} - Email: {1}",
                    !String.IsNullOrEmpty(item.DisplayName) ? item.DisplayName : String.Empty,
                    item.EmailAddresses != null ? item.EmailAddresses.First().Address : String.Empty));
            }

            resultsList.DataSource = results;
            resultsList.DataBind();
        }

        protected async void ListEmailsCommand_Click(object sender, EventArgs e)
        {
            var signInUserId = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            AuthenticationHelper authenticationHelper = new AuthenticationHelper();
            authenticationHelper.EnsureAuthenticationContext(new ADALTokenCache(signInUserId));

            MailHelper mailHelper = new MailHelper(authenticationHelper);
            var mails = await mailHelper.GetMessages();

            List<String> results = new List<String>();

            commandResult.Text = String.Format("Found {0} mails! Showing first 10, if any.", mails.Count());

            foreach (var item in mails.Take(10))
            {
                results.Add(String.Format(
                    "From: {0} - Subject: {1}",
                    item.From != null ? item.From.EmailAddress.Address : "",
                    !String.IsNullOrEmpty(item.Subject) ? item.Subject : String.Empty));
            }

            resultsList.DataSource = results;
            resultsList.DataBind();
        }

        protected async void SendMailCommand_Click(object sender, EventArgs e)
        {
            var signInUserId = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            AuthenticationHelper authenticationHelper = new AuthenticationHelper();
            authenticationHelper.EnsureAuthenticationContext(new ADALTokenCache(signInUserId));

            MailHelper mailHelper = new MailHelper(authenticationHelper);
            await mailHelper.SendMail(TargetEmail.Text, "Let's Hack-A-Thon - Office365Api.WebForms", "This will be <B>fun...</B>");
            commandResult.Text = "Email sent!";

            resultsList.DataSource = new List<String>();
            resultsList.DataBind();
        }
    }
}