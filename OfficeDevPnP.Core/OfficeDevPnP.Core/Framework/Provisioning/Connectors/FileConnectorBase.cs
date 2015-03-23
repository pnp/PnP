using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.Connectors
{
    public abstract class FileConnectorBase
    {
        #region public variables
        public const string CONNECTIONSTRING = "ConnectionString";
        public const string CONTAINER = "Container";
        #endregion


        private Dictionary<string, string> parameters = new Dictionary<string, string>();

        public Dictionary<string, string> Parameters
        {
            get
            {
                return this.parameters;
            }
        }

        public abstract List<string> GetFiles();

        public abstract List<string> GetFiles(string container);

        public abstract string GetFile(string fileName);

        public abstract string GetFile(string fileName, string container);

        public abstract Stream GetFileStream(string fileName);

        public abstract Stream GetFileStream(string fileName, string container);

        public void AddParameter(string key, string value)
        {
            if (!this.Parameters.ContainsKey(key))
            {
                this.Parameters.Add(key, value);
            }
            else
            {
                this.Parameters[key] = value;
            }
        }

        internal string GetConnectionString()
        {
            if (this.Parameters.ContainsKey(CONNECTIONSTRING))
            {
                return this.Parameters[CONNECTIONSTRING];
            }
            else
            {
                throw new Exception("No connection string specified");
            }
        }

        internal string GetContainer()
        {
            if (this.Parameters.ContainsKey(CONTAINER))
            {
                return this.Parameters[CONTAINER];
            }
            else
            {
                throw new Exception("No container string specified");
            }
        }


    }
}
