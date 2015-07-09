using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Utilities;
using File = Microsoft.SharePoint.Client.File;

namespace OfficeDevPnP.Core.Framework.Provisioning.Connectors
{

    /// <summary>
    /// Connector for files in SharePoint
    /// </summary>
    public class SharePointConnector: FileConnectorBase
    {
        #region public variables
        public const string CLIENTCONTEXT = "ClientContext";
        #endregion

        #region Constructors
        /// <summary>
        /// Base constructor
        /// </summary>
        public SharePointConnector()
            : base()
        {

        }

        /// <summary>
        /// SharePointConnector constructor. Allows to directly set root folder and sub folder
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="connectionString">Site collection URL (e.g. https://yourtenant.sharepoint.com/sites/dev) </param>
        /// <param name="container">Library + folder that holds the files (mydocs/myfolder)</param>
        public SharePointConnector(ClientRuntimeContext clientContext, string connectionString, string container)
            : base()
        {
            if (clientContext == null)
            {
                throw new ArgumentNullException("clientContext");
            }

            if (String.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("connectionString");
            }

            if (String.IsNullOrEmpty(container))
            {
                throw new ArgumentException("container");
            }
            this.AddParameter(CLIENTCONTEXT, clientContext);
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
                throw new ArgumentException("container");
            }

            List<string> result = new List<string>();

            using (ClientContext cc = GetClientContext().Clone(GetConnectionString()))
            {
                List list = cc.Web.GetListByUrl(GetDocumentLibrary(container));
                string folders = GetFolders(container);

                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml = @"<View Scope='FilesOnly'><Query></Query></View>";

                if (folders.Length > 0)
                {
                    camlQuery.FolderServerRelativeUrl = string.Format("{0}{1}", list.RootFolder.ServerRelativeUrl, folders);
                }

                ListItemCollection listItems = list.GetItems(camlQuery);
                cc.Load(listItems);
                cc.ExecuteQueryRetry();

                foreach(var listItem in listItems)
                {
                    result.Add(listItem.FieldValues["FileLeafRef"].ToString());

                    
                }
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

                result = Encoding.UTF8.GetString(stream.ToArray());
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
                throw new ArgumentException("container");
            }

            return GetFileFromStorage(fileName, container);
        }

        /// <summary>
        /// Saves a stream to the default container with the given name. If the file exists it will be overwritten
        /// </summary>
        /// <param name="fileName">Name of the file to save</param>
        /// <param name="stream">Stream containing the file contents</param>
        public override void SaveFileStream(string fileName, Stream stream)
        {
            SaveFileStream(fileName, GetContainer(), stream);
        }

        /// <summary>
        /// Saves a stream to the specified container with the given name. If the file exists it will be overwritten
        /// </summary>
        /// <param name="fileName">Name of the file to save</param>
        /// <param name="container">Name of the container to save the file to</param>
        /// <param name="stream">Stream containing the file contents</param>
        public override void SaveFileStream(string fileName, string container, Stream stream)
        {
            try
            {
                using (ClientContext cc = GetClientContext().Clone(GetConnectionString()))
                {
                    List list = cc.Web.GetListByUrl(GetDocumentLibrary(container));

                    string folders = GetFolders(container);

                    Folder spFolder = null;

                    if (folders.Length == 0)
                    {
                        spFolder = list.RootFolder;
                    }
                    else
                    {
                        spFolder = list.RootFolder;
                        string[] parts = container.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 1; i < parts.Length; i++)
                        {
                            var prevFolder = spFolder;
                            spFolder = spFolder.ResolveSubFolder(parts[i]);

                            if (spFolder == null)
                            {
                                spFolder = prevFolder.CreateFolder(parts[i]);
                            }
                        }
                    }

                    spFolder.UploadFile(fileName, stream, true);
                    Log.Info(Constants.LOGGING_SOURCE, CoreResources.Provisioning_Connectors_SharePoint_FileSaved, fileName, GetConnectionString(), container);
                }
            }
            catch (Exception ex)
            {
                Log.Error(Constants.LOGGING_SOURCE, CoreResources.Provisioning_Connectors_SharePoint_FileSaveFailed, fileName, GetConnectionString(), container, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Deletes a file from the default container
        /// </summary>
        /// <param name="fileName">Name of the file to delete</param>
        public override void DeleteFile(string fileName)
        {
            DeleteFile(fileName, GetContainer());
        }

        /// <summary>
        /// Deletes a file from the specified container
        /// </summary>
        /// <param name="fileName">Name of the file to delete</param>
        /// <param name="container">Name of the container to delete the file from</param>
        public override void DeleteFile(string fileName, string container)
        {
            try
            {
                using (ClientContext cc = GetClientContext().Clone(GetConnectionString()))
                {
                    List list = cc.Web.GetListByUrl(GetDocumentLibrary(container));

                    string folders = GetFolders(container);

                    Folder spFolder = null;

                    if (folders.Length == 0)
                    {
                        spFolder = list.RootFolder;
                    }
                    else
                    {
                        spFolder = list.RootFolder;
                        string[] parts = container.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 1; i < parts.Length; i++)
                        {
                            spFolder = spFolder.ResolveSubFolder(parts[i]);
                        }
                    }

                    if (!spFolder.IsPropertyAvailable("ServerRelativeUrl"))
                    {
                        spFolder.Context.Load(spFolder, w => w.ServerRelativeUrl);
                        spFolder.Context.ExecuteQueryRetry();
                    }

                    var fileServerRelativeUrl = UrlUtility.Combine(spFolder.ServerRelativeUrl, fileName);
                    File file = cc.Web.GetFileByServerRelativeUrl(fileServerRelativeUrl);
                    cc.Load(file);
                    cc.ExecuteQueryRetry();

                    if (file != null)
                    {
                        file.DeleteObject();
                        cc.ExecuteQueryRetry();
                        Log.Info(Constants.LOGGING_SOURCE, CoreResources.Provisioning_Connectors_SharePoint_FileDeleted, fileName, GetConnectionString(), container);
                    }
                    else
                    {
                        Log.Warning(Constants.LOGGING_SOURCE, CoreResources.Provisioning_Connectors_SharePoint_FileDeleteNotFound, fileName, GetConnectionString(), container);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(Constants.LOGGING_SOURCE, CoreResources.Provisioning_Connectors_SharePoint_FileDeleteFailed, fileName, GetConnectionString(), container, ex.Message);
                throw;
            }
        }
        #endregion

        #region Private Methods
        private MemoryStream GetFileFromStorage(string fileName, string container)
        {
            try
            {
                using (ClientContext cc = GetClientContext().Clone(GetConnectionString()))
                {
                    List list = cc.Web.GetListByUrl(GetDocumentLibrary(container));
                    string folders = GetFolders(container);

                    File file = null;
                    Folder spFolder = null;

                    if (folders.Length == 0)
                    {
                        spFolder = list.RootFolder;
                    }
                    else
                    {
                        spFolder = list.RootFolder;
                        string[] parts = container.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

                        int startFrom = 1;
                        if (parts[0].Equals("_catalogs", StringComparison.InvariantCultureIgnoreCase))
                        {
                            startFrom = 2;
                        }

                        for (int i = startFrom; i < parts.Length; i++)
                        {
                            spFolder = spFolder.ResolveSubFolder(parts[i]);
                        }                        
                    }

                    if (!spFolder.IsPropertyAvailable("ServerRelativeUrl"))
                    {
                        spFolder.Context.Load(spFolder, w => w.ServerRelativeUrl);
                        spFolder.Context.ExecuteQueryRetry();
                    }

                    var fileServerRelativeUrl = UrlUtility.Combine(spFolder.ServerRelativeUrl, fileName);
                    file = cc.Web.GetFileByServerRelativeUrl(fileServerRelativeUrl);
                    cc.Load(file);
                    cc.ExecuteQueryRetry();

                    MemoryStream stream = new MemoryStream();
                    var streamResult = file.OpenBinaryStream();
                    cc.ExecuteQueryRetry();

                    streamResult.Value.CopyTo(stream);

                    Log.Info(Constants.LOGGING_SOURCE, CoreResources.Provisioning_Connectors_SharePoint_FileRetrieved, fileName, GetConnectionString(), container);

                    // Set the stream position to the beginning
                    stream.Position = 0;
                    return stream;
                }
            }
            catch(Exception ex)
            {
                Log.Error(Constants.LOGGING_SOURCE, CoreResources.Provisioning_Connectors_SharePoint_FileNotFound, fileName, GetConnectionString(), container, ex.Message);
                return null;
            }
        }

        private string GetDocumentLibrary(string container)
        {
            string[] parts = container.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length > 1)
            {
                if (parts[0].Equals("_catalogs", StringComparison.InvariantCultureIgnoreCase))
                {
                    return string.Format("_catalogs/{0}", parts[1]);
                }
            }

            return parts[0];
        }

        private string GetFolders(string container)
        {
            string[] parts = container.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length > 1)
            {
                int startFrom = 1;
                if (parts[0].Equals("_catalogs", StringComparison.InvariantCultureIgnoreCase))
                {
                    startFrom = 2;
                }
                
                string folder = "";
                for (int i = startFrom; i < parts.Length;i++)
                {
                    folder = folder + "/" + parts[i];
                }

                return folder;
            }
            else
            {
                return "";
            }
        }

        private ClientRuntimeContext GetClientContext()
        {
            if (this.Parameters.ContainsKey(CLIENTCONTEXT))
            {
                return this.Parameters[CLIENTCONTEXT] as ClientRuntimeContext;
            }
            else
            {
                throw new Exception("No clientcontext specified");
            }
        }
        #endregion
    }
}
