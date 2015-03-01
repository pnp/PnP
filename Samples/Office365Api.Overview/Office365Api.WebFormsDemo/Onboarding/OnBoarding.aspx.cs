using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Office365Api.WebFormsDemo
{
    public partial class OnBoarding : System.Web.UI.Page
    {
        private ApplicationADALDbContext db = new ApplicationADALDbContext();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SignUpCommand_Click(object sender, EventArgs e)
        {
            Tenant tenant = new Tenant();

            // generate a random value to identify the request
            string stateMarker = Guid.NewGuid().ToString();
            // store it in the temporary entry for the tenant, we'll use it later to assess if the request was originated from us
            // this is necessary if we want to prevent attackers from provisioning themselves to access our app without having gone through our onboarding process (e.g. payments, etc)
            tenant.IssValue = stateMarker;
            tenant.Created = DateTime.Now;
            db.Tenants.Add(tenant);
            db.SaveChanges();

            //create an OAuth2 request, using the web app as the client.
            //this will trigger a consent flow that will provision the app in the target tenant
            string authorizationRequest = String.Format(
                "https://login.windows.net/common/oauth2/authorize?response_type=code&client_id={0}&resource={1}&redirect_uri={2}&state={3}",
                 Uri.EscapeDataString(ConfigurationManager.AppSettings["ida:ClientId"]),
                 Uri.EscapeDataString("https://graph.windows.net"),
                 Uri.EscapeDataString(this.Request.Url.GetLeftPart(UriPartial.Authority).ToString() + "/Onboarding/ProcessCode"),
                 Uri.EscapeDataString(stateMarker)
                 );
            //if the prospect customer wants to provision the app for all users in his/her tenant, the request must be modified accordingly
            if (tenant.AdminConsented)
                authorizationRequest += String.Format("&prompt={0}", Uri.EscapeDataString("admin_consent"));
            // send the user to consent
            Response.Redirect(authorizationRequest);
        }
    }
}