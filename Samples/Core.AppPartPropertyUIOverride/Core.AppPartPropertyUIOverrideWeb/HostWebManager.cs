namespace Contoso.Core.AppPartPropertyUIOverrideWeb
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Web.Hosting;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// A helper class that makes easy to deploy file to and uninstall files from the host web.
    /// </summary>
    public class HostWebManager
    {
        #region (private instance fields)
        /// <summary>
        /// Private instance field that contains the internal name of the app for SharePoint that <see cref="HostWebManager"/> is being called in.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string appForSharePointInternalNameField;

        /// <summary>
        /// Private instance field that contains the <see cref="Folder"/> cache so that excessive SharePoint API calls don't happen 
        /// when crawling folders.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Dictionary<string, Folder> folderCacheField = new Dictionary<string, Folder>();

        /// <summary>
        /// Private instance field that contains the host <see cref="Web"/>.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Web hostWebField;

        /// <summary>
        /// Private instance field that contains the host web <see cref="ClientContext"/>.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ClientContext hostWebClientContextField;

        /// <summary>
        /// Private instance field that contains the host web root <see cref="Folder"/>.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Folder hostWebRootFolderField;

        /// <summary>
        /// Private instance field that contains remote web root physical path.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string remoteWebRootPhysicalPathField;
        #endregion

        #region (constructor)
        /// <summary>
        /// Initializes a new instance of the <see cref="HostWebManager"/> class.
        /// </summary>
        /// <param name="appForSharePointInternalName">
        /// The internal name of the app for SharePoint is being called in.
        /// NOTE: no spaces or special characters allowed.  
        /// Example: "MyUniqueAppName"
        /// </param>
        /// <param name="hostWebClientContext">
        /// The SharePoint <see cref="ClientContext"/> object that represents the host web.
        /// </param>
        public HostWebManager(string appForSharePointInternalName, ClientContext hostWebClientContext)
        {
            this.appForSharePointInternalNameField = appForSharePointInternalName;
            this.hostWebClientContextField = hostWebClientContext;
            this.hostWebField = hostWebClientContext.Web;
            this.remoteWebRootPhysicalPathField = HostingEnvironment.MapPath(@"~/");
        }
        #endregion

        #region PUBLIC PROPERTY: AppForSharePointInternalName
        /// <summary>
        /// Gets the internal name of the app for SharePoint this <see cref="HostWebManager"/> is being run in.
        /// </summary>
        public string AppForSharePointInternalName
        {
            get
            {
                return this.appForSharePointInternalNameField;
            }
        }
        #endregion

        #region PUBLIC METHOD: CreateAppSpecificFile()
        /// <summary>
        /// Creates an app for SharePoint specific file in the host web.
        /// </summary>
        /// <param name="destinationFilePath">
        /// The destination file name and path to create in the host web's "_apps/[AppForSharePointInternalName]" folder. 
        /// Example:  "Scripts/filename.js" 
        /// </param>
        /// <param name="contents">
        /// The text-based contents of the file to create in the host web.
        /// </param>
        public void CreateAppSpecificFile(string destinationFilePath, string contents)
        {
            // setup the destination path
            string destinationPath = "_apps/" + this.appForSharePointInternalNameField + "/" + destinationFilePath;

            // convert string to byte array
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(contents);

            // deploy the file to the host web
            this.DeployFileInternal(byteArray, destinationPath);
        }
        #endregion

        #region PUBLIC METHOD: DeployAppSpecificFile()
        /// <summary>
        /// Deploys an existing application specific file from the ASP.NET application (Remote Web) to the host web's "_apps/[AppForSharePointInternalName]" folder.
        /// </summary>
        /// <param name="sourceFilePath">
        /// The source file's ASP.NET application (Remote Web) file name and path.
        /// Example: "Scripts/myfile.js"
        /// </param>
        public void DeployAppSpecificFile(string sourceFilePath)
        {
            string destinationFilePath = "_apps/" + this.appForSharePointInternalNameField + "/" + sourceFilePath;
            this.DeployFileInternal(sourceFilePath, destinationFilePath);
        }
        #endregion

        #region PUBLIC METHOD: DeployGlobalFile()
        /// <summary>
        /// Deploys an existing global file from the ASP.NET application (Remote Web) to the host web's "_apps/_globals/" folder.  
        /// If the file and path already exists, it will overwrite it.
        /// </summary>
        /// <param name="sourceFilePath">
        /// The source file's ASP.NET application (Remote Web) file name and path.
        /// Example: "Scripts/myfile.js"
        /// </param>
        public void DeployGlobalFile(string sourceFilePath)
        {
            string destinationFilePath = "_apps/_globals/" + sourceFilePath;
            this.DeployFileInternal(sourceFilePath, destinationFilePath);
        }
        #endregion

        #region PUBLIC METHOD: WireUpAppSpecificJSFileOnAllPagesInWeb()
        /// <summary>
        /// Wires up an application specific JavaScript file to be automatically included on all pages in the host web.
        /// </summary>
        /// <param name="fileName">
        /// Name of the JavaScript file that was deployed by the DeployAppSpecificFile() method
        /// Example: "filename.js"
        /// </param>
        /// <param name="sequence">
        /// The sequence number for controlling the order of the JavaScript file loading.   Must be between 0 and 65536.  
        /// </param>
        public void WireUpAppSpecificJSFileOnAllPagesInWeb(string fileName, int sequence)
        {
            string scriptSource = "~site/_apps/" + this.appForSharePointInternalNameField + "/Scripts/" + fileName + "?rev=" + (Guid.NewGuid().ToString("N"));
            string actionName = this.appForSharePointInternalNameField + "_" + System.IO.Path.GetFileNameWithoutExtension(fileName);

            WireUpJSFileInternal(scriptSource, actionName, sequence);
        }
        #endregion

        #region PUBLIC METHOD: UninstallAssets()
        /// <summary>
        /// Uninstalls all app specific assets and wirings up that were deployed to the host web.  
        /// All global assets are left for safety.
        /// </summary>
        public void UninstallAssets()
        {
            // delete all app specific custom actions
            UserCustomActionCollection userCustomActions = this.GetCurrentUserCustomActionsDeclaredOnThisWeb();
            List<UserCustomAction> userCustomActionsToDelete = new List<UserCustomAction>();
            foreach (UserCustomAction userCustomAction in userCustomActions)
            {
                if (!string.IsNullOrEmpty(userCustomAction.Name) && userCustomAction.Name.StartsWith(this.appForSharePointInternalNameField))
                {
                    userCustomActionsToDelete.Add(userCustomAction);
                }
            }

            if (userCustomActionsToDelete.Count > 0)
            {
                foreach (UserCustomAction userCustomActionToDelete in userCustomActionsToDelete)
                {
                    userCustomActionToDelete.DeleteObject();
                }
            }

            // delete app specific folder (and all contents/subfolders)
            this.DeleteFolderRecursive("_apps/" + this.appForSharePointInternalNameField);

            this.hostWebClientContextField.ExecuteQuery();
        }
        #endregion

        #region (private helper methods)
        private void DeleteFolderRecursive(string folderPath)
        {
            this.DeleteFolderRecursive(folderPath, this.GetHostWebRootFolder());
        }

        private void DeleteFolderRecursive(string folderPath, Folder parentFolder)
        {
            // get immediate folder and path remainder
            string immediateFolderName = folderPath;
            string pathRemainder = string.Empty;
            int position = immediateFolderName.IndexOf("/", StringComparison.Ordinal);
            if (position > -1)
            {
                position = position + 1;
                pathRemainder = immediateFolderName.Substring(position, immediateFolderName.Length - position);
                position = position - 1;
                immediateFolderName = immediateFolderName.Substring(0, position);
            }

            // find the immediate folder
            Folder immediateFolder = null;
            try
            {
                immediateFolder = parentFolder.Folders.GetByUrl(immediateFolderName);
                this.hostWebClientContextField.Load(immediateFolder);
                this.hostWebClientContextField.ExecuteQuery();
            }
            catch
            {
                immediateFolder = null;
            }

            if (immediateFolder == null)
            {
                // folder doesn't exist yet
                // create it!
                immediateFolder = parentFolder.Folders.Add(immediateFolderName);
                this.hostWebClientContextField.Load(immediateFolder);
                this.hostWebClientContextField.ExecuteQuery();
            }

            // we now have the immediate folder (existing or created)
            // now see if we have a path remainder
            if (string.IsNullOrEmpty(pathRemainder))
            {
                // no path remainder
                // we are done
                // delete this folder
                immediateFolder.DeleteObject();
                this.hostWebClientContextField.ExecuteQuery();
            }
            else
            {
                // path remainder... call this function again recursively
                this.DeleteFolderRecursive(pathRemainder, immediateFolder);
            }
        }

        private void DeployFileInternal(byte[] byteArray, string destinationFilePath)
        {
            // ensure destination folder structure exists in host web
            string destinationFolderPath = System.IO.Path.GetDirectoryName(destinationFilePath).Replace("\\", "/");
            Folder destinationFolder = this.EnsureHostWebFolderExists(destinationFolderPath);

            // upload file in folder and overwrite it if already present
            FileCreationInformation fileCreationInformation = new FileCreationInformation();
            fileCreationInformation.Content = byteArray;
            fileCreationInformation.Url = destinationFolder.ServerRelativeUrl + "/" + System.IO.Path.GetFileName(destinationFilePath);
            fileCreationInformation.Overwrite = true;
            File fileToUpload = destinationFolder.Files.Add(fileCreationInformation);
            this.hostWebClientContextField.Load(fileToUpload);
            this.hostWebClientContextField.ExecuteQuery();
        }

        private void DeployFileInternal(string sourceFilePath, string destinationFilePath)
        {
            string sourceFilePhysicalPath = this.remoteWebRootPhysicalPathField + sourceFilePath.Replace("/", "\\");
            if (System.IO.File.Exists(sourceFilePhysicalPath))
            {
                // read source file contents into a byte array
                byte[] byteArray = System.IO.File.ReadAllBytes(sourceFilePhysicalPath);

                // deploy the file
                DeployFileInternal(byteArray, destinationFilePath);
            }
            else
            {
                throw new System.IO.FileNotFoundException("The source file in the ASP.NET application (remote web) '" + sourceFilePath + "' was not found.");
            }
        }

        private Folder EnsureHostWebFolderExists(string folderPath)
        {
            return this.EnsureHostWebFolderExists(folderPath, this.GetHostWebRootFolder());
        }

        private Folder EnsureHostWebFolderExists(string folderPath, Folder parentFolder)
        {
            string key = (parentFolder.ServerRelativeUrl + folderPath).ToUpperInvariant();
            if (this.folderCacheField.ContainsKey(key))
            {
                return this.folderCacheField[key];
            }
            else
            {
                // get immediate folder and path remainder
                string immediateFolderName = folderPath;
                string pathRemainder = string.Empty;
                int position = immediateFolderName.IndexOf("/", StringComparison.Ordinal);
                if (position > -1)
                {
                    position = position + 1;
                    pathRemainder = immediateFolderName.Substring(position, immediateFolderName.Length - position);
                    position = position - 1;
                    immediateFolderName = immediateFolderName.Substring(0, position);
                }

                // find the immediate folder
                Folder immediateFolder = null;
                try
                {
                    immediateFolder = parentFolder.Folders.GetByUrl(immediateFolderName);
                    this.hostWebClientContextField.Load(immediateFolder);
                    this.hostWebClientContextField.ExecuteQuery();
                }
                catch
                {
                    immediateFolder = null;
                }

                if (immediateFolder == null)
                {
                    // folder doesn't exist yet
                    // create it!
                    immediateFolder = parentFolder.Folders.Add(immediateFolderName);
                    this.hostWebClientContextField.Load(immediateFolder);
                    this.hostWebClientContextField.ExecuteQuery();
                }

                // we now have the immediate folder (existing or created)
                // now see if we have a path remainder
                if (string.IsNullOrEmpty(pathRemainder))
                {
                    // no path remainder
                    // we are done
                    key = immediateFolder.ServerRelativeUrl.ToUpperInvariant();
                    if (!this.folderCacheField.ContainsKey(key))
                    {
                        this.folderCacheField.Add(key, immediateFolder);
                    }

                    return immediateFolder;
                }
                else
                {
                    // path remainder... 
                    key = immediateFolder.ServerRelativeUrl.ToUpperInvariant();
                    if (!this.folderCacheField.ContainsKey(key))
                    {
                        this.folderCacheField.Add(key, immediateFolder);
                    }

                    // call this function again recursively
                    Folder returnValue = this.EnsureHostWebFolderExists(pathRemainder, immediateFolder);

                    key = returnValue.ServerRelativeUrl.ToUpperInvariant();
                    if (!this.folderCacheField.ContainsKey(key))
                    {
                        this.folderCacheField.Add(key, returnValue);
                    }


                    return returnValue;
                }
            }
        }

        private Folder GetHostWebRootFolder()
        {
            if (this.hostWebRootFolderField == null)
            {
                this.hostWebRootFolderField = this.hostWebField.RootFolder;
                this.hostWebClientContextField.Load(this.hostWebRootFolderField);
                this.hostWebClientContextField.ExecuteQuery();
            }

            return this.hostWebRootFolderField;
        }

        private void WireUpJSFileInternal(string scriptSource, string actionName, int sequence)
        {
            UserCustomActionCollection userCustomActions = GetCurrentUserCustomActionsDeclaredOnThisWeb();
            bool alreadyPresent = false;
            foreach (UserCustomAction userCustomAction in userCustomActions)
            {
                if (!string.IsNullOrEmpty(userCustomAction.Name) && userCustomAction.Name == actionName)
                {
                    alreadyPresent = true;
                    break;
                }
            }

            if (!alreadyPresent)
            {
                // add it
                UserCustomAction action = userCustomActions.Add();
                action.ScriptSrc = scriptSource;
                action.Location = "ScriptLink";
                action.Name = actionName;
                action.Sequence = sequence;
                action.Update();
                this.hostWebClientContextField.ExecuteQuery();
            }
        }

        private UserCustomActionCollection GetCurrentUserCustomActionsDeclaredOnThisWeb()
        {
            this.hostWebClientContextField.Load(this.hostWebField.UserCustomActions);
            this.hostWebClientContextField.ExecuteQuery();
            return this.hostWebField.UserCustomActions;
        }

        #endregion
    }
}