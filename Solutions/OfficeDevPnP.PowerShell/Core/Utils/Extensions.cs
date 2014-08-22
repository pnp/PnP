using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.PowerShell.Core.Utils
{
    public static class Extensions
    {
        public static T Load<T>(this T collection) where T : ClientObjectCollection
        {
            if (collection.ServerObjectIsNull == null || collection.ServerObjectIsNull == true)
            {
                collection.Context.Load(collection);
                collection.Context.ExecuteQuery();
                return collection;
            }
            else
            {
                return collection;
            }
        }
    }
}
