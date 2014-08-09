using Microsoft.Office365.Exchange;
using Microsoft.Office365.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Office365Api.Demo
{
    public static class MailApiSample
    {
        const string ServiceResourceId = "https://outlook.office365.com";
        static readonly Uri ServiceEndpointUri = new Uri("https://outlook.office365.com/ews/odata");

        // Do not make static in Web apps; store it in session or in a cookie instead
        static string _lastLoggedInUser;
        //static DiscoveryContext _discoveryContext;
        public static DiscoveryContext _discoveryContext
        {
            get;
            set;
        }
 
        public static async Task<IEnumerable<IMessage>> GetMessages()
        {
            var client = await EnsureClientCreated();
            client.Context.IgnoreMissingProperties = true;

            List<IMessage> mails = new List<IMessage>();

            var query = from i in client.Me.Inbox.Messages
                        orderby i.DateTimeSent descending
                        select i;

            var messageResults = await query.ExecuteAsync();

            //Server side paging is currently broken
            //do
            //{
            //    mails.AddRange(messageResults.CurrentPage);
            //    messageResults = await messageResults.GetNextPageAsync();
            //}
            //while (messageResults != null);

            //Resort to client side paging
            while (true)
            {
                int sizeBefore = mails.Count;
                mails.AddRange(messageResults.CurrentPage);

                if (mails.Count == sizeBefore)
                {
                    break;
                }

                messageResults = await query.Skip(mails.Count).ExecuteAsync();
            }


            return mails;
        }

        public static async Task<int> GetMailStats()
        {
            var client = await EnsureClientCreated();

            var inbox = await client.Me.Inbox.ExecuteAsync();
            return (int)inbox.TotalCount;
        }

        public static async Task SendMail(string to, string subject, string body)
        {
            var client = await EnsureClientCreated();

            Message mail = new Message();
            mail.ToRecipients.Add(new Recipient() { Address = to});
            mail.Subject = subject;
            mail.Body = new ItemBody() { Content = body, ContentType = BodyType.HTML };
            
            await client.Me.SentItems.Messages.AddMessageAsync(mail);
            await mail.SendAsync();
        }

        public static async Task DraftMail(string to, string subject, string body)
        {
            var client = await EnsureClientCreated();

            Message mail = new Message();
            mail.ToRecipients.Add(new Recipient() { Address = to });
            mail.Subject = subject;
            mail.IsDraft = true;
            mail.Body = new ItemBody() { Content = body, ContentType = BodyType.Text };

            await client.Me.Drafts.Messages.AddMessageAsync(mail);
        }

        public static async Task<ExchangeClient> EnsureClientCreated()
        {
            if (_discoveryContext == null)
            {
                _discoveryContext = await DiscoveryContext.CreateAsync();
            }

            var dcr = await _discoveryContext.DiscoverResourceAsync(ServiceResourceId);

            _lastLoggedInUser = dcr.UserId;

            return new ExchangeClient(ServiceEndpointUri, async () =>
            {
                return (await _discoveryContext.AuthenticationContext.AcquireTokenSilentAsync(ServiceResourceId, _discoveryContext.AppIdentity.ClientId, new Microsoft.IdentityModel.Clients.ActiveDirectory.UserIdentifier(dcr.UserId, Microsoft.IdentityModel.Clients.ActiveDirectory.UserIdentifierType.UniqueId))).AccessToken;
            });
        }

        public static async Task SignOut()
        {
            if (string.IsNullOrEmpty(_lastLoggedInUser))
            {
                return;
            }

            if (_discoveryContext == null)
            {
                _discoveryContext = await DiscoveryContext.CreateAsync();
            }

            await _discoveryContext.LogoutAsync(_lastLoggedInUser);
        }



    }
}
