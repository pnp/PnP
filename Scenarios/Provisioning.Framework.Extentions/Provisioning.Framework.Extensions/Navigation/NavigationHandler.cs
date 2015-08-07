using System;
using System.Linq;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Extensibility;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using Microsoft.SharePoint.Client.Publishing.Navigation;
using Microsoft.SharePoint.Client.Taxonomy;
using SP = Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;

namespace Provisioning.Framework.Extensions
{
    class NavigationHandler : IProvisioningExtensibilityProvider
    {
        public void ProcessRequest(ClientContext ctx, ProvisioningTemplate template, string configurationData)
        {
            var config = !string.IsNullOrEmpty(configurationData) ? XmlHelper.ReadXmlString<NavigationProvisionSchema.NavigationList>(configurationData) : null;
            if (config != null && config.Navigation != null && config.Navigation.Length > 0)
            {
                foreach (var nav in config.Navigation)
                {
                    if (nav.Type == NavigationProvisionSchema.NavigationType.Structural)
                    {
                        ProvisionStructuralNavigation(ctx, nav);
                    }
                    else if (nav.Type == NavigationProvisionSchema.NavigationType.Taxonomy)
                    {
                        ProvisionManagedNavigation(ctx, nav);
                    }
                }
            }
        }

        private void ProvisionStructuralNavigation(ClientContext ctx, NavigationProvisionSchema.NavigationConfiguration config)
        {
            var navRoot = ctx.Web.Navigation.GetNodeById(config.RootNodeId);
            var navNodes = navRoot.Children;
            ctx.Load(navNodes);
            ctx.ExecuteQuery();

            //delete existing nodes
            navNodes.ToList().ForEach(node => node.DeleteObject());

            //add nodes from config
            foreach (var item in config.Items)
            {
                navNodes.Add(new NavigationNodeCreationInformation
                {
                    Title = item.Title,
                    Url = item.Url.ToParsedString(),
                    IsExternal = item.IsExternal,
                    AsLastNode = true
                });
            }
            ctx.ExecuteQuery();
        }


        // TODO : parameter validation
        private void ProvisionManagedNavigation(ClientContext ctx, NavigationProvisionSchema.NavigationConfiguration config)
        {
            var cachedNavTermSet = TaxonomyNavigation.GetTermSetForWeb(ctx, ctx.Web, config.SiteMapProvider, true);
            var session = TaxonomySession.GetTaxonomySession(ctx);

            var navTermSet = cachedNavTermSet.GetAsEditable(session);
            ctx.Load(session);
            ctx.ExecuteQuery();
            ctx.Load(navTermSet, n => n.Terms);
            ctx.ExecuteQuery();

            navTermSet.Terms.ToList().ForEach(term => term.DeleteObject());

            foreach (var item in config.Items)
            {
                NavigationTerm term = navTermSet.CreateTerm(item.Title, NavigationLinkType.SimpleLink, Guid.NewGuid());
                term.SimpleLinkUrl = item.Url;
            }
            navTermSet.GetTaxonomyTermStore().CommitAll();
            ctx.ExecuteQuery();
        }
    }
}
