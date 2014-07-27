namespace Contoso.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Net;
    using System.Security;
    using System.Threading;

    using Contoso.Core.UserProfileService;

    using Microsoft.SharePoint.Client;

    /// <summary>
    /// The user profile mapper instance
    /// </summary>
    public class UserProfileMapper : BaseAction
    {
        #region Fields

        /// <summary>
        /// The fed authentication cookie name
        /// </summary>
        private const string FedAuthCookieName = "FedAuth";

        /// <summary>
        /// The profile service
        /// </summary>
        private const string ProfileService = "/_vti_bin/userprofileservice.asmx";

        /// <summary>
        /// The SharePoint Online cookie value
        /// </summary>
        private const string SPOIDCookieValue = "SPOIDCRL=";

        /// <summary>
        /// The profile service instance
        /// </summary>
        private UserProfileService.UserProfileService profileService = null;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the index of the user name.
        /// </summary>
        /// <value>
        /// The index of the user name.
        /// </value>
        public int UserNameIndex
        {
            get;
            set;
        }

        public int SleepPeriod
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Iterates the row from the CSV file
        /// </summary>
        /// <param name="context">The ClientContext instance.</param>
        /// <param name="entries">The collection values per row.</param>
        /// <param name="logger">The logger.</param>
        public override void IterateCollection(ClientContext context, Collection<string> entries, LogHelper logger)
        {
            List<PropertyData> data = new List<PropertyData>();         

            foreach (PropertyBase item in this.Properties)
            {
                if (item.Index < entries.Count)
                {
                    try
                    {
                        string account = entries[this.UserNameIndex];
                        PropertyData property = new PropertyData();
                        property.Name = item.Name;
                        property = item.Process(property, entries[item.Index], this) as PropertyData;
                        data.Add(property);
                    }
                    catch (Exception ex)
                    {
                        logger.LogException(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Error occured whilst processing account '{0}', Property '{1}'. Stack {2}", entries[this.UserNameIndex], item.Name, ex.ToString()), ex);
                    }
                }
            }

            logger.LogVerbose(string.Format("Attempting to update profile for account '{0}'", entries[this.UserNameIndex]));
            
            try
            {
                this.profileService.ModifyUserPropertyByAccountName(entries[this.UserNameIndex], data.ToArray());
                logger.LogOutcome(entries[this.UserNameIndex], "SUCCESS");
            }
            catch(Exception ex)
            {
                logger.LogException(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Error occured whilst processing account '{0}' - the account does not exist", entries[this.UserNameIndex]), ex);
                logger.LogOutcome(entries[this.UserNameIndex], "FAILURE");
            }

        }

        /// <summary>
        /// Executes the business logic
        /// </summary>
        /// <param name="logger">The logger.</param>
        public override void Run(BaseAction parentAction, DateTime CurrentTime, LogHelper logger)
        {
            if (parentAction != null)
            {
                this.Properties = parentAction.Properties;
            }

            CsvProcessor csvProcessor = new CsvProcessor();

            string[] csvFiles = Directory.GetFiles(this.CSVDirectoryLocation, "*.csv", SearchOption.TopDirectoryOnly);

            logger.LogVerbose(string.Format("Attempting to get files from directory 'location' {0}. Number of files found {1}", this.CSVDirectoryLocation, csvFiles.Length));

            foreach (string csvFile in csvFiles)
            {
                logger.LogVerbose(string.Format("Attempting to read CSV file '{0}' from location {1}", csvFile, this.CSVDirectoryLocation));

                logger.LogVerbose(string.Format("Pausing the utility for '{0}' seconds so ASMX service is not overloaded", this.SleepPeriod));

                Thread.Sleep(this.SleepPeriod * 1000);

                using (StreamReader reader = new StreamReader(csvFile))
                {
                    logger.LogVerbose(string.Format("Establishing connection with tenant at '{0}'", this.TenantSiteUrl));

                    using (ClientContext context = new ClientContext(this.TenantSiteUrl))
                    {
                        Uri site = new Uri(this.TenantSiteUrl);

                        try
                        {
                            UserProfileService.UserProfileService profileService = new UserProfileService.UserProfileService(site.ToString() + ProfileService);
                            this.profileService = profileService;
                            profileService.UseDefaultCredentials = false;

                            using (SecureString password = new SecureString())
                            {
                                foreach (char c in this.TenantAdminPassword.ToCharArray())
                                {
                                    password.AppendChar(c);
                                }

                                logger.LogVerbose(string.Format("Attempting to authenticate against tenant with user name '{1}'", this.TenantSiteUrl, this.TenantAdminUserName));

                                var crudentials = new SharePointOnlineCredentials(this.TenantAdminUserName, password);

                                string cookie = crudentials.GetAuthenticationCookie(site);

                                profileService.CookieContainer = new CookieContainer();
                                profileService.CookieContainer.Add(new Cookie(FedAuthCookieName, cookie.TrimStart(SPOIDCookieValue.ToCharArray()), string.Empty, site.Authority));
                                csvProcessor.Execute(reader, (entries, y) => { IterateCollection(context, entries, logger); }, logger);
                            }
                        }
                        finally
                        {
                            if (this.profileService != null)
                            {
                                this.profileService.Dispose();
                            }
                        }
                    }
                }

                // Clean up current CSV file
                System.IO.File.Delete(csvFile);
            }
        }

        #endregion Methods
    }
}