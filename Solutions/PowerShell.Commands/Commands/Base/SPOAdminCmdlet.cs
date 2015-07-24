using System;
using System.Management.Automation;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.PowerShell.Commands;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.Commands.Enums;
using Resources = OfficeDevPnP.PowerShell.Commands.Properties.Resources;

namespace OfficeDevPnP.PowerShell.Commands.Base
{
    public class SPOAdminCmdlet : SPOCmdlet
    {
        private Tenant _tenant;
        public Tenant Tenant
        {
            get
            {
                if (_tenant == null)
                {
                    _tenant = new Tenant(ClientContext);

                }
                return _tenant;
            }
        }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            if (SPOnlineConnection.CurrentConnection == null)
            {
                throw new InvalidOperationException(Resources.NoConnection);
            }
            if (ClientContext == null)
            {
                throw new InvalidOperationException(Resources.NoConnection);
            }

            SPOnlineConnection.CurrentConnection.CacheContext();

            Uri uri = new Uri(this.ClientContext.Url);
            var urlParts = uri.Authority.Split(new[] { '.' });
            if (!urlParts[0].EndsWith("-admin") && SPOnlineConnection.CurrentConnection.ConnectionType == ConnectionType.O365)
            {
                var adminUrl = string.Format("https://{0}-admin.{1}.{2}", urlParts[0], urlParts[1], urlParts[2]);

                SPOnlineConnection.CurrentConnection.Context = this.ClientContext.Clone(adminUrl);
            }
            
        }

        protected override void EndProcessing()
        {
            SPOnlineConnection.CurrentConnection.RestoreCachedContext();
        }
    }
}
