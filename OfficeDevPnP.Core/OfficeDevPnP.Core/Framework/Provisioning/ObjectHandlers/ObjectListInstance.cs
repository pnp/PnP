using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.ObjectHandlers.TokenDefinitions;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Utilities;
using ContentType = Microsoft.SharePoint.Client.ContentType;
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
        public override void ProvisionObjects(Web web, ProvisioningTemplate template, ProvisioningTemplateApplyingInformation applyingInformation)
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
                var existingLists = web.Lists.AsEnumerable().Select(existingList => existingList.RootFolder.ServerRelativeUrl).ToList();
                var serverRelativeUrl = web.ServerRelativeUrl;

                var processedLists = new List<ListInfo>();

                #region Lists

                foreach (var templateList in template.Lists)
                {
                    var index = existingLists.FindIndex(x => x.Equals(UrlUtility.Combine(serverRelativeUrl, templateList.Url), StringComparison.OrdinalIgnoreCase));
                    if (index == -1)
                    {
                        var createdList = CreateList(web, templateList);
                        processedLists.Add(new ListInfo { SiteList = createdList, TemplateList = templateList });

                        TokenParser.AddToken(new ListIdToken(web, templateList.Title, createdList.Id));

                        TokenParser.AddToken(new ListUrlToken(web, templateList.Title, createdList.RootFolder.ServerRelativeUrl.Substring(web.ServerRelativeUrl.Length + 1)));
                    }
                    else
                    {
                        var existingList = web.Lists[index];
                        var updatedList = UpdateList(web, existingList, templateList);
                        if (updatedList != null)
                        {
                            processedLists.Add(new ListInfo { SiteList = updatedList, TemplateList = templateList });
                        }
                    }
                }

                #endregion

                #region FieldRefs

                foreach (var listInfo in processedLists)
                {

                    if (listInfo.TemplateList.FieldRefs.Any())
                    {

                        foreach (var fieldRef in listInfo.TemplateList.FieldRefs)
                        {
                            var field = rootWeb.GetFieldById<Field>(fieldRef.Id);
                            if (field != null)
                            {
                                if (!listInfo.SiteList.FieldExistsById(fieldRef.Id))
                                {
                                    CreateFieldRef(listInfo, field, fieldRef);
                                }
                                else
                                {
                                    UpdateFieldRef(field, fieldRef);
                                }
                            }

                        }
                        listInfo.SiteList.Update();
                        web.Context.ExecuteQueryRetry();
                    }
                }

                #endregion

                #region Fields

                foreach (var listInfo in processedLists)
                {
                    if (listInfo.TemplateList.Fields.Any())
                    {
                        foreach (var field in listInfo.TemplateList.Fields)
                        {
                            var fieldElement = XElement.Parse(field.SchemaXml.ToParsedString());
                            if (fieldElement.Attribute("ID") == null)
                            {
                                throw new Exception(string.Format("Field schema has no ID attribute: {0}",field.SchemaXml));
                            }
                            var id = fieldElement.Attribute("ID").Value;

                            Guid fieldGuid;
                            if (!Guid.TryParse(id, out fieldGuid))
                            {
                                throw new Exception(string.Format("ID for field is not a valid Guid", field.SchemaXml));
                            }
                            else
                            {
                                var fieldFromList = listInfo.SiteList.GetFieldById<Field>(fieldGuid);
                                if (fieldFromList == null)
                                {
                                    CreateField(fieldElement, listInfo);
                                }
                                else
                                {
                                    UpdateField(web, listInfo, fieldGuid, fieldElement, fieldFromList);
                                }
                            }
                        }
                    }
                    listInfo.SiteList.Update();
                    web.Context.ExecuteQueryRetry();
                }

                #endregion

                #region Views

                foreach (var listInfo in processedLists)
                {
                    var list = listInfo.TemplateList;
                    var createdList = listInfo.SiteList;

                    if (list.Views.Any() && list.RemoveExistingViews)
                    {
                        while (createdList.Views.Any())
                        {
                            createdList.Views[0].DeleteObject();
                        }
                        web.Context.ExecuteQueryRetry();
                    }

                    var existingViews = createdList.Views;
                    web.Context.Load(existingViews, vs => vs.Include(v => v.Title, v => v.Id));
                    web.Context.ExecuteQueryRetry();
                    foreach (var view in list.Views)
                    {
                        var viewElement = XElement.Parse(view.SchemaXml);
                        var displayNameElement = viewElement.Attribute("DisplayName");
                        if (displayNameElement == null)
                        {
                            throw new ApplicationException("Invalid View element, missing a valid value for the attribute DisplayName.");
                        }

                        var existingView = existingViews.FirstOrDefault(v => v.Title == displayNameElement.Value);

                        if (existingView != null)
                        {
                            existingView.DeleteObject();
                            web.Context.ExecuteQueryRetry();
                        }

                        var viewTitle = displayNameElement.Value;

                        // Type
                        var viewTypeString = viewElement.Attribute("Type") != null ? viewElement.Attribute("Type").Value : "None";
                        viewTypeString = viewTypeString[0].ToString().ToUpper() + viewTypeString.Substring(1).ToLower();
                        var viewType = (ViewType)Enum.Parse(typeof(ViewType), viewTypeString);

                        // Fields
                        string[] viewFields = null;
                        var viewFieldsElement = viewElement.Descendants("ViewFields").FirstOrDefault();
                        if (viewFieldsElement != null)
                        {
                            viewFields = (from field in viewElement.Descendants("ViewFields").Descendants("FieldRef") select field.Attribute("Name").Value).ToArray();
                        }

                        // Default view
                        var viewDefault = viewElement.Attribute("DefaultView") != null && Boolean.Parse(viewElement.Attribute("DefaultView").Value);

                        // Row limit
                        var viewPaged = true;
                        uint viewRowLimit = 30;
                        var rowLimitElement = viewElement.Descendants("RowLimit").FirstOrDefault();
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
                        foreach (var queryElement in viewElement.Descendants("Query").Elements())
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

            }
        }

        private static void UpdateFieldRef(Field field, FieldRef fieldRef)
        {
            var isDirty = false;
            if (!string.IsNullOrEmpty(fieldRef.DisplayName) && fieldRef.DisplayName != field.Title)
            {
                field.Title = fieldRef.DisplayName;
                isDirty = true;
            }
            if (fieldRef.Hidden != field.Hidden)
            {
                field.Hidden = fieldRef.Hidden;
                isDirty = true;
            }
            if (fieldRef.Required != field.Required)
            {
                field.Required = fieldRef.Required;
                isDirty = true;
            }
            if (isDirty)
            {
                field.UpdateAndPushChanges(true);
                field.Context.ExecuteQueryRetry();
            }
        }

        private static void CreateFieldRef(ListInfo listInfo, Field field, FieldRef fieldRef)
        {
            XElement element = XElement.Parse(field.SchemaXml);

            element.SetAttributeValue("AllowDeletion", "TRUE");

            field.SchemaXml = element.ToString();

            var createdField = listInfo.SiteList.Fields.Add(field);
            if (!string.IsNullOrEmpty(fieldRef.DisplayName))
            {
                createdField.Title = fieldRef.DisplayName;
            }
            createdField.Hidden = fieldRef.Hidden;
            createdField.Required = fieldRef.Required;

            createdField.Update();
            createdField.Context.ExecuteQueryRetry();
        }

        private static void CreateField(XElement fieldElement, ListInfo listInfo)
        {
            var listIdentifier = fieldElement.Attribute("List") != null ? fieldElement.Attribute("List").Value : null;

            if (listIdentifier != null)
            {
                // Temporary remove list attribute from fieldElement
                fieldElement.Attribute("List").Remove();
            }

            var fieldXml = fieldElement.ToString();
            listInfo.SiteList.Fields.AddFieldAsXml(fieldXml, false, AddFieldOptions.DefaultValue);
            listInfo.SiteList.Context.ExecuteQueryRetry();
        }

        private void UpdateField(ClientObject web, ListInfo listInfo, Guid fieldId, XElement templateFieldElement, Field existingField)
        {
            web.Context.Load(existingField, f => f.SchemaXml);
            web.Context.ExecuteQueryRetry();

            var existingFieldElement = XElement.Parse(existingField.SchemaXml);

            var equalityComparer = new XNodeEqualityComparer();

            // Is field different in template?
            if (equalityComparer.GetHashCode(existingFieldElement) != equalityComparer.GetHashCode(templateFieldElement))
            {
                // Is existing field of the same type?
                if (existingFieldElement.Attribute("Type").Value == templateFieldElement.Attribute("Type").Value)
                {
                    var listIdentifier = templateFieldElement.Attribute("List") != null
                        ? templateFieldElement.Attribute("List").Value
                        : null;

                    if (listIdentifier != null)
                    {
                        // Temporary remove list attribute from list
                        templateFieldElement.Attribute("List").Remove();
                    }

                    foreach (var attribute in templateFieldElement.Attributes())
                    {
                        if (existingFieldElement.Attribute(attribute.Name) != null)
                        {
                            existingFieldElement.Attribute(attribute.Name).Value = attribute.Value;
                        }
                        else
                        {
                            existingFieldElement.Add(attribute);
                        }
                    }
                    foreach (var element in templateFieldElement.Elements())
                    {
                        if (existingFieldElement.HasAttributes && existingFieldElement.Attribute(element.Name) != null)
                        {
                            existingFieldElement.Attribute(element.Name).Remove();
                        }
                        existingFieldElement.Add(element);
                    }

                    if (existingFieldElement.Attribute("Version") != null)
                    {
                        existingFieldElement.Attributes("Version").Remove();
                    }
                    existingField.SchemaXml = existingFieldElement.ToString();
                    existingField.UpdateAndPushChanges(true);
                    web.Context.ExecuteQueryRetry();
                }
                else
                {
                    var fieldName = existingFieldElement.Attribute("Name") != null ? existingFieldElement.Attribute("Name").Value : existingFieldElement.Attribute("StaticName").Value;
                    WriteWarning(string.Format("Field {0} ({1}) exists in list {2} ({3}) but is of different type. Skipping field.", fieldName, fieldId, listInfo.TemplateList.Title, listInfo.SiteList.Id), ProvisioningMessageType.Warning);
                }
            }
        }

        private List UpdateList(Web web, List existingList, ListInstance templateList)
        {
            web.Context.Load(existingList,
                l => l.Title,
                l => l.Description,
                l => l.OnQuickLaunch,
                l => l.Hidden,
                l => l.ContentTypesEnabled,
                l => l.EnableAttachments,
                l => l.EnableFolderCreation,
                l => l.EnableMinorVersions,
                l => l.DraftVersionVisibility
#if !CLIENTSDKV15
                ,l => l.MajorWithMinorVersionsLimit
#endif
                );
            web.Context.ExecuteQueryRetry();

            if (existingList.BaseTemplate == templateList.TemplateType)
            {
                var isDirty = false;
                if (templateList.Title != existingList.Title)
                {
                    existingList.Title = templateList.Title;
                    isDirty = true;
                }
                if (!string.IsNullOrEmpty(templateList.DocumentTemplate))
                {
                    if (existingList.DocumentTemplateUrl != templateList.DocumentTemplate.ToParsedString())
                    {
                        existingList.DocumentTemplateUrl = templateList.DocumentTemplate.ToParsedString();
                        isDirty = true;
                    }
                }
                if (!string.IsNullOrEmpty(templateList.Description) && templateList.Description != existingList.Description)
                {
                    existingList.Description = templateList.Description;
                    isDirty = true;
                }
                if (templateList.Hidden != existingList.Hidden)
                {
                    existingList.Hidden = templateList.Hidden;
                    isDirty = true;
                }
                if (templateList.OnQuickLaunch != existingList.OnQuickLaunch)
                {
                    existingList.OnQuickLaunch = templateList.OnQuickLaunch;
                    isDirty = true;
                }
                if (templateList.ContentTypesEnabled != existingList.ContentTypesEnabled)
                {
                    existingList.ContentTypesEnabled = templateList.ContentTypesEnabled;
                    isDirty = true;
                }
                if (templateList.EnableAttachments != existingList.EnableAttachments)
                {
                    existingList.EnableAttachments = templateList.EnableAttachments;
                    isDirty = true;
                }
                if (existingList.BaseTemplate != (int) ListTemplateType.DiscussionBoard)
                {
                    if (templateList.EnableFolderCreation != existingList.EnableFolderCreation)
                    {
                        existingList.EnableFolderCreation = templateList.EnableFolderCreation;
                        isDirty = true;
                    }
                }
                if (templateList.EnableVersioning)
                {
                    if (existingList.EnableVersioning != templateList.EnableVersioning)
                    {
                        existingList.EnableVersioning = templateList.EnableVersioning;
                        isDirty = true;
                    }
                    if (existingList.MajorVersionLimit != templateList.MaxVersionLimit)
                    {
                        existingList.MajorVersionLimit = templateList.MaxVersionLimit;
                        isDirty = true;
                    }
                    if (existingList.BaseTemplate == (int) ListTemplateType.DocumentLibrary)
                    {
                        // Only supported on Document Libraries
                        if (templateList.EnableMinorVersions != existingList.EnableMinorVersions)
                        {
                            existingList.EnableMinorVersions = templateList.EnableMinorVersions;
                            isDirty = true;
                        }
                        if ((DraftVisibilityType) templateList.DraftVersionVisibility != existingList.DraftVersionVisibility)
                        {
                            existingList.DraftVersionVisibility = (DraftVisibilityType) templateList.DraftVersionVisibility;
                            isDirty = true;
                        }

                        if (templateList.EnableMinorVersions)
                        {
                            if (templateList.MinorVersionLimit != existingList.MajorWithMinorVersionsLimit)
                            {
                                existingList.MajorWithMinorVersionsLimit = templateList.MinorVersionLimit;
                            }

                            if (DraftVisibilityType.Approver ==
                                (DraftVisibilityType) templateList.DraftVersionVisibility)
                            {
                                if (templateList.EnableModeration)
                                {
                                    if ((DraftVisibilityType) templateList.DraftVersionVisibility != existingList.DraftVersionVisibility)
                                    {
                                        existingList.DraftVersionVisibility = (DraftVisibilityType) templateList.DraftVersionVisibility;
                                        isDirty = true;
                                    }
                                }
                            }
                            else
                            {
                                if ((DraftVisibilityType) templateList.DraftVersionVisibility != existingList.DraftVersionVisibility)
                                {
                                    existingList.DraftVersionVisibility = (DraftVisibilityType) templateList.DraftVersionVisibility;
                                    isDirty = true;
                                }
                            }
                        }
                    }
                }
                if (isDirty)
                {
                    existingList.Update();
                    web.Context.ExecuteQueryRetry();
                }


                if (existingList.ContentTypesEnabled)
                {
                    // Check if we need to add a content type
                    
                    var existingContentTypes = existingList.ContentTypes;
                    web.Context.Load(existingContentTypes, cts => cts.Include(ct => ct.StringId));
                    web.Context.ExecuteQueryRetry();

                    var bindingsToAdd = templateList.ContentTypeBindings.Where(ctb => existingContentTypes.All(ct => !ctb.ContentTypeId.Equals(ct.StringId, StringComparison.InvariantCultureIgnoreCase))).ToList();
                    var defaultCtBinding = templateList.ContentTypeBindings.FirstOrDefault(ctb => ctb.Default == true);
                    foreach (var ctb in bindingsToAdd)
                    {
                        existingList.AddContentTypeToListById(ctb.ContentTypeId, searchContentTypeInSiteHierarchy: true);
                    }
                
                    // default ContentTypeBinding should be set last because 
                    // list extension .SetDefaultContentTypeToList() re-sets 
                    // the list.RootFolder UniqueContentTypeOrder property
                    // which may cause missing CTs from the "New Button"
                    if (defaultCtBinding != null)
                    {
                        existingList.SetDefaultContentTypeToList(defaultCtBinding.ContentTypeId);
                    }
                }
                return existingList;
            }
            else
            {
                WriteWarning(string.Format("List {0} ({1}, {2}) exists but is of a different type. Skipping list.", templateList.Title, templateList.Url, existingList.Id), ProvisioningMessageType.Warning);
                return null;
            }
        }

        private List CreateList(Web web, ListInstance list)
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
                                WriteWarning("DraftVersionVisibility not applied because EnableModeration is not set to true", ProvisioningMessageType.Warning);
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
            if (createdList.BaseTemplate != (int)ListTemplateType.DiscussionBoard)
            {
                createdList.EnableFolderCreation = list.EnableFolderCreation;
            }
            createdList.Hidden = list.Hidden;
            createdList.ContentTypesEnabled = list.ContentTypesEnabled;

            createdList.Update();

            web.Context.Load(createdList.Views);
            web.Context.Load(createdList, l => l.Id);
            web.Context.Load(createdList, l => l.RootFolder.ServerRelativeUrl);
            web.Context.Load(createdList.ContentTypes);
            web.Context.ExecuteQueryRetry();

            // Remove existing content types only if there are custom content type bindings
            var contentTypesToRemove = new List<ContentType>();
            if (list.RemoveExistingContentTypes && list.ContentTypeBindings.Count > 0)
            {
                contentTypesToRemove.AddRange(createdList.ContentTypes);
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

            return createdList;
        }


        private class ListInfo
        {
            public List SiteList { get; set; }
            public ListInstance TemplateList { get; set; }
        }



        public override ProvisioningTemplate ExtractObjects(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
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
            var lists = web.Lists;

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

            web.Context.ExecuteQueryRetry();
            foreach (var item in lists.Where(l => l.Hidden == false))
            {
                ListInstance baseTemplateList = null;
                if (creationInfo.BaseTemplate != null)
                {
                    // Check if we need to skip this list...if so let's do it before we gather all the other information for this list...improves performance
                    var index = creationInfo.BaseTemplate.Lists.FindIndex(f => f.Url.Equals(item.RootFolder.ServerRelativeUrl.Substring(serverRelativeUrl.Length + 1)) &&
                                                                               f.TemplateType.Equals(item.BaseTemplate));
                    if (index != -1)
                    {
                        baseTemplateList = creationInfo.BaseTemplate.Lists[index];
                    }
                }

                var contentTypeFields = new List<FieldRef>();
                var list = new ListInstance
                {
                    Description = item.Description,
                    EnableVersioning = item.EnableVersioning,
                    TemplateType = item.BaseTemplate,
                    Title = item.Title,
                    Hidden = item.Hidden,
                    EnableFolderCreation = item.EnableFolderCreation,
                    DocumentTemplate = Tokenize(item.DocumentTemplateUrl, web.Url),
                    ContentTypesEnabled = item.ContentTypesEnabled,
                    Url = item.RootFolder.ServerRelativeUrl.Substring(serverRelativeUrl.Length).TrimStart('/'),
                    TemplateFeatureID = item.TemplateFeatureId,
                    EnableAttachments = item.EnableAttachments,
                    OnQuickLaunch = item.OnQuickLaunch,
                    MaxVersionLimit =
                        item.IsObjectPropertyInstantiated("MajorVersionLimit") ? item.MajorVersionLimit : 0,
                    EnableMinorVersions = item.EnableMinorVersions,
                    MinorVersionLimit =
                        item.IsObjectPropertyInstantiated("MajorWithMinorVersionsLimit")
                            ? item.MajorWithMinorVersionsLimit
                            : 0
                };
                var count = 0;

                foreach (var ct in item.ContentTypes)
                {
                    web.Context.Load(ct, c => c.Parent);
                    web.Context.ExecuteQueryRetry();
                    if (ct.Parent != null)
                    {
                        // Add the parent to the list of content types
                        if (!BuiltInContentTypeId.Contains(ct.Parent.StringId))
                        {
                            list.ContentTypeBindings.Add(new ContentTypeBinding { ContentTypeId = ct.Parent.StringId, Default = count == 0 });
                        }
                    }
                    else
                    {
                        list.ContentTypeBindings.Add(new ContentTypeBinding { ContentTypeId = ct.StringId, Default = count == 0 });
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

                foreach (var view in item.Views.Where(view => !view.Hidden))
                {
                    list.Views.Add(new View { SchemaXml = view.ListViewXml });
                }

                var siteColumns = web.Fields;
                web.Context.Load(siteColumns, scs => scs.Include(sc => sc.Id));
                web.Context.ExecuteQueryRetry();

                foreach (var field in item.Fields.Where(field => !field.Hidden))
                {
                    if (siteColumns.FirstOrDefault(sc => sc.Id == field.Id) != null)
                    {
                        var addField = true;
                        if (item.ContentTypesEnabled && contentTypeFields.FirstOrDefault(c => c.Id == field.Id) == null)
                        {
                            if (contentTypeFields.FirstOrDefault(c => c.Id == field.Id) == null)
                            {
                                addField = false;
                            }
                        }

                        var fieldElement = XElement.Parse(field.SchemaXml);
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
                        list.Fields.Add((new Model.Field { SchemaXml = field.SchemaXml }));
                    }
                }
                if (baseTemplateList != null)
                {
                    if (!baseTemplateList.Equals(list))
                    {
                        template.Lists.Add(list);
                    }
                }
                else
                {
                    template.Lists.Add(list);
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
                var collList = web.Lists;
                var lists = web.Context.LoadQuery(collList.Where(l => l.Hidden == false));

                web.Context.ExecuteQueryRetry();

                _willExtract = lists.Any();
            }
            return _willExtract.Value;
        }
    }
}

