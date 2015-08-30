using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using Provisioning.Common.Authentication;
using Provisioning.Common.Configuration;
using Provisioning.Common.Configuration.Application;
using Provisioning.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Provisioning.Common.Data.Templates.Impl
{
    /// <summary>
    /// Implementation Class for working with Templates in XML Format
    /// </summary>
    internal class SPSiteTemplateManager : AbstractModule, ISiteTemplateManager, ISharePointClientService
    {
        const string LISTTITLE = "Templates";

        #region Constructor
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public SPSiteTemplateManager() : base()
        {  
          
        }
        #endregion

        #region ISiteTemplateManager Members
        public Template GetTemplateByName(string title)
        {
            return GetAvailableTemplates().Where(t => t.Title == title).FirstOrDefault();
        }

        public List<Template> GetAvailableTemplates()
        {
            List<Template> _templates = new List<Template>();
            UsingContext(ctx =>
            {
                var _web = ctx.Web;
                ctx.Load(_web);
                if (!_web.ListExists(LISTTITLE))
                {
                    var _message = String.Format("The List {0} does not exist in Site {1}",
                         LISTTITLE,
                         _web.Url);
                    Log.Fatal("SPSiteTemplateManager.GetAvailableTemplates", _message);
                    throw new DataStoreException(_message);
                }
            

                var _list = ctx.Web.Lists.GetByTitle(LISTTITLE);
                var _listItemCollection = _list.GetItems(CamlQuery.CreateAllItemsQuery());
                ctx.Load(_listItemCollection,
                     eachItem => eachItem.Include(
                     item => item,
                     item => item[TemplateFields.TTILE_NAME],
                     item => item[TemplateFields.DESCRIPTION_NAME],
                     item => item[TemplateFields.TEMPLATEIMAGE_NAME],
                    item => item[TemplateFields.HOSTPATH_NAME],
                    item => item[TemplateFields.TENANTURL_NAME],
                    item => item[TemplateFields.ONPREM_NAME],
                    item => item[TemplateFields.TEMPLATE_NAME],
                    item => item[TemplateFields.STORAGEMAX_NAME],
                    item => item[TemplateFields.STORAGEWARN_NAME],
                    item => item[TemplateFields.USERCODEMAX_NAME],
                    item => item[TemplateFields.USERCODEWARN_NAME],
                     item => item[TemplateFields.PROVISIONINGTEMPLATE_NAME],
                     item => item[TemplateFields.ENABLED_NAME],
                    item => item[TemplateFields.ROOTWEBONLY_NAME],
                    item => item[TemplateFields.SUBWEBONLY_NAME]

                     ));
                ctx.ExecuteQuery();

                foreach (var item in _listItemCollection)
                {
                    _templates.Add(new Template()
                    {
                        Title = item.BaseGet(TemplateFields.TTILE_NAME),
                        Description = item.BaseGet(TemplateFields.DESCRIPTION_NAME),
                        Enabled = item.BaseGet<bool>(TemplateFields.ENABLED_NAME),
                        ProvisioningTemplate = item.BaseGet(TemplateFields.PROVISIONINGTEMPLATE_NAME),
                       // ManagedPath = item.BaseGet(TemplateFields.MANAGEDPATH_NAME),
                        ImageUrl = item.BaseGet<FieldUrlValue>(TemplateFields.TEMPLATEIMAGE_NAME).Url,
                        TenantAdminUrl = item.BaseGet<FieldUrlValue>(TemplateFields.TENANTURL_NAME).Url,
                        HostPath = item.BaseGet<FieldUrlValue>(TemplateFields.HOSTPATH_NAME).Url,
                        RootWebOnly = item.BaseGet<bool>(TemplateFields.ROOTWEBONLY_NAME),
                        SubWebOnly = item.BaseGet<bool>(TemplateFields.SUBWEBONLY_NAME),
                        StorageMaximumLevel = item.BaseGetInt(TemplateFields.STORAGEMAX_NAME),
                        StorageWarningLevel = item.BaseGetInt(TemplateFields.STORAGEWARN_NAME),
                        UserCodeMaximumLevel = item.BaseGetInt(TemplateFields.USERCODEMAX_NAME),
                        UserCodeWarningLevel = item.BaseGetInt(TemplateFields.USERCODEWARN_NAME),
                        SharePointOnPremises = item.BaseGet<bool>(TemplateFields.ONPREM_NAME),
                        RootTemplate = item.BaseGet(TemplateFields.TEMPLATE_NAME)
                    });

                }
            });

            
            return _templates.Where(t => t.Enabled).ToList();
        }

        public List<Template> GetSubSiteTemplates()
        {
            //this.LoadXML();
            //var _t = _data.Templates.FindAll(t => t.RootWebOnly == false && t.Enabled == true);
            //return _t;


            throw new NotImplementedException();
        }

        public ProvisioningTemplate GetProvisioningTemplate(string name)
        {

            Log.Info("Retrieving SP stored template: ", name);
            try
            {
                ReflectionManager _reflectionHelper = new ReflectionManager();
                var _provider = _reflectionHelper.GetTemplateProvider(ModuleKeys.PROVISIONINGPROVIDER_KEY);
                if (_provider.Connector.GetType() == typeof(SharePointConnector))
                { 
                    _provider.Connector.AddParameter(SharePointConnector.CLIENTCONTEXT, Authentication.GetAuthenticatedContext());
                }

                var _pt = _provider.GetTemplate(name);

           

                return _pt;
            }
            catch (Exception _ex)
            {
                var _message = string.Format(PCResources.TemplateProviderBase_Exception_Message, _ex.Message);
                Log.Error("Provisioning.Common.Data.Templates.Impl.SPSiteTemplateManager", PCResources.TemplateProviderBase_Exception_Message, _ex);
                throw new DataStoreException(_message, _ex);
            }
        }
        #endregion
        #region ISharePointClientService Members
        /// <summary>
        /// Class used for working with the ClientContext
        /// </summary>
        /// <param name="action"></param>
        public virtual void UsingContext(Action<ClientContext> action)
        {
            UsingContext(action, Timeout.Infinite);
        }

        /// <summary>
        /// Class used for working with the ClientContext
        /// </summary>
        /// <param name="action"></param>
        /// <param name="csomTimeOut"></param>
        public virtual void UsingContext(Action<ClientContext> action, int csomTimeout)
        {
            using (ClientContext _ctx = Authentication.GetAuthenticatedContext())
            {
                _ctx.RequestTimeout = csomTimeout;
                action(_ctx);
            }
        }
        #endregion
        #region Properties
        /// <summary>
        /// Gets or Sets the services Authentication.
        /// </summary>
        public IAuthentication Authentication
        {
            get
            {
                var auth = new AppOnlyAuthenticationSite();
                auth.SiteUrl = this.ConnectionString;
                return auth;
            }
            
        }
        #endregion


    }
}
