using ECM.DocumentLibrariesWeb.Models;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ECM.DocumentLibrariesWeb
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
           
            if(!Page.IsPostBack)
            {
                if(this.DoesUserHavePermission()) {
                    this.SetHiddenFields();
                    this.SetContentTypes();
                }
                else {
                    Server.Transfer("~/pages/AccessDenied.aspx");
                }
            }
        }

        private bool DoesUserHavePermission()
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            using (var ctx = spContext.CreateUserClientContextForSPHost())
            {
                BasePermissions perms = new BasePermissions();
                perms.Set(PermissionKind.ManageLists);
                ClientResult<bool> _permResult = ctx.Web.DoesUserHavePermissions(perms);
                ctx.ExecuteQuery();
                return _permResult.Value;
            }
        }
        protected void CreateLibrary_Click(object sender, EventArgs e)
        {
            try
            {
                var _spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
                var _templateSelectedItem = this.DocumentTemplateType.Value;
                var _libraryToCreate = this.GetLibraryToCreate();
                using (var _ctx = _spContext.CreateUserClientContextForSPHost())
                {
                    //TODO Change your application name
                    _ctx.ApplicationName = "ECM.DocumentLibraries";
                    ContentTypeManager _manager = new ContentTypeManager();
                    switch(_templateSelectedItem)
                    {
                        case "IT Document":
                            _manager.CreateITDocumentLibrary(_ctx, _libraryToCreate);
                            break;
                        case "Contoso Document":
                            _manager.CreateContosoDocumentLibrary(_ctx, _libraryToCreate);
                            break;
                    }
                 }

                Response.Redirect(this.Url.Value);
            }
            catch (Exception _ex)
            {
                throw;
            }
        }

        #region Private Members

        private Library GetLibraryToCreate()
        {
            Library _libraryToCreate = new Library()
            {
                Title = this.LibraryName.Value,
                Description = this.LibraryDescription.Value,
                VerisioningEnabled = this.onetidVersioningEnabledYes.Checked
            };

            return _libraryToCreate;
        }

        private void SetContentTypes()
        {
            ContentTypeManager _manager = new ContentTypeManager();
            var _contentTypes = _manager.GetContentTypesName();

            this.DocumentTemplateType.DataSource = _contentTypes;
            this.DocumentTemplateType.DataBind();

        }
        protected void SetHiddenFields()
        {
            //Another way is by setting with javascript
            string _url = Request.QueryString["SPHostUrl"];
            this.Url.Value = _url;
        }
        #endregion

    }
}