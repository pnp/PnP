using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Contoso.Provisioning.Hybrid.Core
{
    public class SiteDirectoryManager
    {
        /// <summary>
        /// Returns the next site collection number
        /// </summary>
        /// <param name="siteDirectoryUrl">Url to the site directory site</param>
        /// <returns>The next site collection Url</returns>
        public string GetNextSiteCollectionUrlTenant(ClientContext cc, Web web, ClientContext ccSiteDirectory, Web webSiteDirectory, string siteDirectoryUrl, string siteDirectoryListName, string baseSiteUrl)
        {
            int lastNumber = GetLastSiteCollectionNumber(ccSiteDirectory, webSiteDirectory, siteDirectoryUrl, siteDirectoryListName);

            lastNumber++;

            string nextSiteName = DateTime.Now.ToString("yyyy") + String.Format("{0:0000}", lastNumber);
            string nextUrl = String.Format("{0}{1}", baseSiteUrl, nextSiteName);

            bool validUrl = false;

            Tenant tenant = new Tenant(web.Context);
            while (!validUrl)
            {
                if (!tenant.SiteExists(nextUrl))
                {
                    validUrl = true;
                }
                else
                {
                    lastNumber++;
                    nextSiteName = DateTime.Now.ToString("yyyy") + String.Format("{0:0000}", lastNumber);
                    nextUrl = String.Format("{0}{1}", baseSiteUrl, nextSiteName);
                }
            }

            return nextUrl;
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

        /// <summary>
        /// Updates the provisioning status of an entry in the site directory list
        /// </summary>
        /// <param name="siteDirectoryHost">Url to the site directory site collection</param>
        /// <param name="listName">Name of the site directory list</param>
        /// <param name="siteUrl">Url if the site directory entry</param>
        /// <param name="status">Status to use</param>
        /// <param name="ex">Exception caught during the provisioning process</param>
        public void UpdateSiteDirectoryStatus(ClientContext cc, Web web, string siteDirectoryHost, string listName, string siteUrl, string newSiteUrl, string status, Exception ex)
        {

            List listToInsertQuery = web.Lists.GetByTitle(listName);

            CamlQuery query = new CamlQuery();
            query.ViewXml = "<View>"
               + "<Query>"
               + String.Format("<Where><Eq><FieldRef Name='UrlText' /><Value Type='Text'>{0}</Value></Eq></Where>", siteUrl)
               + "</Query>"
               + "</View>";
            // execute the query
            ListItemCollection listItems = listToInsertQuery.GetItems(query);

            cc.Load(listItems);
            cc.ExecuteQuery();

            if (listItems.Count == 1)
            {
                listItems[0]["Status"] = status;

                if (status.Equals("Provisioning", StringComparison.InvariantCultureIgnoreCase))
                {
                    // update to the new siteUrl
                    listItems[0]["UrlText"] = newSiteUrl;
                    listItems[0]["CreateYear"] = GetCreationYear(newSiteUrl);
                    listItems[0]["CreateSeq"] = GetCreationSequence(newSiteUrl);
                }

                // Update the URL value now that the site status is available
                if (status.Equals("Available", StringComparison.InvariantCultureIgnoreCase))
                {
                    FieldUrlValue url = (FieldUrlValue)(listItems[0]["SiteTitle"]);
                    url.Url = siteUrl;
                    listItems[0]["SiteTitle"] = url;
                }

                if (ex != null)
                {
                    string error = string.Format("Error: {0}\r\nStacktrace: {1}", ex.Message, ex.StackTrace);
                    listItems[0]["Error"] = error;
                }

                listItems[0].Update();
                cc.ExecuteQuery();
            }

        }

        /// <summary>
        /// Updates the status of a site directory entry
        /// </summary>
        /// <param name="siteDirectoryHost">Url to the site directory site collection</param>
        /// <param name="listName">Name of the site directory list</param>
        /// <param name="siteUrl">Url if the site directory entry</param>
        /// <param name="status">Status to use</param>
        public void UpdateSiteDirectoryStatus(ClientContext cc, Web web, string siteDirectoryHost, string listName, string siteUrl, string status)
        {
            UpdateSiteDirectoryStatus(cc, web, siteDirectoryHost, listName, siteUrl, "", status, null);
        }

        /// <summary>
        /// Updates the provisioning status of an entry in the site directory list
        /// </summary>
        /// <param name="siteDirectoryHost">Url to the site directory site collection</param>
        /// <param name="listName">Name of the site directory list</param>
        /// <param name="siteUrl">Url if the site directory entry</param>
        /// <param name="status">Status to use</param>
        /// <param name="ex">Exception caught during the provisioning process</param>
        public void UpdateSiteDirectoryStatus(ClientContext cc, Web web, string siteDirectoryHost, string listName, string siteUrl, string status, Exception ex)
        {
            UpdateSiteDirectoryStatus(cc, web, siteDirectoryHost, listName, siteUrl, "", status, ex);
        }

        /// <summary>
        /// Updates the provisioning status of an entry in the site directory list
        /// </summary>
        /// <param name="siteDirectoryHost">Url to the site directory site collection</param>
        /// <param name="listName">Name of the site directory list</param>
        /// <param name="siteUrl">Url if the site directory entry</param>
        /// <param name="status">Status to use</param>
        /// <param name="ex">Exception caught during the provisioning process</param>
        public void UpdateSiteDirectoryStatus(ClientContext cc, Web web, string siteDirectoryHost, string listName, string siteUrl, string newSiteUrl, string status)
        {
            UpdateSiteDirectoryStatus(cc, web, siteDirectoryHost, listName, siteUrl, newSiteUrl, status, null);
        }
        /// <summary>
        /// Adds a new entry to the site directory list
        /// </summary>
        /// <param name="siteDirectoryHost">Url to the site directory site collection</param>
        /// <param name="siteDirectoryProvisioningPage">Path to a page used as url when the site collection is not yet provisioned</param>
        /// <param name="listName">Name of the site directory list</param>
        /// <param name="title">Title of the site collection</param>
        /// <param name="siteUrl">Url of the site collection</param>
        /// <param name="template">Template used to provision this site collection</param>
        /// <param name="requestor">Person that requested the provisioning</param>
        /// <param name="owner">Person that will be the primary owner of the site collection</param>
        /// <param name="backupOwners">Person(s) that will be the backup owner(s) of the site collection</param>
        /// <param name="permissions">Chosen permission model</param>
        public void AddSiteDirectoryEntry(ClientContext cc, Web web, string siteDirectoryHost, string siteDirectoryProvisioningPage, string listName, string title, string siteUrl, string template, string[] ownerLogins)
        {

            List listToInsertTo = web.Lists.GetByTitle(listName);
            ListItemCreationInformation lici = new ListItemCreationInformation();
            ListItem listItem = listToInsertTo.AddItem(lici);
            listItem["Title"] = title;

            //URL = hyperlink field
            FieldUrlValue url = new FieldUrlValue();
            url.Description = title;
            url.Url = String.Format("{0}/{1}", siteDirectoryHost, siteDirectoryProvisioningPage);
            listItem["SiteTitle"] = url;
            // store url also as text field to facilitate easy CAML querying afterwards
            listItem["UrlText"] = siteUrl;

            // Owners = Person field with multiple values                
            FieldUserValue[] users = new FieldUserValue[ownerLogins.Length];

            int i = 0;
            foreach (string ownerLogin in ownerLogins)
            {
                FieldUserValue ownersField = FieldUserValue.FromUser(ownerLogin);
                users[i] = ownersField;
                i++;
            }
            listItem["Owners"] = users;
            listItem["Template"] = template;
            listItem["Status"] = "Requested";
            listItem.Update();
            cc.ExecuteQuery();
        }

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
    }
}
