using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OfficeDevPnP.SPOnline.Commands.Base
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
                throw new InvalidOperationException(Properties.Resources.NoConnection);
            }
            if (ClientContext == null)
            {
                throw new InvalidOperationException(Properties.Resources.NoConnection);
            }
            if (SPOnlineConnection.CurrentConnection.ConnectionType != SPOnlineConnection.ConnectionTypes.TenantAdmin)
            {
                throw new InvalidOperationException(Properties.Resources.CurrentSiteIsNoTenantAdminSite);
            }
        }
    }
}
