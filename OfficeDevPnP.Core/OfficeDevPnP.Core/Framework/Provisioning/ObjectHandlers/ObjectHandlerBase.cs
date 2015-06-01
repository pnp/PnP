using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using System;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public abstract class ObjectHandlerBase
    {
        private bool _reportProgress = true;
        public abstract string Name { get; }
        public bool ReportProgress
        {
            get { return _reportProgress; }
            set { _reportProgress = value; }
        }

        public abstract void ProvisionObjects(Web web, ProvisioningTemplate template);

        public abstract ProvisioningTemplate CreateEntities(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo);

        protected string Tokenize(string url, string webUrl)
        {
            if (string.IsNullOrEmpty(url))
            {
                return "";
            }
            else
            {
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
                else
                {
                    Uri r;
                    if (Uri.TryCreate(webUrl, UriKind.Absolute, out r))
                    {
                        if (url.IndexOf(r.PathAndQuery, StringComparison.InvariantCultureIgnoreCase) > -1)
                        {
                            return url.Replace(r.PathAndQuery, "{site}");
                        }
                    }
                }

                // nothing to tokenize...
                return url;
            }
        }
    }
}
