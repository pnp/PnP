using Microsoft.SharePoint.Client;
using System;

namespace OfficeDevPnP.Core.Framework.ObjectHandlers.TokenDefinitions
{
    public class ListUrlToken : TokenDefinition
    {
        private string _listUrl = null;
        public ListUrlToken(Web web, string name, string url)
            : base(web, string.Format("{{listurl:{0}}}", name))
        {
            _listUrl = url;
        }

        public override string GetReplaceValue()
        {
            if (string.IsNullOrEmpty(CacheValue))
            {
                CacheValue = _listUrl;
            }
            return CacheValue;
        }
    }
}