using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using System;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    internal delegate bool ShouldProvisionTest(Web web, ProvisioningTemplate template);

    internal abstract class ObjectHandlerBase
    {
        internal bool? _willExtract;
        internal bool? _willProvision;

        private bool _reportProgress = true;
        public abstract string Name { get; }

        public bool ReportProgress
        {
            get { return _reportProgress; }
            set { _reportProgress = value; }
        }

        public ProvisioningMessagesDelegate MessagesDelegate { get; set; }

        public abstract bool WillProvision(Web web, ProvisioningTemplate template);

        public abstract bool WillExtract(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo);

        public abstract void ProvisionObjects(Web web, ProvisioningTemplate template, ProvisioningTemplateApplyingInformation applyingInformation);

        public abstract ProvisioningTemplate ExtractObjects(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo);

        internal void WriteWarning(string message, ProvisioningMessageType messageType)
        {
            if (MessagesDelegate != null)
            {
                MessagesDelegate(message, messageType);
            }
        }

        protected string Tokenize(string url, string webUrl)
        {
            if (string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }
            if (url.IndexOf("azurewebsites.net", StringComparison.OrdinalIgnoreCase) > -1)
            {
                // If refered to the app site itself, don't tokenize.
                return url;
            }
            if (url.IndexOf("/_catalogs/theme", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                return url.Substring(url.IndexOf("/_catalogs/theme", StringComparison.InvariantCultureIgnoreCase)).Replace("/_catalogs/theme", "{themecatalog}");
            }
            if (url.IndexOf("/_catalogs/masterpage", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                return url.Substring(url.IndexOf("/_catalogs/masterpage", StringComparison.InvariantCultureIgnoreCase)).Replace("/_catalogs/masterpage", "{masterpagecatalog}");
            }
            if (url.IndexOf(webUrl, StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                return url.Replace(webUrl, "{site}");
            }
            if (url.IndexOf(webUrl, StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                return url.Substring(url.IndexOf(webUrl, StringComparison.InvariantCultureIgnoreCase)).Replace(webUrl, "{site}");
            }

            Uri r;
            if (Uri.TryCreate(webUrl, UriKind.Absolute, out r) && url.IndexOf(r.PathAndQuery, StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                return r.PathAndQuery.Equals("/") ? url.TrimStart('/').Insert(0, "{site}/") : url.Replace(r.PathAndQuery, "{site}");
            }

            // nothing to tokenize...
            return url;
        }
    }
}
