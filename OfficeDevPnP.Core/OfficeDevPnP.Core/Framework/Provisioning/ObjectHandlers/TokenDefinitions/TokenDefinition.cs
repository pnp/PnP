using System.Text.RegularExpressions;
using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.Core.Framework.ObjectHandlers
{
    public abstract class TokenDefinition
    {

        protected TokenDefinition(Web web, string token)
        {
            this.Token = token;
            this.Web = web;
        }
        
        public string Token { get; private set; }
        public Web Web { get; private set; }

        public Regex Regex
        {
            get { return new Regex(Token); }
        }

        public abstract string GetReplaceValue();

    }
}