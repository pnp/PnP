using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.Connectors
{
    /// <summary>
    /// Base file connector class
    /// </summary>
    public abstract class FileConnectorBase : IFileConnector
    {
        #region public variables
        public const string CONNECTIONSTRING = "ConnectionString";
        public const string CONTAINER = "Container";
        #endregion

        #region Private variables
        private Dictionary<string, string> parameters = new Dictionary<string, string>();
        #endregion

        #region Properties
        public Dictionary<string, string> Parameters
        {
            get
            {
                return this.parameters;
            }
        }
        #endregion

        #region Overridable Methods
        /// <summary>
        /// Get the files available in the default container
        /// </summary>
        /// <returns>List of files</returns>
        public abstract List<string> GetFiles();

        /// <summary>
        /// Get the files available in the specified container
        /// </summary>
        /// <param name="container">Name of the container to get the files from</param>
        /// <returns>List of files</returns>
        public abstract List<string> GetFiles(string container);

        /// <summary>
        /// Gets a file as string from the default container
        /// </summary>
        /// <param name="fileName">Name of the file to get</param>
        /// <returns>String containing the file contents</returns>
        public abstract string GetFile(string fileName);

        /// <summary>
        /// Gets a file as string from the specified container
        /// </summary>
        /// <param name="fileName">Name of the file to get</param>
        /// <param name="container">Name of the container to get the file from</param>
        /// <returns>String containing the file contents</returns>
        public abstract string GetFile(string fileName, string container);

        /// <summary>
        /// Gets a file as stream from the default container
        /// </summary>
        /// <param name="fileName">Name of the file to get</param>
        /// <returns>String containing the file contents</returns>
        public abstract Stream GetFileStream(string fileName);

        /// <summary>
        /// Gets a file as stream from the specified container
        /// </summary>
        /// <param name="fileName">Name of the file to get</param>
        /// <param name="container">Name of the container to get the file from</param>
        /// <returns>String containing the file contents</returns>
        public abstract Stream GetFileStream(string fileName, string container);
        #endregion

        #region Helper methods
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
        #endregion

    }
}
