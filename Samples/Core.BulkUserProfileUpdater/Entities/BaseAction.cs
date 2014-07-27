namespace Contoso.Core
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Security;
    using System.Xml.Serialization;

    using Microsoft.SharePoint.Client;

    /// <summary>
    /// The abstract base action class
    /// </summary>
    [XmlType("Action")]
    [Serializable]
    public abstract class BaseAction
    {
        #region Properties

        /// <summary>
        /// Gets or sets the CSV file location.
        /// </summary>
        /// <value>
        /// The CSV file location.
        /// </value>
        public string CSVDirectoryLocation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the errors.
        /// </summary>
        /// <value>
        /// The errors.
        /// </value>
        public Collection<string> Errors
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        public PropertyCollection Properties
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tenant admin password.
        /// </summary>
        /// <value>
        /// The tenant admin password.
        /// </value>
        public string TenantAdminPassword
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the tenant admin user.
        /// </summary>
        /// <value>
        /// The name of the tenant admin user.
        /// </value>
        public string TenantAdminUserName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tenant site URL.
        /// </summary>
        /// <value>
        /// The tenant site URL.
        /// </value>
        public string TenantSiteUrl
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
        public abstract void IterateCollection(ClientContext context, Collection<string> entries, LogHelper logger);

        /// <summary>
        /// Executes the business logic
        /// </summary>
        /// <param name="logger">The logger.</param>
        public virtual void Run(BaseAction parentAction, DateTime CurrentTime, LogHelper logger = null)
        {
            CsvProcessor csvProcessor = new CsvProcessor();

            string[] csvFiles = Directory.GetFiles(this.CSVDirectoryLocation, "*.csv", SearchOption.TopDirectoryOnly);

            foreach (string csvFile in csvFiles)
            {
                using (StreamReader reader = new StreamReader(csvFile))
                {
                    using (ClientContext context = new ClientContext(this.TenantSiteUrl))
                    {
                        using (SecureString password = new SecureString())
                        {
                            foreach (char c in this.TenantAdminPassword.ToCharArray())
                            {
                                password.AppendChar(c);
                            }

                            context.Credentials = new SharePointOnlineCredentials(this.TenantAdminUserName, password);
                        }

                        csvProcessor.Execute(reader, (entries, y) => { IterateCollection(context, entries, logger); }, logger);
                    }
                }
            }
        }

        #endregion Methods

        public ActionCollection Actions
        {
            get;
            set;
        }
    }
}