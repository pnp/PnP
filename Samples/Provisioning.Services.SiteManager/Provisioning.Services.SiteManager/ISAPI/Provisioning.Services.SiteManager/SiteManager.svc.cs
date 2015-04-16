using Contoso.Provisioning.Services.SiteManager.Services;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client.Services;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;

namespace Contoso.Provisioning.Services.SiteManager
{
    [BasicHttpBindingServiceMetadataExchangeEndpoint]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class SiteManager : ISiteManager
    {

        public string CreateSiteCollection(SiteData site)
        {
            string siteUrl = string.Empty;
            uint siteLcIdint = 1033;
            Guid siteId = SPContext.Current.Site.ID;
            // Elevation - would not actually be neeeded if we call this by using specific account with the right permissions, but is also one option.
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite elevSite = new SPSite(siteId))
                {
                    if (!string.IsNullOrEmpty(site.LcId))
                    {
                        siteLcIdint = Convert.ToUInt16(site.LcId);
                    }
                    siteUrl = SPUrlUtility.CombineUrl(elevSite.Url, site.Url);

                    if (!SPSite.Exists(new Uri(siteUrl)))
                    {
                        using (SPSite newSite = elevSite.SelfServiceCreateSite(siteUrl, site.Title, site.Description,
                                                                            siteLcIdint, site.WebTemplate,
                                                                            site.OwnerLogin, string.Empty, string.Empty,
                                                                            site.SecondaryContactLogin, string.Empty, string.Empty))
                        {
                            //create the default groups
                            newSite.RootWeb.CreateDefaultAssociatedGroups(newSite.Owner.LoginName, newSite.SecondaryContact.LoginName, site.Title);
                        }
                    }
                    else
                    {
                        //TODO - site already existed... abort abort!
                    }
                }
            });
            return siteUrl;
        }

        /// <summary>
        /// Method to get list of site collections under specific web application
        /// </summary>
        /// <returns></returns>
        public List<SiteData> ListSiteCollections()
        {
            List<SiteData> lstSites = new List<SiteData>();
            Guid siteId = SPContext.Current.Site.ID;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                //Create new elevated instances of the site collection
                using (SPSite elevSite = new SPSite(siteId))
                {
                    foreach (SPSite site in elevSite.WebApplication.Sites)
                    {
                        try
                        {
                            if (site.RootWeb != null)
                            {
                                SiteData siteData = new SiteData()
                                {
                                    Title = site.RootWeb.Title,
                                    Description = site.RootWeb.Description,
                                    LcId = site.RootWeb.Language.ToString(),
                                    OwnerLogin = site.Owner.LoginName,
                                    Url = site.Url,
                                    WebTemplate = string.Format("{0}#{1}", site.RootWeb.WebTemplate, site.RootWeb.Configuration)
                                };
                                if (site.SecondaryContact != null)
                                {
                                    siteData.SecondaryContactLogin = site.SecondaryContact.LoginName;
                                }
                                lstSites.Add(siteData);
                            }
                        }
                        catch (Exception ex)
                        {
                            //TODO - Log exception and move to next
                        }
                        finally
                        {
                            if (site != null)
                                site.Dispose();
                        }
                    }
                }
            });
            return lstSites;
        }

        /// <summary>
        /// Create content type with specific ID
        /// </summary>
        /// <param name="contentTypeId">ID for the content type</param>
        /// <param name="name">Name for the content type</param>
        /// <returns>ID of just created content type</returns>
        public string CreateContentType(string contentTypeId, string name)
        {
            string sContentTypeId = string.Empty;
            try
            {
                SPWeb web = SPContext.Current.Site.RootWeb;
                SPContentTypeId id = new SPContentTypeId(contentTypeId);
                SPContentType checkCt = web.ContentTypes[id];
                if (checkCt == null)
                {
                    SPContentType ct = new SPContentType(id, web.ContentTypes, name);
                    SPContentType newCType = web.ContentTypes.Add(ct);
                    sContentTypeId = newCType.Id.ToString();
                    web.Update();
                }

            }
            catch (Exception ex)
            {
                // Logging is missing
            }

            return sContentTypeId;
        }

        public bool SetDocumentInformationPolicySetting(string siteColUrl, string actionManifest, string contentTypeId)
        {
            throw new NotImplementedException();
        }

        public string GetDocumentInformationPolicySetting(string siteColUrl, string contentTypeId)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Sets the root site locale as specified in parameter
        /// </summary>
        /// <param name="siteColUrl"></param>
        /// <param name="localeString"></param>
        /// <returns></returns>
        public bool SetSiteLocale(string siteColUrl, string localeString)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {

                    using (SPSite site = new SPSite(siteColUrl))
                    {
                        site.RootWeb.Locale = CultureInfo.CreateSpecificCulture(localeString);
                        site.RootWeb.Update();
                    }
                    });
            }
            catch (Exception ex)
            {
                // Logging is missing
            }
 
            return true;
        }
    }

}
