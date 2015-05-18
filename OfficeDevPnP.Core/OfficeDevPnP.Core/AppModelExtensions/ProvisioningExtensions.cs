using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Utilities;

namespace Microsoft.SharePoint.Client
{
    /// <summary>
    /// File-based (CAML) provisioning extensions
    /// </summary>
    public static partial class ProvisioningExtensions
    {
        const string SharePointNamespaceName = "http://schemas.microsoft.com/sharepoint/";

        /// <summary>
        /// Provisions the items defined by the specified Elements (CAML) file; currently only supports modules (files).
        /// </summary>
        /// <param name="web">Web to provision the elements to</param>
        /// <param name="path">Path to the XML file containing the Elements CAML defintion</param>
        public static void ProvisionElementFile(this Web web, string path)
        {
            if (path == null) { throw new ArgumentNullException("path"); }
            if (string.IsNullOrWhiteSpace(path)) { throw new ArgumentException(CoreResources.ProvisioningExtensions_ProvisionElementFile_Path_to_the_element_file_is_required, "path"); }

            Log.Info(Constants.LOGGING_SOURCE, CoreResources.ProvisioningExtensions_ProvisionElementFile0, path);

            var baseFolder = Path.GetDirectoryName(path);
            using (var sr = System.IO.File.OpenText(path))
            {
                var xdoc = XDocument.Load(sr);
                var xml = xdoc.Root;
                ProvisionElementXml(web, baseFolder, xml);
            }
        }

        /// <summary>
        /// Provisions the items defined by the specified Elements (CAML) XML; currently only supports modules (files).
        /// </summary>
        /// <param name="web">Web to provision the elements to</param>
        /// <param name="baseFolder">Base local folder to find any referenced items, e.g. files</param>
        /// <param name="elementsXml">Elements (CAML) XML element that defines the items to provision; currently only supports modules (files)</param>
        public static void ProvisionElementXml(this Web web, string baseFolder, XElement elementsXml)
        {
            // TODO: Maybe some sort of stream provider for resolving references (instead of baseFolder)
            if (elementsXml == null) { throw new ArgumentNullException("elementsXml"); }
            if (elementsXml.Name != XName.Get("Elements", SharePointNamespaceName))
            {
                throw new ArgumentException(CoreResources.ProvisioningExtensions_ProvisionElementXml_Expected_element__Elements__, "xml");
            }

            foreach (var child in elementsXml.Elements())
            {
                if (child.Name == XName.Get("Module", SharePointNamespaceName))
                {
                    ProvisionModuleInternal(web, baseFolder, child);
                }
                else
                {
                    throw new NotSupportedException(string.Format("Elements child '{0}' not supported.", child.Name));
                }
            }
        }

        /// <summary>
        /// Uploads all files defined by the moduleXml
        /// </summary>
        static void ProvisionModuleInternal(this Web web, string baseFolder, XElement moduleXml)
        {
            if (moduleXml == null) { throw new ArgumentNullException("module"); }
            if (moduleXml.Name != XName.Get("Module", SharePointNamespaceName))
            {
                throw new ArgumentException(CoreResources.ProvisioningExtensions_ProvisionModuleInternal_Expected_element__Module__, "module");
            }

            var name = moduleXml.Attribute("Name").Value;
            var moduleBaseUrl = moduleXml.Attribute("Url").Value;
            var modulePath = moduleXml.Attribute("Path").Value;
            var moduleBaseFolder = Path.Combine(baseFolder, modulePath);

            Log.Debug(Constants.LOGGING_SOURCE, "Provisioning module '{0}'", name);

            foreach (var child in moduleXml.Elements())
            {
                if (child.Name == XName.Get("File", SharePointNamespaceName))
                {
                    var filePath = child.Attribute("Path").Value;
                    try
                    {
                        ProvisionFileInternal(web, moduleBaseUrl, moduleBaseFolder, child);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(Constants.LOGGING_SOURCE, CoreResources.ProvisioningExtensions_ErrorProvisioningModule0File1, name, filePath, ex.Message);
                    }
                }
                else
                {
                    throw new NotSupportedException(string.Format("Module child '{0}' not supported.", child.Name));
                }
            }
        }


        /// <summary>
        /// Uploads the file defined by the fileXml, creating folders as necessary.
        /// </summary>
        static File ProvisionFileInternal(this Web web, string baseUrl, string baseFolder, XElement fileXml, bool useWebDav = true)
        {
            if (fileXml == null) { throw new ArgumentNullException("fileXml"); }
            if (fileXml.Name != XName.Get("File", SharePointNamespaceName))
            {
                throw new ArgumentException(CoreResources.ProvisioningExtensions_ProvisionFileInternal_Expected_element__File__, "file");
            }

            var fileUrl = fileXml.Attribute("Url").Value;
            var filePath = fileXml.Attribute("Path").Value;
            var replaceContent = string.Equals(fileXml.Attribute("ReplaceContent").Value, "true", StringComparison.InvariantCultureIgnoreCase);
            var fileLevel = fileXml.Attribute("Level").Value;
            FileLevel level;
            if (!Enum.TryParse<FileLevel>(fileLevel, out level))
            {
                level = FileLevel.Published;
            }

            var webRelativeUrl = baseUrl + (baseUrl.EndsWith("/") ? "" : "/") + fileUrl;
            var path = Path.Combine(baseFolder, filePath);

            var propertyDictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            var skipProperties = new List<string>() { "ContentType", "FileDirRef", "FileLeafRef", "_ModerationStatus", "FSObjType" };
            foreach (var child in fileXml.Elements())
            {
                if (child.Name == XName.Get("Property", SharePointNamespaceName))
                {
                    var propertyName = child.Attribute("Name").Value;
                    if (skipProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase))
                    {
                        Log.Debug(Constants.LOGGING_SOURCE, "Skipping property known to cause issues '{0}'", propertyName);
                        //Console.WriteLine("Skipping property '{0}'", propertyName);
                    }
                    else
                    {
                        var propertyValue = child.Attribute("Value").Value;
                        propertyDictionary[propertyName] = propertyValue;
                    }
                }
            }

            string fileName = Path.GetFileName(webRelativeUrl);
            var folderWebRelativeUrl = webRelativeUrl.Substring(0, webRelativeUrl.Length - fileName.Length);
            Folder folder = web.EnsureFolderPath(folderWebRelativeUrl);

            // perform all operations that used to be done in UploadFile
            // Check to see that the file doesn't already exist.
            var file = folder.GetFile(fileName);
            var uploadRequired = true;

            // If file exists, verify the files aren't the same.
            if (file != null)
                uploadRequired = file.VerifyIfUploadRequired(path);
            
            // Upload the file, if required, using the specified process for upload.
            if (uploadRequired) {
                if (useWebDav)
                    file = folder.UploadFileWebDav(fileName, path, replaceContent);
                else
                    file = folder.UploadFile(fileName, path, replaceContent);
            }
            // Set file properties after upload
            file.SetFileProperties(propertyDictionary);

            // Publish the file
            file.PublishFileToLevel(level);

            return file;
        }

    }
}
