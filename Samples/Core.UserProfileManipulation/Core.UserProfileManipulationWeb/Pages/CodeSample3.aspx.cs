using Microsoft.SharePoint.Client.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Core.UserProfileManipulationWeb.Pages
{
    public partial class CodeSample3 : System.Web.UI.Page
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

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            // Update about me value
            if (!Page.IsPostBack)
            {
                RefreshUIValues();
            }
        }

        protected void RefreshUIValues()
        {

            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                // Get the people manager instance and load current properties
                PeopleManager peopleManager = new PeopleManager(clientContext);
                PersonProperties personProperties = peopleManager.GetMyProperties();
                clientContext.Load(personProperties);
                clientContext.ExecuteQuery();

                // Just to output what we have now for skills now
                string skills = personProperties.UserProfileProperties["SPS-Skills"];
                lblSkills.Text = skills;
                var skillArray = skills.Split(new Char[] { '|' });
                lstSkills.Items.Clear();
                foreach (var item in skillArray)
                {
                    lstSkills.Items.Add(item);
                }
            }

        }
        protected void btnRemoveSkill_Click(object sender, EventArgs e)
        {
            if (lstSkills.SelectedIndex > -1)
            {
                lstSkills.Items.RemoveAt(lstSkills.SelectedIndex);
            }
        }

        protected void btnAddSkill_Click(object sender, EventArgs e)
        {
            if (txtSkillToAdd.Text.Length > 0)
            {
                lstSkills.Items.Add(txtSkillToAdd.Text);
            }
        }

        protected void btnScenario3_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                // Get the people manager instance for current context to get account name
                PeopleManager peopleManager = new PeopleManager(clientContext);
                PersonProperties personProperties = peopleManager.GetMyProperties();
                clientContext.Load(personProperties, p => p.AccountName);
                clientContext.ExecuteQuery();

                // Collect values for profile update
                List<string> skills = new List<string>();
                for (int i = 0; i < lstSkills.Items.Count; i++)
                {
                    skills.Add(lstSkills.Items[i].Value);
                }

                // Update the SPS-Skills property for the user using account name from profile.
                peopleManager.SetMultiValuedProfileProperty(personProperties.AccountName, "SPS-Skills", skills);
                clientContext.ExecuteQuery();

                //Refresh the values 
                RefreshUIValues();
            }

        }
    }
}