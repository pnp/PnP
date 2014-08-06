using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contoso.Provisioning.SiteCollectionCreationWeb
{
    public static class Lists
    {
        public static readonly string SiteRepositoryTitle = "Site Requests";
        public static readonly string SiteRepositoryUrl = "Lists/SiteRequests";
        public static readonly string SiteRepositoryDesc = "Repository for managing incoming requests";
    }

    public static class SiteRequestFields
    {
        #region SharePoint Repository Field Constants
        public static readonly string FIELD_GROUP = "SiteProvisioning";

        public static readonly string Title = "Title";
        public static readonly string Url = "SR_Url";
        public static readonly string Description = "SR_Description";
        public static readonly string Owner = "SR_Owner";
        public static readonly string AdditionalOwners = "SR_AdditionalAdmins";
        public static readonly string Policy = "SR_Policy";
        public static readonly string Template = "SR_Template";
        public static readonly string State = "SR_Status";
        public static readonly string Lcid = "SR_Lcid";
        public static readonly string StatusMessage = "SR_StatusMessage";
        public static readonly string TimeZone = "SR_TimeZone";

        public static readonly string UrlDisplayName = "Url";
        public static readonly string DescriptionDisplayName = "Description";
        public static readonly string OwnerDisplayName = "Owner";
        public static readonly string AdditionalOwnersDisplayName = "Additional Admins";
        public static readonly string PolicyDisplayName = "Policy";
        public static readonly string TemplateDisplayName = "Template";
        public static readonly string StateDisplayName = "State";
        public static readonly string LcidDisplayName = "LCID";
        public static readonly string StatusMessageDisplayName = "Status Message";
        public static readonly string TimeZoneDisplayName = "Time Zone";


        public static readonly Guid UrlId = new Guid("BE60C056-26B9-48DF-B954-93F31698D2A4");
        public static readonly Guid DescriptionId = new Guid("258C60A9-0511-4F57-9B64-A8881519B422");
        public static readonly Guid OwnerId = new Guid("874AD80C-4E50-4C58-99B4-D21AB3E8BB15");
        public static readonly Guid AdditionalOwnersId = new Guid("8C25E50C-9D31-483C-9A6D-66BDB39BBFE4");
        public static readonly Guid PolicyId = new Guid("D717873A-9A01-4B3A-97AF-0F6514696C76");
        public static readonly Guid TemplateId = new Guid("63472F5D-E8F5-4B64-9172-7D60726D0133");
        public static readonly Guid StatusId = new Guid("E7BD867C-79F0-46B7-B2D9-02FF239E4947");
        public static readonly Guid LcidId = new Guid("42A75F5D-0F79-44B7-AB9E-D9FB0307A3B5");
        public static readonly Guid StatusMessageId = new Guid("39AE2D39-2E16-47BD-8522-F79C52520D0E");
        public static readonly Guid TimeZoneId = new Guid("2E07875C-07CD-48CF-A93A-40BF0CAB60BD");
        #endregion

    }
}