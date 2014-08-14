using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;


namespace Microsoft.SharePoint.Client
{
    /// <summary>
    /// Class that deals with records management functionality
    /// </summary>
    public static class RecordsManagementExtensions
    {
        public const string INPLACE_RECORDS_MANAGEMENT_FEATURE_ID = "da2e115b-07e4-49d9-bb2c-35e93bb9fca9";
        public const string ECM_SITE_RECORD_DECLARATION_DEFAULT = "ecm_siterecorddeclarationdefault";
        public const string ECM_SITE_RECORD_RESTRICTIONS = "ecm_siterecordrestrictions";
        public const string ECM_SITE_RECORD_DECLARATION_BY = "ecm_siterecorddeclarationby";
        public const string ECM_SITE_RECORD_UNDECLARATION_BY = "ecm_siterecordundeclarationby";

        public static void ActivateInPlaceRecordsManagementFeature(this Site site)
        {
            site.ActivateFeature(new Guid(INPLACE_RECORDS_MANAGEMENT_FEATURE_ID));            
        }

        public static void EnableSiteForInPlaceRecordsManagement(this Site site)
        {
            // Activate the "In place records management" feature if not yet enabled
            if (!site.IsFeatureActive(new Guid(INPLACE_RECORDS_MANAGEMENT_FEATURE_ID)))
            {
                site.ActivateInPlaceRecordsManagementFeature();
            }

            // Enable in place records management in all locations
            site.RootWeb.SetManualRecordDeclarationInAllLocations(true);

            EcmSiteRecordRestrictions restrictions = EcmSiteRecordRestrictions.BlockEdit | EcmSiteRecordRestrictions.BlockDelete;
            site.RootWeb.SetRecordRestrictions(restrictions);
        }

        public static void SetManualRecordDeclarationInAllLocations(this Web web, bool inAllPlaces)
        {
            web.SetPropertyBagValue(ECM_SITE_RECORD_DECLARATION_DEFAULT, inAllPlaces.ToString());
        }

        public static void SetRecordRestrictions(this Web web, EcmSiteRecordRestrictions restrictions)
        {
            string restrictionsProperty = "";

            if (restrictions.Has(EcmSiteRecordRestrictions.None))
            {
                restrictionsProperty = EcmSiteRecordRestrictions.None.ToString();
            }
            else if (restrictions.Has(EcmSiteRecordRestrictions.BlockDelete))
            {
                restrictionsProperty = EcmSiteRecordRestrictions.BlockDelete.ToString();
            }
            else if (restrictions.Has(EcmSiteRecordRestrictions.BlockEdit))
            {
                restrictionsProperty = EcmSiteRecordRestrictions.BlockDelete.ToString() + ", " + EcmSiteRecordRestrictions.BlockEdit.ToString();
            }

            // Set property bag entry
            web.SetPropertyBagValue(ECM_SITE_RECORD_RESTRICTIONS, restrictionsProperty);
        }

        public static void SetRecordDeclarationBy(this Web web, EcmRecordDeclarationBy by)
        {
            web.SetPropertyBagValue(ECM_SITE_RECORD_DECLARATION_BY, by.ToString());
        }

        public static void SetRecordUnDeclarationBy(this Web web, EcmRecordDeclarationBy by)
        {
            web.SetPropertyBagValue(ECM_SITE_RECORD_UNDECLARATION_BY, by.ToString());
        }

    }
}
