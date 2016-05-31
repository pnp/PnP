using System.IO;
using System.Web.Configuration;

namespace Contoso.Office365.common
{
    public class EmailConfig
    {
        #region Variable

        public string SuccessEmailTemplate { get; set; }

        private const string CONFIG_SITEOWNERCHANGETEMPLATE = "EmailSiteOwnerChangeTemplate";
        private const string TOKEN_SITEURL = "[SITEURL]";
        private const string TOKEN_OLDSITEOWNER = "[OLDSITEOWNER]";
        private const string TOKEN_NEWSITEOWNER = "[NEWSITEOWNER]";
        private const string TOKEN_ERROR_MESSAGE = "[ERRORMESSAGE]";

        #endregion
        #region Constructor
        public EmailConfig()
        {
            string successEmail = WebConfigurationManager.AppSettings.Get(CONFIG_SITEOWNERCHANGETEMPLATE);
               
            if (File.Exists(successEmail))
            {
                using (StreamReader sr = new StreamReader(successEmail))
                {
                    this.SuccessEmailTemplate = sr.ReadToEnd();
                }
            }
            else
            {

            }

        }
        #endregion

        #region Method
        public string GetSiteOwnerChangeEmailTemplateContent(SuccessEmailMessage message)
        {
            string template = this.SuccessEmailTemplate;

            //template = template.Replace(TOKEN_TEMPLATEIMAGE, String.Format("cid:{0}", TOKEN_TEMPLATEIMAGE));
            template = template.Replace(TOKEN_SITEURL, message.SiteUrl);
            template = template.Replace(TOKEN_OLDSITEOWNER, message.OldSiteOwner);
            template = template.Replace(TOKEN_NEWSITEOWNER, message.NewSiteOwner);
            //template = template.Replace(TOKEN_STORAGELIMIT,
            //    String.Format(new FileSizeFormatProvider(), "{0:fs}", message.StorageLimit));

            return template;
        }

        
        #endregion
    }
}
