using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Provisioning.SiteModifierWeb
{
    public partial class Modify : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Check which lists are there already
                var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    ListsCheckboxList.Items.Clear();
                    Web web = clientContext.Web;
                    var lists = web.Lists;
                    clientContext.Load(lists);
                    clientContext.ExecuteQuery();

                    var projects = lists.Where(l => l.Title == "Projects").FirstOrDefault();
                    if (projects == null)
                    {
                        ListsCheckboxList.Items.Add(new System.Web.UI.WebControls.ListItem("Projects", "PROJECTS"));
                    }
                    var contacts = lists.Where(l => l.Title == "Contacts").FirstOrDefault();
                    if (contacts == null)
                    {
                        ListsCheckboxList.Items.Add(new System.Web.UI.WebControls.ListItem("Contacts", "CONTACTS"));
                    }
                    if (ListsCheckboxList.Items.Count == 0)
                    {
                        ListsFieldSet.Visible = false;
                    }
                }
            }
        }

        protected void AddArtifacts_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                var web = clientContext.Web;

                clientContext.Load(web, w => w.ServerRelativeUrl);

                clientContext.ExecuteQuery();

                var webUrl = web.ServerRelativeUrl.TrimEnd('/');

                foreach (System.Web.UI.WebControls.ListItem item in ListsCheckboxList.Items)
                {
                    if (item.Selected)
                    {
                        switch (item.Value)
                        {
                            case "PROJECTS":
                                {
                                    web.CreateList(ListTemplateType.GenericList, "Projects", false, urlPath: "lists/projects");
                                    break;
                                }
                            case "CONTACTS":
                                {
                                    web.CreateList(ListTemplateType.Contacts, "Contacts", false, urlPath: "lists/contacts");
                                    break;
                                }
                        }
                    }
                }

                if (ApplyTheme.Checked)
                {
                    clientContext.Web.SetComposedLookByUrl("Blossom");
                }

            }

            ScriptManager.RegisterStartupScript(this, typeof(Page), "UpdateMsg", "window.parent.postMessage('CloseCustomActionDialogRefresh', '*');", true);
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "UpdateMsg", "window.parent.postMessage('CloseCustomActionDialogNoRefresh', '*');", true);
        }

    }
}