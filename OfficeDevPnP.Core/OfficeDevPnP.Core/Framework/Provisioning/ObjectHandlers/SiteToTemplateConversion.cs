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
using OfficeDevPnP.Core.Utilities;

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
            var progressDelegate = creationInfo.ProgressDelegate;

            if (progressDelegate != null) progressDelegate("Start", 0, 0);
            // Create empty object
            ProvisioningTemplate template = new ProvisioningTemplate();

            // Hookup connector, is handy when the generated template object is used to apply to another site
            template.Connector = creationInfo.FileConnector;

            List<ObjectHandlerBase> objectHandlers = new List<ObjectHandlerBase>();

            objectHandlers.Add(new ObjectSiteSecurity());
            objectHandlers.Add(new ObjectTermGroups());
            objectHandlers.Add(new ObjectField());
            objectHandlers.Add(new ObjectContentType());
            objectHandlers.Add(new ObjectListInstance());
            objectHandlers.Add(new ObjectCustomActions());
            objectHandlers.Add(new ObjectFeatures());
            objectHandlers.Add(new ObjectComposedLook());
            objectHandlers.Add(new ObjectFiles());
            objectHandlers.Add(new ObjectPages());
            objectHandlers.Add(new ObjectPropertyBagEntry());
            objectHandlers.Add(new ObjectRetrieveTemplateInfo());

            int step = 1;

            var count = objectHandlers.Count(o => o.ReportProgress);

            foreach (var handler in objectHandlers)
            {
                if (handler.ReportProgress && progressDelegate != null)
                {
                    progressDelegate(handler.Name, step, count);
                    step++;
                }
                template = handler.CreateEntities(web, template, creationInfo);
            }

            return template;
        }

        /// <summary>
        /// Actual implementation of the apply templates
        /// </summary>
        /// <param name="web"></param>
        /// <param name="template"></param>
        internal void ApplyRemoteTemplate(Web web, ProvisioningTemplate template, ProvisioningTemplateApplyingInformation provisioningInfo)
        {
            var progressDelegate = provisioningInfo.ProgressDelegate;

            Log.Info(Constants.LOGGING_SOURCE_FRAMEWORK_PROVISIONING, "START - Provisioning");

            List<ObjectHandlerBase> objectHandlers = new List<ObjectHandlerBase>();

            objectHandlers.Add(new ObjectSiteSecurity());
            objectHandlers.Add(new ObjectFeatures());
            objectHandlers.Add(new ObjectTermGroups());
            objectHandlers.Add(new ObjectField());
            objectHandlers.Add(new ObjectContentType());
            objectHandlers.Add(new ObjectListInstance());
            objectHandlers.Add(new ObjectLookupFields());
            objectHandlers.Add(new ObjectFiles());
            objectHandlers.Add(new ObjectPages());
            objectHandlers.Add(new ObjectCustomActions());
            objectHandlers.Add(new ObjectComposedLook());
            objectHandlers.Add(new ObjectPropertyBagEntry());
            objectHandlers.Add(new ObjectExtensibilityProviders());
            objectHandlers.Add(new ObjectPersistTemplateInfo());

            TokenParser.Initialize(web, template);

            int step = 1;

            var count = objectHandlers.Count(o => o.ReportProgress);

            foreach (var handler in objectHandlers)
            {
                if (handler.ReportProgress && progressDelegate != null)
                {
                    progressDelegate(handler.Name, step, count);
                    step++;
                }
                handler.ProvisionObjects(web, template);
            }
            
            //// Site Security
            //if (progressDelegate != null)  progressDelegate("Site Security", 2, steps);
            //new ObjectSiteSecurity().ProvisionObjects(web, template);

            //// Features
            //if (progressDelegate != null) progressDelegate("Features", 3, steps);
            //new ObjectFeatures().ProvisionObjects(web, template);

            //// TermGroups
            //if (progressDelegate != null) progressDelegate("Termgroups", 4, steps);
            //new ObjectTermGroups().ProvisionObjects(web, template);

            //// Site Fields
            //if (progressDelegate != null) progressDelegate("Site fields", 5, steps);
            //new ObjectField().ProvisionObjects(web, template);

            //// Content Types
            //if (progressDelegate != null) progressDelegate("Content types", 6, steps);
            //new ObjectContentType().ProvisionObjects(web, template);

            //// Lists
            //if (progressDelegate != null) progressDelegate("List instances", 7, steps);
            //new ObjectListInstance().ProvisionObjects(web, template);

            //// During the processing flow fields which refer to to be created lists might be created
            //// These fields will be created initially without a reference to the actual list
            //// and then hooked up to the corresponding source list in the following method
            //ProcessLookupFields(web, template);

            //// Files
            //if (progressDelegate != null) progressDelegate("Files", 8, steps);
            //new ObjectFiles().ProvisionObjects(web, template);

            //// Pages
            //if (progressDelegate != null) progressDelegate("Pages", 9, steps);
            //new ObjectPages().ProvisionObjects(web, template);

            //// Custom actions
            //if (progressDelegate != null) progressDelegate("Custom actions", 10, steps);
            //new ObjectCustomActions().ProvisionObjects(web, template);

            //// Composite look 
            //if (progressDelegate != null) progressDelegate("Composed looks", 11, steps);
            //new ObjectComposedLook().ProvisionObjects(web, template);

            //// Property Bag Entries
            //if (progressDelegate != null) progressDelegate("Property bag entries", 12, steps);
            //new ObjectPropertyBagEntry().ProvisionObjects(web, template);

            //// Extensibility Provider CallOut the last thing we do.
            //if (progressDelegate != null) progressDelegate("Extensibility providers", 13, steps);
            //new ObjectExtensibilityProviders().ProvisionObjects(web, template);

            //web.SetPropertyBagValue("_PnP_ProvisioningTemplateId", template.Id != null ? template.Id : "");
            //web.AddIndexedPropertyBagKey("_PnP_ProvisioningTemplateId");

            //ProvisioningTemplateInfo info = new ProvisioningTemplateInfo();
            //info.TemplateId = template.Id != null ? template.Id : "";
            //info.TemplateVersion = template.Version;
            //info.TemplateSitePolicy = template.SitePolicy;
            //info.Result = true;
            //info.ProvisioningTime = DateTime.Now;

            //var s = new JavaScriptSerializer();
            //string jsonInfo = s.Serialize(info);

            //web.SetPropertyBagValue("_PnP_ProvisioningTemplateInfo", jsonInfo);

            //if (progressDelegate != null) progressDelegate("Finished", 14, steps);
            Log.Info(Constants.LOGGING_SOURCE_FRAMEWORK_PROVISIONING, "FINISH - Provisioning");
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
