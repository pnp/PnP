using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.Connectors
{
    /// <summary>
    /// Connector for files in file system
    /// </summary>
    public class FileSystemConnector : FileConnectorBase
    {
        #region Constructors
        /// <summary>
        /// Base constructor
        /// </summary>
        public FileSystemConnector()
            : base()
        {

        }

        /// <summary>
        /// FileSystemConnector constructor. Allows to directly set root folder and sub folder
        /// </summary>
        /// <param name="connectionString">Root folder (e.g. c:\temp or .\resources or . or .\resources\templates)</param>
        /// <param name="container">Sub folder (e.g. templates or resources\templates or blank</param>
        public FileSystemConnector(string connectionString, string container)
            : base()
        {
            if (String.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("connectionString");
            }

            if (String.IsNullOrEmpty(container))
            {
                container = "";
            }

            this.AddParameterAsString(CONNECTIONSTRING, connectionString);
            this.AddParameterAsString(CONTAINER, container);
        }

        #endregion

        #region Base class overrides
        /// <summary>
        /// Get the files available in the default container
        /// </summary>
        /// <returns>List of files</returns>
        public override List<string> GetFiles()
        {
            return GetFiles(GetContainer());
        }

        /// <summary>
        /// Get the files available in the specified container
        /// </summary>
        /// <param name="container">Name of the container to get the files from</param>
        /// <returns>List of files</returns>
        public override List<string> GetFiles(string container)
        {
            if (String.IsNullOrEmpty(container))
            {
                container = "";
            }

            List<string> result = new List<string>();

            string path = ConstructPath("", container);

            foreach (string file in Directory.EnumerateFiles(path, "*.*"))
            {
                result.Add(Path.GetFileName(file));
            }

            return result;
        }

        /// <summary>
        /// Gets a file as string from the default container
        /// </summary>
        /// <param name="fileName">Name of the file to get</param>
        /// <returns>String containing the file contents</returns>
        public override string GetFile(string fileName)
        {
            return GetFile(fileName, GetContainer());
        }

        /// <summary>
        /// Gets a file as string from the specified container
        /// </summary>
        /// <param name="fileName">Name of the file to get</param>
        /// <param name="container">Name of the container to get the file from</param>
        /// <returns>String containing the file contents</returns>
        public override string GetFile(string fileName, string container)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("fileName");
            }

            if (String.IsNullOrEmpty(container))
            {
                container = "";
            }

            string result = null;
            MemoryStream stream = null;
            try
            {
                stream = GetFileFromStorage(fileName, container);

                if (stream == null)
                {
                    return null;
                }

                result = System.Text.Encoding.UTF8.GetString(stream.ToArray());
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }

            return result;
        }

        /// <summary>
        /// Gets a file as stream from the default container
        /// </summary>
        /// <param name="fileName">Name of the file to get</param>
        /// <returns>String containing the file contents</returns>
        public override Stream GetFileStream(string fileName)
        {
            return GetFileStream(fileName, GetContainer());
        }

        /// <summary>
        /// Gets a file as stream from the specified container
        /// </summary>
        /// <param name="fileName">Name of the file to get</param>
        /// <param name="container">Name of the container to get the file from</param>
        /// <returns>String containing the file contents</returns>
        public override Stream GetFileStream(string fileName, string container)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("fileName");
            }

            if (String.IsNullOrEmpty(container))
            {
                container = "";
            }

            return GetFileFromStorage(fileName, container);
        }
        #endregion

        #region Private methods
        private MemoryStream GetFileFromStorage(string fileName, string container)
        {
            try
            {
                string filePath = ConstructPath(fileName, container);

                MemoryStream stream = new MemoryStream();
                using (FileStream fileStream = File.OpenRead(filePath))
                {
                    stream.SetLength(fileStream.Length);
                    fileStream.Read(stream.GetBuffer(), 0, (int)fileStream.Length);
                }

                Log.Info(Constants.LOGGING_SOURCE, CoreResources.Provisioning_Connectors_FileSystem_FileRetrieved, fileName, container);
                stream.Position = 0;
                return stream;
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException || ex is DirectoryNotFoundException)
                {
                    Log.Error(Constants.LOGGING_SOURCE, CoreResources.Provisioning_Connectors_FileSystem_FileNotFound, fileName, container, ex.Message);
                    return null;
                }

                throw;
            }
        }

        private string ConstructPath(string fileName, string container)
        {
            string filePath = "";

            if (container.IndexOf(@"\") > 0)
            {
                string[] parts = container.Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries);
                filePath = Path.Combine(GetConnectionString(), parts[0]);

                if (parts.Length > 1)
                {
                    for (int i = 1; i < parts.Length; i++)
                    {
                        filePath = Path.Combine(filePath, parts[i]);
                    }
                }

                if (!String.IsNullOrEmpty(fileName))
                {
                    filePath = Path.Combine(filePath, fileName);
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(fileName))
                {
                    filePath = Path.Combine(GetConnectionString(), container, fileName);
                }
                else
                {
                    filePath = Path.Combine(GetConnectionString(), container);
                }
            }

            return filePath;
        }

        #endregion
    }
}
