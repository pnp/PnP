using System;
using System.Configuration;

namespace Governance.EnsurePolicy
{
    class Program
    {
        static void Main(string[] args)
        {

            // Load job settings
            string appId = ConfigurationManager.AppSettings[Constants.AppSettings_AppId];
            string tenant = ConfigurationManager.AppSettings[Constants.AppSettings_AzureTenant];
            string pfxCertificate = ConfigurationManager.AppSettings[Constants.AppSettings_PfxCertificate];
            string pfxCertificatePassword = ConfigurationManager.AppSettings[Constants.AppSettings_PfxCertificatePassword];
            string tenantAdmin = ConfigurationManager.AppSettings[Constants.AppSettings_TenantAdmin];
            string excludeOD4BSites = ConfigurationManager.AppSettings[Constants.AppSettings_ExcludeOD4BSites];
            string numberOfThreads = ConfigurationManager.AppSettings[Constants.AppSettings_NumberOfThreads];
            string siteFilters = ConfigurationManager.AppSettings[Constants.AppSettings_SiteFilters];

            if (string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(tenant) || string.IsNullOrEmpty(pfxCertificate) || string.IsNullOrEmpty(pfxCertificatePassword))
            {
                throw new Exception("Please specify the needed app.config app settings for Azure AD based access: AppId, Tenant, PfxCertificate and PfxCertificatePassword");
            }

            // Instantiate the job
            PolicyJob job = new PolicyJob();
            
            // Provide the job with information to authenticate back to SharePoint Online
            job.UseAzureADAppOnlyAuthentication(appId, tenant, pfxCertificate, pfxCertificatePassword);

            // Set tenant admin, optional for MT usage but required when working with DvNext tenants
            if (!string.IsNullOrEmpty(tenantAdmin))
            {
                job.TenantAdminSite = tenantAdmin;
            }

            // Don't store state inside sites since web property bag cannot be updated for "modern" team sites due to the NoScript setting
            job.ManageState = false;

            // Include/exclude OD4B sites from this policy job
            bool.TryParse(excludeOD4BSites, out bool excludeOD4B);
            job.ExcludeOD4B = excludeOD4B;

            // Set the number of threads
            if (Int32.TryParse(numberOfThreads, out Int32 maximumThreads) && maximumThreads > 1)
            {
                job.MaximumThreads = maximumThreads;
            }

            // Set the PnP provisioning template that needs to be applied
            job.ProvisioningTemplateToApply = "permission.xml";

            // Tell the job which sites to run on. sample urls are:
            // https://bertonline.sharepoint.com/sites/*         : All regular site collections
            // https://bertonline.sharepoint.com/sites/test*     : All regular site collections starting with test
            // https://bertonline-my.sharepoint.com/personal/*   : All OD4B site collections (assuming excludeODB4 was not set)
            if (!string.IsNullOrEmpty(siteFilters))
            {
                string[] filters = siteFilters.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach(var filter in filters)
                {
                    job.AddSite(filter);
                }
            }
            else
            {
                // I'm debugging...
                job.UseThreading = false;
                job.AddSite("https://bertonline.sharepoint.com/sites/bert2");
            }

            // Launch job
            job.Run();

        }
    }
}
