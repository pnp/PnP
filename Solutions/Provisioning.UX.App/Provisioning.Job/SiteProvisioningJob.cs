using Provisioning.Common;
using Provisioning.Common.Authentication;
using Provisioning.Common.Configuration;
using Provisioning.Common.Configuration.Application;
using Provisioning.Common.Data;
using Provisioning.Common.Data.SiteRequests;
using Provisioning.Common.Data.Templates;
using Provisioning.Common.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Job
{
    public class SiteProvisioningJob
    {
        #region Instance Members
        ISiteRequestFactory _requestFactory;
        IConfigurationFactory _configFactory;
        ISiteTemplateFactory _stTemplateFactory;
        IAppSettingsManager _appManager;
        AppSettings _settings;
        #endregion

        #region Constructors
        public SiteProvisioningJob()
        {
            this._requestFactory = SiteRequestFactory.GetInstance();
            var _siteFactory = SiteTemplateFactory.GetInstance();
            var _tm = _siteFactory.GetManager();
            this._appManager = _configFactory.GetAppSetingsManager();
            this._settings = _appManager.GetAppSettings();
        }
        #endregion

        public void ProcessSiteRequests()
        {
            var _srManager = _requestFactory.GetSiteRequestManager();
            var _requests = _srManager.GetApprovedRequests();

            //TODO LOG HOW MANY ITEMS
            if(_requests.Count > 0)
            {
                this.ProvisionSites(_requests);
            }
            else
            {
                //LOG NO ITEMS
            }
        }

        public void ProvisionSites(ICollection<SiteRequestInformation> siterequests)
        {
            var _tm = this._stTemplateFactory.GetManager();
            SiteProvisioningManager _siteProvisioningManager = new SiteProvisioningManager();

            foreach (var siterequest in siterequests)
            {
                try 
                {
                    var _template = _tm.GetTemplateByName(siterequest.Template);
                    var _provisioningTemplate = _tm.GetProvisioningTemplate(_template.ProvisioningTemplate);
                  
                    //NO TEMPLATE FOUND THAT MATCHES WE CANNOT PROVISION A SITE
                    if (_template == null) {
                       //TODO LOG
                    }

                    var _web = _siteProvisioningManager.ProcessSiteRequest(siterequest, _template);
                  //  var _web = _siteProvisioningManager.GetWeb(siterequest, _template);
                    _siteProvisioningManager.ApplyProvisioningTemplates(_web, _provisioningTemplate);
                    this.SendSuccessEmail(siterequest);

                }
                catch(Exception _ex)
                {
                    this.SendFailureEmail(siterequest, _ex.Message);
                }
               
            }
        }

        /// <summary>
        /// Sends a Notification that the Site was created
        /// </summary>
        /// <param name="info"></param>
        protected void SendSuccessEmail(SiteRequestInformation info)
        {
            StringBuilder _admins = new StringBuilder();
            SuccessEmailMessage _message = new SuccessEmailMessage();
            _message.SiteUrl = info.Url;
            _message.SiteOwner = info.SiteOwner.Name;
            _message.Subject = "Notification: Your new SharePoint site is ready";

            _message.To.Add(info.SiteOwner.Email);
            foreach (var admin in info.AdditionalAdministrators)
            {
                _message.Cc.Add(admin.Email);
                _admins.Append(admin.Name);
                _admins.Append(" ");
            }
            _message.SiteAdmin = _admins.ToString();
            EmailHelper.SendNewSiteSuccessEmail(_message);
        }

        /// <summary>
        /// Sends an Failure Email Notification
        /// </summary>
        /// <param name="info"></param>
        /// <param name="errorMessage"></param>
        protected void SendFailureEmail(SiteRequestInformation info, string errorMessage)
        {
            StringBuilder _admins = new StringBuilder();
            FailureEmailMessage _message = new FailureEmailMessage();
            _message.SiteUrl = info.Url;
            _message.SiteOwner = info.SiteOwner.Name;
            _message.Subject = "Alert: Your new SharePoint site request had a problem.";
            _message.ErrorMessage = errorMessage;
            _message.To.Add(info.SiteOwner.Email);

            if (!string.IsNullOrEmpty(this._settings.SupportEmailNotification))
            {
                string[] supportAdmins = this._settings.SupportEmailNotification.Split(';');
                foreach (var supportAdmin in supportAdmins)
                {
                    _message.To.Add(supportAdmin);

                }
            }
            foreach (var admin in info.AdditionalAdministrators)
            {
                _message.Cc.Add(admin.Email);
                _admins.Append(admin.Name);
                _admins.Append(" ");
            }
            _message.SiteAdmin = _admins.ToString();
            EmailHelper.SendFailEmail(_message);
        }

    }
}
