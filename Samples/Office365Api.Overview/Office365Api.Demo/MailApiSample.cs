using Microsoft.Office365.OAuth;
using Microsoft.Office365.OutlookServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Office365Api.Demo
{
    public static class MailApiSample
    {
        public static async Task<IEnumerable<IMessage>> GetMessages()
        {
            var client = await EnsureClientCreated();
            client.Context.IgnoreMissingProperties = true;

            List<IMessage> mails = new List<IMessage>();

            // ***********************************************************
            // Note from @PaoloPia: To not stress the server, limit the
            // the query to no more than 50 email items
            // ***********************************************************

            var query = (from i in client.Me.Messages
                        orderby i.DateTimeSent descending
                        select i).Take(50);

            var messageResults = await query.ExecuteAsync();

            do
            {
                mails.AddRange(messageResults.CurrentPage);
                messageResults = await messageResults.GetNextPageAsync();
            }
            while (messageResults.MorePagesAvailable);

            // ***********************************************************
            // Note from @PaoloPia: The following sample code should 
            // be removed because now server-side paging works properly
            // ***********************************************************

            ////Resort to client side paging
            //while (true)
            //{
            //    int sizeBefore = mails.Count;
            //    mails.AddRange(messageResults.CurrentPage);

            //    if (mails.Count == sizeBefore)
            //    {
            //        break;
            //    }

            //    messageResults = await query.Skip(mails.Count).ExecuteAsync();
            //}

            return mails;
        }

        public static async Task SendMail(string to, string subject, string body)
        {
            var client = await EnsureClientCreated();

            Message mail = new Message();
            mail.ToRecipients.Add(new Recipient() 
            {
                EmailAddress = new EmailAddress
                {
                    Address = to,
                }
            });
            mail.Subject = subject;
            mail.Body = new ItemBody() { Content = body, ContentType = BodyType.HTML };

            await client.Me.SendMailAsync(mail, true);
        }

        public static async Task DraftMail(string to, string subject, string body)
        {
            var client = await EnsureClientCreated();

            Message mail = new Message();
            mail.ToRecipients.Add(new Recipient()
            {
                EmailAddress = new EmailAddress
                {
                    Address = to,
                }
            });
            mail.Subject = subject;
            mail.Body = new ItemBody() { Content = body, ContentType = BodyType.HTML };

            await client.Me.Messages.AddMessageAsync(mail);
        }

        public static async Task<OutlookServicesClient> EnsureClientCreated()
        {
            var discoveryResult = await DiscoveryAPISample.DiscoveryClient.DiscoverCapabilityAsync(Office365Capabilities.Mail.ToString());

            var ServiceResourceId = discoveryResult.ServiceResourceId;
            var ServiceEndpointUri = discoveryResult.ServiceEndpointUri;

            // Create the OutlookServicesClient client proxy:
            return new OutlookServicesClient(
                ServiceEndpointUri,
                async () =>
                {
                    return await AuthenticationHelper.GetAccessTokenForServiceAsync(discoveryResult);
                });
        }
    }
}
