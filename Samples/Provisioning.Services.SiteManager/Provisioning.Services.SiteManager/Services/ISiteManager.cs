using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Contoso.Provisioning.Services.SiteManager.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface ISiteManager
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="site">Data object for the site creation data</param>
        [OperationContract]
        string CreateSiteCollection(SiteData site);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="webAppUrl"></param>
        /// <returns></returns>
        [OperationContract]
        List<SiteData> ListSiteCollections();

        /// <summary>
        /// Create content type with name and specific ID
        /// </summary>
        /// <param name="siteColUrl">Aboslute URL to site collection</param>
        /// <param name="contentTypeId">Unique ID for the content type which is created</param>
        /// <param name="name">Name for the content type to be created</param>
        /// <returns>Content Type ID which was created</returns>
        [OperationContract]
        string CreateContentType(string contentTypeId, string name);

        /// <summary>
        /// Set information policy for content type.
        /// </summary>
        /// <param name="siteColUrl">Absolute URL to site collection</param>
        /// <param name="actionManifest">Information policy action manifest to be added as policy.</param>
        /// <param name="contentTypeId">Unique ID of content type to which the policy will be applied</param>
        /// <returns>Boolean value indicating success of the operation</returns>
        [OperationContract]
        bool SetDocumentInformationPolicySetting(string siteColUrl, string actionManifest, string contentTypeId);

        /// <summary>
        /// Get information policy for content type
        /// </summary>
        /// <param name="siteColUrl">Absolute URL to site collection</param>
        /// <param name="contentTypeId">Unique ID of content type  which the policy will be returned</param>
        /// <returns>Information policy in string format</returns>
        [OperationContract]
        string GetDocumentInformationPolicySetting(string siteColUrl, string contentTypeId);

        /// <summary>
        /// Set locale for site using locale code
        /// </summary>
        /// <param name="siteColUrl">Absolute URL to site collection</param>
        /// <param name="localeString">Locale string to set for the site, like en-us or fi-fi</param>
        /// <returns>Boolean value indicating success of the operation</returns>
        [OperationContract]
        bool SetSiteLocale(string siteColUrl, string localeString);

    }


}
