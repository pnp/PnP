using Framework.Provisioning.Core.Authentication;
using Framework.Provisioning.Core.Configuration;
using Framework.Provisioning.Core.Data;
using Framework.Provisioning.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Provisioning.SiteRequest.Job
{
    /// <summary>
    /// Timer Job implementation to store the Site Request Job to the Azure Queue for processing by the 
    /// Framework.Provisioning.Job Timer Job.
    /// TODO Docs on V2 which will use Timer Job Framework
    /// </summary>
    public class SiteRequestJob
    {
        #region Instance Members
        ISiteRequestFactory _requestFactory;
        IConfigurationFactory _configFactory;
        public EventHandler<SiteRequestEventArgs> ApprovedRequest;
        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        public SiteRequestJob()
        {
            this._requestFactory = SiteRequestFactory.GetInstance();
            this._configFactory = ConfigurationFactory.GetInstance();
        }
        #endregion 
        
        #region Public Members
        /// <summary>
        /// Used to Process Approved Site Requests in the Site Repository Datastore.
        /// </summary>
        public void ProcessSiteRequests()
        {
            SiteRequestHandler _handler = new SiteRequestHandler(this);
            Log.Info("Framework.Provisioning.SiteRequest.Job.SiteRequestJob.ProcessSiteRequests", "Beginning check for Approved site requests");
            var _siteRequestManager = _requestFactory.GetSiteRequestManager();
            var _siteRequests = _siteRequestManager.GetApprovedRequests();
            foreach(var _siteRequest in _siteRequests)
            { 
                OnNewApprovedRequest(new SiteRequestEventArgs(_siteRequest));
            }
            Log.Info("Framework.Provisioning.SiteRequest.Job.SiteRequestJob.ProcessSiteRequests", "There are {0} site requests approved.", _siteRequests.Count);
        }
        #endregion

        #region Protected Members
        /// <summary>
        /// Event Handler
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnNewApprovedRequest(SiteRequestEventArgs e)
        {
            EventHandler<SiteRequestEventArgs> handler = ApprovedRequest;
            if(handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

    }
}
