namespace Governance.SiteUsageReport
{
    using Microsoft.Online.SharePoint.TenantAdministration;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Utilities;
    using OfficeDevPnP.Core.Entities;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            //gather settings from the host process config file
            string tenantName = ConfigurationManager.AppSettings["TenantName"];
            string tenantUpnDomain = ConfigurationManager.AppSettings["TenantUpnDomain"];
            Uri tenantAdminUri = new Uri(string.Format("https://{0}-admin.sharepoint.com", tenantName));
            
            //get the ream and app-only access token
            string adminRealm = TokenHelper.GetRealmFromTargetUrl(tenantAdminUri);
            var adminToken = TokenHelper.GetAppOnlyAccessToken
                (TokenHelper.SharePointPrincipal, tenantAdminUri.Authority, adminRealm).AccessToken;
                
            //we use the app-only access token to authenticate without the interaction of the user
            using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(tenantAdminUri.ToString(), adminToken))
            {
                //load the tenant object
                var tenant = new Tenant(clientContext);
                clientContext.Load(tenant);
                clientContext.ExecuteQuery();

                //call the extension method to get all site collections
                IList<SiteEntity> siteCollections = tenant.GetSiteCollections();

                //at this stage you could build aby report, like an Excel file with OpenXML
                //in this demo we generate a simple email
                EmailProperties emailProperties = GenerateEmailReport(siteCollections);
                
                //use the OffideDev PnP utilities to send out the email
                Utility.SendEmail(clientContext, emailProperties);
                clientContext.ExecuteQuery();
            }
        }

        /// <summary>
        /// This method builds an email with a table of all site collection data
        /// You can replace it with anything that suits your needs
        /// </summary>
        /// <param name="siteCollections">A list of site collections to report on</param>
        /// <returns>An email properties object</returns>
        static private EmailProperties GenerateEmailReport(IList<SiteEntity> siteCollections)
        {
            //configure the email properties
            EmailProperties emailProperties = new EmailProperties();
            emailProperties.To = new List<string> { ConfigurationManager.AppSettings["TargetEmail"] };
            emailProperties.Subject = "Periodic Site Usage Report " + DateTime.Now.ToShortDateString();
            
            //build the body
            StringBuilder mailBody = new StringBuilder();
            mailBody.Append("<html><body>");
            mailBody.AppendLine("<p>Site collections:</p>");
            mailBody.AppendLine("<table style='border:1px solid black'>");
            mailBody.AppendLine("<tr><th>Site Collection Url</th><th>Storage Quota</th><th>Storage Used</th><th>Last Content Modification Date</th><th>Web Count</th></tr>");
            
            //gather data for each site collection
            foreach (SiteEntity item in siteCollections)
            {
                mailBody.AppendLine(
                    string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>", new object[]
				{
					item.Url,
					item.StorageMaximumLevel,
					item.StorageUsage,
					item.LastContentModifiedDate,
					item.WebsCount
				}));
            }

            mailBody.AppendLine("</table>");
            mailBody.AppendLine("</body></html>");
           
            emailProperties.Body = mailBody.ToString();
            return emailProperties;
        }
    }
}
