using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Utilities;
using Field = Microsoft.SharePoint.Client.Field;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    internal class ObjectLookupFields : ObjectHandlerBase
    {
        public override string Name
        {
            get { return "Lookup Fields"; }
        }

        public ObjectLookupFields()
        {
            this.ReportProgress = false;
        }

        public override void ProvisionObjects(Web web, ProvisioningTemplate template)
        {
            ProcessLookupFields(web, template);
        }

        public override ProvisioningTemplate ExtractObjects(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            return template;
        }

        private static void ProcessLookupFields(Web web, ProvisioningTemplate template)
        {
            var ctx = web.Context as ClientContext;
            var rootWeb = ctx.Site.RootWeb;
            ctx.Load(rootWeb, w => w.ServerRelativeUrl);
            var rootLists = rootWeb.Lists;
            ctx.Load(rootLists, lists => lists.Include(l => l.Id, l => l.RootFolder.ServerRelativeUrl, l => l.Fields));
            ctx.ExecuteQueryRetry();

            // update site columns lookup fields
            foreach (var siteField in template.SiteFields)
            {
                var fieldElement = XElement.Parse(siteField.SchemaXml);

                // if field has a List attribute we will process this lookup field here
                if (fieldElement.Attribute("List") != null)
                {
                    var fieldId = Guid.Parse(fieldElement.Attribute("ID").Value);
                    var listIdentifier = fieldElement.Attribute("List").Value;
                    var staticFieldName = fieldElement.Attribute("StaticName") != null
                            ? fieldElement.Attribute("StaticName").Value
                            : string.Empty;
                    var webId = string.Empty;

                    var field = rootWeb.GetFieldById<Field>(fieldId);
                    if (field == null)
                    {
                        Log.Warning(Constants.LOGGING_SOURCE,
                            CoreResources.ObjectLookupFields_FieldNotExist, fieldId, staticFieldName, rootWeb.ServerRelativeUrl);
                        continue;
                    }
                    ctx.Load(field, f => f.SchemaXml);
                    ctx.ExecuteQueryRetry();

                    Guid listGuid;
                    if (!Guid.TryParse(listIdentifier, out listGuid))
                    {
                        var rootWebSourceListUrl = UrlUtility.Combine(rootWeb.ServerRelativeUrl, listIdentifier.ToParsedString());
                        var sourceList = rootLists.FirstOrDefault(l => l.RootFolder.ServerRelativeUrl.Equals(rootWebSourceListUrl, StringComparison.OrdinalIgnoreCase));
                        if (sourceList == null)
                        {
                            Log.Warning(Constants.LOGGING_SOURCE,
                                CoreResources.ObjectLookupFields_ListNotExist, fieldId, staticFieldName,
                                rootWebSourceListUrl);
                        }
                        else
                        {
                            listGuid = sourceList.Id;

                            ctx.Load(sourceList.ParentWeb);
                            ctx.ExecuteQueryRetry();

                            webId = sourceList.ParentWeb.Id.ToString();
                        }
                    }
                    if (!string.IsNullOrEmpty(webId))
                    {
                        ProcessField(field, listGuid, webId);
                    }
                }
            }

            // update list assoc lookup fields
            var webLists = web.Lists;
            ctx.Load(webLists, lists => lists.Include(l => l.Id, l => l.RootFolder.ServerRelativeUrl, l => l.Fields));
            ctx.ExecuteQueryRetry();

            foreach (var listInstance in template.Lists)
            {
                foreach (var listField in listInstance.Fields)
                {
                    var fieldElement = XElement.Parse(listField.SchemaXml);
                    if (fieldElement.Attribute("List") == null) continue;

                    var fieldId = Guid.Parse(fieldElement.Attribute("ID").Value);
                    var listIdentifier = fieldElement.Attribute("List").Value;
                    var staticFieldName = fieldElement.Attribute("StaticName") != null
                            ? fieldElement.Attribute("StaticName").Value
                            : string.Empty;
                    var webId = string.Empty;

                    var listUrl = UrlUtility.Combine(web.ServerRelativeUrl, listInstance.Url.ToParsedString());

                    var webSourceList = webLists.FirstOrDefault(l => l.RootFolder.ServerRelativeUrl.Equals(listUrl, StringComparison.OrdinalIgnoreCase));
                    if (webSourceList != null)
                    {
                        var field = webSourceList.GetFieldById<Field>(fieldId);
                        if (field == null)
                        {
                            Log.Warning(Constants.LOGGING_SOURCE,
                                CoreResources.ObjectLookupFields_FieldNotExist, fieldId, staticFieldName, webSourceList.RootFolder.ServerRelativeUrl);
                            continue;
                        }
                        ctx.Load(field, f => f.SchemaXml);
                        ctx.ExecuteQueryRetry();

                        Guid listGuid;
                        if (!Guid.TryParse(listIdentifier, out listGuid))
                        {
                            var sourceListUrl = UrlUtility.Combine(web.ServerRelativeUrl, listIdentifier.ToParsedString());
                            var sourceList = webLists.FirstOrDefault(l => l.RootFolder.ServerRelativeUrl.Equals(sourceListUrl, StringComparison.OrdinalIgnoreCase));
                            if (sourceList == null)
                            {
                                Log.Warning(Constants.LOGGING_SOURCE,
                                    CoreResources.ObjectLookupFields_ListNotExist, fieldId, staticFieldName,
                                    sourceListUrl);
                            }
                            else
                            {
                                listGuid = sourceList.Id;

                                ctx.Load(sourceList.ParentWeb);
                                ctx.ExecuteQueryRetry();

                                webId = sourceList.ParentWeb.Id.ToString();
                            }
                        }
                        if (!string.IsNullOrEmpty(webId))
                        {
                            ProcessField(field, listGuid, webId);
                        }
                    }
                }
            }
        }

        private static void ProcessField(Field field, Guid listGuid, string webId)
        {
            var existingFieldElement = XElement.Parse(field.SchemaXml);

            var isDirty = UpdateFieldAttribute(existingFieldElement, "List", listGuid.ToString(), false);

            isDirty = UpdateFieldAttribute(existingFieldElement, "WebId", webId, isDirty);

            isDirty = UpdateFieldAttribute(existingFieldElement, "SourceID", webId, isDirty);

            if (isDirty)
            {
                field.SchemaXml = existingFieldElement.ToString();

                field.UpdateAndPushChanges(true);
                field.Context.ExecuteQueryRetry();
            }
        }

        private static bool UpdateFieldAttribute(XElement existingFieldElement, string attributeName, string attributeValue, bool isDirty)
        {
            if (existingFieldElement.Attribute(attributeName) == null)
            {
                existingFieldElement.Add(new XAttribute(attributeName, attributeValue));
                isDirty = true;
            }
            else if (!existingFieldElement.Attribute(attributeName).Value.Equals(attributeValue))
            {
                existingFieldElement.Attribute(attributeName).SetValue(attributeValue);
                isDirty = true;
            }
            return isDirty;
        }


        public override bool WillProvision(Web web, ProvisioningTemplate template)
        {
            if (!_willProvision.HasValue)
            {
                _willProvision = true;
            }
            return _willProvision.Value;
        }

        public override bool WillExtract(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            if (!_willExtract.HasValue)
            {
                _willExtract = false;
            }
            return _willExtract.Value;
        }
    }
}
