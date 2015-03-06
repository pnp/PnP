using Framework.Provisioning.Azure;
using Framework.Provisioning.Core;
using Framework.Provisioning.Core.Authentication;
using Framework.Provisioning.Core.Configuration;
using Framework.Provisioning.Core.Configuration.Application;
using Framework.Provisioning.Core.Configuration.Template;
using Framework.Provisioning.Core.Data;
using Framework.Provisioning.Core.Utilities;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Provisioning.Job
{
    /// <summary>
    /// Site Provisioning Job to which handles processing new site requests in the Azure Queue
    /// </summary>
    public class SiteProvisioningJob
    {
        #region Instance Members
        ISiteRequestFactory _requestFactory;
        IConfigurationFactory _configFactory;
        ITemplateFactory _templateFactory;
        IAppSettingsManager _appManager;
        AppSettings _settings;
        AppOnlyAuthenticationTenant _auth = new AppOnlyAuthenticationTenant();
        ServiceBusManager _azureServiceManager = new ServiceBusManager();
        const string AZURECONNECTION_KEY = "ServiceBus.Connection";
        const string REQUESTQUEUENAME_KEY = "ServiceBus.RequestQueue";
        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        public SiteProvisioningJob()
        {
            this._requestFactory = SiteRequestFactory.GetInstance();
            this._configFactory = ConfigurationFactory.GetInstance();
            this._templateFactory = this._configFactory.GetTemplateFactory();
            _appManager = _configFactory.GetAppSetingsManager();
            _settings = _appManager.GetAppSettings();
        }
        #endregion

        #region Public Members
        /// <summary>
        /// TODO
        /// </summary>
        /// <exception cref="System.Configuration.ConfigurationErrorsException"></exception>
        public void ProcessRequestQueue()
        {
            _azureServiceManager.AzureConnectionString = ConfigurationManager.AppSettings[AZURECONNECTION_KEY];
            _azureServiceManager.RequestQueueName = ConfigurationManager.AppSettings[REQUESTQUEUENAME_KEY];
            if(string.IsNullOrEmpty(_azureServiceManager.AzureConnectionString))
            {
                throw new ConfigurationErrorsException(
                    string.Format("Azure Configuration {0} is missing in the config file", AZURECONNECTION_KEY));
            }

            if(string.IsNullOrEmpty(_azureServiceManager.RequestQueueName))
            {
                throw new ConfigurationErrorsException(
                   string.Format("Azure Configuration {0} is missing in the config file", REQUESTQUEUENAME_KEY));
            }
            var _message = _azureServiceManager.GetMessage();
            if (_message != null) {
                this.ProcessRequest(_message);
            }
            else {
                Log.Info("SiteProvisioningJob.ProcessRequestQueue", "There is no Site Request Messages pending in the queue {0}.", this._azureServiceManager.RequestQueueName);
            }
        }
        #endregion

        #region private members
        /// <summary>
        /// Private Member to process a new site collection request.
        /// </summary>
        /// <param name="requestMessage"></param>
        private void ProcessRequest(ProvisioningRequestMessage requestMessage)
        {
            var _siteRequestPayload = requestMessage.SiteRequest;
            if (!string.IsNullOrEmpty(_siteRequestPayload))
            {
                SiteRequestInformation _siteRequest = null;
                try
                {
                   _siteRequest = XmlSerializerHelper.Deserialize<SiteRequestInformation>(_siteRequestPayload);  
                }
                catch(Exception ex)
                {
                    Log.Fatal("SiteProvisioningJob.ProcessRequest", "There was an error {0} Deserializing the site request. The Message {1} is invalid and unable to be processed.", ex, _siteRequestPayload);
                    this.HandleFaultResponseMessage(requestMessage, ex);
                }
                if (_siteRequest != null)
                {
                    try
                    {
                        SiteProvisioningManager _manager = new SiteProvisioningManager();
                        _manager.CreateSiteCollection(_siteRequest);
                        this.HandleSuccessResponseMessage(requestMessage);
                    }
                    catch (Exception ex)
                    {
                        this.HandleFaultResponseMessage(requestMessage, ex);
                    }
                }
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="requestMessage"></param>
        private void HandleSuccessResponseMessage(ProvisioningRequestMessage requestMessage)
        {
            if (!string.IsNullOrEmpty(requestMessage.ReplyTo))
            {
                try
                {
                    ProvisioningResponseMessage _responseMessage = new ProvisioningResponseMessage();
                    _responseMessage.SiteRequest = requestMessage.SiteRequest;
                    this._azureServiceManager.SendReplyMessage(_responseMessage, requestMessage.ReplyTo);
                }
                catch(Exception ex)
                {
                    Log.Fatal("SiteProvisioningJob.HandleSuccessResponseMessage", "There was an error {0} sending response message {1} to subscriber {1}.", ex, requestMessage.SiteRequest, requestMessage.ReplyTo);
                }
            }
        }
        /// <summary>
        /// Send a Response Message on the azure queue.
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="ex"></param>
        private void HandleFaultResponseMessage(ProvisioningRequestMessage requestMessage, Exception ex)
        {
            if (!string.IsNullOrEmpty(requestMessage.ReplyTo))
            {
                try
                {
                    ProvisioningResponseMessage _responseMessage = new ProvisioningResponseMessage();
                    _responseMessage.SiteRequest = requestMessage.SiteRequest;
                    _responseMessage.IsFaulted = true;
                    _responseMessage.FaultMessage = ex.Message;
                    this._azureServiceManager.SendReplyMessage(_responseMessage, requestMessage.ReplyTo);
                }
                catch(Exception serviceException)
                {
                    Log.Fatal("SiteProvisioningJob.HandleFaultResponseMessage", "There was an error {0} sending response message {1} to subscriber {1}.", serviceException, requestMessage.SiteRequest, requestMessage.ReplyTo);
                }
            }
        }
        #endregion
    }
   
}
