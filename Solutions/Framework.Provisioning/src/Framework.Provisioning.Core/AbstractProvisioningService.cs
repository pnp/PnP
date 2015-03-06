using Framework.Provisioning.Core.Authentication;
using Framework.Provisioning.Core.Configuration;
using Framework.Provisioning.Core.Configuration.Application;
using Framework.Provisioning.Core.Configuration.Template;
using Framework.Provisioning.Core.Utilities;
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


namespace Framework.Provisioning.Core
{
    /// <summary>
    /// Abstract Site Provisioning Service
    /// </summary>
    public abstract class AbstractProvisioningService : IProvisioningService, ISharePointService
    {
        /// <summary>
        /// Gets or Sets the services Authentication.
        /// </summary>
        public IAuthentication Authentication
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a site collection.
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public abstract Guid? ProvisionSite(SiteRequestInformation properties);
       
        /// <summary>
        /// Sets Administrators for the Site Collection
        /// </summary>
        /// <param name="properties"></param>
        public void SetAdministrators(SiteRequestInformation properties)
        {
            UsingContext(ctx =>
            {
                Tenant tenant = new Tenant(ctx);
                var site = tenant.GetSiteByUrl(properties.Url);
                var web = site.RootWeb;
                var spOwner = web.EnsureUser(properties.SiteOwner.LoginName);
                web.AssociatedOwnerGroup.Users.AddUser(spOwner);
                site.Owner = spOwner;
                ctx.ExecuteQuery();
                foreach (var admin in properties.AdditionalAdministrators)
                {
                    try
                    {
                        tenant.SetSiteAdmin(properties.Url, admin.LoginName, true);
                        var spAdmin = web.EnsureUser(admin.LoginName);
                        web.AssociatedOwnerGroup.Users.AddUser(spAdmin);
                        web.AssociatedOwnerGroup.Update();
                        ctx.ExecuteQuery();
                    }
                    catch
                    {
                        Log.Error("SetAdministrators", "Failed to set {0} as admin of {1}", admin.LoginName, properties.Url);
                    }
                }
            });
        }

        /// <summary>
        /// Sets the Description of the Site Collection
        /// </summary>
        /// <param name="properties"></param>
        public void SetSiteDescription(SiteRequestInformation properties)
        {
            UsingContext(ctx =>
            {
                Tenant tenant = new Tenant(ctx);
                var site = tenant.GetSiteByUrl(properties.Url);
                var web = site.RootWeb;
                web.Description = properties.Description;
                web.Update();
                ctx.ExecuteQuery();
                Log.Debug("Framework.Provisioning.Core.SetSiteDescription", "Setting Site Description {0}: for site {1}",
                    properties.Description,
                    properties.Url);
            });
        }

        /// <summary>
        /// Returns the Site Collection ID
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Guid? GetSiteGuidByUrl(string url)
        {
            Guid? _siteID = Guid.Empty;
            UsingContext(ctx =>
            {
                Tenant _tenant = new Tenant(ctx);
                _siteID = _tenant.GetSiteGuidByUrl(url);
            });

            return _siteID;
        }

        /// <summary>
        /// Member to apply the Site Policy to a site collection 
        /// <see cref="https://technet.microsoft.com/en-us/library/jj219569.aspx"/>
        /// </summary>
        /// <param name="url"></param>
        /// <param name="policyName"></param>
        public abstract void ApplySitePolicy(string url, string policyName);
       
        /// <summary>
        /// Adds Site Collection Administrators to a site collection
        /// </summary>
        /// <param name="users">A Collection of Users to add</param>
        /// <param name="siteUrl">The site url</param>
        public abstract void AddAdditionalAdministrators(List<AdditionalAdministrator> users, Uri siteUrl);

        /// <summary>
        ///Adds Site Owners to a site collection
        /// </summary>
        /// <param name="users">A Collection of Users to add</param>
        /// <param name="siteUrl">The site url</param>
        public abstract void AddAdditionalOwners(List<Owner> users, Uri siteUrl);

        /// <summary>
        ///Adds Members to a site collection
        /// </summary>
        /// <param name="users">A Collection of Users to add</param>
        /// <param name="siteUrl">The site url</param>
        public abstract void AddAdditionalMembers(List<Member> users, Uri siteUrl);

        /// <summary>
        ///Adds vistors to a site collection
        /// </summary>
        /// <param name="users">A Collection of Users to add</param>
        /// <param name="siteUrl">The site url</param>
        public abstract void AddAdditionalVisitors(List<Vistor> users, Uri siteUrl);
     
        /// <summary>
        /// Activates Site Features
        /// </summary>
        /// <param name="url">The site url</param>
        /// <param name="featureID">The OOB feature guid</param>
        public abstract void ActivateSiteFeature(string url, Guid featureID);

        /// <summary>
        /// Activates Web Features
        /// </summary>
        /// <param name="url">The site url</param>
        /// <param name="featureID">The OOB feature guid</param>
        public abstract void ActivateWebFeature(string url, Guid featureID);
       
        /// <summary>
        /// Deactivates Site Features
        /// </summary>
        /// <param name="url">The site url</param>
        /// <param name="featureID">The OOB feature guid</param>
        public abstract void DeactivateSiteFeature(string url, Guid featureID);

        /// <summary>
        /// Desctivates Web Features
        /// </summary>
        /// <param name="url">The site url</param>
        /// <param name="featureID">The OOB feature guid</param>
        public abstract void DeactivateWebFeature(string url, Guid featureID);

        /// <summary>
        /// Deploys BrandingPackage to the Site using Composed Looks
        /// </summary>
        /// <param name="url">The Url of the Site</param>
        /// <param name="theme">The BrandingPackage to apply</param>
        public abstract void DeployTheme(string url, BrandingPackage theme);
      
        /// <summary>
        /// Apply CSS to the Site
        /// </summary>
        /// <param name="web">The Web</param>
        /// <param name="theme">The BrandingPackage</param>
        public void ApplyCSS(Web web, BrandingPackage theme)
        {
            if (!string.IsNullOrEmpty(theme.AlternateCSS))
            {
                List assetLibrary = web.GetListByUrl("SiteAssets");
                web.Context.Load(assetLibrary, l => l.RootFolder);

                try
                {
                    FileInfo _cssFile = new FileInfo(theme.AlternateCSS);

                    FileCreationInformation newFile = new FileCreationInformation();
                    newFile.Content = System.IO.File.ReadAllBytes(theme.AlternateCSS);
                    newFile.Url = _cssFile.Name;
                    newFile.Overwrite = true;
                    Microsoft.SharePoint.Client.File uploadFile = assetLibrary.RootFolder.Files.Add(newFile);
                    web.Context.Load(uploadFile);
                    web.Context.ExecuteQuery();

                    string _url = string.Format("{0}/{1}/{2}", web.ServerRelativeUrl, "SiteAssets", _cssFile.Name);
                    web.AlternateCssUrl = _url;
                    web.Update();
                    web.Context.ExecuteQuery();
                }
                catch (Exception _ex)
                {
                    Log.Fatal("Framework.Provisioning.Core.ApplyCSS", "Exception occured during processing the request for Site {0}.  Message: {1} Stack: {2} ",
                         web.Url,
                         _ex.Message,
                         _ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Applies Site Logo to a site
        /// </summary>
        /// <param name="web">The Web</param>
        /// <param name="theme">The BrandingPackage</param>
        public void ApplySiteLogo(Web web, BrandingPackage theme)
        {
            if (!string.IsNullOrEmpty(theme.SiteLogo))
            {
                List assetLibrary = web.GetListByUrl("SiteAssets");
                web.Context.Load(assetLibrary, l => l.RootFolder);

                try
                {
                    FileInfo _file = new FileInfo(theme.SiteLogo);
                    FileCreationInformation newFile = new FileCreationInformation();
                    newFile.Content = System.IO.File.ReadAllBytes(theme.SiteLogo);
                    newFile.Url = _file.Name;
                    newFile.Overwrite = true;
                    Microsoft.SharePoint.Client.File uploadFile = assetLibrary.RootFolder.Files.Add(newFile);
                    web.Context.Load(uploadFile);
                    web.Context.ExecuteQuery();

                    Log.Info("Framework.Provisioning.Core.ApplySiteLogo", "Uploaded Site Logo {0} to list {1} for site {2} ", _file.Name, "SiteAssets", web.Url);
                    string _url = string.Format("{0}/{1}/{2}", web.ServerRelativeUrl, "SiteAssets", _file.Name);
                    web.SiteLogoUrl = _url;
                    web.Update();
                    web.Context.ExecuteQuery();
                    Log.Info("Framework.Provisioning.Core.ApplySiteLogo", "Setting Site Logo {0}: for site {1}", _file.Name, web.Url);
                }
                catch (Exception _ex)
                {
                    Log.Fatal("Framework.Provisioning.Core.ApplySiteLogo", "Exception occured during processing the request for Site {0}.  Message: {1} Stack: {2} ",
                         web.Url,
                         _ex.Message,
                         _ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Deploys Web CustomActions to the Site Collection
        /// </summary>
        /// <param name="url"></param>
        /// <param name="customAction"></param>
        public abstract void DeployWebCustomAction(string url, CustomActionEntity customAction);

        /// <summary>
        /// Deploys CustomActions to the Site Collection
        /// </summary>
        /// <param name="url"></param>
        /// <param name="customAction"></param>
        public abstract void DeploySiteCustomAction(string url, CustomActionEntity customAction);

        /// <summary>
        /// Deploys Fields to the Site 
        /// </summary>
        /// <param name="url">The Site Url</param>
        /// <param name="fieldXML">Represents a field XML element of the field</param>
        public abstract void DeployFields(string url, string fieldXML);

        /// <summary>
        /// Deploys Content Types to a site
        /// </summary>
        /// <param name="url">Url of the site</param>
        /// <param name="contentTypeXML">Represents a content type xml element</param>
        public abstract void DeployContentType(string url, string contentTypeXML);

        /// <summary>
        /// Creates a List/Libary in the site
        /// </summary>
        /// <param name="url">The site url</param>
        /// <param name="listToProvision">An object that represents the List to create</param>
        public abstract void DeployList(string url, ListInstance listToProvision);

        /// <summary>
        /// Sets a Property bag for the site
        /// </summary>
        /// <param name="web"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        public void SetPropertyBag(Web web, string propertyName, string propertyValue)
        {
            try
            {
                web.SetPropertyBagValue(propertyName, propertyValue);
                web.AddIndexedPropertyBagKey(propertyName);
            }
            catch (Exception ex)
            {
                Log.Error("Framework.Provisioning.Core.SetSitePropertyBag", "Exception occured during processing the request for Site {0}. TraceCorrelationId: {1}  Message: {2} Stack: {3} ",
                           web.Url,
                           web.Context.TraceCorrelationId,
                           ex.Message,
                           ex.StackTrace);
            }
        }

        /// <summary>
        /// Sets a Property bag for the site
        /// </summary>
        /// <param name="web"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        public void SetPropertyBag(Web web, string propertyName, int propertyValue)
        {
            try
            {
                web.SetPropertyBagValue(propertyName, propertyValue);
                web.AddIndexedPropertyBagKey(propertyName);
            }
            catch (Exception ex)
            {
                Log.Error("Framework.Provisioning.Core.SetSitePropertyBag", "Exception occured during processing the request for Site {0}. TraceCorrelationId: {1}  Message: {2} Stack: {3} ",
                          web.Url,
                          web.Context.TraceCorrelationId,
                          ex.Message,
                          ex.StackTrace);
            }
        }

        /// <summary>
        /// Checks to see if a site already exists.
        /// </summary>
        /// <param name="siteUrl"></param>
        /// <returns></returns>
        public bool SiteExists(string siteUrl)
        {
            bool _doesSiteExist = false;
            UsingContext(ctx =>
            {
                var tenant = new Tenant(ctx);
                _doesSiteExist = tenant.SiteExists(siteUrl);
            });
            return _doesSiteExist;
        }
  
        /// <summary>
        /// Delegate that is used to handle creation of ClientContext that is authenticated
        /// </summary>
        /// <param name="action"></param>
        public void UsingContext(Action<ClientContext> action)
        {
            UsingContext(action, Timeout.Infinite);
        }

        /// <summary>
        /// Delegate that is used to handle creation of ClientContext that is authenticated
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
    }
}
