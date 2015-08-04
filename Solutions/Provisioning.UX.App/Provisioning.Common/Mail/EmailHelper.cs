using Provisioning.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Mail
{
    public class EmailHelper
    {
        #region Constructor
        public EmailHelper()
        {

        }
        #endregion

        #region Method
      
        /// <summary>
        /// Helper Method to Send Site Creation Success Email notifcation.
        /// </summary>
        /// <param name="message"></param>
        public static void SendNewSiteSuccessEmail(SuccessEmailMessage message)
        {
         
            try
            {
                EmailConfig _emailConfig = new EmailConfig();
                using (SmtpClient client = new SmtpClient())
                {
                    using (MailMessage emailMessage = new MailMessage())
                    {
                        emailMessage.Subject = message.Subject;

                        foreach (string to in message.To)
                        {
                            emailMessage.To.Add(to);
                        }

                        foreach (string cc in message.Cc)
                        {
                            emailMessage.CC.Add(cc);
                        }
                        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(_emailConfig.GetNewSiteEmailTemplateContent(message), null, "text/html");
                        emailMessage.AlternateViews.Add(htmlView);
                        client.Send(emailMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Provisioning.Common.Mail.EmailHelper.SendNewSiteSuccessEmail", "There was an error sending email. The exception is {0}", ex);
            }
        }

        /// <summary>
        /// Helper Method to Send Failed Sites email notifcation.
        /// </summary>
        /// <param name="message"></param>
        public static void SendFailEmail(FailureEmailMessage message)
        {
            try
            {
                EmailConfig _emailConfig = new EmailConfig();
                using (SmtpClient client = new SmtpClient())
                {
                    using (MailMessage emailMessage = new MailMessage())
                    {
                        emailMessage.Subject = message.Subject;
                        foreach (string to in message.To)
                        {
                            emailMessage.To.Add(to);
                        }

                        foreach (string cc in message.Cc)
                        {
                            emailMessage.CC.Add(cc);
                        }
                        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(_emailConfig.GetFailureEmailTemplateContent(message), null, "text/html");
                        emailMessage.AlternateViews.Add(htmlView);
                        client.Send(emailMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Provisioning.Common.Mail.EmailHelper.SendFailEmail", "There was an error sending email. The exception is {0}", ex);
            }
        }
        #endregion
    }
}
