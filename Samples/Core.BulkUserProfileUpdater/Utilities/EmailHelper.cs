namespace Contoso.Core
{
    using System.Net.Mail;

    public static class EmailHelper
    {
        public static void SendEmail(string fromAddress, string toAddress, string host, int port, string subject, string body, string filepath)
        {
            try
            {
                Attachment file = new Attachment(filepath);

                MailMessage mail = new MailMessage(fromAddress, toAddress);
                SmtpClient client = new SmtpClient();
                client.Port = port;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Host = host;
                mail.Subject = subject;
                mail.Body = body;
                mail.Attachments.Add(file);
                client.Send(mail);
            }
            catch
            {

            }
        }
    }
}
