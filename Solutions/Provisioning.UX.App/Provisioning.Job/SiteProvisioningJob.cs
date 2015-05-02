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
        ISiteTemplateFactory _siteTemplateFactory;
        IAppSettingsManager _appManager;
        AppSettings _settings;
        #endregion

        #region Constructors
        public SiteProvisioningJob()
        {
            this._configFactory = ConfigurationFactory.GetInstance();
            this._appManager = _configFactory.GetAppSetingsManager();
            this._settings = _appManager.GetAppSettings();
            this._requestFactory = SiteRequestFactory.GetInstance();
            this._siteTemplateFactory = SiteTemplateFactory.GetInstance();
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

        public void ProvisionSites(ICollection<SiteRequestInformation> siteRequests)
        {
            var _tm = this._siteTemplateFactory.GetManager();
            var _requestManager = this._requestFactory.GetSiteRequestManager();

            foreach (var siteRequest in siteRequests)
            {
                try 
                {
                    var _template = _tm.GetTemplateByName(siteRequest.Template);
                    var _provisioningTemplate = _tm.GetProvisioningTemplate(_template.ProvisioningTemplate);
                  
                    //NO TEMPLATE FOUND THAT MATCHES WE CANNOT PROVISION A SITE
                    if (_template == null) {
                       //TODO LOG
                    }

                    _requestManager.UpdateRequestStatus(siteRequest.Url, SiteRequestStatus.Processing);
                    SiteProvisioningManager _siteProvisioningManager = new SiteProvisioningManager(siteRequest, _template);
                    _siteProvisioningManager.ProcessSiteRequest(siteRequest, _template);
                    _siteProvisioningManager.ApplyProvisioningTemplates(_provisioningTemplate, siteRequest);
                    
                    this.SendSuccessEmail(siteRequest);
                    _requestManager.UpdateRequestStatus(siteRequest.Url, SiteRequestStatus.Complete);
                }
                catch(Exception _ex)
                {
                    _requestManager.UpdateRequestStatus(siteRequest.Url, SiteRequestStatus.Exception);
                    this.SendFailureEmail(siteRequest, _ex.Message);
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
