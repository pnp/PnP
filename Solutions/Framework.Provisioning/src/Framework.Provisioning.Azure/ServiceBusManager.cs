using Framework.Provisioning.Core.Utilities;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Provisioning.Azure
{
    /// <summary>
    /// Implementation class for working with Azure Service Bus and working with the Site Provisioning Request
    /// </summary>
    public class ServiceBusManager
    {
        #region instance Members
        private string _azureConnection;
        private string _requestQueueName;
        #endregion

        #region Properties
        /// <summary>
        /// Azure Connection String Property for working with the service bus
        /// </summary>
        public string AzureConnectionString
        {
            get
            {
                return _azureConnection;
            }
            set
            {
                this._azureConnection = value;
            }
        }
  
        /// <summary>
        /// Azure Request Queue Name that is used to send messages to the Request Queue for the provisioning engine.
        /// </summary>
        public string RequestQueueName
        {
            get
            {
                return _requestQueueName;
            }
            set
            {
                this._requestQueueName = value;
            }
        }

        #endregion

        /// <summary>
        /// Member to Send a Site Request Message to the Provisoning Engine.
        /// </summary>
        /// <param name="payload"></param>
        /// <exception cref="ConfigurationErrorsException"></exception>
        public void SendProvisioningRequest(ProvisioningRequestMessage payload)
        {
            var _nameSpaceManager = this.GetNameSpaceManager();

            if (string.IsNullOrEmpty(this.AzureConnectionString)) {
                throw new ConfigurationErrorsException(
                    string.Format("Azure Configuration {0} is missing in the config file"));
            }

            if (string.IsNullOrEmpty(this.RequestQueueName)){
                throw new ConfigurationErrorsException(
                   string.Format("Azure Configuration {0} is missing in the config file"));
            }

            try {
                QueueClient _client = this.GetQueueClient(_nameSpaceManager, this.RequestQueueName);
                using (BrokeredMessage _message = new BrokeredMessage(payload)) {
                    _client.Send(_message);
                    Log.Info("Framework.Provisioning.Azure.ServiceBusManager.SendMessage", "Successfully Sent Messages {0} to Queue {1} ", payload.SiteRequest, this.RequestQueueName);
                }
            }
            catch (Exception _ex)
            {
                Log.Info("Framework.Provisioning.Azure.ServiceBusManager.SendMessage",
                    "There was an Error Sending the Message {0} to Queue {1} Exception {2}",
                    payload,
                    this.RequestQueueName,
                    _ex);
                throw;
            }
        }

        /// <summary>
        /// Used to Send Response Messages from the Site Provisioning Engine.
        /// If replyTo is null or whitespace an ArguementException will be thrown
        /// </summary>
        /// <param name="message">The Response Message</param>
        /// <param name="replyTo">The Queue Name to send the response</param>
        /// <exception cref="ArgumentException">Occurs if a passed arguement is invalid</exception>
        public void SendReplyMessage(ProvisioningResponseMessage message, string replyTo)
        {
            if(string.IsNullOrEmpty(replyTo))
            {
                throw new ArgumentException("replyTo");
            }
            var _nameSpaceManager = this.GetNameSpaceManager();

            try
            {
                QueueClient _client = this.GetQueueClient(_nameSpaceManager, replyTo);
                using (BrokeredMessage _message = new BrokeredMessage(message))
                {
                    _client.Send(_message);
                    Log.Info("Framework.Provisioning.Azure.ServiceBusManager.SendMessage", 
                        "Successfully Sent Messages {0} to Queue {1} ", 
                        message.ToString(),
                        replyTo);
                }
            }
            catch (Exception _ex)
            {
                Log.Info("Framework.Provisioning.Azure.ServiceBusManager.SendMessage",
                    "There was an Error Sending the Message {0} to Queue {1} Exception {2}",
                    message,
                    replyTo,
                    _ex);
            }
        }

        /// <summary>
        /// Returns a Request Message from the Queue. The method will return null if no message exists
        /// or is not in a valid format.
        /// </summary>
        /// <returns>A Message object containing the RequestMessage.</returns>
        public ProvisioningRequestMessage GetMessage()
        {
            var _nameSpaceManager = this.GetNameSpaceManager();
            QueueClient _client = this.GetRequestQueueClientForRead(_nameSpaceManager, ReceiveMode.ReceiveAndDelete);

            var _message = _client.Receive(TimeSpan.FromSeconds(10));
            if(_message != null)
            {
                try { 

                    var _requestMessage = _message.GetBody<ProvisioningRequestMessage>();
                    //This will return null if its not our message we just ignore it
                    if(_requestMessage != null)
                    {
                        return _requestMessage;
                    }
                    else
                    {
                        //Message isnt our Request Type.
                        return null;
                    }
               
                }
                catch(Exception ex)
                {
                    Log.Error("ProvisioningRequestMessage", 
                        "There was an error processing this request. {0}", 
                        ex);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Private Member to Return the NameSpaceManager. AzureConnectionString Property is used to create
        /// the NamespaceManager
        /// </summary>
        /// <returns></returns>
        private NamespaceManager GetNameSpaceManager()
        {
            return NamespaceManager.CreateFromConnectionString(this.AzureConnectionString);
        }
       
        /// <summary>
        /// Returns a Azure QueueClient. If the Queue does not exist the queue will be created.
        /// </summary>
        /// <param name="nameSpaceManager">Azure NameSpaceManager</param>
        /// <param name="queueName">Queuename of the Azure Queue.</param>
        /// <returns></returns>
        private QueueClient GetQueueClient(NamespaceManager nameSpaceManager, string queueName)
        {
            if (!nameSpaceManager.QueueExists(this.RequestQueueName))
            {
                Log.Info("Framework.Provisioning.Azure.ServiceBusManager.SendMessage", "Queue {0} doesn't exist creating it", this.RequestQueueName);
                nameSpaceManager.CreateQueue(this.RequestQueueName);
                Log.Info("Framework.Provisioning.SiteRequest.Job.ServiceBusManager.SendMessage", "Successfully created Queue {0} ", this.RequestQueueName);
            }
            return QueueClient.CreateFromConnectionString(this.AzureConnectionString, queueName);
        }

        /// <summary>
        /// Gets the QueueClient for Reading operations. Uses RequestQueueName property to connect to the Queue.
        /// If the Queue does not exist the queue will be created.
        /// </summary>
        /// <param name="nameSpaceManager">Azure NameSpaceManager</param>
        /// <param name="mode">Azure ReceiveMode</param>
        /// <returns></returns>
        private QueueClient GetRequestQueueClientForRead(NamespaceManager nameSpaceManager, ReceiveMode mode)
        {
            if (!nameSpaceManager.QueueExists(this.RequestQueueName))
            {
                Log.Info("Framework.Provisioning.Azure.ServiceBusManager.SendMessage", "Queue {0} doesn't exist. Creating it", this.RequestQueueName);
                nameSpaceManager.CreateQueue(this.RequestQueueName);
                Log.Info("Framework.Provisioning.SiteRequest.Job.ServiceBusManager.SendMessage", "Successfully created Queue {0} ", this.RequestQueueName);
            }
            return QueueClient.CreateFromConnectionString(this.AzureConnectionString, this.RequestQueueName, mode);
        }

    }
}
