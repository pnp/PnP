using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.SiteClassification.Common
{   
    public static class SiteClassificationKeys {
        public const string AudienceReachKey = "sc_AudienceReach";
        public const string BusinessImpactKey = "sc_BusinessImpact";
        
    }
    /// <summary>
    /// Site Classification List
    /// </summary>
    public static class SiteClassificationList  {
        public static readonly string SiteClassificationListTitle = "Site Information";
        public static readonly string SiteClassificationUrl = "Lists/SiteInformation";
        public static readonly string SiteClassificationDesc = "Repository for managing site classification - DONT TOUCH";
     
    }
    /// <summary>
    /// SiteClassification Content Type
    /// </summary>
    public static class SiteClassificationContentType
    {
        public static readonly string SITEINFORMATION_CT_ID = "0x01002C35D7B588F141B08F547CE0B5520B5C";
        public static readonly string SITEINFORMATION_CT_NAME = "SiteInformation";
        public static readonly string SITEINFORMATION_CT_GROUP = "Site Classification Content Types";
        public static readonly string SITEINFORMATION_CT_DESC = "Site Classification Content DO NOT MODIFY";
    }
    /// <summary>
    /// Site Classification Fields
    /// </summary>
    public static class SiteClassificationFields {
        #region SharePoint Field Constants
        public static readonly string FIELDS_GROUPNAME = "Site Classification Columns";
        public static readonly Guid FLD_KEY_ID = new Guid("C911F4D9-956E-4D1E-B8E9-59FCF8FFCBF1");
        public static readonly string FLD_KEY_INTERNAL_NAME = "SC_METADATA_KEY";
        public static readonly string FLD_KEY_DISPLAY_NAME = "Key";

        public static readonly Guid FLD_VALUE_ID = new Guid("A51C215A-3ED5-4954-B406-CA19F103C37F");
        public static readonly string FLD_VALUE_INTERNAL_NAME = "SC_METADATA_VALUE";
        public static readonly string FLD_VALUE_DISPLAY_NAME = "Value";
        #endregion

    }

}
