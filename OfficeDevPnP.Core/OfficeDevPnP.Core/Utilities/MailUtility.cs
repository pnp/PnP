using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Utilities
{
    public class MailUtility
    {
        /// <summary>
        /// Sends an email via Office 365 SMTP
        /// </summary>
        /// <param name="servername">Office 365 SMTP address. By default this is smtp.office365.com.</param>
        /// <param name="fromAddress">The user setting up the SMTP connection. This user must have an EXO license.</param>
        /// <param name="fromUserPassword">The password of the user.</param>
        /// <param name="to">List of TO addresses.</param>
        /// <param name="cc">List of CC addresses.</param>
        /// <param name="subject">Subject of the mail.</param>
        /// <param name="body">HTML body of the mail.</param>
        /// <param name="sendAsync">Sends the email asynchronous so as to not block the current thread (default: false).</param>
        /// <param name="asyncUserToken">The user token that is used to correlate the asynchronous email message.</param>
        public static void SendEmail(string servername, string fromAddress, string fromUserPassword, IEnumerable<String> to, IEnumerable<String> cc, string subject, string body, bool sendAsync = false, object asyncUserToken = null)
        {
            SmtpClient server = new SmtpClient(servername);
            server.Port = 587;
            server.EnableSsl = true;
            server.Credentials = new System.Net.NetworkCredential(fromAddress, fromUserPassword);

            MailMessage mail = new MailMessage();
            //mail from and the network credentials must match!
            mail.From = new MailAddress(fromAddress);

            foreach (string user in to)
            {
                mail.To.Add(user);
            }

            if (cc != null)
            {
                foreach (string user in cc)
                {
                    mail.CC.Add(user);
                }
            }

            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            try
            {
                if (sendAsync)
                {
                    server.SendCompleted += (sender, args) =>
                    {
                        if (args.Error != null)
                        {
                            LoggingUtility.Internal.TraceError((int)EventId.MailSendFailed, args.Error, CoreResources.MailUtility_SendFailed);
                        }
                        else if (args.Cancelled)
                        {
                            LoggingUtility.Internal.TraceInformation((int)EventId.SendMailCancelled, CoreResources.MailUtility_SendMailCancelled);
                        }
                    };
                    server.SendAsync(mail, asyncUserToken);
                }
                else
                    server.Send(mail);
            }
            catch (SmtpException smtpEx)
            {
                LoggingUtility.Internal.TraceError((int)EventId.MailSendException, smtpEx, CoreResources.MailUtility_SendException);
            }
            catch (Exception ex)
            {
                LoggingUtility.Internal.TraceVerbose(CoreResources.MailUtility_SendExceptionRethrow0, ex);
                throw;
            }
        }
    }
}
