using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;

namespace Core.UploadWebPartWeb
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

        protected override void OnLoad(EventArgs e)
        {
            //check status
            if (!this.IsPostBack)
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    var wpGallery = clientContext.Web.Lists.GetByTitle("Web Part Gallery");
                    clientContext.Load(wpGallery);
                    clientContext.ExecuteQuery();

                    if (webPartExistsInGallery(clientContext, wpGallery))
                    {
                        imgWPG.ImageUrl = "~/Images/Yes.png";
                        btnAddToGallery.Enabled = false;

                        //check if the webpart is on the page
                        if (webPartExistsOnPage(clientContext))
                        {
                            imgWPP.ImageUrl = "~/Images/Yes.png";
                            btnAddToPage.Enabled = false;
                        }
                    }
                }
            }
        }

        protected void btnAddToGallery_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                var folder = clientContext.Web.Lists.GetByTitle("Web Part Gallery").RootFolder;
                clientContext.Load(folder);
                clientContext.ExecuteQuery();

                //upload the "OneDrive for Business Usage Guidelines.docx"
                using (var stream = System.IO.File.OpenRead(Server.MapPath("~/MyPicWebPart.dwp")))
                {
                    FileCreationInformation fileInfo = new FileCreationInformation();
                    fileInfo.ContentStream = stream;
                    fileInfo.Overwrite = true;
                    fileInfo.Url = "MyPicWebPart.dwp";
                    folder.Files.Add(fileInfo);
                    clientContext.ExecuteQuery();
                    imgWPG.ImageUrl = "~/Images/Yes.png";
                }
            }
        }

        protected void btnAddToPage_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                //get the web part page (DevHome.aspx) which is specific to a developer site template
                var list = clientContext.Web.Lists.GetByTitle("Site Pages");
                CamlQuery camlQuery = new CamlQuery();
                var items = list.GetItems(camlQuery);
                clientContext.Load(items, i =>
                    i.Include(item => item.DisplayName, item => item["WikiField"]).Where(item => item.DisplayName == "DevHome"));
                clientContext.ExecuteQuery();
                
                //get the webpart xml
                string wpXML = "";
                using (var stream = System.IO.File.OpenRead(Server.MapPath("~/MyPicWebPart.dwp")))
                {
                    XDocument xdoc = XDocument.Load(stream);
                    wpXML = xdoc.ToString();
                    wpXML = wpXML.Replace("\r\n", "");
                }

                //add the webpart to the page
                var wikiPage = items[0].File;
                LimitedWebPartManager limitedWebPartManager = wikiPage.GetLimitedWebPartManager(PersonalizationScope.Shared);
                WebPartDefinition wpd = limitedWebPartManager.ImportWebPart(wpXML);
                var newWP = limitedWebPartManager.AddWebPart(wpd.WebPart, "wpz", 0);
                clientContext.Load(newWP);
                clientContext.ExecuteQuery();

                // Create reference to WebPart in HTML
                string wikiField = items[0]["WikiField"] as string;
                XmlDocument xd = new XmlDocument();
                xd.PreserveWhitespace = true;
                xd.LoadXml(wikiField);
                XmlElement layoutsZoneInner = xd.SelectSingleNode("div/table/tbody/tr/td/div/div") as XmlElement;

                //create wrapper
                XmlElement wpWrapper = xd.CreateElement("div");
                layoutsZoneInner.AppendChild(wpWrapper);
                XmlAttribute attribute = xd.CreateAttribute("class");
                wpWrapper.Attributes.Append(attribute);
                attribute.Value = "ms-rtestate-read ms-rte-wpbox";

                //create inner elements
                XmlElement div1 = xd.CreateElement("div");
                wpWrapper.AppendChild(div1);
                div1.IsEmpty = false;
                attribute = xd.CreateAttribute("class");
                div1.Attributes.Append(attribute);
                attribute.Value = "ms-rtestate-notify ms-rtestate-read " + newWP.Id.ToString("D");
                attribute = xd.CreateAttribute("id");
                div1.Attributes.Append(attribute);
                attribute.Value = "div_" + newWP.Id.ToString("D");

                XmlElement div2 = xd.CreateElement("div");
                wpWrapper.AppendChild(div2);
                div2.IsEmpty = false;
                attribute = xd.CreateAttribute("class");
                div2.Attributes.Append(attribute);
                attribute.Value = "ms-rtestate-read";
                attribute = xd.CreateAttribute("style");
                div2.Attributes.Append(attribute);
                attribute.Value = "display:none";
                attribute = xd.CreateAttribute("id");
                div2.Attributes.Append(attribute);
                attribute.Value = "vid_" + newWP.Id.ToString("D");

                // Update
                items[0]["WikiField"] = xd.OuterXml;
                items[0].Update();
                clientContext.ExecuteQuery();

                //toggle the UI
                imgWPP.ImageUrl = "~/Images/Yes.png";
                btnAddToPage.Enabled = false;
            }
        }

        private bool webPartExistsInGallery(ClientContext clientContext, List wpGallery)
        {

            CamlQuery query = new CamlQuery();
            string camlString = @"
                <View>
                    <Query>                
                        <Where>
                            <Eq>
                                <FieldRef Name='LinkWebPart' />
                                <Value Type='Text'>{0}</Value>
                            </Eq>
                        </Where>
                     </Query>
                </View>";
            // Let's update the theme name accordingly
            camlString = string.Format(camlString, "MyPicWebPart.dwp");
            query.ViewXml = camlString;
            var found = wpGallery.GetItems(query);
            clientContext.Load(found);
            clientContext.ExecuteQuery();
            if (found.Count > 0)
            {
                return true;
            }
            return false;
        }

        private bool webPartExistsOnPage(ClientContext clientContext)
        {
            bool found = false;

            //get the web part page (DevHome.aspx) which is specific to a developer site template
            var list = clientContext.Web.Lists.GetByTitle("Site Pages");
            CamlQuery camlQuery = new CamlQuery();
            var items = list.GetItems(camlQuery);
            clientContext.Load(items, i =>
                i.Include(item => item.DisplayName, item => item["WikiField"]).Where(item => item.DisplayName == "DevHome"));
            clientContext.ExecuteQuery();

            //check the page for the webpart
            var wikiPage = items[0].File;
            LimitedWebPartManager limitedWebPartManager = wikiPage.GetLimitedWebPartManager(PersonalizationScope.Shared);
            var wps = limitedWebPartManager.WebParts;
            clientContext.Load(wps);
            clientContext.ExecuteQuery();
            for (int i = 0; i < wps.Count; i++)
            {
                var wp = wps[i].WebPart;
                clientContext.Load(wp);
                clientContext.ExecuteQuery();

                if (wp.Title.Equals("My Profile Pic", StringComparison.CurrentCultureIgnoreCase))
                {
                    found = true;
                    break;
                }
            }

            return found;
        }
    }
}