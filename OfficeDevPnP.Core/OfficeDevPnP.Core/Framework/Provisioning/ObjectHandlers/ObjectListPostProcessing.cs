using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Utilities;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public class ObjectListPostProcessing : ObjectHandlerBase
    {
        public override string Name
        {
            get { return "List Post Processing"; }
        }

        public ObjectListPostProcessing()
        {
            this.ReportProgress = false;
        }

        public override void ProvisionObjects(Microsoft.SharePoint.Client.Web web, Model.ProvisioningTemplate template)
        {
            Log.Info(Constants.LOGGING_SOURCE_FRAMEWORK_PROVISIONING, CoreResources.Provisioning_ObjectHandlers_ListPostProcess);

            ProcessLookupFields(web, template);
            ProcessListData(web, template);
        }

        public override Model.ProvisioningTemplate CreateEntities(Microsoft.SharePoint.Client.Web web, Model.ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            return template;
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

        public void ProcessListData(Web web, ProvisioningTemplate template)
        {

            if (template.Lists.Any(l => l.DataRows.Any()))
            {
                if (!web.IsPropertyAvailable("ServerRelativeUrl"))
                {
                    web.Context.Load(web, w => w.ServerRelativeUrl);
                    web.Context.ExecuteQueryRetry();
                }

                web.Context.Load(web.Lists, lc => lc.IncludeWithDefaultProperties(l => l.RootFolder.ServerRelativeUrl));
                web.Context.ExecuteQueryRetry();
                var existingLists = web.Lists.AsEnumerable<List>().Select(existingList => existingList.RootFolder.ServerRelativeUrl).ToList();
                var serverRelativeUrl = web.ServerRelativeUrl;

                foreach (var listInstance in template.Lists.Where(l => l.DataRows.Any()))
                {
                    if (!existingLists.Contains(UrlUtility.Combine(serverRelativeUrl, listInstance.Url)))
                    {
                        continue;
                    }

                    var list = web.GetListByUrl(UrlUtility.Combine(serverRelativeUrl, listInstance.Url));
                    web.Context.Load(list, l => l.ItemCount);
                    web.Context.ExecuteQueryRetry();

                    //Can include DataRows in main template or separate template for organizational purposes
                    if (list.ItemCount == 0)
                    {
                        foreach (var dataRow in listInstance.DataRows)
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

            }
        }
    }
}
