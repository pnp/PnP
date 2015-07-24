using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Contoso.Core.TaxonomyPickerWeb
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
            Page.ClientScript.RegisterClientScriptBlock(typeof(Default), "ChromeLoadScript", script, true);


            /*
            //The following code shows how to set a taxonomy field server-side
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                var list = clientContext.Web.Lists.GetByTitle("MyList");
                var listItem = list.GetItemById(1);

                clientContext.Load(listItem);
                clientContext.ExecuteQuery();

                taxPickerGeographySingle.Value = ((TaxonomyFieldValue)listItem["MyTaxFieldSingle"]).Serialize();
                taxPickerGeographyMulti.Value = ((TaxonomyFieldValueCollection)listItem["MyTaxFieldMulti"]).Serialize();
            }
            */
        }

        protected void SubmitButton_Click(object sender, EventArgs e) {
            var fieldValue = taxPickerKeywords.Value;

            // The item's JSON value will be added to a bulletted list
            // comment this out if the next section is used
            SelectedValues.Items.Add(fieldValue);

            //var myFieldInternalName = "MyFieldInternalName";
            //var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            //using (var clientContext = spContext.CreateUserClientContextForSPHost()) {
            //    clientContext.Load(clientContext.Web);
            //    clientContext.Load(clientContext.Web.Fields, fs => fs.Where(f => f.InternalName == myFieldInternalName));
            //    var field = (TaxonomyField)clientContext.Web.Fields.FirstOrDefault();

            //    if (field == null)
            //        throw new IndexOutOfRangeException(string.Format("{0} does not exist in the current site's fields.", myFieldInternalName));

            //    var taxValues = new TaxonomyFieldValueCollection(clientContext, fieldValue, field);
            //    SelectedValues.DataSource = taxValues.Cast<TaxonomyFieldValue>().ToList();
            //    SelectedValues.DataBind();
            //}
        }
    }
}