using Provisioning.Common.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Data.Metadata
{
    public interface IMetadataManager
    {
        /// <summary>
        /// Returns a collection of the available site classification
        /// </summary>
        /// <returns></returns>
        ICollection<SiteClassification> GetAvailableSiteClassifications();

        /// <summary>
        /// Returns a collection of the available site classification
        /// </summary>
        /// <returns></returns>
        SiteEditMetadata SetSitePolicy(SiteEditMetadata metadata);

        /// <summary>
        /// Gets a site classification by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        SiteClassification GetSiteClassificationByName(string name);

        /// <summary>
        /// Creates a new site classification
        /// </summary>
        /// <param name="classification"></param>
        void CreateNewSiteClassification(SiteClassification classification);

        /// <summary>
        /// Updates an existing site classification
        /// </summary>
        /// <param name="classification"></param>
        void UpdateSiteClassification(SiteClassification classification);

        /// <summary>
        /// Checks to see if user has permissions
        /// </summary>
        /// <returns>bool</returns>
        bool DoesUserHavePermissions();        

        /// <summary>
        /// Get site metadata
        /// </summary>
        /// <returns>bool</returns>
        SiteEditMetadata GetSiteMetadata(SiteEditMetadata metadata);

        /// <summary>
        /// Set site metadata
        /// </summary>
        /// <returns>bool</returns>
        SiteEditMetadata SetSiteMetadata(SiteEditMetadata metadata);

        ICollection<SiteMetadata> GetAvailableOrganizationalFunctions();
        ICollection<SiteMetadata> GetAvailableRegions();
        ICollection<SiteMetadata> GetAvailableDivisions();
        ICollection<SiteMetadata> GetAvailableBusinessUnits();
        ICollection<SiteMetadata> GetAvailableTimeZones();
        ICollection<SiteMetadata> GetAvailableSiteRegions();        
        ICollection<SiteMetadata> GetAvailableLanguages();


    }
}
