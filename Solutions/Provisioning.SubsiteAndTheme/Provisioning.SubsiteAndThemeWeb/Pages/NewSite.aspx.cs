using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Serialization;

namespace Provisioning.SubsiteAndThemeWeb {
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
            try {
                ProvisioningContext.RenderChromeScript(this);
                var config = ProvisioningContext.Current.Configuration;
                var siteUrl = Request.QueryString["SPHostUrl"].TrimEnd('/') + '/';
                ParentSiteLabel.Text = siteUrl;
                ApplyThemeLink.NavigateUrl = string.Format("~/Pages/ApplyThemeToSite.aspx?SPHostUrl={0}", Uri.EscapeUriString(siteUrl));

                if (!IsPostBack) {

                    TemplateList.DataSource = config.Templates;
                    TemplateList.DataBind();

                    ThemeList.DataSource = config.Branding.Themes;
                    ThemeList.DataBind();

                    if (ThemeList.Items.Count == 2) {
                        ThemeList.Enabled = false;
                        ThemeList.SelectedIndex = 1;
                    }

                    if (TemplateList.Items.Count == 2) {
                        TemplateList.Enabled = false;
                        TemplateList.SelectedIndex = 1;
                    }
                }
            }
            catch (Exception ex) {
                ErrorMessage.Text = ex.Message;
            }
        }


        void TestWriteConfigurationFile() {
            var config = new ProvisioningConfiguration() {
                Branding = new Branding() {
                    LogoFilePath = "",
                    LogoUrl = "//blogs.msdn.com/themes/MSDN2/images/MSDN/logo_msdn.png",
                    Themes = new Theme[] { 
                        new Theme { Name="Contoso", ColorFile="/Themes/contoso.spcolor"},
                        new Theme { Name="Contoso Red", ColorFile="/Themes/contosored.spcolor"}
                    }
                },
                Templates = new Template[] { 
                    new Template { DisplayName="Default", TemplateId="STS#0" }
                }
            };

            ProvisioningContext.WriteConfiguration(config);
        }

        protected void SubmitButton_Click(object sender, EventArgs e) {
            try {
                if (Page.IsValid) {
                    var siteUrl = ParentSiteLabel.Text;

                    ProvisioningContext.Current.CreateSite(
                            siteUrl.Trim(),
                            TitleTextBox.Text.Trim(),
                            PathTextBox.Text.Trim(),
                            DescriptionTextBox.Text.Trim(),
                            ThemeList.SelectedValue,
                            TemplateList.SelectedValue
                        );
                }
            }
            catch (Exception ex) {
                ErrorMessage.Text = ex.Message;
            }
        }

        protected void CancelButton_Click(object sender, EventArgs e) {
            try {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext.Current);
                var siteUrl = UrlUtility.Combine(spContext.SPHostUrl.ToString(), "/_layouts/15/viewlsts.aspx");
                Response.Redirect(siteUrl);
            }
            catch (Exception ex) {
                ErrorMessage.Text = ex.Message;
            }
        }
    }
}