using Framework.Provisioning.Core;
using Framework.Provisioning.Core.Authentication;
using Framework.Provisioning.Core.Configuration;
using Framework.Provisioning.Core.Configuration.Application;
using Framework.Provisioning.Core.Extensibility;
using Framework.Provisioning.Core.Utilities;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Framework.Provisioning.Extensiblity.Designer
{
    /// <summary>
    /// Sample Provider to Disable SharePoint Designer Settings
    /// </summary>
    public class DisableDesignerProvider : IPostProvisioningProvider
    {
        #region Instance Members
        /// <summary>
        /// We are going to use the existing configuration files 
        /// </summary>
        IConfigurationFactory _configFactory = ConfigurationFactory.GetInstance();
        AppSettings _settings = null;
        #endregion

        #region Constructor
        public DisableDesignerProvider()
        {
            IAppSettingsManager _appManager = _configFactory.GetAppSetingsManager();
            _settings = _appManager.GetAppSettings();
        }
        #endregion

        /// <summary>
        /// Used to Set AppOnlyAutehntication 
        /// SharePoint 
        /// </summary>
        public IAuthentication Authentication
        {
            get { return new AppOnlyAuthenticationTenant(); }
        }
    
        /// <summary>
        /// Member to disable SharePoint Designer.
        /// </summary>
        /// <param name="url"></param>
        protected void DisableDesigner(string url)
        {
            UsingContext(ctx =>
            {
                try
                {
                    Tenant tenant = new Tenant(ctx);
                    var site = tenant.GetSiteByUrl(url);
                    site.AllowDesigner = false;
                    //Allow Site Owners and Designers to Customize Master Pages and Page Layouts 
                    site.AllowMasterPageEditing = false;
                    //Allow Site Owners and Designers to Detach Pages from the Site Definition 
                    site.AllowRevertFromTemplate = false;
                    //Allow Site Owners and Designers to See the Hidden URL structure of their Web Site 
                    site.ShowUrlStructure = false;
                    ctx.ExecuteQuery();
                    Log.Info("Framework.Provisioning.Extensiblity.Designer.ProcessRequest", "Call Out completed. Disabled SharePoint Designer Settings.");
                }
                catch (Exception ex)
                {
                    Log.Fatal("Framework.Provisioning.Extensiblity.Designer.ProcessRequest", "Exception Disabled SharePoint Designer Settings. There error is {0}", ex);
                }

            });
        }

        #region IPostProvisioningProvider
        /// <summary>
        /// You must implement <see cref="Framework.Provisioning.Core.Extensibility.IPostProvisioningProvider"/> in order..
        /// </summary>
        /// <param name="request"></param>
        public void ProcessRequest(SiteRequestInformation request, string configuration)
        {   
            ///We dont have to worry about the configuration parm.
            Log.Info("Framework.Provisioning.Extensiblity.Designer.ProcessRequest", "I have received the request {0}", request.Url);
            //Here we can do our custom logic
            this.DisableDesigner(request.Url);
        }

        /// <summary>
        /// Delegate that is used to create a ClientContext
        /// </summary>
        /// <param name="action"></param>
        public void UsingContext(Action<ClientContext> action)
        {
            UsingContext(action, Timeout.Infinite);
        }

        /// <summary>
        /// Delegate that is used to create a ClientContext
        /// </summary>
        /// <param name="action"></param>
        public void UsingContext(Action<ClientContext> action, int csomTimeout)
        {
            using (ClientContext _ctx = Authentication.GetAuthenticatedContext())
            {
                _ctx.RequestTimeout = csomTimeout;
                action(_ctx);
            }
        }
        #endregion

    }
}
