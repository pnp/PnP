using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Publishing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Provisioning.PreventDeleteSites.HostWeb {
    public partial class Default : System.Web.UI.Page {
        protected void Page_PreInit(object sender, EventArgs e) {
            Uri redirectUrl;
            switch (SharePointContextProvider.CheckRedirectionStatus(Context, out redirectUrl)) {
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

        protected void Page_Load(object sender, EventArgs e) {
            #region Load chrome script
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
            #endregion

            if (!IsPostBack) {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

                using (var clientContext = spContext.CreateUserClientContextForSPHost()) {
                    clientContext.Load(clientContext.Web);
                    clientContext.Load(clientContext.Web.Webs);
                    clientContext.ExecuteQuery();

                    //bind data
                    var sites = clientContext.Web.Webs;

                    SitesList1.DataSource = sites;
                    SitesList1.DataBind();
                    SitesList2.DataSource = sites;
                    SitesList2.DataBind();
                }
            }
        }

        protected void DeploySandboxSolution_Click(object sender, EventArgs e) {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost()) {
                // get the solution gallery
                var solutionGallery = clientContext.Web.Lists.GetByTitle("Solution Gallery");
                clientContext.Load(solutionGallery);
                clientContext.ExecuteQuery();

                // get the file from the server path in the provider site
                var filePath = Server.MapPath("~/PreventDeleteSites.wsp");
                var file = new FileStream(filePath, FileMode.Open);

                // create the FileCreationInformation object and prepare
                // to upload it to the solution gallery
                var fileCI = new FileCreationInformation() {
                    ContentStream = file,
                    Url = "PreventDeleteSites.wsp",
                    Overwrite = true
                };

                // upload the solution to the gallery
                var uploadedFile = solutionGallery.RootFolder.Files.Add(fileCI);
                clientContext.Load(uploadedFile);
                clientContext.ExecuteQuery();
            }
        }

        protected void ActivateSandboxSolution_Click(object sender, EventArgs e) {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost()) {
                // get the solution gallery
                var solutionGallery = clientContext.Web.Lists.GetByTitle("Solution Gallery");
                clientContext.Load(solutionGallery);
                clientContext.Load(solutionGallery.RootFolder);
                clientContext.ExecuteQuery();

                // get the DesignPackageInfo (which is the same name for a sandbox solution)
                var wsp = new DesignPackageInfo(){
                    // during deployment, the solution ID is not necessary
                    PackageGuid = Guid.Empty, // 4c16c0b9-0162-43ad-a8e9-a4b810e58a56
                    PackageName = "PreventDeleteSites"
                };

                // install the solution from the file url
                var filerelativeurl = solutionGallery.RootFolder.ServerRelativeUrl + "/PreventDeleteSites.wsp";
                DesignPackage.Install(clientContext, clientContext.Site, wsp, filerelativeurl);
                clientContext.ExecuteQuery();
            }
        }

        protected void DeactivateSandboxSolution_Click(object sender, EventArgs e) {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost()) {
                // get the solution gallery
                var solutionGallery = clientContext.Web.Lists.GetByTitle("Solution Gallery");
                clientContext.Load(solutionGallery);
                clientContext.Load(solutionGallery.RootFolder);
                clientContext.ExecuteQuery();

                var wsp = new DesignPackageInfo() {
                    // the package guid is necessary to explicitly 
                    PackageGuid = new Guid("4c16c0b9-0162-43ad-a8e9-a4b810e58a56"),
                    PackageName = "PreventDeleteSites"
                };

                // uninstall the solution
                DesignPackage.UnInstall(clientContext, clientContext.Site, wsp);
                clientContext.ExecuteQuery();
            }
        }

        protected void RemoveSandboxSolution_Click(object sender, EventArgs e) {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost()) {
                // get the solution gallery
                var solutionGallery = clientContext.Web.Lists.GetByTitle("Solution Gallery");
                clientContext.Load(solutionGallery);
                clientContext.Load(solutionGallery.RootFolder);
                clientContext.ExecuteQuery();

                // find the solution in the gallery and delete it
                var files = solutionGallery.RootFolder.Files;
                clientContext.Load(files, fs => fs.Where(f => f.Name == "PreventDeleteSites.wsp"));
                clientContext.ExecuteQuery();

                var file = files.FirstOrDefault();

                if (file == null)
                    throw new InvalidOperationException("Solution does not exist");

                file.DeleteObject();
                clientContext.ExecuteQuery();
            }
        }

        protected void NavigateToDeleteSitePage_Click(object sender, EventArgs e) {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            string url;
            if (sender == NavButton1) {
                url = spContext.SPHostUrl +
                    SitesList1.SelectedValue.TrimStart('/') +
                    "/_layouts/15/deleteweb.aspx";
            }
            else {
                url = spContext.SPHostUrl +
                    SitesList2.SelectedValue.TrimStart('/') +
                    "/_layouts/15/deleteweb.aspx";
            }
            Response.Redirect(url, false);
        }
    }
}