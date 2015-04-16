namespace Contoso.Core
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Security;
    using System.Xml.Serialization;

    using Microsoft.SharePoint.Client;
    using Contoso.Core.BulkDocumentUploader;

    /// <summary>
    /// The abstract base action class
    /// </summary>
    [XmlType("Action")]
    [Serializable]
    public abstract class BaseAction
    {
        #region Properties

        /// <summary>
        /// Gets or sets the file upload directory.
        /// </summary>
        /// <value>
        /// The file upload directory.
        /// </value>
        public string DirectoryLocation
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
        /// Gets or sets the tenant admin password.
        /// </summary>
        /// <value>
        /// The tenant admin password.
        /// </value>
        public string Password
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
        public string UserName
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
        public abstract void IterateCollection(Collection<string> entries, LogHelper logger);

        /// <summary>
        /// Executes the business logic
        /// </summary>
        /// <param name="logger">The logger.</param>
        public virtual void Run(BaseAction parentAction, DateTime CurrentTime, LogHelper logger = null)
        {
            
        }

        #endregion Methods

        public ActionCollection Actions
        {
            get;
            set;
        }
    }
}