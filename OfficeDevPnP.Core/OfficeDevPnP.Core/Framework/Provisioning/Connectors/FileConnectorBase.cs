using System;
using System.Collections.Generic;
using System.IO;

namespace OfficeDevPnP.Core.Framework.Provisioning.Connectors
{
    /// <summary>
    /// Base file connector class
    /// </summary>
    public abstract class FileConnectorBase
    {
        #region public variables
        public const string CONNECTIONSTRING = "ConnectionString";
        public const string CONTAINER = "Container";
        #endregion

        #region Private variables
        private Dictionary<string, object> parameters = new Dictionary<string, object>();
        #endregion

        #region Properties
        public Dictionary<string, object> Parameters
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

        /// <summary>
        /// Saves a stream to the default container with the given name. If the file exists it will be overwritten
        /// </summary>
        /// <param name="fileName">Name of the file to save</param>
        /// <param name="stream">Stream containing the file contents</param>
        public abstract void SaveFileStream(string fileName, Stream stream);

        /// <summary>
        /// Saves a stream to the specified container with the given name. If the file exists it will be overwritten
        /// </summary>
        /// <param name="fileName">Name of the file to save</param>
        /// <param name="container">Name of the container to save the file to</param>
        /// <param name="stream">Stream containing the file contents</param>
        public abstract void SaveFileStream(string fileName, string container, Stream stream);

        /// <summary>
        /// Deletes a file from the default container
        /// </summary>
        /// <param name="fileName">Name of the file to delete</param>
        public abstract void DeleteFile(string fileName);

        /// <summary>
        /// Deletes a file from the specified container
        /// </summary>
        /// <param name="fileName">Name of the file to delete</param>
        /// <param name="container">Name of the container to delete the file from</param>
        public abstract void DeleteFile(string fileName, string container);

        /// <summary>
        /// Returns a filename without a path
        /// </summary>
        /// <param name="fileName">Path to the file to retrieve the filename from</param>
        public abstract string GetFilenamePart(string fileName);
        #endregion

        #region Helper methods
        public void AddParameterAsString(string key, string value)
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

        public void AddParameter(string key, object value)
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
                return this.Parameters[CONNECTIONSTRING].ToString();
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
                return this.Parameters[CONTAINER].ToString();
            }
            else
            {
                throw new Exception("No container string specified");
            }
        }
        #endregion
    }
}
