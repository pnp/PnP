using Microsoft.SharePoint.Client;
using System;

namespace OfficeDevPnP.Core.Framework.ObjectHandlers.TokenDefinitions
{
    public class TermSetIdToken : TokenDefinition
    {
        private readonly string _value = null;
        public TermSetIdToken(Web web, string groupName, string termsetName, Guid id)
            : base(web, string.Format("{{termsetid:{0}:{1}}}", groupName, termsetName))
        {
            _value = id.ToString();
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