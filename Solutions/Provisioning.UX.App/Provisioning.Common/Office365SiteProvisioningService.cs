using Provisioning.Common.Authentication;
using Provisioning.Common.Configuration;
using Provisioning.Common.Configuration.Application;
using Provisioning.Common.Utilities;
using Provisioning.Common.Data;
using Provisioning.Common.Data.Templates;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.Online.SharePoint.TenantManagement;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Net;
using System.ServiceModel;




namespace Provisioning.Common
{
    /// <summary>
    /// Implementation class for Provisioning an Office 365 Site.
    /// </summary>
    public class Office365SiteProvisioningService : AbstractSiteProvisioningService
    {
        #region Private Instance Members
        private int _retryCount = 3;        
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Office365SiteProvisioningService() : base()
        {
        }
        #endregion

        public override void CreateSiteCollection(SiteInformation siteRequest, Template template)
        {
            Log.Info("Provisioning.Common.Office365SiteProvisioningService.CreateSiteCollection", PCResources.SiteCreation_Creation_Starting, siteRequest.Url);

            UsingContext(ctx =>
            {
                try
                {
                    Stopwatch _timespan = Stopwatch.StartNew();                    

                    Tenant _tenant = new Tenant(ctx);
                    var _newsite = new SiteCreationProperties();
                    _newsite.Title = siteRequest.Title;                    
                    _newsite.Url = siteRequest.Url;
                    _newsite.Owner = siteRequest.SiteOwner.Email;
                    _newsite.Template = template.RootTemplate;
                    _newsite.Lcid = siteRequest.Lcid;
                    _newsite.TimeZoneId = siteRequest.TimeZoneId;
                    _newsite.StorageMaximumLevel = template.StorageMaximumLevel;
                    _newsite.StorageWarningLevel = template.StorageWarningLevel;
                    _newsite.UserCodeMaximumLevel = template.UserCodeMaximumLevel;
                    _newsite.UserCodeMaximumLevel = template.UserCodeWarningLevel;


                    try
                    {
                        SpoOperation _spoOperation = _tenant.CreateSite(_newsite);
                        ctx.Load(_tenant);
                        ctx.Load(_spoOperation);
                        ctx.ExecuteQuery();

                        try
                        {
                            this.OperationWithRetry(ctx, _spoOperation, siteRequest);
                        }
                        catch(ServerException ex)
                        {
                            var _message = string.Format("Error occured while provisioning site {0}, ServerErrorTraceCorrelationId: {1} Exception: {2}", siteRequest.Url, ex.ServerErrorTraceCorrelationId, ex);
                            Log.Error("Provisioning.Common.Office365SiteProvisioningService.CreateSiteCollection", _message);
                            throw;
                        }
                                                

                    }
                    catch (ServerException ex)
                    {
                        var _message = string.Format("Error occured while provisioning site {0}, ServerErrorTraceCorrelationId: {1} Exception: {2}", siteRequest.Url, ex.ServerErrorTraceCorrelationId, ex);
                        Log.Error("Provisioning.Common.Office365SiteProvisioningService.CreateSiteCollection", _message);
                        throw;
                    }


                    var _site = _tenant.GetSiteByUrl(siteRequest.Url);
                    var _web = _site.RootWeb;
                    _web.Description = siteRequest.Description;
                    _web.Update();
                    ctx.Load(_web);
                    ctx.ExecuteQuery();
                    User newOwner = _web.EnsureUser(siteRequest.SiteOwner.Email);
                    ctx.Load(newOwner);
                    ctx.ExecuteQuery();

                    if (!newOwner.ServerObjectIsNull.Value)
                    {
                        //_site.Owner = newOwner;
                        //ctx.Load(_site);
                        //ctx.Load(_site.Owner);
                        //ctx.ExecuteQuery();
                        newOwner.IsSiteAdmin = true;
                        newOwner.Update();
                        ctx.Load(newOwner);
                        ctx.ExecuteQuery();
               }
                    _timespan.Stop();
                    Log.TraceApi("SharePoint", "Office365SiteProvisioningService.CreateSiteCollection", _timespan.Elapsed, "SiteUrl={0}", siteRequest.Url);
                }

                catch (Exception ex)
                {
                    Log.Error("Provisioning.Common.Office365SiteProvisioningService.CreateSiteCollection",
                        PCResources.SiteCreation_Creation_Failure,
                        siteRequest.Url, ex.Message, ex);
                    throw;
                }
                Log.Info("Provisioning.Common.Office365SiteProvisioningService.CreateSiteCollection", PCResources.SiteCreation_Creation_Successful, siteRequest.Url);
            }, SPDataConstants.CSOM_WAIT_TIME);
        }

        public override Web CreateSubSite(SiteInformation siteRequest, Template template)
        {
            Web newWeb;
            int pos = siteRequest.Url.LastIndexOf("/");
            string parentUrl = siteRequest.Url.Substring(0, pos);
            string subSiteUrl = siteRequest.Url.Substring(pos + 1);

            Log.Info("Provisioning.Common.Office365SiteProvisioningService.CreateSubSite", PCResources.SiteCreation_Creation_Starting, siteRequest.Url);
            Uri siteUri = new Uri(siteRequest.Url);
            Uri subSiteParent = new Uri(parentUrl);

            string realm = TokenHelper.GetRealmFromTargetUrl(subSiteParent);
            string accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, subSiteParent.Authority, realm).AccessToken;

            using (var ctx = TokenHelper.GetClientContextWithAccessToken(parentUrl, accessToken))
            {
                try
                {
                    Stopwatch _timespan = Stopwatch.StartNew();                  

                    try
                    {
                        // Get a reference to the parent Web
                        Web parentWeb = ctx.Web;

                        // Create the new sub site as a new child Web
                        WebCreationInformation webinfo = new WebCreationInformation();
                        webinfo.Description = siteRequest.Description;
                        webinfo.Language = (int)siteRequest.Lcid;
                        webinfo.Title = siteRequest.Title;
                        webinfo.Url = subSiteUrl;
                        webinfo.UseSamePermissionsAsParentSite = true;
                        webinfo.WebTemplate = template.RootTemplate;  

                        newWeb = parentWeb.Webs.Add(webinfo);
                        ctx.ExecuteQueryRetry();
                        
                    }
                    catch (ServerException ex)
                    {
                        var _message = string.Format("Error occured while provisioning site {0}, ServerErrorTraceCorrelationId: {1} Exception: {2}", siteRequest.Url, ex.ServerErrorTraceCorrelationId, ex);
                        Log.Error("Provisioning.Common.Office365SiteProvisioningService.CreateSubSite", _message);
                        throw;
                    }
                    
                    _timespan.Stop();
                    Log.TraceApi("SharePoint", "Office365SiteProvisioningService.CreateSubSite", _timespan.Elapsed, "SiteUrl={0}", siteRequest.Url);
                }

                catch (Exception ex)
                {
                    Log.Error("Provisioning.Common.Office365SiteProvisioningService.CreateSubSite",
                        PCResources.SiteCreation_Creation_Failure,
                        siteRequest.Url, ex.Message, ex);
                    throw;
                }
                Log.Info("Provisioning.Common.Office365SiteProvisioningService.CreateSubSite", PCResources.SiteCreation_Creation_Successful, siteRequest.Url);
                
            };

            return newWeb;
        }

        private void OperationWithRetry(ClientContext ctx, SpoOperation operation, SiteInformation siteRequest)
        {
            int currentRetry = 0;
            for (;;)
            {
                try
                {
                    System.Threading.Thread.Sleep(30000);
                    ctx.Load(operation);
                    ctx.ExecuteQuery();
                    Log.Info("Provisioning.Common.Office365SiteProvisioningService.CreateSiteCollection", "Waiting for Site Collection {0} to be created", siteRequest.Url);
                    if (operation.IsComplete) break;
                }
                catch (Exception ex)
                {
                    currentRetry++;

                    if (currentRetry > this._retryCount || !IsTransientException(ex))
                    {
                        throw;
                    }
                }
            }
        }

        private bool IsTransientException(Exception ex)
        {
            if (ex is ServerTooBusyException) return true;

            var webException = ex as WebException;
            if (webException != null)
            {
                // If the web exception contains one of the following status values it may be transient.
                return new[] {WebExceptionStatus.ConnectionClosed,
                  WebExceptionStatus.Timeout,
                  WebExceptionStatus.RequestCanceled }.
                        Contains(webException.Status);
            }

            return false;
        }
        /// <summary>
        /// Used to set External Sharing
        /// </summary>
        /// <param name="siteInfo"></param>
        public override void SetExternalSharing(SiteInformation siteInfo)
        {
            UsingContext(ctx =>
            {
                try
                {
                    Stopwatch _timespan = Stopwatch.StartNew();

                    Tenant _tenant = new Tenant(ctx);

                    //_tenant.SetSiteProperties(siteInfo.Url, null, null, SharingCapabilities.ExternalUserSharingOnly, null, null, null, null);
                    SiteProperties _siteProps = _tenant.GetSitePropertiesByUrl(siteInfo.Url, false);
                    ctx.Load(_tenant);
                    ctx.Load(_siteProps);
                    ctx.ExecuteQuery();
                    
                    var _tenantSharingCapability = _tenant.SharingCapability;
                    var _siteSharingCapability = _siteProps.SharingCapability;
                    var _targetSharingCapability = SharingCapabilities.Disabled;

                    if(!siteInfo.EnableExternalSharing && _tenantSharingCapability != SharingCapabilities.Disabled)
                    {
                        _targetSharingCapability = SharingCapabilities.Disabled;                        

                        _siteProps.SharingCapability = _targetSharingCapability;
                        _siteProps.Update();
                        ctx.ExecuteQuery();
                        Log.Info("Provisioning.Common.Office365SiteProvisioningService.SetExternalSharing", PCResources.ExternalSharing_Successful, siteInfo.Url);
                    }
                    if (siteInfo.EnableExternalSharing && _tenantSharingCapability != SharingCapabilities.Disabled)
                    {
                        _targetSharingCapability = SharingCapabilities.ExternalUserSharingOnly;                        

                        _siteProps.SharingCapability = _targetSharingCapability;
                        _siteProps.Update();
                        ctx.ExecuteQuery();
                        Log.Info("Provisioning.Common.Office365SiteProvisioningService.SetExternalSharing", PCResources.ExternalSharing_Successful, siteInfo.Url);
                    }

                    _timespan.Stop();
                    Log.TraceApi("SharePoint", "Office365SiteProvisioningService.SetExternalSharing", _timespan.Elapsed, "SiteUrl={0}", siteInfo.Url);
       
                   
                }
                catch(ServerException _ex)
                {
                    Log.Info("Provisioning.Common.Office365SiteProvisioningService.SetExternalSharing", PCResources.ExternalSharing_Exception, siteInfo.Url, _ex);
     
                }
                catch(Exception _ex)
                {
                    Log.Info("Provisioning.Common.Office365SiteProvisioningService.SetExternalSharing", PCResources.ExternalSharing_Exception, siteInfo.Url, _ex);
                }
             
            });
        }

        
    }
}
