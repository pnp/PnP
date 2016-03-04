using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Data.SiteRequests
{
    public interface ISiteRequestManager
    {
        /// <summary>
        /// Creates a New SharePoint Site Request
        /// </summary>
        /// <param name="siteRequest">The domain object for the site request</param>
        void CreateNewSiteRequest(SiteInformation siteRequest);

        /// <summary>
        /// Returns a Site Request from the site repository
        /// </summary>
        /// <param name="url">The Url of the site</param>
        /// <returns>A site Request Object or NULL</returns>
        SiteInformation GetSiteRequestByUrl(string url);

        /// <summary>
        /// Returns a Collection of Requests by Users.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        ICollection<SiteInformation> GetOwnerRequests(string email);

        /// <summary>
        /// Returns a collection of all new Site Requests
        /// </summary>
        /// <returns>Will return a collection of new SiteRequests or an empty collection will be returned</returns>
        ICollection<SiteInformation> GetNewRequests();

        /// <summary>
        /// Returns a collection of all Approved Site Requests
        /// </summary>
        /// <returns>Will return a collection of new SiteRequests or an empty collection will be returned</returns>
        ICollection<SiteInformation> GetApprovedRequests();

        /// <summary>
        /// Returns a collection of all Incomplete Site Requests
        /// </summary>
        /// <returns>Will return a collection of incomplete SiteRequests or an empty collection will be returned</returns>
        ICollection<SiteInformation> GetIncompleteRequests();

        /// <summary>
        /// Returns if there is an existing site request
        /// </summary>
        /// <param name="url">The Url of the Site Collection</param>
        /// <returns>true if the item exists in the repostiory</returns>
        bool DoesSiteRequestExist(string url);

        /// <summary>
        /// Updates the status of a site request in the site repository
        /// </summary>
        /// <param name="url">Url of the site</param>
        /// <param name="status">Status</param>
        void UpdateRequestStatus(string url, SiteRequestStatus status);

        /// <summary>
        /// Updates the status of a site request in the site repository
        /// </summary>
        /// <param name="url">Url of the site</param>
        /// <param name="status">Status</param>
        /// <param name="statusMessage">Status Message</param>
        void UpdateRequestStatus(string url, SiteRequestStatus status, string statusMessage);


        void UpdateRequestUrl(string url, string newUrl);
    }
}
