using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Office365.common
{
    public class EmailHelper
    {
        #region Variable
        private static readonly EmailConfig _emailConfig = new EmailConfig();

        #endregion
        #region Constructor
        public EmailHelper()
        {

        }
        #endregion

        #region Method
      
        /// <summary>
        /// Helper Method to Send Site Owner Change Email notifcation.
        /// </summary>
        /// <param name="message"></param>
        public static void SendSiteOwnerChangeEmail(SuccessEmailMessage message)
        {
            try
            {               
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
                        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(_emailConfig.GetSiteOwnerChangeEmailTemplateContent(message), null, "text/html");
                        emailMessage.AlternateViews.Add(htmlView);
                        client.Send(emailMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }        
       
        #endregion
    }
}
