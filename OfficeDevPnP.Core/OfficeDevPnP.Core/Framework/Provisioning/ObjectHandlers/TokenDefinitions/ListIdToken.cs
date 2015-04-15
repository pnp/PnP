using Microsoft.SharePoint.Client;
using System;

namespace OfficeDevPnP.Core.Framework.ObjectHandlers.TokenDefinitions
{
    public class ListIdToken : TokenDefinition
    {
        private string _listId = null;
        public ListIdToken(Web web, string name, Guid listid)
            : base(web, string.Format("{{listid:{0}}}", name))
        {
            _listId = listid.ToString();
        }

        public override string GetReplaceValue()
        {
            if (string.IsNullOrEmpty(CacheValue))
            {
                CacheValue = _listId;
            }
            return CacheValue;
        }
    }
}