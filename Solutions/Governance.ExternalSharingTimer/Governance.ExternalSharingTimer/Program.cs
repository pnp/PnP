using Microsoft.Online.SharePoint.TenantManagement;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Governance.ExternalSharingTimer
{
    class Program
    {
        //this is a hack to demonstrate the concept...the real solution would either enumerate sites OR 
        //store site details in a database table populated through custom provisioning (I prefer this approach)
        private static string[] sites = new string[] { 
            "https://rzna.sharepoint.com/sites/team3",
            //"https://rzna.sharepoint.com/sites/team2",
            //"https://rzna.sharepoint.com/sites/team3",
            //"https://rzna.sharepoint.com/sites/team4",
            //"https://rzna-my.sharepoint.com/personal/ridize_rzna_onmicrosoft_com", 
            //"https://rzna-my.sharepoint.com/personal/alexd_rzna_onmicrosoft_com", 
            //"https://rzna-my.sharepoint.com/personal/annew_rzna_onmicrosoft_com",
            //"https://rzna-my.sharepoint.com/personal/roby_rzna_onmicrosoft_com"
        };

        private static XNamespace ns = "http://schemas.microsoft.com/ado/2007/08/dataservices";

        static void Main(string[] args)
        {
            //get governance variables such as warning duration and cutoff duration
            int warningDuration = Convert.ToInt32(ConfigurationManager.AppSettings["WarningDuration"]);
            int cutoffDuration = Convert.ToInt32(ConfigurationManager.AppSettings["CutoffDuration"]);
            string tenantName = ConfigurationManager.AppSettings["TenantName"];
            string tenantUpnDomain = ConfigurationManager.AppSettings["TenantUpnDomain"];
            Uri tenantAdminUri = new Uri(String.Format("https://{0}-admin.sharepoint.com", tenantName));

            string webUrl = "";
            #if DEBUG
            webUrl = "http://localhost:25440/";
            #else
            webUrl = "https://sposharing.azurewebsites.net/";
            #endif

            foreach (var siteUrl in sites)
            {
                //initialize a process date for this site and clean up to match SQL percision
                DateTime processDate = DateTime.Now;
                string stringTicks = processDate.Ticks.ToString();
                int adjustmentTicks = Convert.ToInt32(stringTicks.Substring(stringTicks.Length - 5));
                processDate = processDate.Subtract(TimeSpan.FromTicks(adjustmentTicks));

                //use O365 Tenant Administration to get all the external sharing details for this site
                List<ExternalShareDetails> shares = new List<ExternalShareDetails>();
                string adminRealm = TokenHelper.GetRealmFromTargetUrl(tenantAdminUri);
                var adminToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, tenantAdminUri.Authority, adminRealm).AccessToken;
                using (var clientContext = TokenHelper.GetClientContextWithAccessToken(tenantAdminUri.ToString(), adminToken))
                {
                    //load the tenant
                    var tenant = new Office365Tenant(clientContext);
                    clientContext.Load(tenant);
                    clientContext.ExecuteQuery();

                    //initalize varables to going through the paged results
                    int position = 0;
                    bool hasMore = true;
                    while (hasMore)
                    {
                        //get external users 50 at a time (this is the limit and why we are paging)
                        var externalUsers = tenant.GetExternalUsersForSite(siteUrl, position, 50, String.Empty, SortOrder.Descending);
                        clientContext.Load(externalUsers, i => i.TotalUserCount);
                        clientContext.Load(externalUsers, i => i.ExternalUserCollection);
                        clientContext.ExecuteQuery();

                        //convert each external user to our own entity
                        foreach (var extUser in externalUsers.ExternalUserCollection)
                        {
                            position++;
                            shares.Add(new ExternalShareDetails()
                            {
                                AcceptedAs = extUser.AcceptedAs.ToLower(),
                                DisplayName = extUser.DisplayName,
                                InvitedAs = extUser.InvitedAs.ToLower(),
                                InvitedBy = (String.IsNullOrEmpty(extUser.InvitedBy)) ? null : extUser.InvitedBy.ToLower(),
                                UserId = extUser.UserId,
                                WhenCreated = extUser.WhenCreated
                            });
                        }
                        
                        //determine if we have more pages to process
                        hasMore = (externalUsers.TotalUserCount > position);
                    }
                }

                //get an AppOnly accessToken and clientContext for the site collection
                Uri siteUri = new Uri(siteUrl);
                string realm = TokenHelper.GetRealmFromTargetUrl(siteUri);
                string accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, siteUri.Authority, realm).AccessToken;
                using (var clientContext = TokenHelper.GetClientContextWithAccessToken(siteUri.ToString(), accessToken))
                {
                    //first we need to load the site to determine if external sharing is enabled (Site.ShareByEmailEnabled)
                    var site = clientContext.Site;
                    var siteOwner = clientContext.Site.Owner;
                    clientContext.Load(site);
                    clientContext.Load(siteOwner); //include the site owner in case the share "InvitedBy" is null...we will send them email instead
                    clientContext.ExecuteQuery();

                    //validate that the site has sharing turned on
                    if (site.ShareByEmailEnabled)
                    {
                        //process all of the shares
                        foreach (var externalShare in shares)
                        {
                            //look for an existing record in the database
                            using (ExternalSharingDataEntities entities = new ExternalSharingDataEntities())
                            {
                                var shareRecord = entities.ExternalShares.FirstOrDefault(i => i.LoginName.Equals(externalShare.AcceptedAs));
                                if (shareRecord != null)
                                {
                                    //Update LastProcessedDate column of the record with the processDate
                                    shareRecord.LastProcessedDate = processDate;
                                    entities.SaveChanges();
                                }
                                else
                                {
                                    //get the original share date
                                    var details = getREST(accessToken, String.Format("{0}/_api/Web/SiteUserInfoList/Items({1})/FieldValuesAsText", siteUrl, externalShare.UserId));
                                    externalShare.WhenCreated = Convert.ToDateTime(details.Descendants(ns + "Created").FirstOrDefault().Value);
                                    shareRecord = new ExternalShare()
                                    {
                                        UniqueIdentifier = Guid.NewGuid(),
                                        SiteCollectionUrl = siteUrl.ToLower(),
                                        LoginName = externalShare.AcceptedAs,
                                        UserId = externalShare.UserId,
                                        InvitedBy = (String.IsNullOrEmpty(externalShare.InvitedBy)) ? siteOwner.Email : externalShare.InvitedBy,
                                        OriginalSharedDate = externalShare.WhenCreated,
                                        LastProcessedDate = processDate
                                    };
                                    entities.ExternalShares.Add(shareRecord);
                                    entities.SaveChanges();
                                }

                                //check if the record falls inside the warnings
                                double daysActive = processDate.Subtract(shareRecord.OriginalSharedDate).TotalDays;
                                if (shareRecord.RefreshSharedDate != null)
                                    daysActive = processDate.Subtract((DateTime)shareRecord.RefreshSharedDate).TotalDays;

                                //check for cutoff
                                if (daysActive > cutoffDuration)
                                {
                                    //remove the SPUser from the site
                                    clientContext.Web.SiteUsers.RemoveById(externalShare.UserId);
                                    clientContext.ExecuteQuery();

                                    //delete the record
                                    entities.ExternalShares.Remove(shareRecord);
                                    entities.SaveChanges();
                                }
                                else if (daysActive > warningDuration)
                                {
                                    int expiresIn = Convert.ToInt32(cutoffDuration - daysActive);
                                    //send email to InvitedBy (which will be site collection owner when null)
                                    EmailProperties email = new EmailProperties();
                                    email.To = new List<String>() { shareRecord.InvitedBy };
                                    email.Subject = String.Format("Action Required: External sharing with {0} about to expire", externalShare.AcceptedAs);
                                    email.Body = String.Format("<html><body><p>You are receiving this message because you are the site administrator of <a href='{0}'>{0}</a> OR you shared it with {1}. The external access for this user is set to expire in {2} days. Use the link below to view additional details and perform actions to revoke OR extend access for another {3} days. If you do not act on this notice, the external access for this user to terminate in {2} days.</p><ul><li><a href='{4}Details/{5}'>View Details</a></li><li><a href='{4}Extend/{5}'>Extend {3} Days</a></li><li><a href='{4}Revoke/{5}'>Revoke Access</a></li></ul></body></html>", siteUrl, externalShare.AcceptedAs, expiresIn.ToString(), cutoffDuration.ToString(), webUrl, shareRecord.UniqueIdentifier);
                                    Utility.SendEmail(clientContext, email);
                                    clientContext.ExecuteQuery();
                                }
                            }
                        }
                    }
                 }

                //delete all database records for this site that have an older processDate...these should represent external users deleted by manually
                using (ExternalSharingDataEntities entities = new ExternalSharingDataEntities())
                {
                    var cleanUpRecords = entities.ExternalShares.Where(i => i.SiteCollectionUrl.Equals(siteUrl.ToLower()) && i.LastProcessedDate < processDate);
                    foreach (var record in cleanUpRecords)
                    {
                        entities.ExternalShares.Remove(record);
                        entities.SaveChanges();
                    }
                }
            }
        }

        private static XDocument getREST(string accessToken, string url)
        {
            XDocument xdoc = null;
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Headers.Add("Authorization", "Bearer" + " " + accessToken);
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                StreamReader reader = new StreamReader(response.GetResponseStream(), encode);
                string xmlString = reader.ReadToEnd();
                xdoc = XDocument.Parse(xmlString);
            }
            return xdoc;
        }
    }

    public class ExternalShareDetails
    {
        public string AcceptedAs { get; set; }
        public string DisplayName { get; set; }
        public string InvitedAs { get; set; }
        public string InvitedBy { get; set; }
        public int UserId { get; set; }
        public DateTime WhenCreated { get; set; }
    }
}
