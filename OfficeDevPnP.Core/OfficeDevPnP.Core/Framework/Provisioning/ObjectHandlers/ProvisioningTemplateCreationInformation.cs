using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public class ProvisioningTemplateCreationInformation
    {
        private ProvisioningTemplate baseTemplate;
        private FileConnectorBase fileConnector;
        private bool persistComposedLookFiles = false;
        private bool includeAllTermGroups = false;
        private bool includeSiteCollectionTermGroup = false;

        public ProvisioningTemplateCreationInformation(Web web)
        {
            this.baseTemplate = web.GetBaseTemplate();
        }

        /// <summary>
        /// Base template used to compare against when we're "getting" a template
        /// </summary>
        public ProvisioningTemplate BaseTemplate
        {
            get
            {
                return this.baseTemplate;
            }
            set
            {
                this.baseTemplate = value;
            }
        }

        /// <summary>
        /// Connector used to persist files when needed
        /// </summary>
        public FileConnectorBase FileConnector
        {
            get
            {
                return this.fileConnector;
            }
            set
            {
                this.fileConnector = value;
            }
        }

        /// <summary>
        /// Do composed look files (theme files, site logo, alternate css) need to be persisted to storage when 
        /// we're "getting" a template
        /// </summary>
        public bool PersistComposedLookFiles
        {
            get
            {
                return this.persistComposedLookFiles;
            }
            set
            {
                this.persistComposedLookFiles = value;
            }
        }

        public bool IncludeAllTermGroups
        {
            get
            {
                return this.includeAllTermGroups;
            }
            set { this.includeAllTermGroups = value; }
        }

        public bool IncludeSiteCollectionTermGroup
        {
            get { return this.includeSiteCollectionTermGroup; }
            set { this.includeSiteCollectionTermGroup = value; }
        }

    }
}
