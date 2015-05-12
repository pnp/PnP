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
                XMLFileSystemTemplateProvider _xmlProvider = new XMLFileSystemTemplateProvider(this.ConnectionString, this.Container);
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
                var _fullFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.ConnectionString, "Templates.config");
               
                Log.Info("Provisioning.Common.Data.Templates.Impl.XMLSiteTemplateManager.LoadXML", PCResources.XMLTemplateManager_TryRead_ConfigFile, _fullFilePath);
                bool _fileExist = System.IO.File.Exists(_fullFilePath);

                if (_fileExist)
                {
                    Log.Info("Provisioning.Common.Data.Templates.Impl.XMLSiteTemplateManager.LoadXML", "Found Master Template file {0}", _fullFilePath);
                    XDocument _doc = XDocument.Load(_fullFilePath);
                    this._data = XmlSerializerHelper.Deserialize<XMLSiteTemplateData>(_doc);
                    Log.Info("Provisioning.Common.Data.Templates.Impl.XMLSiteTemplateManager.LoadXML", "Loaded Configuration File {0} for templates", _fullFilePath);
                }
                else
                {  
                   Log.Fatal("Provisioning.Common.Data.Templates.Impl.XMLSiteTemplateManager.LoadXML", "Did not find Master Template file {0}", _fullFilePath);
                   throw new Exception();
                   
                }
            }
            catch (Exception _ex)
            {
                Log.Fatal("Provisioning.Common.Data.Templates.Impl.XMLSiteTemplateManager", PCResources.XMLTemplateManager_Error, _ex.Message, _ex.StackTrace);
                throw;
            }

        }
        #endregion

    }
}
