using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Client;

namespace Core.ExternalSharingWeb.Pages
{
    public partial class ExternalSharingForDocument : System.Web.UI.Page
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
            // The following code gets the client context and Title property by using TokenHelper.
            // To access other properties, the app may need to request permissions on the host web.
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            if (!this.IsPostBack)
            {
                using (var ctx = spContext.CreateUserClientContextForSPHost())
                {
                    ListCollection lists = ctx.Web.Lists;
                    var queryExpression = from list
                          in lists.Include(
                              list => list.Title,
                              list => list.BaseType)
                                          where list.ItemCount != 0
                                              && list.Hidden != true
                                              && list.IsCatalog != true
                                              && list.IsApplicationList != true
                                          select list;

                    var result = ctx.LoadQuery(queryExpression);
                    ctx.ExecuteQuery();
                    foreach (var item in result)
                    {
                        if (item.BaseType == BaseType.DocumentLibrary)
                        {
                            libraries.Items.Add(item.Title);
                        }
                    }
                    if (libraries.Items.Count > 0)
                    {
                        libraries.SelectedIndex = 0;
                        RefreshDocumentsForList(ctx, libraries.SelectedValue);
                    }
                }
                expirationDate.SelectedDate = DateTime.Today;

            }
        }



        protected void btnValidateEmail_Click(object sender, EventArgs e)
        {
            if (txtTargetEmail.Text.Length > 0)
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

                using (var ctx = spContext.CreateUserClientContextForSPHost())
                {
                    string returnedValue = ctx.Web.ResolvePeoplePickerValueForEmail(txtTargetEmail.Text);
                    lblStatus.Text = returnedValue;
                }
                btnShareDocument.Enabled = true;
                btnUnShareDoc.Enabled = true;
                btnSharingStatus.Enabled = true;
            }
        }

        protected void btnAnoLink_Click(object sender, EventArgs e)
        {
            // Get full URL to the document
            if (documents.Items.Count > 0)
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
                using (var ctx = spContext.CreateUserClientContextForSPHost())
                {
                    // Check if this is edit link or not
                    ExternalSharingDocumentOption isEditLink = ShouldLinkBeEdit();
                    // Get file link URL
                    string fileUrl = ResolveShareUrl(ctx, documents.SelectedValue);

                    // Create anonymous link for document
                    string link = ctx.Web.CreateAnonymousLinkForDocument(fileUrl, isEditLink);

                    // Output the created link
                    lblStatus.Text = string.Format("Created link: {0}", link);
                }
            }
            else
            {
                lblStatus.Text = "No document selected to be shared.";
            }
        }

        private ExternalSharingDocumentOption ShouldLinkBeEdit()
        {
            switch (rblSharingOptions.SelectedValue)
            {
                case "edit":
                    return ExternalSharingDocumentOption.Edit;
                default:
                    return ExternalSharingDocumentOption.View;
            }
        }

        protected void btnAnoLinkExp_Click(object sender, EventArgs e)
        {
            // Get full URL to the document
            if (documents.Items.Count > 0 && expirationDate.SelectedDate > DateTime.Today)
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
                using (var ctx = spContext.CreateUserClientContextForSPHost())
                {
                    // Check if this is edit link or not
                    ExternalSharingDocumentOption isEditLink = ShouldLinkBeEdit();
                    // Get file link URL
                    string fileUrl = ResolveShareUrl(ctx, documents.SelectedValue);

                    // Create anonymous link with expiration for document
                    string link = ctx.Web.CreateAnonymousLinkWithExpirationForDocument(fileUrl, isEditLink, expirationDate.SelectedDate);

                    // Output the created link
                    lblStatus.Text = string.Format("Created link: {0}", link);
                }
            }
            else
            {
                lblStatus.Text = "No document selected to be shared or date is older than today.";
            }
        }

        protected void btnShareDocument_Click(object sender, EventArgs e)
        {
            if (txtTargetEmail.Text.Length == 0)
            {
                lblStatus.Text = "Please assign the email address for the person to share the document.";
                return;
            }

            // Get full URL to the document
            if (documents.Items.Count > 0)
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
                using (var ctx = spContext.CreateUserClientContextForSPHost())
                {
                    // Check if this is edit link or not
                    ExternalSharingDocumentOption shareOption = ShouldLinkBeEdit();

                    // Get file link URL
                    string fileUrl = ResolveShareUrl(ctx, documents.SelectedValue);

                    // Share document for given email address
                    SharingResult result = ctx.Web.ShareDocument(fileUrl, txtTargetEmail.Text, shareOption,
                                                                 true, "Here's your important document");

                    // Output the created link
                    lblStatus.Text = string.Format("Document sharing status: {0}", result.StatusCode);
                }
            }
            else
            {
                lblStatus.Text = "No document selected to be shared.";
            }
        }

        protected void btnUnShareDoc_Click(object sender, EventArgs e)
        {
            // Get full URL to the document
            if (documents.Items.Count > 0)
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
                using (var ctx = spContext.CreateUserClientContextForSPHost())
                {
                    // Get file link URL
                    string fileUrl = ResolveShareUrl(ctx, documents.SelectedValue);

                    // Unshare the document - remove all external shares
                    SharingResult result = ctx.Web.UnshareDocument(fileUrl);

                    // Output the created link
                    lblStatus.Text = string.Format("Document unsharing status: {0}", result.StatusCode);
                }
            }
            else
            {
                lblStatus.Text = "No document selected to be unshared.";
            }
        }

        protected void btnSharingStatus_Click(object sender, EventArgs e)
        {
            // Get full URL to the document
            if (documents.Items.Count > 0)
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
                using (var ctx = spContext.CreateUserClientContextForSPHost())
                {
                    // Get file link URL
                    string fileUrl = ResolveShareUrl(ctx, documents.SelectedValue);

                    // Get current sharing settings
                    ObjectSharingSettings result = ctx.Web.GetObjectSharingSettingsForDocument(fileUrl, true);

                    // For outputting the list of people site is being shared
                    if (result.ObjectSharingInformation.SharedWithUsersCollection.Count > 0)
                    {
                        string userList = "";
                        foreach (var item in result.ObjectSharingInformation.SharedWithUsersCollection)
                        {
                            userList += string.Format(" - {0}", item.Email);
                        }
                        lblStatus.Text = string.Format("Document shared with: {0}", userList);
                    }
                    else
                    {
                        lblStatus.Text = string.Format("Document not shared with anyone");
                    }
                }
            }
            else
            {
                lblStatus.Text = "No document selected to be unshared.";
            }
        }

        protected void libraries_SelectedIndexChanged(object sender, EventArgs e)
        {
            documents.Items.Clear();

            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var ctx = spContext.CreateUserClientContextForSPHost())
            {
                RefreshDocumentsForList(ctx, libraries.SelectedValue);
            }
        }

        private void RefreshDocumentsForList(ClientContext ctx, string selectedLibraryTitle)
        {
            List list = ctx.Web.Lists.GetByTitle(selectedLibraryTitle);
            ctx.Load(list);
            ctx.ExecuteQuery();
            FileCollection files = list.RootFolder.Files;
            ctx.Load(files);
            ctx.ExecuteQuery();

            foreach (var item in files)
            {
                // output docs to second list
                documents.Items.Add(new System.Web.UI.WebControls.ListItem(item.Name, item.ServerRelativeUrl));
            }
            if (documents.Items.Count > 0)
            {
                documents.SelectedIndex = 0;
            }
        }

        private string ResolveShareUrl(ClientContext ctx, string fileServerRelativeUrl)
        {
            if (!ctx.Web.IsObjectPropertyInstantiated("Url"))
            {
                ctx.Load(ctx.Web, w => w.Url);
                ctx.ExecuteQuery();
            }
            var tenantStr = ctx.Web.Url.ToLower().Replace("-my", "").Substring(8);
            tenantStr = tenantStr.Substring(0, tenantStr.IndexOf("."));
            return String.Format("https://{0}.sharepoint.com{1}", tenantStr, fileServerRelativeUrl);
        }
    }
}