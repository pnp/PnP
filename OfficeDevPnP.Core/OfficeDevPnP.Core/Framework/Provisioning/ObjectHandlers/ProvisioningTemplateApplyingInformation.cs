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
    public delegate void ProvisioningProgressDelegate(string message, int step, int total);

    public delegate void ProvisioningMessagesDelegate(string message, ProvisioningMessageType messageType);
    public class ProvisioningTemplateApplyingInformation
    {
        public ProvisioningProgressDelegate ProgressDelegate { get; set; }
        public ProvisioningMessagesDelegate MessageDelegate { get; set; }

        /// <summary>
        /// If true, system propertybag entries that start with _, vti_, dlc_ etc. will be overwritten if overwrite = true on the PropertyBagEntry. If not true those keys will be skipped, regardless of the overwrite property of the entry.
        /// </summary>
        public bool OverwriteSystemPropertyBagValues { get; set; }
    }
}
