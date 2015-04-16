using Contoso.Provisioning.Hybrid.Contract;
using Contoso.Provisioning.Hybrid.Core.SiteTemplates;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Contoso.Provisioning.Hybrid
{
    class SiteProvisioningOnPremises : ISiteProvisioningOnPremises
    {

        public void CreateSiteCollectionOnPremises(Contract.SharePointProvisioningData sharePointProvisioningData)
        {
            //On-Prem settings
            string generalOnPremUserName = GetConfiguration("General.OnPremUserName");
            string generalOnPremUserPassword = EncryptionUtility.Decrypt(GetConfiguration("General.OnPremUserPassword"), GetConfiguration("General.EncryptionThumbPrint"));
            string generalOnPremUserDomain = GetConfiguration("General.OnPremUserDomain");
            string generalOnPremWebApplication = GetConfiguration("General.OnPremWebApplication");

            SiteManager.SiteData siteData = new SiteManager.SiteData();
            siteData.Url = sharePointProvisioningData.Url.Substring(sharePointProvisioningData.Url.IndexOf("/sites"));
            siteData.Title = sharePointProvisioningData.Title;
            siteData.LcId = sharePointProvisioningData.Lcid.ToString();
            siteData.OwnerLogin = GetOwnerLogin(sharePointProvisioningData, generalOnPremUserDomain);
            siteData.SecondaryContactLogin = string.Format(@"{0}\{1}", generalOnPremUserDomain, generalOnPremUserName);
            siteData.WebTemplate = sharePointProvisioningData.Template;

            using (SiteManager.SiteManagerClient siteManager = GetSiteManagerClient(generalOnPremWebApplication, generalOnPremUserName, generalOnPremUserPassword, generalOnPremUserDomain))
            {
                siteManager.CreateSiteCollection(siteData);
            }
        }

        public string GetNextSiteCollectionUrl(ClientContext cc, Web web, string siteDirectoryUrl, string siteDirectoryListName, string baseSiteUrl)
        {
            int lastNumber = GetLastSiteCollectionNumber(cc, web, siteDirectoryUrl, siteDirectoryListName);

            lastNumber++;

            string nextSiteName = DateTime.Now.ToString("yyyy") + String.Format("{0:0000}", lastNumber);
            string nextUrl = String.Format("{0}/sites/{1}", baseSiteUrl, nextSiteName);

            string generalOnPremUserName = GetConfiguration("General.OnPremUserName");
            string generalOnPremUserPassword = EncryptionUtility.Decrypt(GetConfiguration("General.OnPremUserPassword"), GetConfiguration("General.EncryptionThumbPrint"));
            string generalOnPremUserDomain = GetConfiguration("General.OnPremUserDomain");
            string generalOnPremWebApplication = GetConfiguration("General.OnPremWebApplication");

            SiteManager.SiteData[] onPremSiteCollectionList = null;
            using (SiteManager.SiteManagerClient siteManager = GetSiteManagerClient(generalOnPremWebApplication, generalOnPremUserName, generalOnPremUserPassword, generalOnPremUserDomain))
            {
                onPremSiteCollectionList = siteManager.ListSiteCollections();
            }

            bool validUrl = false;

            while (!validUrl)
            {
                if (!IsSiteExisted(nextUrl, onPremSiteCollectionList))
                {
                    validUrl = true;
                }
                else
                {
                    Thread.Sleep(500);
                    lastNumber++;
                    nextSiteName = DateTime.Now.ToString("yyyy") + String.Format("{0:0000}", lastNumber);
                    nextUrl = String.Format("{0}/sites/{1}", baseSiteUrl, nextSiteName);
                }
            }

            return nextUrl;
        }

        public bool IsSiteExisted(string nextUrl, SiteManager.SiteData[] onPremSiteCollectionList)
        {
            foreach (SiteManager.SiteData siteData in onPremSiteCollectionList)
            {
                if (siteData.Url.Equals(nextUrl, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the last used site collection number
        /// </summary>
        /// <param name="siteDirectoryHost">Url to the site directory site collection</param>
        /// <param name="listName">Name of the site directory list</param>
        /// <returns>last used site collection number</returns>
        public int GetLastSiteCollectionNumber(ClientContext cc, Web web, string siteDirectoryHost, string listName)
        {
            int lastNumber = 0;

            List listToInsertQuery = web.Lists.GetByTitle(listName);

            CamlQuery query = new CamlQuery();
            query.ViewXml = "<View>"
               + "<Query>"
               + String.Format("<Where><Eq><FieldRef Name='CreateYear' /><Value Type='Int'>{0}</Value></Eq></Where>", DateTime.Now.ToString("yyyy"))
               + "<OrderBy><FieldRef Name='CreateSeq' Ascending='FALSE'/></OrderBy>"
               + "</Query>"
               + "<RowLimit>1</RowLimit>"
               + "</View>";
            
            // execute the query
            ListItemCollection listItems = listToInsertQuery.GetItems(query);
            cc.Load(listItems);
            cc.ExecuteQuery();

            if (listItems.Count == 1)
            {
                int.TryParse(listItems[0]["CreateSeq"].ToString(), out lastNumber);
            }

            return lastNumber;
        }

        ///// <summary>
        ///// Updates the provisioning status of an entry in the site directory list
        ///// </summary>
        ///// <param name="siteDirectoryHost">Url to the site directory site collection</param>
        ///// <param name="listName">Name of the site directory list</param>
        ///// <param name="siteUrl">Url if the site directory entry</param>
        ///// <param name="status">Status to use</param>
        ///// <param name="ex">Exception caught during the provisioning process</param>
        //public void UpdateSiteDirectoryStatus(ClientContext cc, Web web, string siteDirectoryHost, string listName, string siteUrl, string newSiteUrl, string status, Exception ex)
        //{
        //    List listToInsertQuery = web.Lists.GetByTitle(listName);

        //    CamlQuery query = new CamlQuery();
        //    query.ViewXml = "<View>"
        //       + "<Query>"
        //       + String.Format("<Where><Eq><FieldRef Name='UrlText' /><Value Type='Text'>{0}</Value></Eq></Where>", siteUrl)
        //       + "</Query>"
        //       + "</View>";
        //    // execute the query
        //    ListItemCollection listItems = listToInsertQuery.GetItems(query);

        //    cc.Load(listItems);
        //    cc.ExecuteQuery();

        //    if (listItems.Count == 1)
        //    {
        //        listItems[0]["Status"] = status;

        //        if (status.Equals("Provisioning", StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            // update to the new siteUrl
        //            listItems[0]["UrlText"] = newSiteUrl;
        //            listItems[0]["CreateYear"] = GetCreationYear(newSiteUrl);
        //            listItems[0]["CreateSeq"] = GetCreationSequence(newSiteUrl);
        //        }

        //        // Update the URL value now that the site status is available
        //        if (status.Equals("Available", StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            FieldUrlValue url = (FieldUrlValue)(listItems[0]["SiteTitle"]);
        //            url.Url = siteUrl;
        //            listItems[0]["SiteTitle"] = url;
        //        }

        //        if (ex != null)
        //        {
        //            string error = string.Format("Error: {0}\r\nStacktrace: {1}", ex.Message, ex.StackTrace);
        //            listItems[0]["Error"] = error;
        //        }

        //        listItems[0].Update();
        //        cc.ExecuteQuery();
        //    }

        //}

        /// <summary>
        /// Returns the year from the site collection name (eg.20140034) returns 2014 as year
        /// </summary>
        /// <param name="siteUrl">Url of the site collection</param>
        /// <returns>The year value</returns>
        private static int GetCreationYear(string siteUrl)
        {
            string[] words = siteUrl.Split('/');
            string lastWord = words[words.Length - 1];
            //year is the first two digits
            int result;
            if (int.TryParse(lastWord.Substring(0, 4), out result))
            {
                return result;
            }

            return 0;
        }

        /// <summary>
        /// Returns the sequence number from the site collection name (eg.20140034) returns 34 as sequence number
        /// </summary>
        /// <param name="siteUrl">Url of the site collection</param>
        /// <returns>The sequence number value</returns>
        private static int GetCreationSequence(string siteUrl)
        {
            string[] words = siteUrl.Split('/');
            string lastWord = words[words.Length - 1];
            //year is the first two digits
            int result;
            if (int.TryParse(lastWord.Substring(4, 4), out result))
            {
                return result;
            }

            return 0;
        }

        private string GetOwnerLogin(Contract.SharePointProvisioningData sharePointProvisioningData, string domain)
        {
            string ownerLogin = sharePointProvisioningData.Owners[0].Login.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries)[0];
            return string.Format(@"{0}\{1}", domain, ownerLogin);
        }

        private string GetConfiguration(string key)
        {
            return ConfigurationManager.AppSettings[key]; 
        }

        private SiteManager.SiteManagerClient GetSiteManagerClient(string webApplicationUrl, string account, string password, string domain)
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            if (webApplicationUrl.ToLower().Contains("https://"))
            {
                binding.Security.Mode = BasicHttpSecurityMode.Transport;
            }
            else
            {
                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            }
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;

            EndpointAddress endPoint = new EndpointAddress(webApplicationUrl + "/_vti_bin/provisioning.services.sitemanager/sitemanager.svc");
            //Set time outs
            binding.ReceiveTimeout = TimeSpan.FromMinutes(15);
            binding.CloseTimeout = TimeSpan.FromMinutes(15);
            binding.OpenTimeout = TimeSpan.FromMinutes(15);
            binding.SendTimeout = TimeSpan.FromMinutes(15);

            //Create proxy instance
            SiteManager.SiteManagerClient managerClient = new SiteManager.SiteManagerClient(binding, endPoint);
            managerClient.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;

            var impersonator = new System.Net.NetworkCredential(account, password, domain);
            managerClient.ClientCredentials.Windows.ClientCredential = impersonator;

            return managerClient;
        }

        public ClientContext SpOnPremiseAuthentication(string siteUrl)
        {
            string generalOnPremUserName = GetConfiguration("General.OnPremUserName");
            string generalOnPremUserPassword = EncryptionUtility.Decrypt(GetConfiguration("General.OnPremUserPassword"), GetConfiguration("General.EncryptionThumbPrint"));
            string generalOnPremUserDomain = GetConfiguration("General.OnPremUserDomain");
            ClientContext ctx = new ClientContext(siteUrl);
            NetworkCredential credentials = new NetworkCredential(generalOnPremUserName, generalOnPremUserPassword, generalOnPremUserDomain);
            ctx.Credentials = credentials;
            ctx.ExecutingWebRequest += ctx_ExecutingWebRequest;
            
            return ctx;
        }

        void ctx_ExecutingWebRequest(object sender, WebRequestEventArgs e)
        {
            try
            {
                e.WebRequestExecutor.WebRequest.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
            }
            catch
            {
                throw;
            }
        }

    }
}
