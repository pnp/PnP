using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;

namespace Core.ContentTypesAndFieldsWeb.Pages
{
    public partial class Scenario2 : System.Web.UI.Page
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
                GenerateTaxonomyDropDowns();
            }
        }

        protected void btnScenario2_Click(object sender, EventArgs e)
        {
            // Taxonomy field to host web - Note that this requires that group and taxonomy set exists when the code is executed.
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            var taxFieldId = Guid.NewGuid();

            using (var ctx = spContext.CreateUserClientContextForSPHost())
            {
                var groupName = drpGroups.SelectedItem.Text;
                var termSetName = drpTermSets.SelectedItem.Text;
                var taxFieldName = "ContosoTaxonomySample";

                if (!ctx.Web.FieldExistsByName(taxFieldName))
                {
                    ctx.Web.CreateTaxonomyField(taxFieldId, taxFieldName, "Contoso Taxonomy Sample", "Contoso Fields", groupName, termSetName);
                    lblStatus2.Text = "Created new taxonomy field with name of 'Contoso Taxonomy Sample'. Move to host web and test the functionality.";
                }
                else
                {
                    ctx.Web.WireUpTaxonomyField(taxFieldId, groupName, termSetName);
                    lblStatus2.Text = "Taxonomy field with planned Id already existed";
                }
            }
        }
        protected void drpGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTermSetsBasedOnSelectedGroup(drpGroups.SelectedValue);
        }

        private void UpdateTermSetsBasedOnSelectedGroup(string groupId)
        {
            // Update Term set drop down for the taxonomy field creation.
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var ctx = spContext.CreateUserClientContextForSPHost())
            {
                TaxonomySession taxonomySession = TaxonomySession.GetTaxonomySession(ctx);
                TermStore termStore = taxonomySession.GetDefaultSiteCollectionTermStore();
                ctx.Load(termStore);
                TermGroup group = termStore.GetGroup(new Guid(groupId));
                ctx.Load(group);
                TermSetCollection termSets = group.TermSets;
                ctx.Load(termSets);
                ctx.ExecuteQuery();

                drpTermSets.DataTextField = "Name";
                drpTermSets.DataValueField = "Id";
                drpTermSets.DataSource = termSets.OrderBy(ts => ts.Name);
                drpTermSets.DataBind();
            }
        }

        /// <summary>
        /// Used to update the taxonomoy drop downs for creating taxonomy field to host web.
        /// </summary>
        private void GenerateTaxonomyDropDowns()
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var ctx = spContext.CreateUserClientContextForSPHost())
            {
                TaxonomySession taxonomySession = TaxonomySession.GetTaxonomySession(ctx);
                TermStore termStore = taxonomySession.GetDefaultSiteCollectionTermStore();
                ctx.Load(termStore,
                        store => store.Name,
                        store => store.Groups.Include(
                            group => group.Name,
                            group => group.Id)
                        );
                ctx.ExecuteQuery();

                foreach (TermGroup group in termStore.Groups)
                {
                    drpGroups.Items.Add(new System.Web.UI.WebControls.ListItem(group.Name, group.Id.ToString()));
                }

            }
            // Updated term setup drop down
            UpdateTermSetsBasedOnSelectedGroup(drpGroups.SelectedValue);
        }
    }
}