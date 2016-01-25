using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Contoso.Core.EventReceiversWeb.Cache
{
    using StackExchange.Redis;

    public static class StackExchangeRedisExtensions
    {
        public static bool Add(this IDatabase cache, string key, object value, TimeSpan expiration)
        {
            if (null == Deserialize<object>(cache.StringGet(key)))
            {
                cache.StringSet(key, Serialize(value), expiration);
                return true;
            }
            return false;
        }

        private static byte[] Serialize(object o)
        {
            if (o == null)
                return null;

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, o);
                byte[] objectDataAsStream = memoryStream.ToArray();
                return objectDataAsStream;
            }
        }

        private static T Deserialize<T>(byte[] stream)
        {
            if (stream == null)
                return default(T);

            using (MemoryStream memoryStream = new MemoryStream(stream))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                T result = (T)binaryFormatter.Deserialize(memoryStream);
                return result;
            }
        }
    }
}