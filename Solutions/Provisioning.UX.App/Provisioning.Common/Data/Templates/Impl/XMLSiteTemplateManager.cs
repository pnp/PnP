using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using Provisioning.Common.Configuration;
using Provisioning.Common.Configuration.Application;
using Provisioning.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Provisioning.Common.Data.Templates.Impl
{
    /// <summary>
    /// Implementation Class for working with Templates in XML Format
    /// </summary>
    internal class XMLSiteTemplateManager : AbstractModule, ISiteTemplateManager
    {
        #region Instance Members
        internal XMLSiteTemplateData _data = null;
        
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public XMLSiteTemplateManager() : base()
        {  
          
        }
        #endregion

        #region ISiteTemplateManager Members
        public Template GetTemplateByName(string title)
        {
            this.LoadXML();
            if (String.IsNullOrEmpty(title)) throw new ArgumentException("title");
            var _result = _data.Templates.FirstOrDefault(t => t.Title == title);
            return _result;
        }

        public List<Template> GetAvailableTemplates()
        {
            this.LoadXML();
            var _t = _data.Templates.FindAll(t => t.Enabled == true);
            return _t;
        }

        public List<Template> GetSubSiteTemplates()
        {
            this.LoadXML();
            var _t = _data.Templates.FindAll(t => t.RootWebOnly == false && t.Enabled == true);
            return _t;
        }

        public ProvisioningTemplate GetProvisioningTemplate(string name)
        {
            try
            {
                ReflectionManager _reflectionHelper = new ReflectionManager();
                var _provider = _reflectionHelper.GetTemplateProvider(ModuleKeys.PROVISIONINGPROVIDER_KEY);
                var _pt = _provider.GetTemplate(name);
                return _pt;
            }
            catch(Exception _ex)
            {
                var _message = string.Format(PCResources.TemplateProviderBase_Exception_Message, _ex.Message);
                Log.Error("Provisioning.Common.Data.Templates.Impl.XMLSiteTemplateManager", PCResources.TemplateProviderBase_Exception_Message, _ex);
                throw new DataStoreException(_message, _ex);
            }
        }
        
        #endregion

        #region Private Members
        private void LoadXML()
        {
            try
            {
                var _filePath = Path.Combine(this.ConnectionString.HandleEnvironmentToken(), "Templates.config");
                bool _fileExists = System.IO.File.Exists(_filePath);
               
               Log.Info("Provisioning.Common.Data.Templates.Impl.XMLSiteTemplateManager.LoadXML", PCResources.XMLTemplateManager_TryRead_ConfigFile, _filePath);

                if(_fileExists)
                {
                    XDocument _doc = XDocument.Load(_filePath);
                    this._data = XmlSerializerManager.Deserialize<XMLSiteTemplateData>(_doc);
                   Log.Info("Provisioning.Common.Data.Templates.Impl.XMLSiteTemplateManager.LoadXML", PCResources.XMLTemplateManager_Loaded_ConfigFile, _filePath);
                }
                else
                {
                    _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.ConnectionString, "Templates.config");
                   Log.Info("Provisioning.Common.Data.Templates.Impl.XMLSiteTemplateManager.LoadXML", PCResources.XMLTemplateManager_TryRead_ConfigFile, _filePath);
                    _fileExists = System.IO.File.Exists(_filePath);
                    if (_fileExists)
                    {
                       Log.Info("Provisioning.Common.Data.Templates.Impl.XMLSiteTemplateManager.LoadXML", PCResources.XMLTemplateManager_File_Found, _filePath);
                        XDocument _doc = XDocument.Load(_filePath);
                        this._data = XmlSerializerManager.Deserialize<XMLSiteTemplateData>(_doc);
                       Log.Info("Provisioning.Common.Data.Templates.Impl.XMLSiteTemplateManager.LoadXML", PCResources.XMLTemplateManager_Loaded_ConfigFile, _filePath);
                    }
                    else
                    {
                        var _message = string.Format(PCResources.Exception_MasterTemplateNotFound, _filePath);
                        Log.Error("Provisioning.Common.Data.Templates.Impl.XMLSiteTemplateManager.LoadXML", _message);
                        throw new DataStoreException(_message);
                    }
                }
            }
            catch (Exception _ex)
            {
                Log.Error("Provisioning.Common.Data.Templates.Impl.XMLSiteTemplateManager", PCResources.XMLTemplateManager_Error, _ex.Message, _ex.StackTrace);
                throw;
            }

        }
        #endregion

    }
}
