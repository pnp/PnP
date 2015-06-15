using System;
using System.Collections.Generic;
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
    internal class ObjectListInstanceDataRows : ObjectHandlerBase
    {

        public override string Name
        {
            get { return "List instances Data Rows"; }
        }
        public override void ProvisionObjects(Web web, ProvisioningTemplate template)
        {
            Log.Info(Constants.LOGGING_SOURCE_FRAMEWORK_PROVISIONING, CoreResources.Provisioning_ObjectHandlers_ListInstancesDataRows);

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

                #region DataRows

                foreach (var listInstance in template.Lists)
                {
                    if (listInstance.DataRows != null && listInstance.DataRows.Any())
                    {
                        // Retrieve the target list
                        var list = web.Lists.GetByTitle(listInstance.Title);
                        web.Context.Load(list);

                        // Retrieve the fields' types from the list
                        FieldCollection fields = list.Fields;
                        web.Context.Load(fields, fs => fs.Include(f => f.InternalName, f => f.FieldTypeKind));
                        web.Context.ExecuteQueryRetry();

                        foreach (var dataRow in listInstance.DataRows)
                        {
                            var listitemCI = new ListItemCreationInformation();
                            var listitem = list.AddItem(listitemCI);

                            foreach (var dataValue in dataRow.Values)
                            {
                                Field dataField = fields.FirstOrDefault(
                                    f => f.InternalName == dataValue.Key.ToParsedString());

                                if (dataField != null)
                                {
                                    String fieldValue = dataValue.Value.ToParsedString();

                                    switch (dataField.FieldTypeKind)
                                    {
                                        case FieldType.Geolocation:
                                            // FieldGeolocationValue - Expected format: Altitude,Latitude,Longitude,Measure
                                            var geolocationArray = fieldValue.Split(',');
                                            if (geolocationArray.Length == 4)
                                            {
                                                var geolocationValue = new FieldGeolocationValue
                                                {
                                                    Altitude = Double.Parse(geolocationArray[0]),
                                                    Latitude = Double.Parse(geolocationArray[1]),
                                                    Longitude = Double.Parse(geolocationArray[2]),
                                                    Measure = Double.Parse(geolocationArray[3]),
                                                };
                                                listitem[dataValue.Key.ToParsedString()] = geolocationValue;
                                            }
                                            else
                                            {
                                                listitem[dataValue.Key.ToParsedString()] = fieldValue;
                                            }
                                            break;
                                        case FieldType.Lookup:
                                            // FieldLookupValue - Expected format: LookupID
                                            var lookupValue = new FieldLookupValue
                                            {
                                                LookupId = Int32.Parse(fieldValue),
                                            };
                                            listitem[dataValue.Key.ToParsedString()] = lookupValue;
                                            break;
                                        case FieldType.URL:
                                            // FieldUrlValue - Expected format: URL,Description
                                            var urlArray = fieldValue.Split(',');
                                            var linkValue = new FieldUrlValue();
                                            if (urlArray.Length == 2)
                                            {
                                                linkValue.Url = urlArray[0];
                                                linkValue.Description = urlArray[1];
                                            }
                                            else
                                            {
                                                linkValue.Url = urlArray[0];
                                                linkValue.Description = urlArray[0];
                                            }
                                            listitem[dataValue.Key.ToParsedString()] = linkValue;
                                            break;
                                        case FieldType.User:
                                            // FieldUserValue - Expected format: loginName
                                            var user = web.EnsureUser(fieldValue);
                                            web.Context.Load(user);
                                            web.Context.ExecuteQueryRetry();

                                            if (user != null)
                                            {
                                                var userValue = new FieldUserValue
                                                {
                                                    LookupId = user.Id,
                                                };
                                                listitem[dataValue.Key.ToParsedString()] = userValue;
                                            }
                                            else
                                            {
                                                listitem[dataValue.Key.ToParsedString()] = fieldValue;
                                            }
                                            break;
                                        default:
                                            listitem[dataValue.Key.ToParsedString()] = fieldValue;
                                            break;
                                    }
                                }
                                listitem.Update();
                            }
                            web.Context.ExecuteQueryRetry(); // TODO: Run in batches?
                        }
                    }
                }
                #endregion
            }
        }

        public override ProvisioningTemplate CreateEntities(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            return template;
        }

        public override bool WillProvision(Web web, ProvisioningTemplate template)
        {
            if (!_willProvision.HasValue)
            {
                _willProvision = template.Lists.Any(l => l.DataRows.Any());
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

