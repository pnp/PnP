using System;
using Microsoft.Online.SharePoint.TenantAdministration;
using OfficeDevPnP.PowerShell.Commands.Enums;
using OfficeDevPnP.PowerShell.Commands.Properties;

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
                throw new InvalidOperationException(Resources.CurrentSiteIsNoTenantAdminSite);
            }
        }
    }
}
