using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Contoso.Patterns.Provisioning
{
    /// <summary>
    /// Class responsible for delivering the site configuration information. Currently simple, but 
    /// can be used to insert additional logic like "trimming" away site configuration information 
    /// for certain user groups
    /// </summary>
    public class SiteProvisioningConfiguration
    {
        #region private variables
        private Configurations config = null;
        #endregion

        #region Constructor
        public SiteProvisioningConfiguration(string configurationFile)
        {
            string xml = System.IO.File.ReadAllText(configurationFile);
            this.config = xml.ParseXML<Configurations>();
            PrepareSiteConfigurationData();
        }
        #endregion

        #region public properties
        public Configurations Config
        {
            get
            {
                return this.config;
            }
        }
        #endregion

        #region public methods
        public void PrepareSiteConfigurationData()
        {
            // add code that manipulates the currently loaded site configuration

        }

        public ConfigurationsTemplate LoadTemplate(string templateName)
        {
            ConfigurationsTemplate template = null;
            try
            {
                template = this.config.Templates.Single(p => p.Name.Equals(templateName, StringComparison.InvariantCultureIgnoreCase)
                                                                          && Convert.ToBoolean(p.Enabled) == true);
            }
            catch(InvalidOperationException)
            {
                return null;
            }

            return template;
        }

        public bool TemplateExists(string templateName)
        {
            ConfigurationsTemplate t = LoadTemplate(templateName);
            if (t != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region helper methods


        #endregion



    }
}
