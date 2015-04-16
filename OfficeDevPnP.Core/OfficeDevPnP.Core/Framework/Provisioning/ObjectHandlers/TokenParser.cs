using System.Collections.Generic;
using System.Linq;
using System.Web.Management;
using Microsoft.IdentityModel.Protocols.WSIdentity;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.ObjectHandlers.TokenDefinitions;
using OfficeDevPnP.Core.Framework.Provisioning.Model;

namespace OfficeDevPnP.Core.Framework.ObjectHandlers
{
    public class TokenParser
    {
        private Web _web;
        private List<TokenDefinition> _tokens = new List<TokenDefinition>();

        public List<TokenDefinition> Tokens
        {
            get {  return _tokens;}
            private set
            {
                _tokens = value;
            }
        }

        public void AddToken(TokenDefinition tokenDefinition)
        {
            
            this.Tokens.Add(tokenDefinition);
             // ORDER IS IMPORTANT!
            var sortedTokens = from t in _tokens
                               orderby t.GetTokenLength() descending
                               select t;

            this.Tokens = sortedTokens.ToList();
        }

        public TokenParser(Web web, ProvisioningTemplate template )
        {
            if (!web.IsPropertyAvailable("ServerRelativeUrl"))
            {
                web.Context.Load(web, w => w.ServerRelativeUrl);
                web.Context.ExecuteQueryRetry();
            }
            _web = web;

            this.Tokens = new List<TokenDefinition>();

            this.Tokens.Add(new SiteCollectionToken(web));
            this.Tokens.Add(new SiteToken(web));
            this.Tokens.Add(new MasterPageCatalogToken(web));
            this.Tokens.Add(new SiteCollectionTermStoreIdToken(web));
            this.Tokens.Add(new KeywordsTermStoreIdToken(web));
            this.Tokens.Add(new ThemeCatalogToken(web));

            // Add lists
            web.Context.Load(web.Lists, ls => ls.Include(l => l.Id, l => l.Title, l => l.RootFolder.ServerRelativeUrl));
            web.Context.ExecuteQueryRetry();
            foreach (var list in web.Lists)
            {
                this.Tokens.Add(new ListIdToken(web, list.Title, list.Id));
                this.Tokens.Add(new ListUrlToken(web, list.Title, list.RootFolder.ServerRelativeUrl.Substring(web.ServerRelativeUrl.Length+1)));
            }

            // Add parameters
            foreach (var parameter in template.Parameters)
            {
                this.Tokens.Add(new ParameterToken(web, parameter.Key,parameter.Value));
            }

            var sortedTokens = from t in _tokens
                               orderby t.GetTokenLength() descending
                               select t;

            this.Tokens = sortedTokens.ToList();
        }

        public void Rebase(Web web)
        {
            _web = web;

            foreach (var token in this.Tokens)
            {
                token.ClearCache();
                token.Web = web;
            }
        }

        public string Parse(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                foreach (var token in _tokens)
                {
                    foreach (var regex in token.GetRegex())
                    {
                        if (regex.IsMatch(input))
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
