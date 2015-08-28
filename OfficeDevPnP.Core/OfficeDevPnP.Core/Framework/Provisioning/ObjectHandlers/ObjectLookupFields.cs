using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
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

        public override void ProvisionObjects(Web web, ProvisioningTemplate template, ProvisioningTemplateApplyingInformation applyingInformation)
        {
            ProcessLookupFields(web, template);
        }

        public override ProvisioningTemplate ExtractObjects(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            return template;
        }

        private static void ProcessLookupFields(Web web, ProvisioningTemplate template)
        {
            var rootWeb = (web.Context as ClientContext).Site.RootWeb;
            rootWeb.Context.Load(rootWeb.Lists, lists => lists.Include(l => l.Id, l => l.RootFolder.ServerRelativeUrl, l => l.Fields).Where(l => l.Hidden == false));
            rootWeb.Context.ExecuteQueryRetry();

            foreach (var siteField in template.SiteFields)
            {
                var fieldElement = XElement.Parse(siteField.SchemaXml);

                if (fieldElement.Attribute("List") != null)
                {
                    var fieldId = Guid.Parse(fieldElement.Attribute("ID").Value);
                    var listIdentifier = fieldElement.Attribute("List").Value;
                    var webId = string.Empty;

                    var field = rootWeb.Fields.GetById(fieldId);
                    rootWeb.Context.Load(field, f => f.SchemaXml);
                    rootWeb.Context.ExecuteQueryRetry();

                    Guid listGuid;
                    if (!Guid.TryParse(listIdentifier, out listGuid))
                    {
                        var sourceListUrl = UrlUtility.Combine(web.ServerRelativeUrl, listIdentifier.ToParsedString());
                        var sourceList = rootWeb.Lists.FirstOrDefault(l => l.RootFolder.ServerRelativeUrl.Equals(sourceListUrl, StringComparison.OrdinalIgnoreCase));
                        if (sourceList != null)
                        {
                            listGuid = sourceList.Id;

                            rootWeb.Context.Load(sourceList.ParentWeb);
                            rootWeb.Context.ExecuteQueryRetry();

                            webId = sourceList.ParentWeb.Id.ToString();
                        }
                    }
                    if (listGuid != Guid.Empty)
                    {
                        ProcessField(field, listGuid, webId);
                    }
                }
            }

            web.Context.Load(web.Lists, lists => lists.Include(l => l.Id, l => l.Title, l => l.RootFolder.ServerRelativeUrl, l => l.Fields).Where(l => l.Hidden == false));
            web.Context.ExecuteQueryRetry();

            foreach (var listInstance in template.Lists)
            {
                foreach (var listField in listInstance.Fields)
                {
                    var fieldElement = XElement.Parse(listField.SchemaXml);
                    if (fieldElement.Attribute("List") == null) continue;

                    var fieldId = Guid.Parse(fieldElement.Attribute("ID").Value);
                    var listIdentifier = fieldElement.Attribute("List").Value;
                    var listUrl = UrlUtility.Combine(web.ServerRelativeUrl, listInstance.Url.ToParsedString());
                    var createdList = web.Lists.FirstOrDefault(l => l.RootFolder.ServerRelativeUrl.Equals(listUrl, StringComparison.OrdinalIgnoreCase));

                    if (createdList == null)
                        continue;

                    var field = createdList.Fields.GetById(fieldId);
                    web.Context.Load(field, f => f.SchemaXml);
                    web.Context.ExecuteQueryRetry();

                    Guid listGuid;
                    List sourceList;
                    if (!Guid.TryParse(listIdentifier, out listGuid))
                    {
                        var sourceListUrl = UrlUtility.Combine(web.ServerRelativeUrl, listIdentifier.ToParsedString());
                        sourceList = web.Lists.FirstOrDefault(l => l.RootFolder.ServerRelativeUrl.Equals(sourceListUrl, StringComparison.OrdinalIgnoreCase));
                    }
                    else
                    {
                        // The Guid came from the parsed value and is still the template guid.
                        // Fetch the list from the original ID so we can retrieve it again
                        var templateSourceList = template.Lists.Single(l => l.OriginalId.Equals(listGuid));
                        sourceList = web.Lists.FirstOrDefault(l => l.Title.Equals(templateSourceList.Title));
                    }

                    ProcessField(field, sourceList);
                }
            }
        }

        private static void ProcessField(Field field, List sourceList)
        {
            if (sourceList == null)
                return;

            sourceList.ParentWeb.Context.Load(sourceList.ParentWeb);
            sourceList.ParentWeb.Context.ExecuteQueryRetry();

            ProcessField(field, sourceList.Id, sourceList.ParentWeb.Id.ToString());
        }

        private static void ProcessField(Field field, Guid listGuid, string webId)
        {
            var isDirty = false;

            var existingFieldElement = XElement.Parse(field.SchemaXml);

            isDirty = UpdateFieldAttribute(existingFieldElement, "List", listGuid.ToString(), false);

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
