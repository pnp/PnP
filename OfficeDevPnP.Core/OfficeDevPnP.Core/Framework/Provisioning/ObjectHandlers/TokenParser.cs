using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OfficeDevPnP.Core.Framework.ObjectHandlers.TokenDefinitions;

namespace OfficeDevPnP.Core.Framework.ObjectHandlers
{
    public class TokenParser
    {
        private Web _web;
        private List<TokenDefinition> _tokens = new List<TokenDefinition>();

        public TokenParser(Web web)
        {
            _web = web;

          
            _tokens.Add(new SiteCollectionToken(web));
            _tokens.Add(new SiteToken(web));
            _tokens.Add(new MasterPageCatalogToken(web));
            _tokens.Add(new SiteCollectionTermStoreIdToken(web));
            _tokens.Add(new ThemeCatalogToken(web));

            // ORDER IS IMPORTANT!
            var sortedTokens = from t in _tokens 
                               orderby t.GetTokenLength() descending 
                               select t;

            _tokens = sortedTokens.ToList();
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
