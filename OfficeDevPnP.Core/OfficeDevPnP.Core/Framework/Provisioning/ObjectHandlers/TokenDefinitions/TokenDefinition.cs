using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.Core.Framework.ObjectHandlers
{
    public abstract class TokenDefinition
    {
        protected string CacheValue;
        private readonly string[] _tokens;
     
        protected TokenDefinition(Web web, params string[] token)
        {
            this._tokens = token;
            this.Web = web;
        }

        public string[] GetTokens()
        {
            return _tokens;
        }

        // public string[] Token { get; private set; }
        public Web Web { get; set; }

        public Regex[] GetRegex()
        {
            var regexs = new Regex[this._tokens.Length];
            for (var q = 0; q < this._tokens.Length; q++)
            {
                regexs[q] = new Regex(this._tokens[q], RegexOptions.IgnoreCase);
            }
            return regexs;
        }

        public Regex GetRegexForToken(string token)
        {
            return new Regex(token, RegexOptions.IgnoreCase);
        }

        public int GetTokenLength()
        {
            return _tokens.Select(t => t.Length).Concat(new[] { 0 }).Max();
        }

        public abstract string GetReplaceValue();

        public void ClearCache()
        {
            this.CacheValue = null;
        }
    }
}