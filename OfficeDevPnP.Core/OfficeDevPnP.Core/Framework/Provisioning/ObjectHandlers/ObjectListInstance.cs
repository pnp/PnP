using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using System.Web.Instrumentation;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.ObjectHandlers;
using OfficeDevPnP.Core.Framework.ObjectHandlers.TokenDefinitions;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Utilities;
using Field = Microsoft.SharePoint.Client.Field;
using View = OfficeDevPnP.Core.Framework.Provisioning.Model.View;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    internal class ObjectListInstance : ObjectHandlerBase
    {

        public override string Name
        {
            get { return "List instances"; }
        }
        public override void ProvisionObjects(Web web, ProvisioningTemplate template)
        {
            Log.Info(Constants.LOGGING_SOURCE_FRAMEWORK_PROVISIONING, CoreResources.Provisioning_ObjectHandlers_ListInstances);

            if (template.Lists.Any())
            {
                var rootWeb = (web.Context as ClientContext).Site.RootWeb;
                if (!web.IsPropertyAvailable("ServerRelativeUrl"))
                {
                    web.Context.Load(web, w => w.ServerRelativeUrl);
                    web.Context.ExecuteQueryRetry();
                }

                web.Context.Load(web.Lists, lc => lc.IncludeWithDefaultProperties(l => l.RootFolder.ServerRelativeUrl));
                web.Context.ExecuteQueryRetry();
                var existingLists = web.Lists.AsEnumerable<List>().Select(existingList => existingList.RootFolder.ServerRelativeUrl).ToList();
                var serverRelativeUrl = web.ServerRelativeUrl;

                var createdLists = new List<ListInfo>();

                #region Lists

                foreach (var list in template.Lists)
                {
                    if (existingLists.FindIndex(x => x.Equals(UrlUtility.Combine(serverRelativeUrl, list.Url), StringComparison.OrdinalIgnoreCase)) == -1)
                    {
                        var listCreate = new ListCreationInformation();
                        listCreate.Description = list.Description;
                        listCreate.TemplateType = list.TemplateType;
                        listCreate.Title = list.Title;

                        // the line of code below doesn't add the list to QuickLaunch
                        // the OnQuickLaunch property is re-set on the Created List object
                        listCreate.QuickLaunchOption = list.OnQuickLaunch ? QuickLaunchOptions.On : QuickLaunchOptions.Off;

                        listCreate.Url = list.Url.ToParsedString();
                        listCreate.TemplateFeatureId = list.TemplateFeatureID;

                        var createdList = web.Lists.Add(listCreate);
                        createdList.Update();
                        web.Context.Load(createdList, l => l.BaseTemplate);
                        web.Context.ExecuteQueryRetry();

                        if (!String.IsNullOrEmpty(list.DocumentTemplate))
                        {
                            createdList.DocumentTemplateUrl = list.DocumentTemplate.ToParsedString();
                        }

                        // EnableAttachments are not supported for DocumentLibraries and Surveys
                        // TODO: the user should be warned
                        if (createdList.BaseTemplate != (int)ListTemplateType.DocumentLibrary && createdList.BaseTemplate != (int)ListTemplateType.Survey)
                        {
                            createdList.EnableAttachments = list.EnableAttachments;
                        }

                        createdList.EnableModeration = list.EnableModeration;

                        createdList.EnableVersioning = list.EnableVersioning;
                        if (list.EnableVersioning)
                        {
                            createdList.MajorVersionLimit = list.MaxVersionLimit;

                            if (createdList.BaseTemplate == (int)ListTemplateType.DocumentLibrary)
                            {
                                // Only supported on Document Libraries
                                createdList.EnableMinorVersions = list.EnableMinorVersions;
                                createdList.DraftVersionVisibility = (DraftVisibilityType)list.DraftVersionVisibility;

                                // TODO: User should be notified that MinorVersionLimit and DraftVersionVisibility will not be applied
                                if (list.EnableMinorVersions)
                                {
                                    createdList.MajorWithMinorVersionsLimit = list.MinorVersionLimit; // Set only if enabled, otherwise you'll get exception due setting value to zero.

                                    // DraftVisibilityType.Approver is available only when the EnableModeration option of the list is true
                                    if (DraftVisibilityType.Approver ==
                                        (DraftVisibilityType)list.DraftVersionVisibility)
                                    {
                                        if (list.EnableModeration)
                                        {
                                            createdList.DraftVersionVisibility =
                                                (DraftVisibilityType)list.DraftVersionVisibility;
                                        }
                                        else
                                        {
                                            // TODO: User should be notified that DraftVersionVisibility is not applied because .EnableModeration is false
                                        }
                                    }
                                    else
                                    {
                                        createdList.DraftVersionVisibility = (DraftVisibilityType)list.DraftVersionVisibility;
                                    }
                                }
                            }
                        }

                        createdList.OnQuickLaunch = list.OnQuickLaunch;
                        createdList.EnableFolderCreation = list.EnableFolderCreation;
                        createdList.Hidden = list.Hidden;
                        createdList.ContentTypesEnabled = list.ContentTypesEnabled;

                        createdList.Update();

                        web.Context.Load(createdList.Views);
                        web.Context.Load(createdList, l => l.Id);
                        web.Context.Load(createdList, l => l.RootFolder.ServerRelativeUrl);
                        web.Context.Load(createdList.ContentTypes);
                        web.Context.ExecuteQueryRetry();

                        // Remove existing content types only if there are custom content type bindings
                        List<Microsoft.SharePoint.Client.ContentType> contentTypesToRemove =
                            new List<Microsoft.SharePoint.Client.ContentType>();
                        if (list.RemoveExistingContentTypes && list.ContentTypeBindings.Count > 0)
                        {
                            foreach (var ct in createdList.ContentTypes)
                            {
                                contentTypesToRemove.Add(ct);
                            }
                        }

                        ContentTypeBinding defaultCtBinding = null;
                        foreach (var ctBinding in list.ContentTypeBindings)
                        {
                            createdList.AddContentTypeToListById(ctBinding.ContentTypeId, searchContentTypeInSiteHierarchy: true);
                            if (ctBinding.Default)
                            {
                                defaultCtBinding = ctBinding;
                            }
                        }

                        // default ContentTypeBinding should be set last because 
                        // list extension .SetDefaultContentTypeToList() re-sets 
                        // the list.RootFolder UniqueContentTypeOrder property
                        // which may cause missing CTs from the "New Button"
                        if (defaultCtBinding != null)
                        {
                            createdList.SetDefaultContentTypeToList(defaultCtBinding.ContentTypeId);
                        }

                        // Effectively remove existing content types, if any
                        foreach (var ct in contentTypesToRemove)
                        {
                            ct.DeleteObject();
                            web.Context.ExecuteQueryRetry();
                        }
                        createdLists.Add(new ListInfo { CreatedList = createdList, ListInstance = list });

                        TokenParser.AddToken(new ListIdToken(web, list.Title, createdList.Id));

                        TokenParser.AddToken(new ListUrlToken(web, list.Title, createdList.RootFolder.ServerRelativeUrl.Substring(web.ServerRelativeUrl.Length + 1)));


                    }

                }

                #endregion

                #region Fields

                foreach (var listInfo in createdLists)
                {
                    if (listInfo.ListInstance.Fields.Any())
                    {
                        foreach (var field in listInfo.ListInstance.Fields)
                        {
                            XElement fieldElement = XElement.Parse(field.SchemaXml.ToParsedString());
                            var id = fieldElement.Attribute("ID").Value;

                            Guid fieldGuid = Guid.Empty;
                            if (Guid.TryParse(id, out fieldGuid))
                            {
                                if (!listInfo.CreatedList.FieldExistsById(fieldGuid))
                                {
                                    var listIdentifier = fieldElement.Attribute("List") != null ? fieldElement.Attribute("List").Value : null;

                                    if (listIdentifier != null)
                                    {
                                        // Temporary remove list attribute from fieldElement
                                        fieldElement.Attribute("List").Remove();
                                    }

                                    var fieldXml = fieldElement.ToString();
                                    listInfo.CreatedList.Fields.AddFieldAsXml(fieldXml, false, AddFieldOptions.DefaultValue);
                                }
                            }
                        }
                    }
                    listInfo.CreatedList.Update();
                    web.Context.ExecuteQueryRetry();
                }

                #endregion

                #region FieldRefs

                foreach (var listInfo in createdLists)
                {

                    if (listInfo.ListInstance.FieldRefs.Any())
                    {

                        foreach (var fieldRef in listInfo.ListInstance.FieldRefs)
                        {
                            var field = rootWeb.GetFieldById<Field>(fieldRef.Id);
                            if (field != null)
                            {
                                if (!listInfo.CreatedList.FieldExistsById(fieldRef.Id))
                                {
                                    var createdField = listInfo.CreatedList.Fields.Add(field);
                                    if (!string.IsNullOrEmpty(fieldRef.DisplayName))
                                    {
                                        createdField.Title = fieldRef.DisplayName;
                                    }
                                    createdField.Hidden = fieldRef.Hidden;
                                    createdField.Required = fieldRef.Required;

                                    createdField.Update();
                                }
                            }

                        }
                        listInfo.CreatedList.Update();
                        web.Context.ExecuteQueryRetry();
                    }
                }

                #endregion

                #region Views

                foreach (var listInfo in createdLists)
                {
                    var list = listInfo.ListInstance;
                    var createdList = listInfo.CreatedList;

                    if (list.Views.Any() && list.RemoveExistingViews)
                    {
                        while (createdList.Views.Any())
                        {
                            createdList.Views[0].DeleteObject();
                        }
                        web.Context.ExecuteQueryRetry();
                    }

                    foreach (var view in list.Views)
                    {
                        var viewDoc = XDocument.Parse(view.SchemaXml);

                        var displayNameXml = viewDoc.Root.Attribute("DisplayName");
                        if (displayNameXml == null)
                        {
                            throw new ApplicationException("Invalid View element, missing a valid value for the attribute DisplayName.");
                        }
                        var viewTitle = displayNameXml.Value;

                        // Type
                        var viewTypeString = viewDoc.Root.Attribute("Type") != null ? viewDoc.Root.Attribute("Type").Value : "None";
                        viewTypeString = viewTypeString[0].ToString().ToUpper() + viewTypeString.Substring(1).ToLower();
                        var viewType = (ViewType)Enum.Parse(typeof(ViewType), viewTypeString);

                        // Fields
                        string[] viewFields = null;
                        var viewFieldsElement = viewDoc.Descendants("ViewFields").FirstOrDefault();
                        if (viewFieldsElement != null)
                        {
                            viewFields = (from field in viewDoc.Descendants("ViewFields").Descendants("FieldRef") select field.Attribute("Name").Value).ToArray();
                        }

                        // Default view
                        var viewDefault = viewDoc.Root.Attribute("DefaultView") != null && Boolean.Parse(viewDoc.Root.Attribute("DefaultView").Value);

                        // Row limit
                        bool viewPaged = true;
                        uint viewRowLimit = 30;
                        var rowLimitElement = viewDoc.Descendants("RowLimit").FirstOrDefault();
                        if (rowLimitElement != null)
                        {
                            if (rowLimitElement.Attribute("Paged") != null)
                            {
                                viewPaged = bool.Parse(rowLimitElement.Attribute("Paged").Value);
                            }
                            viewRowLimit = uint.Parse(rowLimitElement.Value);
                        }

                        // Query
                        var viewQuery = new StringBuilder();
                        foreach (var queryElement in viewDoc.Descendants("Query").Elements())
                        {
                            viewQuery.Append(queryElement.ToString());
                        }

                        var viewCI = new ViewCreationInformation
                        {
                            ViewFields = viewFields,
                            RowLimit = viewRowLimit,
                            Paged = viewPaged,
                            Title = viewTitle,
                            Query = viewQuery.ToString(),
                            ViewTypeKind = viewType,
                            PersonalView = false,
                            SetAsDefaultView = viewDefault
                        };

                        createdList.Views.Add(viewCI);
                        createdList.Update();
                        web.Context.ExecuteQueryRetry();
                    }

                    // Removing existing views set the OnQuickLaunch option to false and need to be re-set.
                    if (list.OnQuickLaunch && list.RemoveExistingViews && list.Views.Count > 0)
                    {
                        createdList.RefreshLoad();
                        web.Context.ExecuteQueryRetry();
                        createdList.OnQuickLaunch = list.OnQuickLaunch;
                        createdList.Update();
                        web.Context.ExecuteQueryRetry();
                    }
                }



                #endregion

                #region DataRows

                foreach (var listInfo in createdLists)
                {
                    var listInstance = listInfo.ListInstance;
                    if (listInstance.DataRows != null && listInstance.DataRows.Any())
                    {
                        var list = listInfo.CreatedList;
                        foreach (var dataRow in listInfo.ListInstance.DataRows)
                        {
                            ListItemCreationInformation listitemCI = new ListItemCreationInformation();
                            var listitem = list.AddItem(listitemCI);
                            foreach (var dataValue in dataRow.Values)
                            {
                                listitem[dataValue.Key.ToParsedString()] = dataValue.Value.ToParsedString();
                            }
                            listitem.Update();
                            web.Context.ExecuteQueryRetry(); // TODO: Run in batches?
                        }
                    }
                }

                #endregion
            }
        }


        private class ListInfo
        {
            public List CreatedList { get; set; }
            public ListInstance ListInstance { get; set; }
        }



        public override ProvisioningTemplate CreateEntities(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            var propertyLoadRequired = false;
            if (!web.IsPropertyAvailable("ServerRelativeUrl"))
            {
                web.Context.Load(web, w => w.ServerRelativeUrl);
                propertyLoadRequired = true;
            }
            if (!web.IsPropertyAvailable("Url"))
            {
                web.Context.Load(web, w => w.Url);
                propertyLoadRequired = true;
            }
            if (propertyLoadRequired)
            {
                web.Context.ExecuteQueryRetry();
            }

            var serverRelativeUrl = web.ServerRelativeUrl;

            // For each list in the site
            ListCollection lists = web.Lists;

            web.Context.Load(lists,
                lc => lc.IncludeWithDefaultProperties(
                    l => l.ContentTypes,
                    l => l.Views,
                    l => l.OnQuickLaunch,
                    l => l.RootFolder.ServerRelativeUrl,
                    l => l.Fields.IncludeWithDefaultProperties(
                        f => f.Id,
                        f => f.Title,
                        f => f.Hidden,
                        f => f.InternalName,
                        f => f.Required)));

            web.Context.ExecuteQuery();
            foreach (var item in lists)
            {
                // Do not export system lists
                if (!item.Hidden)
                {

                    int index = -1;
                    if (creationInfo.BaseTemplate != null)
                    {
                        // Check if we need to skip this list...if so let's do it before we gather all the other information for this list...improves performance
                        index = creationInfo.BaseTemplate.Lists.FindIndex(f => f.Url.Equals(item.RootFolder.ServerRelativeUrl.Substring(serverRelativeUrl.Length + 1)) &&
                                                                  f.TemplateType.Equals(item.BaseTemplate));
                    }

                    if (index == -1)
                    {
                        var contentTypeFields = new List<FieldRef>();
                        ListInstance list = new ListInstance();
                        list.Description = item.Description;
                        list.EnableVersioning = item.EnableVersioning;
                        list.TemplateType = item.BaseTemplate;
                        list.Title = item.Title;
                        list.Hidden = item.Hidden;
                        list.EnableFolderCreation = item.EnableFolderCreation;
                        list.DocumentTemplate = Tokenize(item.DocumentTemplateUrl, web.Url);
                        list.ContentTypesEnabled = item.ContentTypesEnabled;
                        list.Url = item.RootFolder.ServerRelativeUrl.Substring(serverRelativeUrl.Length).TrimStart('/');
                        list.TemplateFeatureID = item.TemplateFeatureId;
                        list.EnableAttachments = item.EnableAttachments;
                        list.OnQuickLaunch = item.OnQuickLaunch;
                        list.MaxVersionLimit = item.IsObjectPropertyInstantiated("MajorVersionLimit") ? item.MajorVersionLimit : 0;
                        list.EnableMinorVersions = item.EnableMinorVersions;
                        list.MinorVersionLimit = item.IsObjectPropertyInstantiated("MajorWithMinorVersionsLimit") ? item.MajorWithMinorVersionsLimit : 0;
                        int count = 0;

                        foreach (var ct in item.ContentTypes)
                        {
                            web.Context.Load(ct, c => c.Parent);
                            web.Context.ExecuteQuery();
                            if (ct.Parent != null)
                            {
                                // Add the parent to the list of content types
                                if (!BuiltInContentTypeId.Contains(ct.Parent.StringId))
                                {
                                    list.ContentTypeBindings.Add(new ContentTypeBinding() { ContentTypeId = ct.Parent.StringId, Default = count == 0 ? true : false });
                                }
                            }
                            else
                            {
                                list.ContentTypeBindings.Add(new ContentTypeBinding() { ContentTypeId = ct.StringId, Default = count == 0 });
                            }

                            web.Context.Load(ct.FieldLinks);
                            web.Context.ExecuteQueryRetry();
                            foreach (var fieldLink in ct.FieldLinks)
                            {
                                if (!fieldLink.Hidden)
                                {
                                    contentTypeFields.Add(new FieldRef() { Id = fieldLink.Id });
                                }
                            }
                            count++;
                        }

                        foreach (var view in item.Views)
                        {
                            if (!view.Hidden)
                            {
                                list.Views.Add(new View() { SchemaXml = view.ListViewXml });
                            }
                        }

                        var siteColumns = web.Fields;
                        web.Context.Load(siteColumns, scs => scs.Include(sc => sc.Id));
                        web.Context.ExecuteQueryRetry();

                        foreach (var field in item.Fields)
                        {
                            if (!field.Hidden)
                            {
                                if (siteColumns.FirstOrDefault(sc => sc.Id == field.Id) != null)
                                {
                                    bool addField = true;
                                    if (item.ContentTypesEnabled && contentTypeFields.FirstOrDefault(c => c.Id == field.Id) == null)
                                    {
                                        if (contentTypeFields.FirstOrDefault(c => c.Id == field.Id) == null)
                                        {
                                            addField = false;
                                        }
                                    }

                                    XElement fieldElement = XElement.Parse(field.SchemaXml);
                                    var sourceId = fieldElement.Attribute("SourceID") != null ? fieldElement.Attribute("SourceID").Value : null;

                                    if (sourceId != null && sourceId == "http://schemas.microsoft.com/sharepoint/v3")
                                    {
                                        if (field.InternalName == "Editor" ||
                                            field.InternalName == "Author" ||
                                            field.InternalName == "Title" ||
                                            field.InternalName == "ID" ||
                                            field.InternalName == "Created" ||
                                            field.InternalName == "Modified" ||
                                            field.InternalName == "Attachments" ||
                                            field.InternalName == "_UIVersionString" ||
                                            field.InternalName == "DocIcon" ||
                                            field.InternalName == "LinkTitleNoMenu" ||
                                            field.InternalName == "LinkTitle" ||
                                            field.InternalName == "Edit" ||
                                            field.InternalName == "AppAuthor" ||
                                            field.InternalName == "AppEditor" ||
                                            field.InternalName == "ContentType" ||
                                            field.InternalName == "ItemChildCount" ||
                                            field.InternalName == "FolderChildCount" ||
                                            field.InternalName == "LinkFilenameNoMenu" ||
                                            field.InternalName == "LinkFilename" ||
                                            field.InternalName == "_CopySource" ||
                                            field.InternalName == "ParentVersionString" ||
                                            field.InternalName == "ParentLeafName" ||
                                            field.InternalName == "_CheckinComment" ||
                                            field.InternalName == "FileLeafRef" ||
                                            field.InternalName == "FileSizeDisplay" ||
                                            field.InternalName == "Preview" ||
                                            field.InternalName == "ThumbnailOnForm")
                                        {
                                            addField = false;
                                        }
                                    }
                                    if (addField)
                                    {

                                        list.FieldRefs.Add(new FieldRef(field.InternalName)
                                        {
                                            Id = field.Id,
                                            DisplayName = field.Title,
                                            Required = field.Required,
                                            Hidden = field.Hidden,
                                        });
                                    }
                                }
                                else
                                {
                                    list.Fields.Add((new Model.Field() { SchemaXml = field.SchemaXml }));
                                }
                            }
                        }
                        template.Lists.Add(list);
                    }
                }
            }

            return template;
        }

        public override bool WillProvision(Web web, ProvisioningTemplate template)
        {
            if (!_willProvision.HasValue)
            {
                _willProvision = template.Lists.Any();
            }
            return _willProvision.Value;
        }

        public override bool WillExtract(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            if (!_willExtract.HasValue)
            {
                ListCollection collList = web.Lists;
                var lists = web.Context.LoadQuery(collList.Where(l => l.Hidden));

                web.Context.ExecuteQuery();

                _willExtract = lists.Any();
            }
            return _willExtract.Value;

        }
    }
}

