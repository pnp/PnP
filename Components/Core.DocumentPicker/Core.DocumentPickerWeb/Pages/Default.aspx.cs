using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Core.DocumentPickerWeb
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
            RenderChromeControl();

            if (!IsPostBack)
            {
                //provision data needed for this demo
                var provisionner = new Provisionner();
                provisionner.ProvisionData(Context);

                //data to use later in javascript (set document from javascript)
                var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
                defaultDocumentUrl.Value = provisionner.GetSampleDocumentUrl2(spContext);
                defaultDocumentPath.Value = provisionner.GetSampleDocumentPath2();
                DocList1Id.Value = provisionner.ProvisionnedList1Id.ToString();
                DocList2Id.Value = provisionner.ProvisionnedList2Id.ToString();


                //set default document in standard peoplepicker
                List<PickedDocument> documents = new List<PickedDocument>();

                //the sample is setting the url and (fake) id data manually. In a real situation you get this data from sharepoint.
                PickedDocument doc = new PickedDocument();
                doc.DocumentUrl = provisionner.GetSampleDocumentUrl1(spContext);
                doc.DocumentPath = provisionner.GetSampleDocumentPath1();
                doc.ItemId = "1"; 
                documents.Add(doc);

                DocumentPickerHelper.SetData(BasicDocumentPickerValue, documents);
            }
        }

        private void RenderChromeControl()
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

        //this method will get the selected values from the document picker
        protected void GetValuesButton_Click(object sender, EventArgs e)
        {
            var selectedDocuments = DocumentPickerHelper.GetData(BasicDocumentPickerValue);

            string output = "<b>Selected documents:</b><br>";
            foreach (var document in selectedDocuments)
            {
                output += "<b>Item:</b> " + document.ItemId + " <br> ";
                output += "<b>Path:</b> " + document.DocumentPath + " <br> ";
                output += "<b>Url:</b> " + document.DocumentUrl + " <br> ";
                output += "------------- <br> ";
            }

            OutputLabel.Text = output;
        }
    }
}