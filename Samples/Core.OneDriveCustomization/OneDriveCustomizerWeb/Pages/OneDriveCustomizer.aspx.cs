using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Contoso.Core.OneDriveCustomizerWeb.Pages
{
    public partial class OneDriveCustomizer : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (ClientContext clientContext = spContext.CreateUserClientContextForSPHost())
            {
                // Get user profile
                ProfileLoader loader = Microsoft.SharePoint.Client.UserProfiles.ProfileLoader.GetProfileLoader(clientContext);
                UserProfile profile = loader.GetUserProfile();
                Microsoft.SharePoint.Client.Site personalSite = profile.PersonalSite;

                clientContext.Load(personalSite);
                clientContext.ExecuteQuery();

                // Let's check if the site already exists The following code uses a timer job-based approach to schedule the creation of a OneDrive for Business site if it has not yet been created for a particular user.
                if (personalSite.ServerObjectIsNull.Value)
                {
                    // Let's queue the personal site creation using an approach based on the out-of-the-box timer job.
                    // Using async mode, since end user could go away from browser, you also could do this using an out-of-the-box web part.
                    profile.CreatePersonalSiteEnque(true);
                    clientContext.ExecuteQuery();
                }
                else
                {
                    Web rootWeb = personalSite.RootWeb;
                    clientContext.Load(rootWeb);
                    clientContext.ExecuteQuery();

                    // Setting the custom theme to host web
                    SetThemeBasedOnName(clientContext, rootWeb, "Orange");
                }
            }
        }
        public void SetThemeBasedOnName(ClientContext cc, Web rootWeb, string themeName)
        {
            // Let's get instance to the composite look gallery
            List themeList = rootWeb.GetCatalog(124);
            cc.Load(themeList);
            cc.ExecuteQuery();

            CamlQuery query = new CamlQuery();
            string camlString = @"
                <View>
                    <Query>                
                        <Where>
                            <Eq>
                                <FieldRef Name='Name' />
                                <Value Type='Text'>{0}</Value>
                            </Eq>
                        </Where>
                     </Query>
                </View>";
            // Let's update the theme name accordingly
            camlString = string.Format(camlString, themeName);
            query.ViewXml = camlString;
            var found = themeList.GetItems(query);
            cc.Load(found);
            cc.ExecuteQuery();
            if (found.Count > 0)
            {
                Microsoft.SharePoint.Client.ListItem themeEntry = found[0];
                //Set the properties for applying custom theme which was jus uplaoded
                string spColorURL = null;
                if (themeEntry["ThemeUrl"] != null && themeEntry["ThemeUrl"].ToString().Length > 0)
                {
                    spColorURL = MakeAsRelativeUrl((themeEntry["ThemeUrl"] as FieldUrlValue).Url);
                }
                string spFontURL = null;
                if (themeEntry["FontSchemeUrl"] != null && themeEntry["FontSchemeUrl"].ToString().Length > 0)
                {
                    spFontURL = MakeAsRelativeUrl((themeEntry["FontSchemeUrl"] as FieldUrlValue).Url);
                }
                string backGroundImage = null;
                if (themeEntry["ImageUrl"] != null && themeEntry["ImageUrl"].ToString().Length > 0)
                {
                    backGroundImage = MakeAsRelativeUrl((themeEntry["ImageUrl"] as FieldUrlValue).Url);
                }

                // Set theme for demonstration
                rootWeb.ApplyTheme(spColorURL,
                                    spFontURL,
                                    backGroundImage,
                                    false);

                // Let's also update master page, if needed
                if (themeEntry["MasterPageUrl"] != null && themeEntry["MasterPageUrl"].ToString().Length > 0)
                {
                    rootWeb.MasterUrl = MakeAsRelativeUrl((themeEntry["MasterPageUrl"] as FieldUrlValue).Url); ;
                }

                cc.ExecuteQuery();
            }
        }

        private string MakeAsRelativeUrl(string urlToProcess)
        {
            Uri uri = new Uri(urlToProcess);
            return uri.AbsolutePath;
        }
    }
}