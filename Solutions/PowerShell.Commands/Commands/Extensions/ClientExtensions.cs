using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.PowerShell.Commands.Extensions
{
    public static class ClientExtensions
    {
        public static T Load<T>(this T collection) where T : ClientObjectCollection
        {
            if (collection.ServerObjectIsNull == null || collection.ServerObjectIsNull == true)
            {
                collection.Context.Load(collection);
                collection.Context.ExecuteQueryRetry();
                return collection;
            }
            else
            {
                return collection;
            }
        }
    }
}
