using OfficeDevPnP.Core.Framework.Provisioning.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Data.Templates
{
    /// <summary>
    /// Interface
    /// </summary>
    public interface ISiteTemplateManager
    {
        /// <summary>
        /// Returns a Provisioning Template by Name
        /// Will Return Null if the Template is not found
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        Template GetTemplateByName(string title);

        /// <summary>
        /// Returns the collection of Templates that are available for creating Web sites within the site collection.
        /// </summary>
        /// <returns></returns>
        List<Template> GetAvailableTemplates();

        /// <summary>
        /// Returns the collection of Templates that are available for creating Web sites within the site collection.
        /// </summary>
        /// <returns></returns>
        List<Template> GetSubSiteTemplates();

        /// <summary>
        /// Returns the Provisioning Template by Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ProvisioningTemplate GetProvisioningTemplate(string name);

    }
}
