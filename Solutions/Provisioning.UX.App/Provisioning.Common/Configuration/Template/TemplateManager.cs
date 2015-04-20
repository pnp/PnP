using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Configuration.Template
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
        /// <param name="title"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public Template GetTemplateByName(string title)
        {
            if (String.IsNullOrEmpty(title)) throw new ArgumentException(title);
            var _result = TemplateConfig.Templates.FirstOrDefault(t => t.Title == title);
            return _result;
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

    

    }
}
