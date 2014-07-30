using Microsoft.Office365.Exchange;
using Microsoft.Office365.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Office365Api.Overview
{
    public static class MailApiSample
    {
        const string ExchangeResourceId = "https://outlook.office365.com";
        const string ExchangeServiceRoot = "https://outlook.office365.com/ews/odata";

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

        private static async Task<ExchangeClient> EnsureClientCreated()
        {
            Authenticator authenticator = new Authenticator();
            var authInfo = await authenticator.AuthenticateAsync(ExchangeResourceId);

            return new ExchangeClient(new Uri(ExchangeServiceRoot), authInfo.GetAccessToken);
        }
        public static async Task SignOut()
        {
            await new Authenticator().LogoutAsync();
        }



    }
}
