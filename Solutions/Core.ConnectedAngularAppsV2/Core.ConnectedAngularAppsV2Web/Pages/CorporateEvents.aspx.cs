using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.UserProfiles;
using System.Text;



namespace Core.ConnectedAngularAppsV2Web.Pages
{
    public partial class CorporateEvents : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                string hostweburl = Request["SPHostUrl"];
                string appweburl = Request["SPAppWebUrl"];

                //get context token for 
                var contextToken = TokenHelper.GetContextTokenFromRequest(Page.Request);

                string userAcctName = string.Empty;
                using (var clientContext = TokenHelper.GetClientContextWithContextToken(appweburl, contextToken, Request.Url.Authority))
                {
                    //// PeopleManager class provides the methods for operations related to people
                    PeopleManager peopleManager = new PeopleManager(clientContext);

                    //// PersonProperties class is used to represent the user properties
                    //// GetMyProperties method is used to get the current user's properties 
                    PersonProperties personProperties = peopleManager.GetMyProperties();
                    clientContext.Load(personProperties, p => p.AccountName);
                    clientContext.ExecuteQuery();

                    userAcctName = personProperties.AccountName;
                    //userLogin.Text = userAcctName;
                    //lblUserLogin.Text = userAcctName;                   
                    
                } 
            }
        }
    }
}