using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace SharePointPnP.IdentityModel.Extensions.S2S
{
    public static class DictionaryExtension
    {
        public delegate string Encoder(string input);

        public const char DefaultSeparator = '&';

        public const char DefaultKeyValueSeparator = '=';

        public static DictionaryExtension.Encoder DefaultDecoder = new DictionaryExtension.Encoder(HttpUtility.UrlDecode);

        public static DictionaryExtension.Encoder DefaultEncoder = new DictionaryExtension.Encoder(HttpUtility.UrlEncode);

        public static DictionaryExtension.Encoder NullEncoder = new DictionaryExtension.Encoder(DictionaryExtension.NullEncode);

        private static string NullEncode(string value)
        {
            return value;
        }

        public static void Decode(this System.Collections.Generic.IDictionary<string, string> self, string encodedDictionary)
        {
            self.Decode(encodedDictionary, '&', '=', DictionaryExtension.DefaultDecoder, DictionaryExtension.DefaultDecoder, false);
        }

        public static void Decode(this System.Collections.Generic.IDictionary<string, string> self, string encodedDictionary, DictionaryExtension.Encoder decoder)
        {
            self.Decode(encodedDictionary, '&', '=', decoder, decoder, false);
        }

        public static void Decode(this System.Collections.Generic.IDictionary<string, string> self, string encodedDictionary, char separator, char keyValueSplitter, bool endsWithSeparator)
        {
            self.Decode(encodedDictionary, separator, keyValueSplitter, DictionaryExtension.DefaultDecoder, DictionaryExtension.DefaultDecoder, endsWithSeparator);
        }

        public static void Decode(this System.Collections.Generic.IDictionary<string, string> self, string encodedDictionary, char separator, char keyValueSplitter, DictionaryExtension.Encoder keyDecoder, DictionaryExtension.Encoder valueDecoder, bool endsWithSeparator)
        {
            if (encodedDictionary == null)
            {
                throw new System.ArgumentNullException("encodedDictionary");
            }
            if (keyDecoder == null)
            {
                throw new System.ArgumentNullException("keyDecoder");
            }
            if (valueDecoder == null)
            {
                throw new System.ArgumentNullException("valueDecoder");
            }
            if (endsWithSeparator && encodedDictionary.LastIndexOf(separator) == encodedDictionary.Length - 1)
            {
                encodedDictionary = encodedDictionary.Substring(0, encodedDictionary.Length - 1);
            }
            string[] array = encodedDictionary.Split(new char[]
            {
                separator
            });
            for (int i = 0; i < array.Length; i++)
            {
                string text = array[i];
                string[] array2 = text.Split(new char[]
                {
                    keyValueSplitter
                });
                if ((array2.Length == 1 || array2.Length > 2) && !string.IsNullOrEmpty(array2[0]))
                {
                    throw new System.ArgumentException("The request is not properly formatted.", "encodedDictionary");
                }
                if (array2.Length != 2)
                {
                    throw new System.ArgumentException("The request is not properly formatted.", "encodedDictionary");
                }
                string text2 = keyDecoder(array2[0].Trim());
                string value = valueDecoder(array2[1].Trim().Trim(new char[]
                {
                    '"'
                }));
                try
                {
                    self.Add(text2, value);
                }
                catch (System.ArgumentException)
                {
                    string message = string.Format(System.Globalization.CultureInfo.InvariantCulture, "The request is not properly formatted. The parameter '{0}' is duplicated.", new object[]
                    {
                        text2
                    });
                    throw new System.ArgumentException(message, "encodedDictionary");
                }
            }
        }

        public static string Encode(this System.Collections.Generic.IDictionary<string, string> self)
        {
            return self.Encode('&', '=', DictionaryExtension.DefaultEncoder, DictionaryExtension.DefaultEncoder, false);
        }

        public static string Encode(this System.Collections.Generic.IDictionary<string, string> self, DictionaryExtension.Encoder encoder)
        {
            return self.Encode('&', '=', encoder, encoder, false);
        }

        public static string Encode(this System.Collections.Generic.IDictionary<string, string> self, char separator, char keyValueSplitter, bool endsWithSeparator)
        {
            return self.Encode(separator, keyValueSplitter, DictionaryExtension.DefaultEncoder, DictionaryExtension.DefaultEncoder, endsWithSeparator);
        }

        public static string Encode(this System.Collections.Generic.IDictionary<string, string> self, char separator, char keyValueSplitter, DictionaryExtension.Encoder keyEncoder, DictionaryExtension.Encoder valueEncoder, bool endsWithSeparator)
        {
            if (keyEncoder == null)
            {
                throw new System.ArgumentNullException("keyEncoder");
            }
            if (valueEncoder == null)
            {
                throw new System.ArgumentNullException("valueEncoder");
            }
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            foreach (System.Collections.Generic.KeyValuePair<string, string> current in self)
            {
                if (stringBuilder.Length != 0)
                {
                    stringBuilder.Append(separator);
                }
                stringBuilder.AppendFormat("{0}{1}{2}", keyEncoder(current.Key), keyValueSplitter, valueEncoder(current.Value));
            }
            if (endsWithSeparator)
            {
                stringBuilder.Append(separator);
            }
            return stringBuilder.ToString();
        }

        public static string EncodeToJson(this System.Collections.Generic.IDictionary<string, string> self)
        {
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            return javaScriptSerializer.Serialize(self);
        }

        public static void DecodeFromJson(this System.Collections.Generic.IDictionary<string, string> self, string encodedDictionary)
        {
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            System.Collections.Generic.Dictionary<string, object> dictionary = javaScriptSerializer.DeserializeObject(encodedDictionary) as System.Collections.Generic.Dictionary<string, object>;
            if (dictionary == null)
            {
                throw new System.ArgumentException("Invalid request format.", "encodedDictionary");
            }
            foreach (System.Collections.Generic.KeyValuePair<string, object> current in dictionary)
            {
                if (current.Value == null)
                {
                    self.Add(current.Key, null);
                }
                else if (current.Value is object[])
                {
                    self.Add(current.Key, javaScriptSerializer.Serialize(current.Value));
                }
                else
                {
                    self.Add(current.Key, current.Value.ToString());
                }
            }
        }
    }
}
