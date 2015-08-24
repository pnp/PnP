using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Entities;

namespace Core.ContentTypesAndFieldsWeb.Pages
{
    public partial class Scenario1 : System.Web.UI.Page
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

            if (!Page.IsPostBack)
            {
                txtContentTypeExtension.Text = Guid.NewGuid().ToString().Replace("-", "");
                txtContentTypeName.Text = "ContosoDoc-" + DateTime.Now.Ticks.ToString();
                GenerateContentTypeList();
                drpContentTypes.SelectedValue = "0x0101";
            }
        }

        protected void btnScenario1_Click(object sender, EventArgs e)
        {
            // Create New Content Type
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var ctx = spContext.CreateUserClientContextForSPHost())
            {
                // Call the custom web object extension 
                Guid fieldId = new Guid("439c9788-ea39-4e74-941d-ed29b22f9b6b");
                string ctId = string.Format("{0}00{1}", drpContentTypes.SelectedValue, txtContentTypeExtension.Text);

                // Do not re-create it
                if (!ctx.Web.ContentTypeExistsByName(txtContentTypeName.Text))
                {
                    ctx.Web.CreateContentType(txtContentTypeName.Text, ctId, "Contoso Content Types");
                }
                else
                {
                    lblStatus1.Text = string.Format("Content type with given name and/or ID already existed. Name -  {0} ID - {1}",
                                txtContentTypeName.Text, ctId);
                    return;
                }
                if (!ctx.Web.FieldExistsByName("ContosoFieldText"))
                {
                    FieldCreationInformation field = new FieldCreationInformation(FieldType.Text)
                    {
                        Id = fieldId,
                        InternalName = "ContosoFieldText",
                        DisplayName = "Contoso Field Text",
                        Group = "Contoso Fields"
                    };
                    ctx.Web.CreateField(field);
                }
                // This will never be true for this sample, but shows the pattern
                if (!ctx.Web.FieldExistsByNameInContentType(txtContentTypeName.Text, "ContosoFieldText"))
                {
                    ctx.Web.AddFieldToContentTypeByName(txtContentTypeName.Text, fieldId);
                }

                // Done - all good to go.
                lblStatus1.Text = string.Format("Created new content type to host web with name {0} and ID {1}",
                                                txtContentTypeName.Text, ctId);
            }
        }

        private void GenerateContentTypeList()
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var ctx = spContext.CreateUserClientContextForSPHost())
            {
                ContentTypeCollection contentTypes = ctx.Web.ContentTypes;
                ctx.Load(contentTypes);
                ctx.ExecuteQuery();

                drpContentTypes.DataTextField = "Name";
                drpContentTypes.DataValueField = "StringId";
                drpContentTypes.DataSource = contentTypes.OrderBy(ct => ct.Name);
                drpContentTypes.DataBind();
            }
        }
    }
}