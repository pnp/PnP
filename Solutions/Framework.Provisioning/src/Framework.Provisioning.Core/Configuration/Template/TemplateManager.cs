using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Provisioning.Core.Configuration.Template
{
    /// <summary>
    /// Class for working with Provisioning Templates
    /// </summary>
    public partial class TemplateManager
    {
        internal TemplateConfiguration TemplateConfig = null;

        /// <summary>
        /// Returns a Provisioning Template by Name
        /// Will Return Null if the Template is not found
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public Template GetTemplateByName(string name)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentException(name);
            var _result = TemplateConfig.Templates.FirstOrDefault(t => t.Name == name);
            return _result;
        }

        /// <summary>
        /// Returns a Template object by ID. 
        /// Will return Null if the Template is not found
        /// </summary>
        /// <param name="ID">The ID of the template</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public Template GetTemplateByID(string ID)
        {
            if (String.IsNullOrEmpty(ID)) throw new ArgumentException(ID);
            return TemplateConfig.Templates.FirstOrDefault(t => t.ID == ID);
        }

        /// <summary>
        /// Returns the collection of Templates that are available for creating Web sites within the site collection.
        /// </summary>
        /// <returns></returns>
        public List<Template> GetAvailableTemplates()
        {
            var _t = TemplateConfig.Templates.FindAll(t => t.Enabled == true);
            return _t;
        }

        /// <summary>
        /// Returns the collection of Templates that are available for creating Web sites within the site collection.
        /// </summary>
        /// <returns></returns>
        public List<Template> GetSubSiteTemplates()
        {
            var _t = TemplateConfig.Templates.FindAll(t => t.RootWebOnly == false && t.Enabled == true);
            return _t;
        }

        /// <summary>
        /// Returns a <seealso cref="BrandingPackage"/> that is used for applying branding to a Web site.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BrandingPackage GetBrandingPackageByName(string name)
        {
            var _p = TemplateConfig.BrandingPackage.Find(p => p.Name == name);
            return _p;

        }

        /// <summary>
        /// Returns a collection of <seealso cref="CustomAction"/>. These Custom Actions should be applied to the Web site.
        /// </summary>
        /// <returns></returns>
        public List<CustomAction> GetCustomActions()
        {
            return TemplateConfig.CustomActions.FindAll(c => c.Enabled == true);
        }

    }
}
