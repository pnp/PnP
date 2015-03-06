using Framework.Provisioning.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Provisioning.Core.Mail
{
    public class EmailConfig
    {
        #region Variable

        public string SuccessEmailTemplate { get; set; }
        public string FailureEmailTemplate { get; set; }
        public Stream SuccessEmailTemplateImage { get; set; }
        public Stream FailEmailTemplateImage { get; set; }

        private const string CONFIG_NEWSITETEMPLATE = "EmailNewSiteTemplate";
        private const string CONFIG_NEWSITETEMPLATEIMAGE = "EmailNewSiteTemplateImage";
        private const string CONFIG_FAILEMAILTEMPLATE = "EmailFailureSiteTemplate";
        private const string CONFIG_FAILEMAILTEMPLATEIMAGE = "EmailFailureTemplateImage";
        public const string TOKEN_TEMPLATEIMAGE = "imageid";
        private const string TOKEN_SITEURL = "[SITEURL]";
        private const string TOKEN_SITEOWNER = "[SITEOWNER]";
        private const string TOKEN_SITEADMIN = "[SITEADMIN]";
        private const string TOKEN_STORAGELIMIT = "[STORAGELIMIT]";
        private const string TOKEN_ERROR_MESSAGE = "[ERRORMESSAGE]";

        #endregion
        #region Constructor
        public EmailConfig()
        {
            string successEmail = ConfigurationManager.AppSettings[CONFIG_NEWSITETEMPLATE];
            string failEmail = ConfigurationManager.AppSettings[CONFIG_FAILEMAILTEMPLATE];

            if (File.Exists(successEmail))
            {
                using (StreamReader sr = new StreamReader(successEmail))
                {
                    this.SuccessEmailTemplate = sr.ReadToEnd();
                }
            }
            else
            {
                Log.Warning("Framework.Provisioning.Core.Mail.EmailConfig", "Your Email Template doesn't exist");
            }

            string successEmailImage = ConfigurationManager.AppSettings[CONFIG_NEWSITETEMPLATEIMAGE];
            if (File.Exists(successEmailImage))
            {
                this.SuccessEmailTemplateImage = File.OpenRead(successEmailImage);
            }
            else
            {
                Log.Warning("Framework.Provisioning.Core.Mail.EmailConfig", "Your Email Template Image doesn't exist");
            }
            
            if(File.Exists(failEmail))
            {
                using (StreamReader sr = new StreamReader(failEmail))
                {
                    this.FailureEmailTemplate = sr.ReadToEnd();
                }
            }
            else
            {
                Log.Warning("Framework.Provisioning.Core.Mail.EmailConfig", "Your Email Template doesn't exist");
            }
            string failImage = ConfigurationManager.AppSettings[CONFIG_FAILEMAILTEMPLATEIMAGE];
            if (File.Exists(failImage))
            {
                this.FailEmailTemplateImage = File.OpenRead(failImage);
            }
            else
            {
                Log.Warning("Framework.Provisioning.Core.Mail.EmailConfig", "Your Email Template Image doesn't exist");
            }
        }
        #endregion

        #region Method
        public string GetNewSiteEmailTemplateContent(SuccessEmailMessage message)
        {
            string template = this.SuccessEmailTemplate;

            template = template.Replace(TOKEN_TEMPLATEIMAGE, String.Format("cid:{0}", TOKEN_TEMPLATEIMAGE));
            template = template.Replace(TOKEN_SITEURL, message.SiteUrl);
            template = template.Replace(TOKEN_SITEOWNER, message.SiteOwner);
            template = template.Replace(TOKEN_SITEADMIN, message.SiteAdmin);
            //template = template.Replace(TOKEN_STORAGELIMIT,
            //    String.Format(new FileSizeFormatProvider(), "{0:fs}", message.StorageLimit));

            return template;
        }

        public string GetFailureEmailTemplateContent(FailureEmailMessage message)
        {
            string template = this.FailureEmailTemplate;

            template = template.Replace(TOKEN_TEMPLATEIMAGE, String.Format("cid:{0}", TOKEN_TEMPLATEIMAGE));
            template = template.Replace(TOKEN_SITEURL, message.SiteUrl);
            template = template.Replace(TOKEN_SITEOWNER, message.SiteOwner);
            template = template.Replace(TOKEN_SITEADMIN, message.SiteAdmin);
            template = template.Replace(TOKEN_ERROR_MESSAGE, message.ErrorMessage);

            return template;
        }
        #endregion
    }
}
