using System;
using System.Linq;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Extensibility;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using Microsoft.SharePoint.Client.Publishing.Navigation;
using Microsoft.SharePoint.Client.Taxonomy;
using SP = Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using System.Web;

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
            if (config.Items != null && config.Items.Length > 0)
            {
                var navRoot = ctx.Web.Navigation.GetNodeById(config.RootNodeId);
                var navNodes = navRoot.Children;
                ctx.Load(navNodes);
                ctx.ExecuteQuery();

                //resolve node URls
                foreach (var node in config.Items)
                {
                    node.Url = ResolveNodeUrl(node.Url);
                }

                //exclude existing nodes
                var existingNodes = navNodes.ToList();
                var newNodes = config.Items.Where(n => existingNodes.Find(e => AreEqualNodes(e, n)) == null);
                if (newNodes.Count() > 0)
                {
                    //add nodes from config
                    foreach (var item in newNodes)
                    {
                        navNodes.Add(new NavigationNodeCreationInformation
                        {
                            Title = item.Title,
                            Url = item.Url,
                            IsExternal = item.IsExternal,
                            AsLastNode = true
                        });
                    }
                    ctx.ExecuteQuery();
                }
            }
        }

        private string ResolveNodeUrl(string tokenizedUrl)
        {
            string res = tokenizedUrl;
            if (!string.IsNullOrEmpty(res))
            {
                res = res.ToParsedString();
                if (res == string.Empty)
                {
                    res = "/";
                }
            }
            return res;
        }

        private bool AreEqualUrls(string urlA, string urlB)
        {
            bool res = false;
            if (!string.IsNullOrEmpty(urlA) && !string.IsNullOrEmpty(urlB))
            {
                Uri uriA = new Uri(HttpUtility.UrlDecode(urlA), UriKind.RelativeOrAbsolute);
                Uri uriB = new Uri(HttpUtility.UrlDecode(urlB), UriKind.RelativeOrAbsolute);
                res = Uri.Equals(uriA, uriB);
            }
            else
            {
                res = urlA == urlB;
            }
            return res;
        }

        private bool AreEqualNodes(NavigationNode nodeA, NavigationProvisionSchema.NavigationNode nodeB)
        {
            return string.Equals(nodeA.Title, nodeB.Title) && AreEqualUrls(nodeA.Url, nodeB.Url);
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
