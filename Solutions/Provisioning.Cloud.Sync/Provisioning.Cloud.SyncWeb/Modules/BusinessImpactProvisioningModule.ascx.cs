using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Contoso.Provisioning.Cloud.SyncWeb.Modules
{
    public partial class BusinessImpactProvisioningModule : BaseProvisioningModule
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (cboSensitivity.Items.Count == 0)
            {
                cboSensitivity.Items.Add(new ListItem("Low Business Impact (LBI)", "LBI"));
                cboSensitivity.Items.Add(new ListItem("Medium Business Impact (MBI)", "MBI"));
                cboSensitivity.Items.Add(new ListItem("High Business Impact (HBI)", "HBI"));
            }
        }

        public override void Provision(Microsoft.SharePoint.Client.ClientContext context, Microsoft.SharePoint.Client.Web web)
        {
            //get the web's property bag
            var props = web.AllProperties;
            context.Load(props);
            context.ExecuteQuery();

            //set the ContosoBusinessImpact property and update
            props["ContosoBusinessImpact"] = cboSensitivity.SelectedValue;
            web.Update();
            context.ExecuteQuery();

            //call the base
            base.Provision(context, web);
        }
    }
}