using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ECM.RecordsManagementWeb
{
    public partial class Default : System.Web.UI.Page
    {
        private ClientContext cc;
        private const string IPR_LIBRARY = "IPRTest";

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

            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            cc = spContext.CreateUserClientContextForSPHost();

            if (!IsPostBack)
            {
                if (cc.Site.IsInPlaceRecordsManagementActive())
                {
                    IPRStatusUpdate(true);
                }
                else
                {
                    IPRStatusUpdate(false);
                }
            }
        }

        private void IPRStatusUpdate(bool enabled)
        {
            // Scenario 1
            lblIPREnabled.Text = enabled ? "activated" : "deactivated";
            btnToggleIPRStatus.Text = enabled ? "Deactivate" : "Activate";
            rdAvailability.Enabled = enabled;
            rdDeclarationBy.Enabled = enabled;
            rdRestrictions.Enabled = enabled;
            rdUndeclarationBy.Enabled = enabled;
            rdListAvailability.Enabled = enabled;
            chbAutoDeclare.Enabled = enabled;
            btnSaveSiteScopedIPRSettings.Enabled = enabled;
            btnSaveListScopedIPRSettings.Enabled = enabled;

            if (cc.Site.IsInPlaceRecordsManagementActive())
            {
                rdRestrictions.SelectedValue = Convert.ToString((int)cc.Site.GetRecordRestrictions());
                rdAvailability.SelectedValue = cc.Site.GetManualRecordDeclarationInAllLocations().ToString();
                rdDeclarationBy.SelectedValue = Convert.ToString((int)cc.Site.GetRecordDeclarationBy());
                rdUndeclarationBy.SelectedValue = Convert.ToString((int)cc.Site.GetRecordUnDeclarationBy());

                // Scenario 2
                rdListAvailability.Enabled = enabled;
                chbAutoDeclare.Enabled = enabled;
                btnSaveListScopedIPRSettings.Enabled = enabled;

                if (!cc.Web.ListExists(IPR_LIBRARY))
                {
                    cc.Web.CreateDocumentLibrary(IPR_LIBRARY);
                }

                List ipr = cc.Web.GetListByTitle(IPR_LIBRARY);
                if (ipr.IsListRecordSettingDefined())
                {
                    rdListAvailability.SelectedValue = Convert.ToString((int)ipr.GetListManualRecordDeclaration());
                    chbAutoDeclare.Checked = ipr.GetListAutoRecordDeclaration();
                    //Refresh the settings as AutoDeclare changes the manual settings
                    rdListAvailability.Enabled = !chbAutoDeclare.Checked;
                }
            }
        }

        protected void btnToggleIPRStatus_Click(object sender, EventArgs e)
        {
            if (cc.Site.IsInPlaceRecordsManagementActive())
            {
                cc.Site.DisableInPlaceRecordsManagementFeature();
                IPRStatusUpdate(false);
            }
            else
            {
                cc.Site.EnableSiteForInPlaceRecordsManagement();
                IPRStatusUpdate(true);
            }
        }

        protected void btnSaveSiteScopedIPRSettings_Click(object sender, EventArgs e)
        {
            EcmSiteRecordRestrictions restrictions = (EcmSiteRecordRestrictions)Convert.ToInt32(rdRestrictions.SelectedValue);
            cc.Site.SetRecordRestrictions(restrictions);
            cc.Site.SetManualRecordDeclarationInAllLocations(Convert.ToBoolean(rdAvailability.SelectedValue));
            EcmRecordDeclarationBy declareBy = (EcmRecordDeclarationBy)Convert.ToInt32(rdDeclarationBy.SelectedValue);
            cc.Site.SetRecordDeclarationBy(declareBy);
            EcmRecordDeclarationBy unDeclareBy = (EcmRecordDeclarationBy)Convert.ToInt32(rdUndeclarationBy.SelectedValue);
            cc.Site.SetRecordUnDeclarationBy(unDeclareBy);
        }

        protected void btnSaveListScopedIPRSettings_Click(object sender, EventArgs e)
        {
            List ipr = cc.Web.GetListByTitle(IPR_LIBRARY);
            EcmListManualRecordDeclaration listManual = (EcmListManualRecordDeclaration)Convert.ToInt32(rdListAvailability.SelectedValue);
            ipr.SetListManualRecordDeclaration(listManual);
            ipr.SetListAutoRecordDeclaration(chbAutoDeclare.Checked);

            //Refresh the settings as AutoDeclare changes the manual settings
            if (ipr.IsListRecordSettingDefined())
            {
                rdListAvailability.SelectedValue = Convert.ToString((int)ipr.GetListManualRecordDeclaration());
                chbAutoDeclare.Checked = ipr.GetListAutoRecordDeclaration();
                rdListAvailability.Enabled = !chbAutoDeclare.Checked;
            }

        }

    }
}