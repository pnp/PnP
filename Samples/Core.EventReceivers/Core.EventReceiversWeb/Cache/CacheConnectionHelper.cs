using System;

namespace Contoso.Core.EventReceiversWeb.Cache
{
    using StackExchange.Redis;

    public class CacheConnectionHelper
    {
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect(SettingsHelper.AzureRedisCache);
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
    }
}