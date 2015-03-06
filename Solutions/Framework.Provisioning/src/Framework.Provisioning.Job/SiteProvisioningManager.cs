using Framework.Provisioning.Core;
using Framework.Provisioning.Core.Authentication;
using Framework.Provisioning.Core.Configuration;
using Framework.Provisioning.Core.Configuration.Application;
using Framework.Provisioning.Core.Configuration.Template;
using Framework.Provisioning.Core.Data;
using Framework.Provisioning.Core.Extensibility;
using Framework.Provisioning.Core.Mail;
using Framework.Provisioning.Core.Utilities;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Provisioning.Job
{
    /// <summary>
    /// Implementation class that works with the Framework engine to provision site collections.
    /// </summary>
    public class SiteProvisioningManager
    {
        #region Instance Members
        private IConfigurationFactory _configFactory = ConfigurationFactory.GetInstance();
        private AppSettings _settings;
        private IAppSettingsManager _appManager;
        ISiteRequestFactory _requestFactory;
        private AbstractProvisioningService _provisioningService;
        ITemplateFactory _templateFactory;
        private IAuthentication _auth;
        #endregion

        #region Private Members
        private void Init()
        {
            if (_settings.SharePointOnPremises)
            {
                this._provisioningService = new Office365ProvisioningService();
            }
            else
            {
                this._provisioningService = new OnPremProvisioningService();
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Property that is used for working with Authenication. 
        /// By default ApponlyAuthention will be used
        /// <seealso cref="Framework.Provisioning.Core.Authentication.IAuthentication"/>
        /// </summary>
        public IAuthentication Authentication
        {
            get
            {
                if(_auth == null)
                {
                    _auth = new AppOnlyAuthenticationTenant();
                }
                return _auth;
            }
            set
            {
                _auth = value;
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        public SiteProvisioningManager()
        {
            this._requestFactory = SiteRequestFactory.GetInstance();
            this._appManager = _configFactory.GetAppSetingsManager();
            this._settings = _appManager.GetAppSettings();
            this._templateFactory = this._configFactory.GetTemplateFactory();
        }
        #endregion

        #region Public Members
        /// <summary>
        /// Main Entry Point to Create Site Collection
        /// </summary>
        /// <param name="request">The Site Request to be processed.</param>
        public void CreateSiteCollection(SiteRequestInformation request)
        {
            var _siteRequestManager = this._requestFactory.GetSiteRequestManager();
            var _templateManager = this._templateFactory.GetTemplateManager();
            var _masterTemplate = _templateManager.GetTemplateByName(request.Template);

            if(_settings.SharePointOnPremises) {
                this._provisioningService = new OnPremProvisioningService();
                Log.Debug("Framework.Provisioning.Job.CreateSiteCollection", "Setting Provisioning Service to onpremises");
            }
            else {
                this._provisioningService = new Office365ProvisioningService();
                Log.Info("Framework.Provisioning.Job.CreateSiteCollection", "Setting Provisioning Service to Office365");
            }
            _provisioningService.Authentication = this.Authentication;
            
            try
            {
                if (_masterTemplate == null)
                {
                    Log.Warning("Framework.Provisioning.Job.CreateSiteCollection", "There is no master template defined for this site {0} ", request.Url );
                }
                else
                {
                    Log.Info("Framework.Provisioning.Job.CreateSiteCollection", "Processing Site Request for URL: {0}", request.Url);
                    _siteRequestManager.UpdateRequestStatus(request.Url, SiteRequestStatus.Processing);
                    var guid = _provisioningService.ProvisionSite(request);
                    // site is created now lets apply the site template settings
                    //get the site template
                    var _siteTemplate = _masterTemplate.GetSiteTemplate();
                    if(_siteTemplate == null)
                    {
                        Log.Warning("Framework.Provisioning.Job.CreateSiteCollection", "There is no Site Template Defined for {0} URL: {1}", request.Template, request.Url);
                    }
                    this.HandleSitePolicy(request, _siteTemplate);
                    this.SetAdditionalAdmins(request, _siteTemplate);
                    this.SetAdditionalOwners(request, _siteTemplate);
                    this.SetAdditionalMembers(request, _siteTemplate);
                    this.SetAdditionalVisitors(request, _siteTemplate);
                    this.ProcessFeatures(request, _siteTemplate);
                    this.DeployFields(request, _siteTemplate);
                    this.DeployContentTypes(request, _siteTemplate);
                    this.DeployLibraries(request, _siteTemplate);
                    this.ApplyBrandingToWeb(request, _masterTemplate);
                    this.DeployTemplateCustomActions(request, _siteTemplate);
                    this.PostProvisioningProviderCallOut(request, _siteTemplate);
                    _siteRequestManager.UpdateRequestStatus(request.Url, SiteRequestStatus.Complete);
                    this.SendSuccessEmail(request);
                    Log.Info("Framework.Provisioning.Job.CreateSiteCollection", "Completed Provisioning Site Collection URL: {0}", request.Url);
                }
            }
            catch(Exception ex)
            {
                Log.Fatal("Framework.Provisioning.Job.CreateSiteCollection", "There was an error provisioning Site Collection URL: {0}. Exception {1}", request.Url, ex);
                _siteRequestManager.UpdateRequestStatus(request.Url,SiteRequestStatus.Exception, ex.Message);
                this.SendFailureEmail(request, ex.Message);
                throw;
            }
        }
        #endregion

        #region Provisioning Private Members
        /// <summary>
        /// Handles appling the Site Policy to the newly created Site Collection.
        /// If the info.SitePolicy is an empty string, the Policy will be default to the Site Template.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="siteTemplate"></param>
        protected void HandleSitePolicy(SiteRequestInformation info, SiteTemplate siteTemplate)
        {
            //If info.SitePolicy has a value we will not apply the Site Policy that is define in the template.
            //If not we will default site policy that that is define in site template default policy to the site collection.
            if(string.IsNullOrEmpty(info.SitePolicy))
            {
                if (siteTemplate != null)
                {
                    if (!string.IsNullOrEmpty(siteTemplate.DefaultSitePolicy))
                    {
                        this._provisioningService.ApplySitePolicy(info.Url, siteTemplate.DefaultSitePolicy);
                        Log.Info("Framework.Provisioning.Job.HandleSitePolcy", "Setting site policy {0} on site collection: {1}", siteTemplate.DefaultSitePolicy, info.Url);
                    }
                }
            }
            else
            {
                Log.Info("Framework.Provisioning.Job.HandleSitePolicy", "Site Request has Site Policy {0} on site collection: {1}", siteTemplate.DefaultSitePolicy, info.Url);
            }
        }
       
        /// <summary>
        /// Deploys Custom Fields to the newly created site collection 
        /// </summary>
        /// <param name="info">The SiteRequestInformation Object. The site to process</param>
        /// <param name="siteTemplate"></param>
        protected void DeployFields(SiteRequestInformation info, SiteTemplate siteTemplate)
        {   
            if (siteTemplate != null)
            {
                foreach (var _fields in siteTemplate.SiteFields)
                {
                    try
                    {
                        this._provisioningService.DeployFields(info.Url, _fields.SchemaXml);
                        Log.Info("Framework.Provisioning.Job.DeployFields", "Provisioning Fields at: {0} Fields: {1}", info.Url, _fields.SchemaXml);
                    }
                    catch(Exception ex)
                    {
                        Log.Error("Framework.Provisioning.Job.DeployFields", "There was an error Provisioning Fields at: {0} Fields: {1} Exception: {2}", info.Url, _fields.SchemaXml, ex);
                    }
                }
            }
        }
     
        /// <summary>
        /// Deploys Content Types defined in the SiteTemplate
        /// </summary>
        /// <param name="info"></param>
        /// <param name="siteTemplate"></param>
        protected void DeployContentTypes(SiteRequestInformation info, SiteTemplate siteTemplate)
        {
            if (siteTemplate != null)
            {
                foreach (var contentTypes in siteTemplate.ContentTypes)
                {
                    try
                    {
                        this._provisioningService.DeployContentType(info.Url, contentTypes.SchemaXml);
                        Log.Info("Framework.Provisioning.Job.DeployContentTypes", "Provisioning Content Types at: {0} Content Type Schema: {1}", info.Url, contentTypes.SchemaXml);
                    }
                    catch(Exception ex)
                    {
                        Log.Error("Framework.Provisioning.Job.DeployContentTypes", "There was an error provisioning Content at: {0} Content Type Schema: {1} Exception: {2}", info.Url, contentTypes.SchemaXml, ex);
                    }
                }
            }
        }
       
        /// <summary>
        /// Deploys Libraries that are defined in the SiteTemplate.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="siteTemplate"></param>
        protected void DeployLibraries(SiteRequestInformation info, SiteTemplate siteTemplate)
        {
            if (siteTemplate != null)
            {
                foreach (var list in siteTemplate.ListInstances)
                {
                    try {
                        
                        this._provisioningService.DeployList(info.Url, list);
                        Log.Info("Framework.Provisioning.Job.DeployLibraries", "Provisioning Library at: {0} Library: {1}", info.Url, list.Title);
                    }
                    catch(Exception ex)
                    {
                        Log.Error("Framework.Provisioning.Job.DeployLibraries", "There was an error provisioning Library at: {0} Library: {1} Exception {2}", info.Url, list.Title, ex);
                    }
                }
            }
        }

        /// <summary>
        /// Active or deactivates OOB features based on the SiteTemplate
        /// </summary>
        /// <param name="info"></param>
        /// <param name="template"></param>
        protected void ProcessFeatures(SiteRequestInformation info, SiteTemplate siteTemplate)
        {
            if (siteTemplate != null)
            {
                var _features = siteTemplate.Features;
                if (_features != null)
                {
                    foreach (var siteFeature in _features.SiteFeatures)
                    {
                        Guid featureGuid;
                        if (Guid.TryParse(siteFeature.ID, out featureGuid))
                        {
                            if (siteFeature.Deactivate)
                            {
                                this._provisioningService.DeactivateSiteFeature(info.Url, featureGuid);
                                Log.Info("Framework.Provisioning.Job.ProcessFeatures", "Deactivating Site Feature ID {0} Site : {1}", featureGuid, info.Url);
                            }
                            else
                            {
                                this._provisioningService.ActivateSiteFeature(info.Url, featureGuid);
                                Log.Info("Framework.Provisioning.Job.ProcessFeatures", "Activating Site Feature ID {0} Site : {1}", featureGuid, info.Url);
                            }
                        }
                        else
                        {
                            Log.Warning("Framework.Provisioning.Job.ProcessFeatures", "Invalid Guid is defined in the Site Template. Features.SiteFeatures The ID {0}",  siteFeature.ID);
                        }
                    }
                    foreach (var webFeature in _features.WebFeatures)
                    {
                        Guid featureGuid;
                        if (Guid.TryParse(webFeature.ID, out featureGuid))
                        {
                            if (webFeature.Deactivate)
                            {
                                this._provisioningService.DeactivateWebFeature(info.Url, featureGuid);
                                Log.Info("Framework.Provisioning.Job.ProcessFeatures", "Dectivating Web Feature ID {0} Site : {1}", featureGuid, info.Url);
                            }
                            else
                            {
                                this._provisioningService.ActivateWebFeature(info.Url, featureGuid);
                                Log.Info("Framework.Provisioning.Job.ProcessFeatures", "Activating Web Feature ID {0} Site : {1}", featureGuid, info.Url);
                            }
                        }
                        else
                        {
                            Log.Warning("Framework.Provisioning.Job.ProcessFeatures", "Invalid Guid is defined in the Site Template. Features.WebFeatures The ID {0}", webFeature.ID);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Applies Branding Artificats to the Site that is defined in the Master Template
        /// </summary>
        /// <param name="info"></param>
        /// <param name="template"></param>
        protected void ApplyBrandingToWeb(SiteRequestInformation info, Template template)
        {
            var _tm = this._templateFactory.GetTemplateManager();
            var _tp = _tm.GetBrandingPackageByName(template.BrandingPackage);
            if (_tp != null)
            {
                this._provisioningService.DeployTheme(info.Url, _tp);
                Log.Info("Framework.Provisioning.Job.ApplyBrandingToWeb", "Deployed branding packages {0} for site {1}", template.BrandingPackage, info.Url);
            }
            else
            {
                Log.Info("Framework.Provisioning.Job.ApplyBrandingToWeb", "There is no branding package defined for template {0}", template.Name);
            }
        }

        /// <summary>
        /// Assigns additional administrators to the site that is defined in the SiteTemplate.
        /// You can also define custom business logic in this member.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="template"></param>
        protected void SetAdditionalAdmins(SiteRequestInformation info, SiteTemplate siteTemplate)
        {
            if (siteTemplate != null)
            {
                var users = siteTemplate.Security.AdditionalAdministrators;
                //you can add additional logic here if you wanted to add more users
                if (users.Count > 0)
                {
                    this._provisioningService.AddAdditionalAdministrators(users, new Uri(info.Url));
                    Log.Info("Framework.Provisioning.Job.SetAdditionalAdmins", "Adding Additional Administrators for site {0}", info.Url);
                }
            }
        }

        /// <summary>
        /// Assigns additional site owners to the site that is defined in the SiteTemplate.
        /// You can also define custom business logic in this member.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="template"></param>
        protected void SetAdditionalOwners(SiteRequestInformation info, SiteTemplate siteTemplate)
        {
            if (siteTemplate != null)
            {
                var users = siteTemplate.Security.AdditionalOwners;
                //you can add additional logic here if you wanted to add more users
                if (users.Count > 0)
                {
                    this._provisioningService.AddAdditionalOwners(users, new Uri(info.Url));
                    Log.Info("Framework.Provisioning.Job.SetAdditionalOwners", "Adding Additional Owners for site {0}", info.Url);
                }
            }
        }

        /// <summary>
        /// Assigns additional site members to the site that is defined in the SiteTemplate.
        /// You can also define custom business logic in this member.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="template"></param>
        protected void SetAdditionalMembers(SiteRequestInformation info, SiteTemplate siteTemplate)
        {
            if (siteTemplate != null)
            {
                var users = siteTemplate.Security.AdditionalMembers;
                //you can add additional logic here if you wanted to add more users
                if (users.Count > 0)
                {
                    this._provisioningService.AddAdditionalMembers(users, new Uri(info.Url));
                    Log.Info("Framework.Provisioning.Job.SetAdditionalMembers", "Adding Members for site {0}", info.Url);
                }
            }
        }

        /// <summary>
        /// Assigns additional site visitors to the site that is defined in the SiteTemplate.
        /// You can also define custom business logic in this member.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="template"></param>
        protected void SetAdditionalVisitors(SiteRequestInformation info, SiteTemplate siteTemplate)
        {
            if (siteTemplate != null)
            {
                var users = siteTemplate.Security.AdditionalVisitors;
                //you can add additional logic here if you wanted to add more users
                if (users.Count > 0)
                {
                    this._provisioningService.AddAdditionalVisitors(users, new Uri(info.Url));
                    Log.Info("Framework.Provisioning.Job.SetAdditionalVisitors", "Adding Visitors for site {0}", info.Url);
                }
            }
        }
      
        /// <summary>
        /// Deploys Custom Actions to the site collection that is define in the Site Template
        /// </summary>
        /// <param name="info"></param>
        /// <param name="template"></param>
        protected void DeployTemplateCustomActions(SiteRequestInformation info, SiteTemplate siteTemplate)
        {
            if (siteTemplate != null)
            {
                var _customActions = siteTemplate.CustomActions;
                if(_customActions != null)
                {
                    this.DeploySiteCustomActions(info, _customActions.SiteCustomActions);
                    this.DeployWebCustomActions(info, _customActions.WebCustomActions);
                }
            }
        }
        /// <summary>
        /// Deploys Site Custom Actions
        /// </summary>
        /// <param name="info"></param>
        /// <param name="customActions"></param>
        protected void DeploySiteCustomActions(SiteRequestInformation info, List<CustomAction> customActions)
        {
            foreach (var _customAction in customActions)
            {
                if (_customAction.Enabled == true)
                {
                    var caEntity = new CustomActionEntity()
                    {
                        Name = _customAction.Name,
                        Title = _customAction.Title,
                        Description = _customAction.Description,
                    };

                    if (_customAction.Location == JavaScriptExtensions.SCRIPT_LOCATION)
                    {
                        caEntity.ScriptBlock = _customAction.ScriptBlock;
                        caEntity.ScriptSrc = _customAction.ScriptSrc;
                    }
                    else
                    {
                        caEntity.Group = _customAction.Group;
                        caEntity.Location = _customAction.Location;
                        caEntity.Sequence = _customAction.Sequence;
                        caEntity.Url = string.Format(_customAction.Url, info.Url);
                        try
                        {
                            if (Enum.IsDefined(typeof(PermissionKind), _customAction.Rights))
                            {
                                BasePermissions _permissions = new BasePermissions();
                                var _permissionKind = (PermissionKind)_customAction.Rights;
                                _permissions.Set(_permissionKind);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Warning("Framework.Provisioning.Job.DeploySiteCustomActions", "There was an error processing the CustomAction.Rights please validate your template.Custom Action {0}, Rights {1}, Exception {2}",
                            _customAction.Name,
                            _customAction.Rights,
                            ex);
                        }
                    }
                    try
                    {
                        this._provisioningService.DeploySiteCustomAction(info.Url, caEntity);
                        Log.Info("Framework.Provisioning.Job.DeploySiteCustomActions", "Deploying Custom Action {0} to site collection {1}", caEntity.Name, info.Url);
                    }
                    catch(Exception ex)
                    {
                        Log.Error("Framework.Provisioning.Job.DeploySiteCustomActions", "There was an error deploying Custom Action {0} to site collection {1} Exception {2}", caEntity.Name, info.Url, ex);
                    }
                }
            }
        }
       
        /// <summary>
        /// Deploys Web custom Actions
        /// </summary>
        /// <param name="info"></param>
        /// <param name="customActions"></param>
        protected void DeployWebCustomActions(SiteRequestInformation info, List<CustomAction> customActions)
        {
            foreach (var _customAction in customActions)
            {
                if (_customAction.Enabled == true)
                {
                    var caEntity = new CustomActionEntity()
                    {
                        Name = _customAction.Name,
                        Title = _customAction.Title,
                        Description = _customAction.Description,
                        
                    };

                    if(_customAction.Location == JavaScriptExtensions.SCRIPT_LOCATION)
                    {
                        caEntity.ScriptBlock = _customAction.ScriptBlock;
                        caEntity.ScriptSrc = _customAction.ScriptSrc;
                    }
                    else
                    {
                        caEntity.Group = _customAction.Group;
                        caEntity.Location = _customAction.Location;
                        caEntity.Sequence = _customAction.Sequence;
                        caEntity.Url = string.Format(_customAction.Url, info.Url);
                        try
                        {
                            if (Enum.IsDefined(typeof(PermissionKind), _customAction.Rights))
                            {
                                BasePermissions _permissions = new BasePermissions();
                                var _permissionKind = (PermissionKind)_customAction.Rights;
                                _permissions.Set(_permissionKind);
                            }
                        }
                        catch(Exception ex)
                        {
                            Log.Error("Framework.Provisioning.Job.DeployWebCustomActions", "There was an error processing the CustomAction.Rights please validate your template.Custom Action {0}, Rights {1}, Exception {2}",
                            _customAction.Name,
                            _customAction.Rights,
                            ex);
           
                        }
                    }

                    try
                    {
                        this._provisioningService.DeployWebCustomAction(info.Url, caEntity);
                        Log.Info("Framework.Provisioning.Job.DeployWebCustomActions", "Deploying Custom Action {0} to site collection {1}", caEntity.Name, info.Url);
                    }
                    catch(Exception ex)
                    {
                        Log.Error("Framework.Provisioning.Job.DeployWebCustomActions", "There was an error deploying Custom Action {0} to site collection {1} Exception {2}", caEntity.Name, info.Url, ex);
                    }
                }
              }
        }

        /// <summary>
        /// Used to call out to the Extensiblity PipeLine
        /// </summary>
        /// <param name="info"></param>
        /// <param name="siteTemplate"></param>
        protected void PostProvisioningProviderCallOut(SiteRequestInformation info, SiteTemplate siteTemplate)
        {
            if (siteTemplate != null)
            {
                Log.Info("Framework.Provisioning.Job.PostProvisioningProviderCallOut", "In Extensibility Pipeline");
                foreach(var ppProvider in siteTemplate.Providers)
                {
                    if(ppProvider.Enabled)
                    {
                        PostProvisioningManager _extManager = new PostProvisioningManager();
                        if (!string.IsNullOrEmpty(ppProvider.Assembly) && !string.IsNullOrEmpty(ppProvider.Type))
                        {
                            try
                            {
                                _extManager.Execute(ppProvider, info);
                                Log.Info("Framework.Provisioning.Job.PostProvisioningProviderCallOut", "Provider callout Assembly {0} Type {1}.", ppProvider.Assembly, ppProvider.Type);
                            }
                            catch(Exception ex)
                            {
                                Log.Fatal("Framework.Provisioning.Job.PostProvisioningProviderCallOut", "There was an exception while invocating the custom provider. Assembly {0}, Type {1} Exception is {2}.", ppProvider.Assembly, ppProvider.Type, ex);
                            }
                        }
                        else
                        {
                            Log.Fatal("Framework.Provisioning.Job.PostProvisioningProviderCallOut", "Either Provider.Assembly or Provider.Type was an empty string defined in the template. Skipping");
                        }
                    }
                    else{
                        Log.Info("Framework.Provisioning.Job.PostProvisioningProviderCallOut", "Provider {0}, is disabled. Skipping", ppProvider.Assembly);
                    }
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
            foreach(var admin in info.AdditionalAdministrators)
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
            
            if(!string.IsNullOrEmpty(this._settings.SupportEmailNotification))
            {
                string[] supportAdmins = this._settings.SupportEmailNotification.Split(';');
                foreach(var supportAdmin in supportAdmins)
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

        #endregion
    }
}
