using Provisioning.Common;
using Provisioning.Common.Authentication;
using Provisioning.Common.Configuration;
using Provisioning.Common.Data;
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
        ITemplateFactory _templateFactory;
        #endregion

        #region Constructors
        public SiteProvisioningJob()
        {
            this._requestFactory = SiteRequestFactory.GetInstance();
            this._configFactory = ConfigurationFactory.GetInstance();
            this._templateFactory = this._configFactory.GetTemplateFactory();
        }
        #endregion

        public void BeginProcessing()
        {
            var _srManager = _requestFactory.GetSiteRequestManager();
            var _requests = _srManager.GetApprovedRequests();

            if(_requests.Count > 0)
            {
                this.ProvisionSites(_requests);
            }
        }

        public void ProvisionSites(ICollection<SiteRequestInformation> siterequests)
        {
            var _tm = this._templateFactory.GetTemplateManager();

            foreach (var siterequest in siterequests)
            {
                try 
                {
                    var _template = _tm.GetTemplateByName(siterequest.Template);
                    //NO TEMPLATE FOUND THAT MATCHES WE CANNOT PROVISION A SITE
                    if (_template == null) {
                       //TODO LOG
                    }

                    AbstractSiteProvisioningService _siteprovisioningService;
                    if (_template.SharePointOnPremises) {
                        _siteprovisioningService = new OnPremSiteProvisioningService();
                    }
                    else {
                        _siteprovisioningService = new Office365SiteProvisioningService();
                    }

                    _siteprovisioningService.Authentication = new AppOnlyAuthenticationTenant();
                    _siteprovisioningService.Authentication.TenantAdminUrl = _template.TenantAdminUrl;

                    var _web = _siteprovisioningService.CreateSiteCollection(siterequest, _template);
                }
                catch(Exception _ex)
                {

                }
               
            }
        }

    }
}
