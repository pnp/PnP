using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;

namespace GeoTenantInformationCollection
{
    /// <summary>
    /// Multi-Geo helper class
    /// </summary>
    public class MultiGeoManager
    {
        private List<GeoProperties> geosCache = null;
        private ClientContext clientContext = null;

        /// <summary>
        /// Constructs the multi geo manager
        /// </summary>
        /// <param name="clientContext">ClientContext for the tenant</param>
        public MultiGeoManager(ClientContext clientContext)
        {
            this.clientContext = clientContext;
        }

        /// <summary>
        /// Return the geo locations from the tenant linked to the Azure AD hosting the defined Azure AD application
        /// </summary>
        /// <returns>List of geo locations in this tenant</returns>
        public List<GeoProperties> GetTenantGeoLocations()
        {
            // Return data from cache...geos are fairly stable :-)
            if (this.geosCache != null)
            {
                return this.geosCache;
            }

            List<GeoProperties> geoList = new List<GeoProperties>();

            Tenant tenant = new Tenant(this.clientContext);

            var tenantInstances = tenant.GetTenantInstances();
            this.clientContext.Load(tenantInstances);
            this.clientContext.ExecuteQuery();

            foreach(var instance in tenantInstances)
            {
                geoList.Add(new GeoProperties()
                {
                    GeoLocation = instance.DataLocation,
                    RootSiteUrl = instance.RootSiteUrl,
                    MySiteHostUrl = instance.MySiteHostUrl,
                    TenantAdminUrl = instance.TenantAdminUrl,
                });
            }

            // cache data as geos are fairly stable
            this.geosCache = geoList;

            return geoList;
        }
    }
}
