using Microsoft.SharePoint.Client;
using System;

namespace OfficeDevPnP.Core.Framework.ObjectHandlers.TokenDefinitions
{
    public class ParameterToken : TokenDefinition
    {
        private readonly string _value = null;
        public ParameterToken(Web web, string name, string value)
            : base(web, string.Format("{{parameter:{0}}}", name), string.Format("{{\\${0}}}", name))
        {
            _value = value;
        }

        public override string GetReplaceValue()
        {
            if (string.IsNullOrEmpty(CacheValue))
            {
                CacheValue = _value;
            }
            return CacheValue;
        }
    }
}