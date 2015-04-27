using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using Provisioning.Common.Configuration.Application;
using Provisioning.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Provisioning.Common.Configuration.Template.Impl
{
    internal class XMLSiteTemplateManager : ISiteTemplateManager
    {
        internal XMLSiteTemplateData _data = null;
        const string PROVISIONINGTEMPLATES_XML_CONTAINER = "Resources/SiteTemplates/ProvisioningTemplates/";
     
        #region Constructor
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public XMLSiteTemplateManager()
        {
            this.LoadXML();
        }
        #endregion

        #region ISiteTemplateManager Members
        public Template GetTemplateByName(string title)
        {
            if (String.IsNullOrEmpty(title)) throw new ArgumentException(title);
            var _result = _data.Templates.FirstOrDefault(t => t.Title == title);
            return _result;
        }

        public List<Template> GetAvailableTemplates()
        {
            var _t = _data.Templates.FindAll(t => t.Enabled == true);
            return _t;
        }

        public List<Template> GetSubSiteTemplates()
        {
            var _t = _data.Templates.FindAll(t => t.RootWebOnly == false && t.Enabled == true);
            return _t;
        }

        public ProvisioningTemplate GetProvisioningTemplate(string name)
        {
            try
            { 
                XMLFileSystemTemplateProvider _xmlProvider = new XMLFileSystemTemplateProvider(PROVISIONINGTEMPLATES_XML_CONTAINER, string.Empty);
                var _pt = _xmlProvider.GetTemplate(name);
                return _pt;
            }
            catch(Exception _ex)
            {
                throw;
            }
        }
        
        #endregion

        #region Private Members
        private void LoadXML()
        {
            try
            {
                var _fullFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/SiteTemplates/Templates.config");
                Log.Debug("Provisioning.Common.Configuration.Template.Impl.XMLSiteTemplateManager", PCResources.XMLTemplateManager_TryRead_ConfigFile, _fullFilePath);

                bool _fileExist = System.IO.File.Exists(_fullFilePath);

                if (_fileExist)
                {
                    Log.Debug("Provisioning.Common.Configuration.Template.Impl.XMLSiteTemplateManager", "Found Master Template file {0}", _fullFilePath);
                    XDocument _doc = XDocument.Load(_fullFilePath);

                    this._data = XmlSerializerHelper.Deserialize<XMLSiteTemplateData>(_doc);
                    Log.Debug("Provisioning.Common.Configuration.Template.Impl.XMLSiteTemplateManager", "Loaded Configuration File {0} for templates", _fullFilePath);
                }
                else
                {
                    Log.Fatal("Provisioning.Common.Configuration.Template.Impl.XMLSiteTemplateManager", "Did not find Master Template file {0}", _fullFilePath);
                    throw new Exception();
                }
            }
            catch (Exception _ex)
            {
                Log.Fatal("Provisioning.Common.Configuration.Template.Impl.XMLSiteTemplateManager", PCResources.XMLTemplateManager_Error, _ex.Message, _ex.StackTrace);
                throw;
            }

        }
        #endregion

    }
}
