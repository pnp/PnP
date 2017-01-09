using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharePointPnP.IdentityModel.Extensions.S2S.Protocols.OAuth2
{
    public abstract class OAuth2Message
    {
        private System.Collections.Generic.Dictionary<string, string> _message = new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.Ordinal);

        protected string this[string index]
        {
            get
            {
                return this.GetValue(index);
            }
            set
            {
                this._message[index] = value;
            }
        }

        protected System.Collections.Generic.IEnumerable<string> Keys
        {
            get
            {
                return this._message.Keys;
            }
        }

        public System.Collections.Generic.Dictionary<string, string> Message
        {
            get
            {
                return this._message;
            }
        }

        public override string ToString()
        {
            return this.Encode();
        }

        protected bool ContainsKey(string key)
        {
            return this._message.ContainsKey(key);
        }

        protected void Decode(string message)
        {
            this._message.Decode(message);
        }

        protected void DecodeFromJson(string message)
        {
            this._message.DecodeFromJson(message);
        }

        protected string Encode()
        {
            return this._message.Encode();
        }

        protected string EncodeToJson()
        {
            return this._message.EncodeToJson();
        }

        protected string GetValue(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new System.ArgumentException("The input string parameter is either null or empty.", "key");
            }
            string result = null;
            this._message.TryGetValue(key, out result);
            return result;
        }
    }
}
