using System;
using System.Collections.Generic;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;

namespace OfficeDevPnP.Core.Framework.Provisioning.Providers
{
    public abstract class TemplateProviderBase
    {
        private Dictionary<string, string> _parameters = new Dictionary<string, string>();
        private bool _supportSave = false;
        private bool _supportDelete = false;
        private FileConnectorBase _connector = null;
        private string _identifier = "";

        #region Constructors
        
        public TemplateProviderBase()
        {

        }

        public TemplateProviderBase(FileConnectorBase connector)
        {
            this._connector = connector;
        }

        #endregion

        #region Public Properties

        public Dictionary<string, string> Parameters
        {
            get
            {
                return this._parameters;
            }
        }

        public virtual bool SupportsSave
        {
            get
            {
                return _supportSave;
            }
        }

        public virtual bool SupportsDelete
        {
            get
            {
                return _supportDelete;
            }
        }

        public virtual FileConnectorBase Connector
        {
            get
            {
                return _connector;
            }
            set
            {
                _connector = value;
            }
        }

        public String Identifier
        {
            get
            {
                return _identifier;
            }
            set
            {
                _identifier = value;
            }
        }

        #endregion

        #region Abstract Methods

        public abstract List<ProvisioningTemplate> GetTemplates();

        public abstract List<ProvisioningTemplate> GetTemplates(ITemplateFormatter formatter);

        public abstract ProvisioningTemplate GetTemplate(string uri);

        public abstract ProvisioningTemplate GetTemplate(string uri, string identifier);

        public abstract ProvisioningTemplate GetTemplate(string uri, ITemplateFormatter formatter);

        public abstract ProvisioningTemplate GetTemplate(string uri, string identifier, ITemplateFormatter formatter);

        public abstract void Save(ProvisioningTemplate template);

        public abstract void Save(ProvisioningTemplate template, ITemplateFormatter formatter);

        public abstract void SaveAs(ProvisioningTemplate template, string uri);

        public abstract void SaveAs(ProvisioningTemplate template, string uri, ITemplateFormatter formatter);

        public abstract void Delete(string uri);

        #endregion
    }
}
