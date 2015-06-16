using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;

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
            ReportProgress = false;
        }

        public override void ProvisionObjects(Web web, ProvisioningTemplate template)
        {
            ProcessLookupFields(web, template);
        }

        public override ProvisioningTemplate CreateEntities(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            return template;
        }

        private static void ProcessLookupFields(Web web, ProvisioningTemplate template)
        {
            web.Context.Load(web.Lists, lists => lists.Include(l => l.Id, l => l.RootFolder.ServerRelativeUrl, l => l.Fields));
            web.Context.ExecuteQueryRetry();
            var rootWeb = (web.Context as ClientContext).Site.RootWeb;

            foreach (var siteField in template.SiteFields)
            {
                var fieldElement = XElement.Parse(siteField.SchemaXml);

                if (fieldElement.Attribute("List") == null) continue;

                var fieldId = Guid.Parse(fieldElement.Attribute("ID").Value);
                var listIdentifier = fieldElement.Attribute("List").Value;

                var webId = fieldElement.Attribute("WebId") != null ? fieldElement.Attribute("WebId").Value : string.Empty;
                
                var field = rootWeb.Fields.GetById(fieldId);
                web.Context.Load(field, f => f.SchemaXml);
                web.Context.ExecuteQueryRetry();

                Guid listGuid;
                if (!Guid.TryParse(listIdentifier, out listGuid))
                {
                    var sourceListUrl = UrlUtility.Combine(web.ServerRelativeUrl, listIdentifier.ToParsedString());
                    var sourceList = web.Lists.FirstOrDefault(l => l.RootFolder.ServerRelativeUrl.Equals(sourceListUrl, StringComparison.OrdinalIgnoreCase));
                    if (sourceList != null)
                    {
                        listGuid = sourceList.Id;
                        webId = web.Id.ToString();
                    }
                }
                if (listGuid == Guid.Empty || string.IsNullOrEmpty(webId)) continue;

                var existingFieldElement = XElement.Parse(field.SchemaXml);

                if (existingFieldElement.Attribute("List") == null)
                {
                    existingFieldElement.Add(new XAttribute("List", listGuid.ToString()));
                }
                else
                {
                    existingFieldElement.Attribute("List").SetValue(listGuid.ToString());
                }

                if (!string.IsNullOrEmpty(webId))
                {
                    if (existingFieldElement.Attribute("WebId") == null)
                    {
                        existingFieldElement.Add(new XAttribute("WebId", webId));
                    }
                    else
                    {
                        existingFieldElement.Attribute("WebId").SetValue(webId);
                    }
                } 
                field.SchemaXml = existingFieldElement.ToString();

                field.UpdateAndPushChanges(true);
                web.Context.ExecuteQueryRetry();
            }

            foreach (var listInstance in template.Lists)
            {
                foreach (var listField in listInstance.Fields)
                {
                    var fieldElement = XElement.Parse(listField.SchemaXml);
                    if (fieldElement.Attribute("List") == null) continue;

                    var fieldId = Guid.Parse(fieldElement.Attribute("ID").Value);
                    var listIdentifier = fieldElement.Attribute("List").Value;

                    var webId = fieldElement.Attribute("WebId") != null ? fieldElement.Attribute("WebId").Value : string.Empty;

                    var listUrl = UrlUtility.Combine(web.ServerRelativeUrl, listInstance.Url.ToParsedString());

                    var createdList = web.Lists.FirstOrDefault(l => l.RootFolder.ServerRelativeUrl.Equals(listUrl, StringComparison.OrdinalIgnoreCase));

                    if (createdList == null) continue;

                    var field = createdList.Fields.GetById(fieldId);
                    web.Context.Load(field, f => f.SchemaXml);
                    web.Context.ExecuteQueryRetry();

                    Guid listGuid;
                    if (!Guid.TryParse(listIdentifier, out listGuid))
                    {
                        var sourceListUrl = UrlUtility.Combine(web.ServerRelativeUrl, listIdentifier.ToParsedString());
                        var sourceList = web.Lists.FirstOrDefault(l => l.RootFolder.ServerRelativeUrl.Equals(sourceListUrl, StringComparison.OrdinalIgnoreCase));
                        if (sourceList != null)
                        {
                            listGuid = sourceList.Id;
                            webId = web.Id.ToString();
                        }
                    }
                    if (listGuid == Guid.Empty || string.IsNullOrEmpty(webId)) continue;

                    var existingFieldElement = XElement.Parse(field.SchemaXml);

                    if (existingFieldElement.Attribute("List") == null)
                    {
                        existingFieldElement.Add(new XAttribute("List", listGuid.ToString()));
                    }
                    else
                    {
                        existingFieldElement.Attribute("List").SetValue(listGuid.ToString());
                    }

                    if (!string.IsNullOrEmpty(webId))
                    {
                        if (existingFieldElement.Attribute("WebId") == null)
                        {
                            existingFieldElement.Add(new XAttribute("WebId", webId));
                        }
                        else
                        {
                            existingFieldElement.Attribute("WebId").SetValue(webId);
                        }
                    }

                    field.SchemaXml = existingFieldElement.ToString();

                    field.Update();
                    web.Context.ExecuteQueryRetry();
                }
            }
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
