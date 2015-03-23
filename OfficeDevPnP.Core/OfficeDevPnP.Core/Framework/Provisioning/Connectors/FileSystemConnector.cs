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
        public FileSystemConnector() : base()
        {

        }

        public FileSystemConnector(string connectionString, string container): base ()
        {
            if (String.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("connectionString");
            }

            if (String.IsNullOrEmpty(container))
            {
                container = "";
            }

            this.AddParameter(CONNECTIONSTRING, connectionString);
            this.AddParameter(CONTAINER, container);
        }

        #endregion

        #region overrides
        public override List<string> GetFiles()
        {
            return GetFiles(GetContainer());
        }

        public override List<string> GetFiles(string container)
        {
            if (String.IsNullOrEmpty(container))
            {
                throw new ArgumentException("container");
            }

            List<string> result = new List<string>();

            string path = Path.Combine(GetConnectionString(), container);

            foreach (string file in Directory.EnumerateFiles(path, "*.*"))
            {
                result.Add(Path.GetFileName(file));
            }

            return result;
        }

        public override string GetFile(string fileName)
        {
            return GetFile(fileName, GetContainer());
        }

        public override string GetFile(string fileName, string container)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("fileName");
            }

            if (String.IsNullOrEmpty(container))
            {
                throw new ArgumentException("container");
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

        public override Stream GetFileStream(string fileName)
        {
            return GetFileStream(fileName, GetContainer());
        }

        public override Stream GetFileStream(string fileName, string container)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("fileName");
            }

            if (String.IsNullOrEmpty(container))
            {
                throw new ArgumentException("container");
            }

            return GetFileFromStorage(fileName, container);
        }
        #endregion

        #region Private methods
        private MemoryStream GetFileFromStorage(string fileName, string container)
        {
            try
            {
                string filePath = Path.Combine(GetConnectionString(), GetContainer(), fileName);

                MemoryStream stream = new MemoryStream();
                using (FileStream fileStream = File.OpenRead(filePath))
                {
                    stream.SetLength(fileStream.Length);
                    fileStream.Read(stream.GetBuffer(), 0, (int)fileStream.Length);
                }
                return stream;
            }
            catch (Exception ex)
            {
                Log.Error(Constants.LOGGING_SOURCE, "File {0} not found in Azure storage container {1}. Exception = {2}", fileName, container, ex.Message);
                return null;
            }
        }
        #endregion


    }
}
