using System.Text.RegularExpressions;
using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.Core.Framework.ObjectHandlers
{
    public abstract class TokenDefinition
    {
        protected string CacheValue;

        protected TokenDefinition(Web web, params string[] token)
        {
            this.Token = token;
            this.Web = web;
        }
        
        public string[] Token { get; private set; }
        public Web Web { get; private set; }

        public Regex[] Regex
        {
            get
            {
                var regexs = new Regex[this.Token.Length];
                for (var q = 0; q < this.Token.Length;q++)
                {
                    regexs[q] = new Regex(this.Token[q], RegexOptions.IgnoreCase);
                }
                return regexs;
            }
        }

        public abstract string GetReplaceValue();

    }
}