using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Client;

namespace Core.ContentTypesAndFieldsWeb.Pages
{
    public partial class Scenario3 : System.Web.UI.Page
    {
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
        }



        protected void btnScenario3_Click(object sender, EventArgs e)
        {
            // List and content types
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var ctx = spContext.CreateUserClientContextForSPHost())
            {
                string contentTypeId = "0x01010085a744636d22405caea0615c68aafda7";
                // Notice that followign line does break if the content type already exists
                if (!ctx.Web.ContentTypeExistsById(contentTypeId))
                {
                    ctx.Web.CreateContentType("LitwareDoc", contentTypeId, "Contoso Content Types");
                }
                if (!ctx.Web.ListExists(txtListName.Text))
                {
                    ctx.Web.CreateList(ListTemplateType.DocumentLibrary, txtListName.Text, false);
                    // Enable content types in list
                    List list = ctx.Web.GetListByTitle(txtListName.Text);
                    list.ContentTypesEnabled = true;
                    list.Update();
                    ctx.Web.Context.ExecuteQuery();
                }
                if (!ctx.Web.ContentTypeExistsByName(txtListName.Text, "LitwareDoc"))
                {
                    ctx.Web.AddContentTypeToListByName(txtListName.Text, "LitwareDoc");
                }
                
                // Set the content type as default content type to the TestLib list
                ctx.Web.SetDefaultContentTypeToList(txtListName.Text, contentTypeId);

                lblStatus3.Text = string.Format("Created new list called '{0}' and asscoated document type called 'LitwareDoc' to it'. Move to <a href='{1}'>host web</a> and test the functionality.", txtListName.Text, spContext.SPHostUrl.ToString());
            }
        }
    }
}