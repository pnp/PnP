using Provisioning.Common;
using Provisioning.Common.Authentication;
using Provisioning.Common.Configuration;
using Provisioning.Common.Configuration.Application;
using Provisioning.Common.Data;
using Provisioning.Common.Data.SiteRequests;
using Provisioning.Common.Data.Templates;
using Provisioning.Common.Mail;
using Provisioning.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            Log.Info("Provisioning.Job.SiteProvisioningJob.ProcessSiteRequests", "Beginning Processing the site request repository");
            var _siteManager = _requestFactory.GetSiteRequestManager();
            var _requests = _siteManager.GetApprovedRequests();
            Log.Info("Provisioning.Job.SiteProvisioningJob.ProcessSiteRequests", "There is {0} site requests pending in the repository.", _requests.Count);
            if(_requests.Count > 0)
            {
                this.ProvisionSites(_requests);
            }
            else
            {
               Log.Info("Provisioning.Job.SiteProvisioningJob.ProcessSiteRequests", "There is no site requests pending in the repository");
            }
        }

        /// <summary>
        /// Member to handle provisioning sites
        /// </summary>
        /// <param name="siteRequests">The site request</param>
        public void ProvisionSites(ICollection<SiteInformation> siteRequests)
        {
            var _tm = this._siteTemplateFactory.GetManager();
            var _requestManager = this._requestFactory.GetSiteRequestManager();

            foreach (var siteRequest in siteRequests)
            {
                try 
                {
                    var _template = _tm.GetTemplateByName(siteRequest.Template);
              
                    if (_template == null)
                    {   
                        //NO TEMPLATE FOUND THAT MATCHES WE CANNOT PROVISION A SITE
                        var _message = string.Format("Template: {0} was not found for site {1}. Ensure that the template file exits.", siteRequest.Template, siteRequest.Url);
                        Log.Error("Provisioning.Job.SiteProvisioningJob.ProvisionSites", _message );
                        throw new ConfigurationErrorsException(_message);
                    }
                   
                    var _provisioningTemplate = _tm.GetProvisioningTemplate(_template.ProvisioningTemplate);
                    _requestManager.UpdateRequestStatus(siteRequest.Url, SiteRequestStatus.Processing);
                    SiteProvisioningManager _siteProvisioningManager = new SiteProvisioningManager(siteRequest, _template);
                    Log.Info("Provisioning.Job.SiteProvisioningJob.ProvisionSites", "Provisioning Site Request for Site Url {0}.", siteRequest.Url);
                
                    _siteProvisioningManager.CreateSiteCollection(siteRequest, _template);
                    _siteProvisioningManager.ApplyProvisioningTemplate(_provisioningTemplate, siteRequest);
                    this.SendSuccessEmail(siteRequest);
                    _requestManager.UpdateRequestStatus(siteRequest.Url, SiteRequestStatus.Complete);
                }
                catch(ProvisioningTemplateException _pte)
                {
                    _requestManager.UpdateRequestStatus(siteRequest.Url, SiteRequestStatus.CompleteWithErrors, _pte.Message);
                }
                catch(Exception _ex)
                {
                  _requestManager.UpdateRequestStatus(siteRequest.Url, SiteRequestStatus.Exception, _ex.Message);
                  this.SendFailureEmail(siteRequest, _ex.Message);
                }
            }
        }

        /// <summary>
        /// Sends a Notification that the Site was created
        /// </summary>
        /// <param name="info"></param>
        protected void SendSuccessEmail(SiteInformation info)
        {
            //TODO CLEAN UP EMAILS
            try
            { 
                StringBuilder _admins = new StringBuilder();
                SuccessEmailMessage _message = new SuccessEmailMessage();
                _message.SiteUrl = info.Url;
                _message.SiteOwner = info.SiteOwner.Name;
                _message.Subject = "Notification: Your new SharePoint site is ready";

                _message.To.Add(info.SiteOwner.Name);
                foreach (var admin in info.AdditionalAdministrators)
                {
                    _message.Cc.Add(admin.Name);
                    _admins.Append(admin.Name);
                    _admins.Append(" ");
                }
                _message.SiteAdmin = _admins.ToString();
                EmailHelper.SendNewSiteSuccessEmail(_message);
            }
            catch(Exception ex)
            {
                Log.Error("Provisioning.Job.SiteProvisioningJob.SendSuccessEmail",
                    "There was an error sending email. The Error Message: {0}, Exception: {1}", 
                     ex.Message,
                     ex);
         
            }
        }

        /// <summary>
        /// Sends an Failure Email Notification
        /// </summary>
        /// <param name="info"></param>
        /// <param name="errorMessage"></param>
        protected void SendFailureEmail(SiteInformation info, string errorMessage)
        {
            try
            {
                StringBuilder _admins = new StringBuilder();
                FailureEmailMessage _message = new FailureEmailMessage();
                _message.SiteUrl = info.Url;
                _message.SiteOwner = info.SiteOwner.Name;
                _message.Subject = "Alert: Your new SharePoint site request had a problem.";
                _message.ErrorMessage = errorMessage;
                _message.To.Add(info.SiteOwner.Name);

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
                    _message.Cc.Add(admin.Name);
                    _admins.Append(admin.Name);
                    _admins.Append(" ");
                }
                _message.SiteAdmin = _admins.ToString();
                EmailHelper.SendFailEmail(_message);
            }
            catch(Exception ex)
            {
                Log.Error("Provisioning.Job.SiteProvisioningJob.SendSuccessEmail",
                    "There was an error sending email. The Error Message: {0}, Exception: {1}",
                     ex.Message,
                     ex);
            }
          
        }

    }
}
