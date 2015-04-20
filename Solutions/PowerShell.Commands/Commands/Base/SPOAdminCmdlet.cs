using System;
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
            if (SPOnlineConnection.CurrentConnection.ConnectionType != ConnectionType.TenantAdmin)
            {
                Uri uri = new Uri(this.ClientContext.Url);
                var urlParts = uri.Authority.Split(new[] { '.' });
                if (!urlParts[0].EndsWith("-admin"))
                {
                    var adminUrl = string.Format("https://{0}-admin.{1}.{2}", urlParts[0], urlParts[1], urlParts[2]);

                    SPOnlineConnection.CurrentConnection.Context = this.ClientContext.Clone(adminUrl);
                }
                else
                {
                    throw new InvalidOperationException(Resources.CurrentSiteIsNoTenantAdminSite);
                }
            }
        }

        protected override void EndProcessing()
        {
            SPOnlineConnection.CurrentConnection.RestoreCachedContext();
        }
    }
}
