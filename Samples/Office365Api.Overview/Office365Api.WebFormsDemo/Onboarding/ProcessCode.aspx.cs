using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Office365Api.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Office365Api.WebFormsDemo.Onboarding
{
    public partial class ProcessCode : System.Web.UI.Page
    {
        private ApplicationADALDbContext db = new ApplicationADALDbContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            String code = Request["code"];
            String error = Request["error"];
            String error_description = Request["error_description"];
            String resource = Request["resource"];
            String state = Request["state"];

            // Is this a response to a request we generated? Let's see if the state is carrying an ID we previously saved
            // ---if we don't, return an error            
            if (db.Tenants.FirstOrDefault(a => a.IssValue == state) == null)
            {
                failedOnBoarding.Visible = true;
                errorMessage.Text = error_description;
                errorDescription.Text = error_description;
            }
            else
            {
                successedOnBoarding.Visible = true;

                // ---if the response is indeed from a request we generated
                // ------get a token for the Graph, that will provide us with information abut the caller
                ClientCredential credential = new ClientCredential(AuthenticationHelper.ClientId, AuthenticationHelper.SharedSecret);

                AuthenticationContext authContext = new AuthenticationContext(AuthenticationHelper.AuthorityMultitenant);
                AuthenticationResult result = authContext.AcquireTokenByAuthorizationCode(
                    code, new Uri(Request.Url.GetLeftPart(UriPartial.Path)), credential);

                var myTenant = db.Tenants.FirstOrDefault(a => a.IssValue == state);
                // if this was an admin consent, save the tenant
                if (myTenant.AdminConsented)
                {
                    // ------read the tenantID out of the Graph token and use it to create the issuer string
                    string issuer = String.Format("https://sts.windows.net/{0}/", result.TenantId);
                    myTenant.IssValue = issuer;
                }
                else
                //otherwise, remove the temporary entry and save just the user
                {
                    if (db.Users.FirstOrDefault(a => (a.UPN == result.UserInfo.DisplayableId) && (a.TenantID == result.TenantId)) == null)
                    {
                        db.Users.Add(new User { UPN = result.UserInfo.DisplayableId, TenantID = result.TenantId });
                    }
                    db.Tenants.Remove(myTenant);
                }

                // remove older, unclaimed entries
                DateTime tenMinsAgo = DateTime.Now.Subtract(new TimeSpan(0, 10, 0)); // workaround for Linq to entities
                var garbage = db.Tenants.Where(a => (!a.IssValue.StartsWith("https") && (a.Created < tenMinsAgo)));
                foreach (Tenant t in garbage)
                    db.Tenants.Remove(t);

                db.SaveChanges();
            }
        }
    }
}