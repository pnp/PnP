using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.UPAWebService;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    internal class SiteToTemplateConversion
    {
        /// <summary>
        /// Actual implementation of extracting configuration from existing site.
        /// </summary>
        /// <param name="web"></param>
        /// <param name="baseTemplate"></param>
        /// <returns></returns>
        internal ProvisioningTemplate GetRemoteTemplate(Web web, ProvisioningTemplateCreationInformation creationInfo)
        {
            // Create empty object
            ProvisioningTemplate template = new ProvisioningTemplate();

            // Hookup connector, is handy when the generated template object is used to apply to another site
            template.Connector = creationInfo.FileConnector;

            // Get Security
            template = new ObjectSiteSecurity().CreateEntities(web, template, creationInfo);
            // Get TermGroups
            template = new ObjectTermGroups().CreateEntities(web, template, creationInfo);
            // Site Fields
            template = new ObjectField().CreateEntities(web, template, creationInfo);
            // Content Types
            template = new ObjectContentType().CreateEntities(web, template, creationInfo);
            // Get Lists 
            template = new ObjectListInstance().CreateEntities(web, template, creationInfo);
            // Get custom actions
            template = new ObjectCustomActions().CreateEntities(web, template, creationInfo);
            // Get features
            template = new ObjectFeatures().CreateEntities(web, template, creationInfo);
            // Get composite look
            template = new ObjectComposedLook().CreateEntities(web, template, creationInfo);
            // Get files
            template = new ObjectFiles().CreateEntities(web, template, creationInfo);
            // Get Pages
            template = new ObjectPages().CreateEntities(web, template, creationInfo);
            // Get Property Bag Entries
            template = new ObjectPropertyBagEntry().CreateEntities(web, template, creationInfo);
            // In future we could just instantiate all objects which are inherited from object handler base dynamically 

            // Set default values for Template ID and Version
            template.ID = String.Format("TEMPLATE-{0:N}", Guid.NewGuid()).ToUpper();
            template.Version = 1;

            // Retrieve original Template ID and remove it from Property Bag Entries
            int provisioningTemplateIdIndex = template.PropertyBagEntries.FindIndex(f => f.Key.Equals("_PnP_ProvisioningTemplateId"));
            if (provisioningTemplateIdIndex > -1)
            {
                var templateId = template.PropertyBagEntries[provisioningTemplateIdIndex].Value;
                if (!String.IsNullOrEmpty(templateId))
                {
                    template.ID = templateId;
                }
                template.PropertyBagEntries.RemoveAt(provisioningTemplateIdIndex);
            }

            // Retrieve original Template Info and remove it from Property Bag Entries
            int provisioningTemplateInfoIndex = template.PropertyBagEntries.FindIndex(f => f.Key.Equals("_PnP_ProvisioningTemplateInfo"));
            if (provisioningTemplateInfoIndex > -1)
            {
                var jsonInfo = template.PropertyBagEntries[provisioningTemplateInfoIndex].Value;

                var s = new JavaScriptSerializer();
                ProvisioningTemplateInfo info = s.Deserialize<ProvisioningTemplateInfo>(jsonInfo);

                // Override any previously defined Template ID, Version, and SitePolicy
                // with the one stored in the Template Info, if any
                if (!String.IsNullOrEmpty(info.TemplateID))
                {
                    template.ID = info.TemplateID;
                }
                if (!String.IsNullOrEmpty(info.TemplateSitePolicy))
                {
                    template.SitePolicy = info.TemplateSitePolicy;
                }
                if (info.TemplateVersion > 0)
                {
                    template.Version = info.TemplateVersion;
                }

                template.PropertyBagEntries.RemoveAt(provisioningTemplateInfoIndex);
            }

            return template;
        }

        /// <summary>
        /// Actual implementation of the apply templates
        /// </summary>
        /// <param name="web"></param>
        /// <param name="template"></param>
        internal void ApplyRemoteTemplate(Web web, ProvisioningTemplate template)
        {
            TokenParser.Initialize(web, template);
            // Site Security
            new ObjectSiteSecurity().ProvisionObjects(web, template);

            // Features
            new ObjectFeatures().ProvisionObjects(web, template);

            // TermGroups
            new ObjectTermGroups().ProvisionObjects(web, template);

            // Site Fields
            new ObjectField().ProvisionObjects(web, template);

            // Content Types
            new ObjectContentType().ProvisionObjects(web, template);

            // Lists
            new ObjectListInstance().ProvisionObjects(web, template);

            // During the processing flow fiels which refer to to be created lists might be created
            // These fields will be created initially without a reference to the actual list
            // and then hooked up to the corresponding source list in the following method
            ProcessLookupFields(web, template);

            // Files
            new ObjectFiles().ProvisionObjects(web, template);

            // Pages
            new ObjectPages().ProvisionObjects(web, template);

            // Custom actions
            new ObjectCustomActions().ProvisionObjects(web, template);

            // Composite look 
            new ObjectComposedLook().ProvisionObjects(web, template);

            // Property Bag Entries
            new ObjectPropertyBagEntry().ProvisionObjects(web, template);

            // Extensibility Provider CallOut the last thing we do.
            new ObjectExtensibilityProviders().ProvisionObjects(web, template);

            web.SetPropertyBagValue("_PnP_ProvisioningTemplateId", template.ID != null ? template.ID : "");
            web.AddIndexedPropertyBagKey("_PnP_ProvisioningTemplateId");

            ProvisioningTemplateInfo info = new ProvisioningTemplateInfo();
            info.TemplateID = template.ID != null ? template.ID : "";
            info.TemplateVersion = template.Version;
            info.TemplateSitePolicy = template.SitePolicy;
            info.Result = true;
            info.ProvisioningTime = DateTime.Now;

            var s = new JavaScriptSerializer();
            string jsonInfo = s.Serialize(info);

            web.SetPropertyBagValue("_PnP_ProvisioningTemplateInfo", jsonInfo);
        }

        private void ProcessLookupFields(Web web, ProvisioningTemplate template)
        {
            web.Context.Load(web.Lists, lists => lists.Include(l => l.Id, l => l.RootFolder.ServerRelativeUrl, l => l.Fields));
            web.Context.ExecuteQueryRetry();

            foreach (var siteField in template.SiteFields)
            {
                var fieldElement = XElement.Parse(siteField.SchemaXml);

                if (fieldElement.Attribute("List") != null)
                {
                    var fieldId = Guid.Parse(fieldElement.Attribute("ID").Value);
                    var listIdentifier = fieldElement.Attribute("List").Value;

                    var field = web.Fields.GetById(fieldId);
                    web.Context.Load(field, f => f.SchemaXml);
                    web.Context.ExecuteQueryRetry();

                    var listGuid = Guid.Empty;
                    if (!Guid.TryParse(listIdentifier, out listGuid))
                    {
                        var sourceListUrl = UrlUtility.Combine(web.ServerRelativeUrl, listIdentifier.ToParsedString());
                        var sourceList = web.Lists.FirstOrDefault(l => l.RootFolder.ServerRelativeUrl.Equals(sourceListUrl, StringComparison.OrdinalIgnoreCase));
                        if (sourceList != null)
                        {
                            listGuid = sourceList.Id;
                        }
                    }
                    if (listGuid != Guid.Empty)
                    {
                        var existingFieldElement = XElement.Parse(field.SchemaXml);

                        if (existingFieldElement.Attribute("List") == null)
                        {
                            existingFieldElement.Add(new XAttribute("List", listGuid.ToString()));
                        }
                        else
                        {
                            existingFieldElement.Attribute("List").SetValue(listGuid.ToString());
                        }
                        field.SchemaXml = existingFieldElement.ToString();

                        field.UpdateAndPushChanges(true);
                        web.Context.ExecuteQueryRetry();
                    }
                }
            }



            foreach (var listInstance in template.Lists)
            {
                foreach (var listField in listInstance.Fields)
                {
                    var fieldElement = XElement.Parse(listField.SchemaXml);
                    if (fieldElement.Attribute("List") != null)
                    {
                        var fieldId = Guid.Parse(fieldElement.Attribute("ID").Value);
                        var listIdentifier = fieldElement.Attribute("List").Value;

                        var listUrl = UrlUtility.Combine(web.ServerRelativeUrl, listInstance.Url.ToParsedString());

                        var createdList = web.Lists.FirstOrDefault(l => l.RootFolder.ServerRelativeUrl.Equals(listUrl, StringComparison.OrdinalIgnoreCase));

                        if (createdList != null)
                        {
                            var field = createdList.Fields.GetById(fieldId);
                            web.Context.Load(field, f => f.SchemaXml);
                            web.Context.ExecuteQueryRetry();

                            var listGuid = Guid.Empty;
                            if (!Guid.TryParse(listIdentifier, out listGuid))
                            {
                                var sourceListUrl = UrlUtility.Combine(web.ServerRelativeUrl, listIdentifier.ToParsedString());
                                var sourceList = web.Lists.FirstOrDefault(l => l.RootFolder.ServerRelativeUrl.Equals(sourceListUrl, StringComparison.OrdinalIgnoreCase));
                                if (sourceList != null)
                                {
                                    listGuid = sourceList.Id;
                                }
                            }
                            if (listGuid != Guid.Empty)
                            {
                                var existingFieldElement = XElement.Parse(field.SchemaXml);

                                if (existingFieldElement.Attribute("List") == null)
                                {
                                    existingFieldElement.Add(new XAttribute("List", listGuid.ToString()));
                                }
                                else
                                {
                                    existingFieldElement.Attribute("List").SetValue(listGuid.ToString());
                                }
                                field.SchemaXml = existingFieldElement.ToString();

                                field.Update();
                                web.Context.ExecuteQueryRetry();
                            }
                        }
                    }
                }
            }
        }
    }
}
