using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Provisioning.SubsiteAndThemeWeb.Pages {
    public partial class ApplyThemeToSite : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            try {
                ProvisioningContext.RenderChromeScript(this);

                if (!IsPostBack) {
                    var config = ProvisioningContext.Current.Configuration;

                    ThemeList.DataSource = config.Branding.Themes;
                    ThemeList.DataBind();

                    if (ThemeList.Items.Count == 2) {
                        ThemeList.Enabled = false;
                        ThemeList.SelectedIndex = 1;
                    }
                }
            }
            catch (Exception ex) {
                ErrorMessage.Text = ex.Message;
            }
        }

        protected void SubmitButton_Click(object sender, EventArgs e) {
            try {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext.Current);

                using (var context = spContext.CreateUserClientContextForSPHost()) {
                    var rootWeb = context.Site.RootWeb;
                    context.Load(rootWeb);
                    context.ExecuteQuery();

                    ProvisioningContext.Current.ApplyTheme(rootWeb, rootWeb, ThemeList.SelectedValue);
                    var logoUrl = ProvisioningContext.Current.SetSiteLogo(rootWeb, rootWeb);
                    ApplyThemeRecursive(rootWeb, rootWeb, ThemeList.SelectedValue, SetSiteLogoCheckbox.Checked, logoUrl);
                }
            }
            catch (Exception ex) {
                ErrorMessage.Text = ex.Message;
            }
        }

        protected void CancelButton_Click(object sender, EventArgs e) {
            try {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext.Current);
                var siteUrl = spContext.SPHostUrl.ToString();
                Response.Redirect(siteUrl);
            }
            catch (Exception ex) {
                ErrorMessage.Text = ex.Message;
            }
        }

        void ApplyThemeRecursive(Web targetWeb, Web rootWeb, string themeName, bool setSiteLogo, string logoUrl) {
            var subwebs = targetWeb.Webs;
            targetWeb.Context.Load(subwebs);
            targetWeb.Context.ExecuteQuery();

            foreach (var subweb in subwebs) {
                ProvisioningContext.Current.ApplyTheme(subweb, rootWeb, themeName, alreadyUploaded: true);

                if (setSiteLogo)
                    ProvisioningContext.Current.SetSiteLogo(subweb, rootWeb);

                ApplyThemeRecursive(subweb, rootWeb, ThemeList.SelectedValue, setSiteLogo, logoUrl);
            }
        }
    }
}