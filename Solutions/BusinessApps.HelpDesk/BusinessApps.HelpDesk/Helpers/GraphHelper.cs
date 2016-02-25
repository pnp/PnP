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
            JObject results = await QueryGraph("groups/9fbc4bf3-da97-489a-bf31-afd1707c4b39/members");  //  9fbc4bf3-da97-489a-bf31-afd1707c4b39 is the id of the Help Desk Operators Office 365 group

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