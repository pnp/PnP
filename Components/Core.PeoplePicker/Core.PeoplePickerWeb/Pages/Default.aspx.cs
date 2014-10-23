using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Contoso.Core.PeoplePickerWeb
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            Uri redirectUrl;
            switch (SharePointContextProvider.CheckRedirectionStatus(Context, out redirectUrl))
            {
                case RedirectionStatus.Ok:
                    return;
                case RedirectionStatus.ShouldRedirect:
                    Response.Redirect(redirectUrl.AbsoluteUri, endResponse: true);
                    break;
                case RedirectionStatus.CanNotRedirect:
                    Response.Write("An error occurred while processing your request.");
                    Response.End();
                    break;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // define initial script, needed to render the chrome control
            string script = @"
            function chromeLoaded() {
                $('body').show();
            }

            //function callback to render chrome after SP.UI.Controls.js loads
            function renderSPChrome() {
                //Set the chrome options for launching Help, Account, and Contact pages
                var options = {
                    'appTitle': document.title,
                    'onCssLoaded': 'chromeLoaded()'
                };

                //Load the Chrome Control in the divSPChrome element of the page
                var chromeNavigation = new SP.UI.Controls.Navigation('divSPChrome', options);
                chromeNavigation.setVisible(true);
            }";

            //register script in page
            Page.ClientScript.RegisterClientScriptBlock(typeof(Default), "BasePageScript", script, true);

            if (!Page.IsPostBack)
            {
                // prefil people pickers with current user
                var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    clientContext.Load(clientContext.Web, web => web.Title, user => user.CurrentUser);
                    clientContext.ExecuteQuery();
                    Microsoft.SharePoint.Client.User currentUser = clientContext.Web.CurrentUser;
                    
                    //fill json meoplepicker
                    List<PeoplePickerUser> peoplePickerUsers = new List<PeoplePickerUser>(1);
                    peoplePickerUsers.Add(new PeoplePickerUser() {  Name=currentUser.Title, Email=currentUser.Email, Login=currentUser.LoginName});
                    hdnAdministrators.Value = JsonHelper.Serialize<List<PeoplePickerUser>>(peoplePickerUsers);   
                 
                    //fill csom peoplepicker
                    PeoplePickerHelper.FillPeoplePickerValue(hdnCsomAdministrators, currentUser);
                }
            }
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            List<PeoplePickerUser> users = JsonHelper.Deserialize<List<PeoplePickerUser>>(hdnAdministrators.Value);

            string parsedResult = "";
            foreach (var user in users)
            {
                if (parsedResult.Length > 0)
                {
                    parsedResult = parsedResult + "," + user.Name;
                }
                else
                {
                    parsedResult = user.Name;
                }
            }

            this.lblEnteredData.Text = parsedResult;
        }

        //This webmethod is called by the csom peoplepicker to retrieve search data
        //In a MVC application you can use a Json Action method
        [WebMethod]
        public static string GetPeoplePickerData()
        {
            //peoplepickerhelper will get the needed values from the querrystring, get data from sharepoint, and return a result in Json format
            return PeoplePickerHelper.GetPeoplePickerSearchData();
        }

        protected void btnGetValueByServer_Click(object sender, EventArgs e)
        {
            //get values from csom peoplepicker
            List<PeoplePickerUser> users = PeoplePickerHelper.GetValuesFromPeoplePicker(hdnCsomAdministrators);

            string parsedResult = "";
            foreach (var user in users)
            {
                if (parsedResult.Length > 0)
                {
                    parsedResult = parsedResult + "," + user.Name;
                }
                else
                {
                    parsedResult = user.Name;
                }
            }

            this.lblCsomEnteredData.Text = parsedResult;
        }
    }
}