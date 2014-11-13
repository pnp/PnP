using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace OfficeDevPnP.Core.Utilities
{
    /// <summary>
    /// Utility class that supports the serialization from Json to type and vice versa
    /// </summary>
    public static class JsonUtility
    {
        /// <summary>
        /// Serializes an object of type T to a json string
        /// </summary>
        /// <typeparam name="T">Type of obj</typeparam>
        /// <param name="obj">Object to serialize</param>
        /// <returns>json string</returns>
        public static string Serialize<T>(T obj)
        {
            //string retVal = null;
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
            //    serializer.WriteObject(ms, obj);
            //    retVal = Encoding.Default.GetString(ms.ToArray());
            //}
            var s = new JavaScriptSerializer();
            var retVal = s.Serialize(obj);
            return retVal;
        }

        /// <summary>
        /// Deserializes a json string to an object of type T
        /// </summary>
        /// <typeparam name="T">Type of the returned object</typeparam>
        /// <param name="json">json string</param>
        /// <returns>Object of type T</returns>
        public static T Deserialize<T>(string json)
        {
            //var obj = Activator.CreateInstance<T>();
            //using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            //{
            //    System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
            //    obj = (T)serializer.ReadObject(ms);
            //}
            var s = new JavaScriptSerializer();
            var obj = s.Deserialize<T>(json);
            return obj;
        }

    }
}
