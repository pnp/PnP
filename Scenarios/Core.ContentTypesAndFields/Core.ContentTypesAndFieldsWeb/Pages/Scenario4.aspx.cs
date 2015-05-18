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
    public partial class Scenario4 : System.Web.UI.Page
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

        protected void btnScenario4_Click(object sender, EventArgs e)
        {
            // Localize content type and site column
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var ctx = spContext.CreateUserClientContextForSPHost())
            {
                // Create a document content type inherited from Document
                string contentTypeId = "0x0101001f11771d89214705b6c4baf77b0f219c";
                Guid fieldId = new Guid("92faab58-65ed-4035-8e16-446a6c575f4d");

                // Notice that followign line does break if the content type already exists
                if (!ctx.Web.ContentTypeExistsById(contentTypeId))
                {
                    ctx.Web.CreateContentType("LitwareDoc2", contentTypeId, "Contoso Content Types");
                }
                // Add site colummn to the content type, create column if it does not exist
                if (!ctx.Web.FieldExistsByName("LitwareFieldText"))
                {
                    FieldCreationInformation field = new FieldCreationInformation(FieldType.Text)
                    {
                        Id = fieldId,
                        InternalName = "LitwareFieldText",
                        DisplayName = "Litware Field Text",
                        Group = "Contoso Fields"
                    };
                    ctx.Web.CreateField(field);
                }
                // Add field to content type
                //ctx.Web.AddFieldToContentTypeById(contentTypeId, fieldId);
                // Create list and associate document to the list
                if (!ctx.Web.ListExists(txtListName.Text))
                {
                    ctx.Web.CreateList(ListTemplateType.DocumentLibrary, txtListName.Text, false);
                    // Enable content types in list
                    List list = ctx.Web.GetListByTitle(txtListName.Text);
                    list.ContentTypesEnabled = true;
                    list.Update();
                    ctx.Web.Context.ExecuteQuery();
                }
                // Create list and associate document to the list
                if (!ctx.Web.ContentTypeExistsByName(txtListName.Text, "LitwareDoc2"))
                {
                    ctx.Web.AddContentTypeToListByName(txtListName.Text, "LitwareDoc2");
                }

                // Set the content type as default content type to the TestLib list
                ctx.Web.SetDefaultContentTypeToList(txtListName.Text, contentTypeId);

                //Set translations to content type
                ctx.Web.SetLocalizationForContentType("LitwareDoc2", "fi-fi", "Litware Dokumentti", "Litware  dokumentti on tässä");
                ctx.Web.SetLocalizationForContentType("LitwareDoc2", "es-es", "Litware documento", "Litware  documento");
                
                //Set translations to site columns
                ctx.Web.SetLocalizationForField(fieldId, "fi-fi", "Litware Teksti kenttä", "Litware Teksti kenttä");
                ctx.Web.SetLocalizationForField(fieldId, "es-es", "Field Name (es)", "Field Name (es)");

                //Set translations to list - Seems to have issues right now, so commented
                // ctx.Web.SetLocalizationLabelsForList(txtListName.Text, "fi-fi", "Listan nimi suomeksi", "Listan nimi suomeksi.");
                // ctx.Web.SetLocalizationLabelsForList(txtListName.Text, "es-es", "List name (es)", "List description (es)");

                lblStatus4.Text = "Created new content type and list with translations. Check the blog posts for requirements for end users to see translations in practice.";
                
            }
        }
    }
}