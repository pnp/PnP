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
    }
}
