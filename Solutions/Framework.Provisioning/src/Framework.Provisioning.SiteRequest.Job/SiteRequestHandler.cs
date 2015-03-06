using Framework.Provisioning.Azure;
using Framework.Provisioning.Core;
using Framework.Provisioning.Core.Data;
using Framework.Provisioning.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Provisioning.SiteRequest.Job
{
    /// <summary>
    /// SiteRequestHandler that is the subscriber to handle saving SiteRequest to the Azure Queue
    /// </summary>
    public class SiteRequestHandler
    {
        const string AZURECONNECTION_KEY = "ServiceBus.Connection";
        const string REQUESTQUEUENAME_KEY = "ServiceBus.RequestQueue";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="publisher"></param>
        public SiteRequestHandler(SiteRequestJob publisher)
        {
            publisher.ApprovedRequest += HandleNewSiteRequest;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HandleNewSiteRequest(object sender, SiteRequestEventArgs e)
        {
            var _siteRequest = e.SiteRequest;
            var _payload = XmlSerializerHelper.Serialize<SiteRequestInformation>(_siteRequest);
            ServiceBusManager _manager = new ServiceBusManager();
            _manager.AzureConnectionString = ConfigurationManager.AppSettings[AZURECONNECTION_KEY];
            _manager.RequestQueueName = ConfigurationManager.AppSettings[REQUESTQUEUENAME_KEY];

            if (string.IsNullOrEmpty(_manager.AzureConnectionString)) {
                throw new ConfigurationErrorsException(
                    string.Format("Azure Configuration {0} is missing in the config file", AZURECONNECTION_KEY));
            }

            if (string.IsNullOrEmpty(_manager.RequestQueueName)) {
                throw new ConfigurationErrorsException(
                   string.Format("Azure Configuration {0} is missing in the config file", REQUESTQUEUENAME_KEY));
            }

            try
            {
                //Save to the message to the queue.
                ProvisioningRequestMessage _message = new ProvisioningRequestMessage();
                _message.SiteRequest = _payload;
                _manager.SendProvisioningRequest(_message);
                //update the site request repostory so that we dont process again
                var _requestFactory = SiteRequestFactory.GetInstance();
                var _requestManager = _requestFactory.GetSiteRequestManager();
                _requestManager.UpdateRequestStatus(_siteRequest.Url, SiteRequestStatus.Pending);
            }
            catch(Exception ex)
            {
                Log.Fatal("Framework.Provisioning.SiteRequest.Job.HandleNewSiteRequest", "There was an error {0} processing the request {1} this request.", ex, _payload);
            }
        }
    }
}
