# The Help Desk Demo #

### Summary ###
This solution demonstrates combining various Office 365 and Azure resources into a single application that can be consumed simultaneously.  The application includes functionality and data from
Azure Active Directory, Microsoft Graph, SQL Azure, Yammer, and SharePoint Online.


### Full walkthrough ###

A full walkthrough of the development process (including deployment to Azure) can be found at - 
[http://blog.jonathanhuss.com/the-help-desk-demo](http://blog.jonathanhuss.com/the-help-desk-demo)

### Applies to ###
- Office 365

### Prerequisites ###
N/A

### Solution ###
Solution | Author(s)
---------|----------
BusinessApps.HelpDesk | Jonathan Huss (**Microsoft**)

### Version history ###

Version  | Date | Comments
---------| -----| --------
1.0 | December 7th, 2015 | Initial Release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------

### User Interface ###

The user interface combines data directly from Microsoft Graph, SQL Azure, Yammer, and SharePoint.  It's designed to display the most pertinent
information to a user on a single screen, in the form of a dashboard.  The application is a single page application and is completely driven by MVC, JavaScript and AJAX.
As the users navigate through the application, various AJAX calls are made into the MVC controllers.  In most cases, the data is return in JSON format.

![Web page with four areas. One for emails, one for tickets, one for Yammer feed and one for announcements](http://blog.jonathanhuss.com/wp-content/uploads/2015/12/image66.png)

### Authentication Bits ###

The Help Desk Demo depends on Azure for authentication/authorization into the Microsoft Graph and SharePoint Online.  The application authenticates to Azure Active Directory
via a certificate.  Azure Active Directory then provides tokens that allow the application access to the necessary resources.  This work is done via the AuthenticationHelper.cs class
that looks like this:

```
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace BusinessApps.HelpDesk.Helpers
{
    public class AuthenticationHelper
    {
        public ClientAssertionCertificate GetClientAssertionCertificate()
        {
            X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            certStore.Open(OpenFlags.ReadOnly);

            X509Certificate2 cert = certStore.Certificates.Find(X509FindType.FindByThumbprint, SettingsHelper.CertThumbprint, false)[0];

            ClientAssertionCertificate cac = new ClientAssertionCertificate(SettingsHelper.ClientId, cert);

            return cac;
        }

        public async Task<AuthenticationResult> GetToken(string resource)
        {
            ClientAssertionCertificate cac = GetClientAssertionCertificate();

            AuthenticationContext authContext = new AuthenticationContext(SettingsHelper.AzureADAuthority);
            AuthenticationResult authResult = await authContext.AcquireTokenAsync(resource, cac);

            return authResult;
        }
    }
}
```

When GetToken(...) is called, it requires a parameter to indicate the resource to provide the token.  For Graph, that resource is https://graph.microsoft.com/.
For SharePoint Online, it's the root SharePoint site.

### Graph ###

Microsoft Graph (formerly Unified API) facilitates the connection into Exchange Online, which provides data about e-mail messages, 
mail folders, and security group members.  Microsoft Graph also allows e-mail messages to be updated in the mailbox once they've 
been processed by the Help Desk site.  A wrapper class provides easy access to Graph from the rest of the application and looks like this:


```
using BusinessApps.HelpDesk.Models.Email;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BusinessApps.HelpDesk.Helpers
{
    public class GraphHelper
    {
        private AuthenticationResult _token;
       

        #region Email

        public async Task<IEnumerable<EmailMessage>> GetNewEmailMessages()
        {
            JObject results = await QueryGraph("users/" + SettingsHelper.HelpDeskEmailAddress + "/mailFolders/Inbox/messages");

            List<EmailMessage> emailMessages = new List<EmailMessage>();

            foreach(JToken result in results["value"])
            {
                if (!result["categories"].Any())
                {
                    EmailMessage emailMessage = new EmailMessage();
                    emailMessage.MessageID = result["id"].ToString();
                    emailMessage.Sender = result["sender"]["emailAddress"]["name"].ToString();
                    emailMessage.SentTimestamp = result["sentDateTime"].ToString();
                    emailMessage.Subject = result["subject"].ToString();
                    emailMessage.Body = result["body"]["content"].ToString();

                    emailMessages.Add(emailMessage);
                }
            }

            return emailMessages;
        }

        public async Task<EmailMessage> GetEmailMessage(string messageID)
        {
            JObject result = await QueryGraph("users/" + SettingsHelper.HelpDeskEmailAddress + "/messages/" + messageID);

            EmailMessage emailMessage = new EmailMessage();
            emailMessage.MessageID = result["id"].ToString();
            emailMessage.Sender = result["sender"]["emailAddress"]["name"].ToString();
            emailMessage.SentTimestamp = result["sentDateTime"].ToString();
            emailMessage.Subject = result["subject"].ToString();
            emailMessage.Body = result["body"]["content"].ToString();

            return emailMessage;
        }

        public async Task MarkEmailMessageAssigned(string messageID)
        {
            StringContent content = new StringContent("{ \"categories\" : [ \"Assigned\" ] }");

            await QueryGraph("users/" + SettingsHelper.HelpDeskEmailAddress + "/messages/" + messageID, new HttpMethod("PATCH"), content);
        }

        public async Task MarkEmailMessageClosed(string messageID)
        {
            StringContent content = new StringContent("{ \"categories\" : [ \"Closed\" ] }");

            await QueryGraph("users/" + SettingsHelper.HelpDeskEmailAddress + "/messages/" + messageID, new HttpMethod("PATCH"), content);
        }

        public async Task DeleteEmailMessage(string messageID)
        {
            JObject results = await QueryGraph("users/" + SettingsHelper.HelpDeskEmailAddress + "/mailFolders/DeletedItems");

            string json = "{ \"DestinationId\" : \"" + results["id"].ToString() + "\" }";
            StringContent content = new StringContent(json);

            await QueryGraph("users/" + SettingsHelper.HelpDeskEmailAddress + "/messages/" + messageID + "/microsoft.graph.move", HttpMethod.Post, content);
        }

        #endregion



        #region People

        public async Task<IEnumerable<HelpdeskOperator>> GetHelpdeskOperators()
        {
            JObject results = await QueryGraph("groups/9fbc4bf3-da97-489a-bf31-afd1707c4b39/members");  //  9fbc4bf3-da97-489a-bf31-afd1707c4b39 is the id of the Help Desk Operators group

            List <HelpdeskOperator> helpdeskOperators = new List<HelpdeskOperator>();

            foreach (JToken result in results["value"])
            {
                HelpdeskOperator helpdeskOperator = new HelpdeskOperator();
                helpdeskOperator.EmailAddress = result["userPrincipalName"].ToString();
                helpdeskOperator.Name = result["displayName"].ToString();

                helpdeskOperators.Add(helpdeskOperator);
            }

            return helpdeskOperators;
        }
        
       
        #endregion

        private async Task<JObject> QueryGraph(string endpoint, HttpMethod method = null, StringContent content = null)
        {
            if (method == null)
                method = HttpMethod.Get;

            JObject json = null;

            using (HttpClient client = new HttpClient())
            {
                Uri requestUri = new Uri(new Uri(SettingsHelper.GraphUrl), endpoint);
                using (HttpRequestMessage request = new HttpRequestMessage(method, requestUri))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", (await GetAccessToken()).AccessToken);

                    if (content != null)
                    {
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        request.Content = content;
                    }

                    using (HttpResponseMessage response = await client.SendAsync(request))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            json = JObject.Parse(await response.Content.ReadAsStringAsync());
                        }
                    }
                }
            }

            return json;
        }

        private async Task SetAccessToken()
        {
            AuthenticationHelper authHelper = new AuthenticationHelper();
            _token = await authHelper.GetToken(SettingsHelper.GraphResource);
        }
        
        private async Task<AuthenticationResult> GetAccessToken()
        {
            if (_token == null)
                await SetAccessToken();

            if (_token.ExpiresOn < DateTime.Now)
                await SetAccessToken();

            return _token;
        }
    }
}
```

### Yammer ###

Perhaps the easiest external data to incorporate into the demo is the Yammer group feed.  While Yammer has an API of it's own,
simply displaying a group feed in the website doesn't require the API.  Yammer provides an 'embed' option in the group which 
generates HTML and JavaScript to display the feed.  Navigating to the Yammer group and selecting 'Embed this feed in your site',
opens a window containing the necessary bits:

![SharePoint's Embed This Feed in your site UI with Yammer embed information](http://blog.jonathanhuss.com/wp-content/uploads/2015/12/image18.png)

The JavaScript provided looks like this:

```
<script type="text/javascript">
    yam.connect.embedFeed({
	container: "#embedded-feed",
	network: "jonhussdev.com",
	feedType: "group",
	feedId: "6791432"});
</script>
```

<img  src="https://telemetry.sharepointpnp.com/pnp/solutions/BusinessApps.HelpDesk" />