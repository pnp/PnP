using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Utilities;

namespace Microsoft.SharePoint.Client
{
    /// <summary>
    /// File-based (CAML) provisioning extensions
    /// </summary>
    public static class ProvisioningExtensions
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
            if (string.IsNullOrWhiteSpace(path)) { throw new ArgumentException("Path to the element file is required", "path"); }

            LoggingUtility.Internal.TraceInformation((int)EventId.ProvisionElementFile, CoreResources.ProvisioningExtensions_ProvisionElementFile0, path);

            var baseFolder = System.IO.Path.GetDirectoryName(path);
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
            if (elementsXml == null) { throw new ArgumentNullException("xml"); }
            if (elementsXml.Name != XName.Get("Elements", SharePointNamespaceName))
            {
                throw new ArgumentException("Expected element 'Elements'.", "xml");
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
                throw new ArgumentException("Expected element 'Module'.", "module");
            }

            var name = moduleXml.Attribute("Name").Value;
            var moduleBaseUrl = moduleXml.Attribute("Url").Value;
            var modulePath = moduleXml.Attribute("Path").Value;
            var moduleBaseFolder = System.IO.Path.Combine(baseFolder, modulePath);

            LoggingUtility.Internal.TraceVerbose("Provisioning module '{0}'", name);

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
                        LoggingUtility.Internal.TraceError((int)EventId.ProvisionModuleFileError, ex, CoreResources.ProvisioningExtensions_ErrorProvisioningModule0File1, name, filePath);
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
            if (fileXml == null) { throw new ArgumentNullException("file"); }
            if (fileXml.Name != XName.Get("File", SharePointNamespaceName))
            {
                throw new ArgumentException("Expected element 'File'.", "file");
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
            var path = System.IO.Path.Combine(baseFolder, filePath);

            var propertyDictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            var skipProperties = new List<string>() { "ContentType", "FileDirRef", "FileLeafRef", "_ModerationStatus", "FSObjType" };
            foreach (var child in fileXml.Elements())
            {
                if (child.Name == XName.Get("Property", SharePointNamespaceName))
                {
                    var propertyName = child.Attribute("Name").Value;
                    if (skipProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase))
                    {
                        LoggingUtility.Internal.TraceVerbose("Skipping property known to cause issues '{0}'", propertyName);
                        //Console.WriteLine("Skipping property '{0}'", propertyName);
                    }
                    else
                    {
                        var propertyValue = child.Attribute("Value").Value;
                        propertyDictionary[propertyName] = propertyValue;
                    }
                }
            }

            string fileName = System.IO.Path.GetFileName(webRelativeUrl);
            var folderWebRelativeUrl = webRelativeUrl.Substring(0, webRelativeUrl.Length - fileName.Length);
            Folder folder = web.EnsureFolderPath(folderWebRelativeUrl);

            var checkHashBeforeUpload = true;
            return folder.UploadFile(fileName, path, propertyDictionary, replaceContent, checkHashBeforeUpload, level, useWebDav);
        }

    }
}
