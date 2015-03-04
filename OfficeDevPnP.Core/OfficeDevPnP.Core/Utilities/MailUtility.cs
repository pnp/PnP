using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Security;

namespace OfficeDevPnP.Core.Utilities
{
    public class MailUtility
    {
#if CLIENTSDKV15

        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="context">Context for SharePoint objects and operations</param>
        /// <param name="to">List of TO addresses.</param>
        /// <param name="cc">List of CC addresses.</param>
        /// <param name="subject">Subject of the mail.</param>
        /// <param name="body">HTML body of the mail.</param>
        public static void SendEmail(ClientContext context, IEnumerable<String> to, IEnumerable<String> cc, string subject, string body)
        {
            EmailProperties properties = new EmailProperties();
            properties.To = to;

            if (cc != null)
            {
                properties.CC = cc;
            }

            properties.Subject = subject;
            properties.Body = body;

            Microsoft.SharePoint.Client.Utilities.Utility.SendEmail(context, properties);
            context.ExecuteQueryRetry();
        }

#endif

#if !CLIENTSDKV15

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
            // Get the secure password
            var secureString = new SecureString();
            foreach (char c in fromUserPassword.ToCharArray())
            {
                secureString.AppendChar(c);
            }

            SendEmail(servername, fromAddress, fromUserPassword, to, cc, subject, body, sendAsync, asyncUserToken);
        }

        /// <summary>
        /// Sends an email via Office 365 SMTP
        /// </summary>
        /// <param name="servername">Office 365 SMTP address. By default this is smtp.office365.com.</param>
        /// <param name="fromAddress">The user setting up the SMTP connection. This user must have an EXO license.</param>
        /// <param name="fromUserPassword">The secure password of the user.</param>
        /// <param name="to">List of TO addresses.</param>
        /// <param name="cc">List of CC addresses.</param>
        /// <param name="subject">Subject of the mail.</param>
        /// <param name="body">HTML body of the mail.</param>
        /// <param name="sendAsync">Sends the email asynchronous so as to not block the current thread (default: false).</param>
        /// <param name="asyncUserToken">The user token that is used to correlate the asynchronous email message.</param>
        public static void SendEmail(string servername, string fromAddress, SecureString fromUserPassword, IEnumerable<String> to, IEnumerable<String> cc, string subject, string body, bool sendAsync = false, object asyncUserToken = null)
        {
            SmtpClient server = new SmtpClient(servername);
            server.Port = 587;
            server.EnableSsl = true;
            server.Credentials = new NetworkCredential(fromAddress, fromUserPassword);

            MailMessage mail = new MailMessage();
            // Mail from and the network credentials must match!
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
                            Log.Error(Constants.LOGGING_SOURCE, CoreResources.MailUtility_SendFailed, args.Error.Message);
                        }
                        else if (args.Cancelled)
                        {
                            Log.Info(Constants.LOGGING_SOURCE, CoreResources.MailUtility_SendMailCancelled);
                        }
                    };
                    server.SendAsync(mail, asyncUserToken);
                }
                else
                {
                    server.Send(mail);
                }
            }
            catch (SmtpException smtpEx)
            {
                Log.Error(Constants.LOGGING_SOURCE, CoreResources.MailUtility_SendException, smtpEx.Message);
            }
            catch (Exception ex)
            {
                Log.Error(Constants.LOGGING_SOURCE, CoreResources.MailUtility_SendExceptionRethrow0, ex);
                throw;
            }
        }

#endif
    }
}
