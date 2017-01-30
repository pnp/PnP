using OfficeDevPnP.Core.Framework.Provisioning.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Diagnostics;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers.TokenDefinitions;
using System.Xml.Linq;
using OfficeDevPnP.Core.Entities;

namespace Intranet.Providers
{
    public class DefaultColumnValues : IProvisioningExtensibilityHandler
    {
        public ProvisioningTemplate Extract(ClientContext ctx, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInformation, PnPMonitoredScope scope, string configurationData)
        {
            return null;
        }

        public IEnumerable<TokenDefinition> GetTokens(ClientContext ctx, ProvisioningTemplate template, string configurationData)
        {
            return null;
        }

        public void Provision(ClientContext ctx, ProvisioningTemplate template, ProvisioningTemplateApplyingInformation applyingInformation, TokenParser tokenParser, PnPMonitoredScope scope, string configurationData)
        {
            if (!string.IsNullOrEmpty(configurationData))
            {
                // Get the current web 
                var web = ctx.Web;

                // Read configuration data from the template
                var configuration = XDocument.Parse(configurationData);
                var ns = configuration.Root.GetDefaultNamespace();

                var libraries = configuration.Descendants(ns + "Library");

                foreach (var library in libraries)
                {
                    var libraryTitle = library.Attribute("Title").Value;

                    //Get the library
                    List list = ctx.Web.Lists.GetByTitle(libraryTitle);

                    if (list != null)
                    {
                        var items = library.Descendants(ns + "Default");

                        foreach (var item in items)
                        {
                            // Get configuration infos
                            var fieldName = item.Attribute("InternalName").Value;
                            var fieldValue = item.Attribute("Value").Value;
                            var folder = item.Attribute("Folder").Value;

                            // Get the field
                            var field = list.Fields.GetByInternalNameOrTitle(fieldName);
                            ctx.Load(field, f => f.InternalName, f => f.TypeAsString);
                            ctx.ExecuteQueryRetry();

                            if (field != null)
                            {
                                IDefaultColumnValue defaultColumnValue = null;
                                if (field.TypeAsString == "Text")
                                {
                                    var values = string.Join(";", fieldValue);
                                    defaultColumnValue = new DefaultColumnTextValue()
                                    {
                                        FieldInternalName = field.InternalName,
                                        FolderRelativePath = folder,
                                        Text = values
                                    };
                                }
                                else
                                {
                                    var terms = new List<Microsoft.SharePoint.Client.Taxonomy.Term>();
                                    var values = fieldValue.Split(';');

                                    foreach (var termString in values)
                                    {
                                        var term = ctx.Site.GetTaxonomyItemByPath(termString);
                                        if (term != null)
                                        {
                                            terms.Add(term as Microsoft.SharePoint.Client.Taxonomy.Term);
                                        }
                                    }
                                    if (terms.Any())
                                    {
                                        defaultColumnValue = new DefaultColumnTermValue()
                                        {
                                            FieldInternalName = field.InternalName,
                                            FolderRelativePath = folder,
                                        };
                                        terms.ForEach(t => ((DefaultColumnTermValue)defaultColumnValue).Terms.Add(t));
                                    }
                                }

                                if (defaultColumnValue != null)
                                {
                                    list.SetDefaultColumnValues(new List<IDefaultColumnValue>() { defaultColumnValue });
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
