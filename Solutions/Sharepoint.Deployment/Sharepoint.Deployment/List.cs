using SharePoint.Deployment.Utilities;
using SP = Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using System.IO;

namespace SharePoint.Deployment {
    public class List : Deployable {
        internal SP.List SpList;

        public List(SP.ListTemplateType templateType, string title) {
            this.TemplateType = templateType;
            this.Title = title;
            this.UrlSegment = UrlUtility.JoinUrl("Lists", title.removeNonWordCharacters());

            this.ContentTypes = new List<ContentType>();
            this.Fields = new List<ListField>() { ListField.TitleFieldDefinition };
            this.Views = new List<ListView>() { ListView.GetDefaultView() };
            this.Forms = new List<ListForm>() {
                new ListForm(SP.PageType.DisplayForm),
                new ListForm(SP.PageType.EditForm),
                new ListForm(SP.PageType.NewForm)
                };
        }

        protected override void OnInit() {
            this.ForEachChild((i) => {
                if (i is ListField) {
                    ((ListField)i).ParentList = this;
                }
                });
        }

        internal override void OnInvalidate() {
            this.SpList = null;
        }

        public override void ForEachChild(Action<Deployable> action) {
            if (this.Fields != null) this.Fields.ForEach(action);
            if (this.ContentTypes != null) this.ContentTypes.ForEach(action);
            if (this.Forms != null) this.Views.ForEach(action);
            if (this.Views != null) this.Views.ForEach(action);
        }

        protected override bool GetDeployed() {
            var returnValue = false;
            var spWeb = this.GetParentSPWeb();
            this.SpList = spWeb.Lists.GetByTitle(this.Title);
            this.IsDeployed = returnValue = this.Context.TryExecuteSync(this.SpList);
            return returnValue;
        }

        internal override void OnDelete() {
            throw new NotImplementedException();
        }

        internal override void Create() {
            if (!this.IsDeployed.Value) {
                this.OnCreate();
            } else {
                this.ForEachChild(i => i.Create());
            }
        }

        internal override void OnCreate() {
            if (!this.IsDeployed.Value) {
                var doc = this.GetSchemaXml();
                var list = doc.Root;
                SP.ListCreationInformation info = new SP.ListCreationInformation() {
                    Title = list.Attribute("Title").Value,
                    Url = list.Attribute("Url").Value,
                    TemplateType = (int)SP.ListTemplateType.GenericList,
                    TemplateFeatureId = new Guid("00bfea71-de22-43b2-a848-c05709900100"),
                    CustomSchemaXml = doc.ToString()
                };

                var web = this.GetParentSPWeb();
                this.Context.ExecuteAsync(() => web.Lists.Add(info), () => this.IsDeployed = this.GetDeployed(true));
            } else {
                this.ForEachChild(i => i.OnCreate());
            }
        }

        public void LoadFromXml(System.Xml.XmlReader reader) {
            if (reader.LocalName == "List") {
                this.Title = reader.GetAttribute("title");
                this.Description = reader.GetAttribute("description");
                this.QuickLaunchOption = reader.GetAttributeEnum("quickLaunchOption", SP.QuickLaunchOptions.DefaultValue);
                this.TemplateType = (SP.ListTemplateType)reader.GetAttributeInt("templateType", 101);
                this.UrlSegment = reader.GetAttribute("url");
            }
        }

        public string FullUrl {
            get { return UrlUtility.JoinUrl(this.ParentWeb.FullUrl, this.UrlSegment); }
        }

        protected virtual XDocument GetSchemaXml() {
            XDocument returnValue;
            if (string.IsNullOrEmpty(this.XmlDefinition)) {
                XElement contentTypes = new XElement("ContentTypes", "");
                XElement fields = new XElement("Fields");
                XElement views = new XElement("Views");
                XElement forms = new XElement("Forms");

                XElement list = new XElement(XName.Get("List", "http://schemas.microsoft.com/sharepoint/"),
                                                new XAttribute(XNamespace.Xmlns + "ows", "Microsoft SharePoint"),
                                                new XAttribute("Title", this.Title),
                                                new XAttribute("Url", this.UrlSegment),
                                                new XAttribute("BaseType", 0),
                                                new XAttribute("FolderCreation", this.AllowFolderCreation.ToString(Boolean.Case.Upper)),
                                                new XElement("MetaData", contentTypes, fields, views, forms)
                                                );

                if (!string.IsNullOrEmpty(this.Description)) list.Add(new XAttribute("Description", this.Description));

                //Add fields (include fields from content types that might be missing from the fields collection)
                this.ContentTypes.ForEach(ct => {
                    ct.Fields.Where(ctf => { return !this.Fields.Any(f => ctf.Id == f.Id); })
                             .ForEach(i => { this.Fields.Add(i); });
                });
                this.Fields.ForEach(i => fields.Add(i.GetSchemaXml()));

                //Add content types
                this.ContentTypes.ForEach(i => contentTypes.Add(i.GetSchemaXml()));

                //Add views
                this.Views.ForEach(i => views.Add(i.GetSchemaXml()));

                //Add list forms
                this.Forms.ForEach(i => forms.Add(i.getSchemaXml()));

                list.setNamespaceOnDecendants(XNamespace.Get("http://schemas.microsoft.com/sharepoint/"));

                returnValue = new XDocument(list);
            } else {
                returnValue = XDocument.Parse(this.XmlDefinition);
            }

            return returnValue;
        }

        #region List data
        
        #region GetItems
        public List<ListItem> GetItems(SP.CamlQuery query) {
            var returnValue = new List<ListItem>();
            var items = this.SpList.GetItems(query);
            this.Context.ExecuteSync(items);

            foreach (var item in items) {
                returnValue.Add(ListItem.FromSpItem(item));
            }

            return returnValue;
        }
        public List<ListItem> GetItems(string viewXml, string folderPath, SP.ListItemCollectionPosition position) {
            return this.GetItems(new SP.CamlQuery() { ViewXml = viewXml, FolderServerRelativeUrl = folderPath, ListItemCollectionPosition = position });
        }
        public List<ListItem> GetItems(string folderPath, SP.ListItemCollectionPosition position, string query, int rowLimit, string scope, params string[] fields) {
            return this.GetItems(ListView.GetBasicSchemaXml(query, rowLimit, true, scope, fields).ToString(), folderPath, position);
        }
        public List<ListItem> GetItems() {
            return this.GetItems(SP.CamlQuery.CreateAllItemsQuery());
        }
        #endregion

        #region Add Items
        public void AddItems(params ListItem[] items) { this.AddItems("", items); }
        public void AddItems(string folderUrl, params ListItem[] items) {
            items.ForEach(i => this.Context.ExecuteAsync(() => {
                var spItem = this.SpList.AddItem(new SP.ListItemCreationInformation() {
                    FolderUrl = folderUrl, 
                    LeafName = null,
                    UnderlyingObjectType = SP.FileSystemObjectType.File
                    });
                i.ApplyValues(spItem);
                spItem.Update();
            }));
        }
        #endregion
        
        #endregion

        #region Definition Info
        public string Title { get; set; }
        public string Description { get; set; }
        public SP.QuickLaunchOptions QuickLaunchOption { get; set; }
        public SP.ListTemplateType TemplateType { get; set; }
        public string UrlSegment { get; set; }
        public bool AllowFolderCreation { get; set; }
        public string XmlDefinition { get; set; }
        #endregion

        #region Children
        public List<ListField> Fields { get; set; }
        public List<ContentType> ContentTypes { get; set; }
        public List<ListView> Views { get; set; }
        public List<ListForm> Forms { get; set; }
        #endregion
    }
}
