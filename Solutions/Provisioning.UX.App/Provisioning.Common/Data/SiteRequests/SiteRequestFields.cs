using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Data.SiteRequests
{
    /// <summary>
    /// SharePoint Request List Fields
    /// </summary>
    public static class SiteRequestFields
    {
        #region SharePoint Repository Field Constants
        public const string DEFAULT_FIELD_GROUP = "SiteProvisioning";
        public const string DEFAULT_CTYPE_GROUP = "Site Provisioning Content Types";

        //TITLE
        public const string TITLE = "Title";
        
        //URL
        public const string URL_NAME = "SP_Url";
        public const string URL_DISPLAYNAME = "Url";
        public static readonly Guid URL_ID = new Guid("BE60C056-26B9-48DF-B954-93F31698D2A4");
        public const FieldType URL_TYPE = FieldType.Text;
        public const string URL_ATTRIB = "Required='TRUE' MaxLength='255' EnforceUniqueValues='TRUE' Indexed='TRUE'";
        public const string URL_DESC = "The path used for the site collection URL. Example: https://contoso.sharepoint.com/sites/<urlpath>";

        //ONPREM FLAG
        public const string ONPREM_REQUEST_NAME = "SP_RequestOnPrem";
        public const string ONPREM_REQUEST_DISPLAYNAME = "On Premises";
        public static readonly Guid ONPREM_REQUEST_ID = new Guid("C9D27DA5-BDF4-4128-9E97-FC0D096201B5");
        public const FieldType ONPREM_REQEUST_TYPE = FieldType.Boolean;
        public const string ONPREM_REQUEST_ATTRIB = "";
        public const string ONPREM_REQUEST_DESC = "Indicates if the Site Request is targeting SharePoint On-Premises.";

        //PROPERTIES
        public const string PROPS_NAME = "SP_Props";
        public const string PROPS_DISPLAYNAME = "Properties";
        public static readonly Guid PROPS_ID = new Guid("FF0D9EBB-576F-45FF-8328-73D6B7C6E1A4");
        public const FieldType PROPS_TYPE = FieldType.Note;
        public const string PROPS_ATTRIB = "";
        public const string PROPS_DESC = "Field that stores custom property bag entries.";

        //Description
        public const string DESCRIPTION_NAME = "SP_Description";
        public const string DESCRIPTION_DISPLAYNAME = "Description";
        public static readonly Guid DESCRIPTION_ID = new Guid("258C60A9-0511-4F57-9B64-A8881519B422");
        public const FieldType DESCRIPTION_TYPE = FieldType.Note;
        public const string DESCRIPTION_ATTRIB = "";
        public const string DESCRIPTION_DESC = "The Site Description";

        //Owner
        public const string OWNER_NAME = "SP_Owner";
        public const string OWNER_DISPLAYNAME = "Owner";
        public static readonly Guid OWNER_ID = new Guid("874AD80C-4E50-4C58-99B4-D21AB3E8BB15");
        public const FieldType OWNER_TYPE = FieldType.User;
        public const string OWNER_ATTRIB = "List='UserInfo' UserSelectionMode='0' ShowField='ImnName'";
        public const string OWNER_DESC = "The Primary Site Collection Administrator";
                
        //Aditional Admins
        public const string ADD_ADMINS_NAME = "SP_AdditionalAdmins";
        public const string ADD_ADMINS_DISPLAYNAME = "Additional Admins";
        public static readonly Guid ADD_ADMINS_ID = new Guid("8C25E50C-9D31-483C-9A6D-66BDB39BBFE4");
        public const FieldType ADD_ADMINS_TYPE = FieldType.User;
        public const string ADD_ADMINS_ATTRIB = "Mult='TRUE' List='UserInfo' UserSelectionMode='0' ShowField='ImnName'";
        public const string ADD_ADMINS_DESC = "Additional Administrators";
        
        //Site Policy
        public const string POLICY_NAME = "SP_Policy";
        public const string POLICY_DISPLAYNAME = "Policy";
        public static readonly Guid POLICY_ID = new Guid("D717873A-9A01-4B3A-97AF-0F6514696C76");
        public const FieldType POLICY_TYPE = FieldType.Text;
        public const string POLICY_ATTRIB = "";
        public const string POLICY_DESC = "The Site Policy to apply";
        
        //Template
        public const string TEMPLATE_NAME = "SP_Template";
        public const string TEMPLATE_DISPLAYNAME = "Template";
        public static readonly Guid TEMPLATE_ID = new Guid("63472F5D-E8F5-4B64-9172-7D60726D0133");
        public const FieldType TEMPLATE_TYPE = FieldType.Text;
        public const string TEMPLATE_ATTRIB = "";
        public const string TEMPLATED_DESC = "The Site Template to apply.";
        
        //Status
        public const string PROVISIONING_STATUS_NAME = "SP_ProvisioningStatus";
        public const string PROVISIONING_STATUS_DISPLAYNAME = "Provisioning Status";
        public static readonly Guid PROVISIONING_STATUS_ID = new Guid("E7BD867C-79F0-46B7-B2D9-02FF239E4947");
        public const FieldType PROVISIONING_STATUS_TYPE = FieldType.Text;
        public const string PROVISIONING_STATUS_ATTRIB = "Required='TRUE' Indexed='TRUE' ShowInDisplayForm='TRUE'";
        public const string PROVISIONING_STATUS_DESC = "Status of the site request.";
        
        //LCID
        public const string LCID_NAME = "SP_Lcid";
        public const string LCID_DISPLAYNAME = "LCID";
        public static readonly Guid LCID_ID = new Guid("42A75F5D-0F79-44B7-AB9E-D9FB0307A3B5");
        public const FieldType LCID_TYPE = FieldType.Text;
        public const string LCID_ATTRIB = "Required='TRUE'";
        public const string LCID_DESC = "The Site Language";

        //TimeZone
        public const string TIMEZONE_NAME = "SP_TimeZone";
        public const string TIMEZONE_DISPLAYNAME = "Time Zone";
        public static readonly Guid TIMEZONE_ID = new Guid("2E07875C-07CD-48CF-A93A-40BF0CAB60BD");
        public const FieldType TIMEZONE_TYPE = FieldType.Text;
        public const string TIMEZONE_ATTRIB = "";
        public const string TIMEZONE_DESC = "The time zone selected for the site";

        //ApprovedDate
        public const string APPROVEDDATE_NAME = "SP_ApprovedDate";
        public const string APPROVEDDATE_DISPLAYNAME = "Request Approved Date";
        public static readonly Guid APPROVEDATE_ID = new Guid("33CA4A33-6BD5-42E5-A28F-09EBA9409C6B");
        public const FieldType APPROVEDATE_TYPE = FieldType.DateTime;
        public const string APPROVEDATE_ATTRIB = "Format='DateOnly' ShowInDisplayForm='TRUE'";
        public const string APPROVEDATE_DESC = "Date the request was approved";

        //Status Message
        public const string STATUSMESSAGE_NAME = "SP_StatusMessage";
        public const string STATUSMESSAGE_DISPLAYNAME = "Status Message";
        public static readonly Guid STATUSMESSAGE_ID = new Guid("80FDFCF7-B765-48A4-8391-52CE026BDF42");
        public const FieldType STATUSMESSAGE_TYPE = FieldType.Text;
        public const string STATUSMESSAGE_ATTRIB = "ShowInDisplayForm='TRUE'";
        public const string STATUSMESSAGE_DESC = "Status Message";

        //External Sharing
        public const string EXTERNALSHARING_NAME = "SP_ExternalSharingFlag";
        public const string EXTERNALSHARING_DISPLAYNAME = "External Sharing Enabled";
        public const FieldType EXTERNALSHARING_TYPE = FieldType.Boolean;
        public const string EXTERNALSHARING_DESC = "";
        public const string EXTERNALSHARING_ATTRIB = "ShowInDisplayForm='TRUE'";
        public static readonly Guid EXTERNALSHARING_ID = new Guid("{BE2C62C2-5973-4223-A5B3-95E7379CCE66}");

        //Business Case
        public const string BC_NAME = "SP_BusinessCase";
        public const string BC_DISPLAYNAME = "Business Case";
        public static readonly Guid BC_ID = new Guid("8513D7B5-211A-43CF-9CD8-FD25320A8C66");
        public const FieldType BC_TYPE = FieldType.Note;
        public const string BC_ATTRIB = "";
        public const string BC_DESC = "Business Case";


        #endregion
    }
}
