using Framework.Provisioning.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Framework.Provisioning.Core.Configuration.Template.Impl
{
    internal sealed class XMLTemplateManager : ITemplateFactory
    {
        #region Instance Members
        private static readonly object _lock = new object();
        private static bool _isInit = false;
        private TemplateManager _tm = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        XMLTemplateManager()
        {
           
        }
        #endregion

        internal static XMLTemplateManager Instance { get {return XMLManager.Instance;} 
        }

        #region public Members
        public static void Init()
        {
            lock(_lock)
            {
                if(!_isInit)
                {
                    Instance.LoadXML();
                }
            }
        }

        public TemplateManager GetTemplateManager()
        {
            return _tm;
        }
        #endregion

        #region Private Members
        private void LoadXML()
        {
            try 
            {
                var _path = PathHelper.GetAssemblyDirectory();

                string _assemblyPath = PathHelper.GetAssemblyDirectory();
                var _fullFilePath = System.IO.Path.Combine(_assemblyPath, "Resources/Templates/Templates.config");
                Log.Debug("Framework.Provisioning.Core.Configuration.Template.Impl.XMLTemplateManager", PCResources.XMLTemplateManager_TryRead_ConfigFile, _fullFilePath);
                bool _fileExist = System.IO.File.Exists(_fullFilePath);
             
                if(_fileExist)
                {
                    Log.Debug("Framework.Provisioning.Core.Configuration.Template.Impl.XMLTemplateManager", "Found Master Template file {0}", _fullFilePath);
                    XDocument _doc = XDocument.Load(_fullFilePath);

                    var _templateConfig = XmlSerializerHelper.Deserialize<TemplateConfiguration>(_doc);
                    this._tm = new TemplateManager();
                    _tm.TemplateConfig = _templateConfig;
                    _isInit = true;
                    Log.Debug("Framework.Provisioning.Core.Configuration.Template.Impl.XMLTemplateManager", "Loaded Configuration File {0} for templates", _fullFilePath);
                }
                else
                {
                    Log.Fatal("Framework.Provisioning.Core.Configuration.Template.Impl.XMLTemplateManager", "Did not find Master Template file {0}", _fullFilePath);
                    throw new Exception();
                }
            }
            catch(Exception _ex)
            {
                Log.Fatal("Framework.Provisioning.Core.Configuration.Template.Impl.XMLTemplateManager", PCResources.XMLTemplateManager_Error, _ex.Message, _ex.StackTrace);           
                _isInit = false;
                throw;
            }

        }
        #endregion

        /// <summary>
        /// Private class for Nested Singleton to support initialization
        /// </summary>
        private class XMLManager
        {
            internal static readonly XMLTemplateManager Instance = new XMLTemplateManager();
            static XMLManager()
            {
            }
            XMLManager()
            {
           
            }
        }
    }
}
