using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Contoso.Provisioning.Cloud.SyncWeb.Modules
{
    public class BaseProvisioningModule : UserControl
    {
        public virtual void Provision(ClientContext context, Web web)
        {
        }
    }
}