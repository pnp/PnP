using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Management;
using Microsoft.IdentityModel.Protocols.WSIdentity;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using OfficeDevPnP.Core.Framework.ObjectHandlers;
using OfficeDevPnP.Core.Framework.ObjectHandlers.TokenDefinitions;
using OfficeDevPnP.Core.Framework.Provisioning.Model;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public static class TokenParser
    {
        public static Web _web;
        private static List<TokenDefinition> _tokens = new List<TokenDefinition>();

        public static List<TokenDefinition> Tokens
        {
            get { return _tokens; }
            private set
            {
                _tokens = value;
            }
        }

        public static void AddToken(TokenDefinition tokenDefinition)
        {

            _tokens.Add(tokenDefinition);
            // ORDER IS IMPORTANT!
            var sortedTokens = from t in _tokens
                               orderby t.GetTokenLength() descending
                               select t;

            _tokens = sortedTokens.ToList();
        }

        public static void Initialize(Web web, ProvisioningTemplate template)
        {
            if (!web.IsPropertyAvailable("ServerRelativeUrl"))
            {
                web.Context.Load(web, w => w.ServerRelativeUrl);
                web.Context.ExecuteQueryRetry();
            }
            _web = web;

            _tokens = new List<TokenDefinition>();

            _tokens.Add(new SiteCollectionToken(web));
            _tokens.Add(new SiteToken(web));
            _tokens.Add(new MasterPageCatalogToken(web));
            _tokens.Add(new SiteCollectionTermStoreIdToken(web));
            _tokens.Add(new KeywordsTermStoreIdToken(web));
            _tokens.Add(new ThemeCatalogToken(web));

            // Add lists
            web.Context.Load(web.Lists, ls => ls.Include(l => l.Id, l => l.Title, l => l.RootFolder.ServerRelativeUrl));
            web.Context.ExecuteQueryRetry();
            foreach (var list in web.Lists)
            {
                _tokens.Add(new ListIdToken(web, list.Title, list.Id));
                _tokens.Add(new ListUrlToken(web, list.Title, list.RootFolder.ServerRelativeUrl.Substring(web.ServerRelativeUrl.Length + 1)));
            }

            // Add parameters
            foreach (var parameter in template.Parameters)
            {
                _tokens.Add(new ParameterToken(web, parameter.Key, parameter.Value));
            }

            // Add TermSetIds
            TaxonomySession session = TaxonomySession.GetTaxonomySession(web.Context);
            var termStore = session.GetDefaultSiteCollectionTermStore();
            web.Context.Load(termStore);
            web.Context.ExecuteQueryRetry();
            if (!termStore.ServerObjectIsNull.Value)
            {
                web.Context.Load(termStore.Groups,
                    g => g.Include(
                        tg => tg.Name,
                        tg => tg.TermSets.Include(
                            ts => ts.Name,
                            ts => ts.Id)
                    ));
                web.Context.ExecuteQueryRetry();
                foreach (var termGroup in termStore.Groups)
                {
                    foreach (var termSet in termGroup.TermSets)
                    {
                        _tokens.Add(new TermSetIdToken(web, termGroup.Name, termSet.Name, termSet.Id));
                    }
                }
            }

            var sortedTokens = from t in _tokens
                               orderby t.GetTokenLength() descending
                               select t;

            _tokens = sortedTokens.ToList();
        }

        public static void Rebase(Web web)
        {
            _web = web;

            foreach (var token in _tokens)
            {
                token.ClearCache();
                token.Web = web;
            }
        }

        public static string ToParsedString(this string input)
        {
            return ToParsedString(input, null);
        }

        public static string ToParsedString(this string input, params string[] tokensToSkip)
        {
            if (!string.IsNullOrEmpty(input))
            {
                foreach (var token in _tokens)
                {
                    if (tokensToSkip != null)
                    {
                        if (token.GetTokens().Except(tokensToSkip, StringComparer.InvariantCultureIgnoreCase).Any())
                        {
                            foreach (var regex in token.GetRegex().Where(regex => regex.IsMatch(input)))
                            {
                                input = regex.Replace(input, token.GetReplaceValue());
                            }
                        }
                    }
                    else
                    {
                        foreach (var regex in token.GetRegex().Where(regex => regex.IsMatch(input)))
                        {
                            input = regex.Replace(input, token.GetReplaceValue());
                        }
                    }
                }
            }
            return input;
        }
    }
}
