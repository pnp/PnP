using Microsoft.SharePoint.ApplicationPages.ClientPickerQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.SharePoint.Client
{
    public enum ExternalSharingDocumentOption
    {
        Edit,
        View
    }

    public enum ExternalSharingSiteOption
    {
        Owner,
        Edit,
        View
    }

    static partial class ExternalSharingExtensions
    {
        /// <summary>
        /// Can be used to get needed people picker search result value for given email account. 
        /// </summary>
        /// <param name="web">Web for the context used for people picker search</param>
        /// <param name="emailAddress">Email address to be used as the query parameter. Should be pointing to unique person which is then searched using people picker capability programatically.</param>
        /// <returns>Resolves people picker value which can be used for sharing objects in the SharePoint site</returns>
        /// <see cref="https://msdn.microsoft.com/en-us/library/office/jj179690.aspx"/>
        public static string ResolvePeoplePickerValueForEmail(this Web web, string emailAddress)
        {
            ClientPeoplePickerQueryParameters param = new ClientPeoplePickerQueryParameters();
            param.PrincipalSource = Microsoft.SharePoint.Client.Utilities.PrincipalSource.All;
            param.PrincipalType = Microsoft.SharePoint.Client.Utilities.PrincipalType.All;
            param.MaximumEntitySuggestions = 30;
            param.QueryString = emailAddress;
            param.AllowEmailAddresses = true;
            param.AllowOnlyEmailAddresses = false;
            param.AllUrlZones = false;
            param.ForceClaims = false;
            param.Required = true;
            param.SharePointGroupID = 0;
            param.UrlZone = 0;
            param.UrlZoneSpecified = false;

            // Resolve people picker value based on email
            var ret = ClientPeoplePickerWebServiceInterface.ClientPeoplePickerResolveUser(web.Context, param);
            web.Context.ExecuteQuery();

            // Return people picker return value
            return string.Format("[{0}]", ret.Value);
        }
        /// <summary>
        /// Creates anonymous link to given document.
        /// </summary>
        /// <param name="web">Web for the context used for people picker search</param>
        /// <param name="urlToDocument">Full URL to the file which is shared</param>
        /// <param name="isEditLink">True or false indicating if the link should have edit or only view permission</param>
        /// <returns>Anonymous URL to the file as string</returns>
        /// <see cref="https://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.web.createanonymouslink.aspx"/>
        public static string CreateAnonymousLinkForDocument(this Web web, string urlToDocument, ExternalSharingDocumentOption shareOption)
        {
            bool isEditLink = true;
            switch (shareOption)
            {
                case ExternalSharingDocumentOption.Edit:
                    isEditLink = true;
                    break;
                case ExternalSharingDocumentOption.View:
                    isEditLink = false;
                    break;
                default:
                    break;
            }
            ClientResult<string> result = Microsoft.SharePoint.Client.Web.CreateAnonymousLink(web.Context, urlToDocument, isEditLink);
            web.Context.ExecuteQuery();

            // return anonymous link to caller
            return result.Value;
        }

        /// <summary>
        /// Creates anonymous link to the given document with automatic expiration time.
        /// </summary>
        /// <param name="web">Web for the context used for people picker search</param>
        /// <param name="urlToDocument">Full URL to the file which is shared</param>
        /// <param name="isEditLink">True or false indicating if the link should have edit or only view permission</param>
        /// <param name="expireTime">Date time for link expiration - will be converted to ISO 8601 format automatically</param>
        /// <returns>Anonymous URL to the file as string</returns>
        /// <see cref="https://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.web.createanonymouslinkwithexpiration.aspx"/>
        public static string CreateAnonymousLinkWithExpirationForDocument(this Web web, string urlToDocument, ExternalSharingDocumentOption shareOption, DateTime expireTime)
        {
            // If null given as expiration, there will not be automatic expiration time
            string expirationTimeAsString = null;
            if (expireTime != null)
            {
                expirationTimeAsString = expireTime.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
            }

            bool isEditLink = true;
            switch (shareOption)
            {
                case ExternalSharingDocumentOption.Edit:
                    isEditLink = true;
                    break;
                case ExternalSharingDocumentOption.View:
                    isEditLink = false;
                    break;
                default:
                    break;
            }

            // Get the link
            ClientResult<string> result =
                            Microsoft.SharePoint.Client.Web.CreateAnonymousLinkWithExpiration(
                                web.Context, urlToDocument, isEditLink, expirationTimeAsString);
            web.Context.ExecuteQuery();

            // Return anonymous link to caller
            return result.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="web">Web for the context used for people picker search</param>
        /// <param name="urlToDocument">Full URL to the file which is shared</param>
        /// <param name="targetEmailToShare">Email address for the person to whom the document will be shared</param>
        /// <param name="shareOption">View or Edit option</param>
        /// <param name="sendEmail">Send email or not</param>
        /// <param name="emailBody">Text attached to the email sent for the person to whom the document is shared</param>
        /// <see cref="ShareDocument(Web, string, string, ExternalSharingDocumentOption, bool, string)"/>
        public static SharingResult ShareDocument(this Web web, string urlToDocument, string targetEmailToShare, ExternalSharingDocumentOption shareOption, bool sendEmail = true, string emailBody = "Document shared")
        {
            // Resolve people picker value for given email
            string peoplePickerInput = ResolvePeoplePickerValueForEmail(web, targetEmailToShare);
            // Share document for user
            return ShareDocumentWithPeoplePickerValue(web, urlToDocument, peoplePickerInput, shareOption, sendEmail, emailBody);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="web">Web for the context used for people picker search</param>
        /// <param name="urlToDocument">Full URL to the file which is shared</param>
        /// <param name="peoplePickerInput">People picker JSON string value containing the target person information</param>
        /// <param name="shareOption">View or Edit option</param>
        /// <param name="sendEmail">Send email or not</param>
        /// <param name="emailBody">Text attached to the email sent for the person to whom the document is shared</param>
        public static SharingResult ShareDocumentWithPeoplePickerValue(this Web web, string urlToDocument, string peoplePickerInput,
                                        ExternalSharingDocumentOption shareOption, bool sendEmail = true,
                                        string emailBody = "Document shared for you.")
        {

            int groupId = 0;            // Set groupId to 0 for external share
            bool propageAcl = false;    // Not relevant for external accounts
            string emailSubject = null; // Not relevant, since we can't change subject
            bool includedAnonymousLinkInEmail = false;  // Check if this has any meaning in first place

            // Set role value accordingly based on requested share option
            string roleValue = "";
            switch (shareOption)
            {
                case ExternalSharingDocumentOption.Edit:
                    roleValue = "role:1073741827";
                    break;
                default:
                    // Use this for other options - Means View permission
                    roleValue = "role:1073741826";
                    break;
            }

            // Share the document, send email and return the result value
            SharingResult result = Microsoft.SharePoint.Client.Web.ShareObject(web.Context, urlToDocument,
                                                        peoplePickerInput, roleValue, groupId, propageAcl,
                                                        sendEmail, includedAnonymousLinkInEmail, emailSubject,
                                                        emailBody);

            web.Context.Load(result);
            web.Context.ExecuteQuery();
            return result;
        }

        /// <summary>
        /// Can be used to programatically to unshare any document
        /// </summary>
        /// <param name="web">Web for the context used for people picker search</param>
        /// <param name="urlToDocument">Full URL to the file which is shared</param>
        public static SharingResult UnshareDocument(this Web web, string urlToDocument)
        {
            SharingResult result = Microsoft.SharePoint.Client.Web.UnshareObject(web.Context, urlToDocument);
            web.Context.Load(result);
            web.Context.ExecuteQuery();

            // Return the results
            return result;
        }

        /// <summary>
        /// Get current sharing settings for document and load list of users it has been shared automatically.
        /// </summary>
        /// <param name="web">Web for the context</param>
        /// <param name="urlToDocument"></param>
        /// <param name="useSimplifiedPolicies"></param>
        public static ObjectSharingSettings GetObjectSharingSettingsForDocument(this Web web, string urlToDocument, bool useSimplifiedPolicies = true)
        {
            // Group value for this query is always 0.
            ObjectSharingSettings info =
                Microsoft.SharePoint.Client.Web.GetObjectSharingSettings(web.Context, urlToDocument, 0, useSimplifiedPolicies);
            web.Context.Load(info);
            web.Context.Load(info.ObjectSharingInformation);
            web.Context.Load(info.ObjectSharingInformation.SharedWithUsersCollection);
            web.Context.ExecuteQuery();

            return info;
        }


        /// <summary>
        /// Get current sharing settings for site and load list of users it has been shared automatically.
        /// </summary>
        /// <param name="web">Web for the context</param>
        /// <param name="useSimplifiedPolicies"></param>
        /// <returns></returns>
        public static ObjectSharingSettings GetObjectSharingSettingsForSite(this Web web, bool useSimplifiedPolicies = true)
        { 
            // Ensure that URL exists
            if (!web.IsObjectPropertyInstantiated("Url"))
            {
                web.Context.Load(web, w => w.Url);
                web.Context.ExecuteQuery();
            }

            ObjectSharingSettings info =
                Microsoft.SharePoint.Client.Web.GetObjectSharingSettings(web.Context, web.Url, 0, useSimplifiedPolicies);
            web.Context.Load(info);
            web.Context.Load(info.ObjectSharingInformation);
            web.Context.Load(info.ObjectSharingInformation.SharedWithUsersCollection);
            web.Context.ExecuteQuery();

            return info;
        }


        /// <summary>
        /// Share site for a person using just email. Will resolve needed people picker JSON value automatically.
        /// </summary>
        /// <param name="web">Web for the context of the site to be shared.</param>
        /// <param name="email">Email of the person to whom site should be shared.</param>
        /// <param name="shareOption">Sharing style - View, Edit, Owner</param>
        /// <param name="sendEmail">Should we send email for the given address.</param>
        /// <param name="emailBody">Text to be added on share email sent to receiver.</param>
        /// <returns></returns>
        /// <returns></returns>
        public static SharingResult ShareSite(this Web web, string email, 
                                                ExternalSharingSiteOption shareOption, bool sendEmail = true, 
                                                string emailBody = "Site shared for you.")
        {
            // Solve people picker value for email address
            string peoplePickerValue = ResolvePeoplePickerValueForEmail(web, email);

            // Share with the people picker value
            return ShareSiteWithPeoplePickerValue(web, peoplePickerValue, shareOption, sendEmail, emailBody);
        }

        /// <summary>
        /// Share site for a person using complex JSON object for people picker value.
        /// </summary>
        /// <param name="web">Web for the context of the site to be shared.</param>
        /// <param name="peoplePickerInput">JSON object with the people picker value</param>
        /// <param name="shareOption">Sharing style - View, Edit, Owner</param>
        /// <param name="sendEmail">Should we send email for the given address.</param>
        /// <param name="emailBody">Text to be added on share email sent to receiver.</param>
        /// <returns></returns>
        public static SharingResult ShareSiteWithPeoplePickerValue(this Web web, string peoplePickerInput, 
                                                                    ExternalSharingSiteOption shareOption,
                                                                    bool sendEmail = true, string emailBody = "Site shared for you.")
        {
            // Solve the group id for the shared option based on default groups
            int groupId = SolveGroupIdToShare(web, shareOption);
            string roleValue = string.Format("group:{0}", groupId); // Right permission setup

            // Ensure that web URL has been loaded
            if (!web.IsObjectPropertyInstantiated("Url"))
            {
                web.Context.Load(web, w => w.Url);
                web.Context.ExecuteQuery();
            }

            // Set default settings for site sharing
            bool propageAcl = false; // Not relevant for external accounts
            bool includedAnonymousLinkInEmail = false; // Not when site is shared

            SharingResult result = Microsoft.SharePoint.Client.Web.ShareObject(web.Context, web.Url, peoplePickerInput,
                                                        roleValue, 0, propageAcl,
                                                        sendEmail, includedAnonymousLinkInEmail, null,
                                                        emailBody);
            web.Context.Load(result);
            web.Context.ExecuteQuery();
            return result;
        }

        /// <summary>
        /// Used to solve right group ID to assign user into
        /// </summary>
        /// <param name="web">Web to be shared externally</param>
        /// <param name="shareOption">Permissions to be given for the external user</param>
        /// <returns></returns>
        private static int SolveGroupIdToShare(Web web, ExternalSharingSiteOption shareOption)
        {
            Group group = null;
            switch (shareOption)
            {
                case ExternalSharingSiteOption.Owner:
                    group = web.AssociatedOwnerGroup;
                    break;
                case ExternalSharingSiteOption.Edit:
                    group = web.AssociatedMemberGroup;
                    break;
                case ExternalSharingSiteOption.View:
                    group = web.AssociatedVisitorGroup;
                    break;
                default:
                    group = web.AssociatedVisitorGroup;
                    break;
            }
            // Load right group
            web.Context.Load(group);
            web.Context.ExecuteQuery();
            // Return group ID
            return group.Id;
        }
    }
}