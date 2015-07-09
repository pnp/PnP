using OfficeDevPnP.Core.Framework.Provisioning.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.Providers
{
    /// <summary>
    /// Interface for basic capabilites that any Template Formatter should provide/support
    /// </summary>
    public interface ITemplateFormatter
    {
        /// <summary>
        /// Method to initialize the formatter with the proper TemplateProvider instance
        /// </summary>
        /// <param name="provider">The provider that is calling the current template formatter</param>
        void Initialize(TemplateProviderBase provider);

        /// <summary>
        /// Method to validate the content of a formatted template instace
        /// </summary>
        /// <param name="template">The formatted template instance as a Stream</param>
        /// <returns>Boolean result of the validation</returns>
        Boolean IsValid(Stream template);

        /// <summary>
        /// Method to format a ProvisioningTemplate into a formatted template
        /// </summary>
        /// <param name="template">The input ProvisioningTemplate</param>
        /// <returns>The output formatted template as a Stream</returns>
        Stream ToFormattedTemplate(ProvisioningTemplate template);

        /// <summary>
        /// Method to convert a formatted template into a ProvisioningTemplate
        /// </summary>
        /// <param name="template">The input formatted template as a Stream</param>
        /// <returns>The output ProvisioningTemplate</returns>
        ProvisioningTemplate ToProvisioningTemplate(Stream template);

        /// <summary>
        /// Method to convert a formatted template into a ProvisioningTemplate, based on a specific ID
        /// </summary>
        /// <param name="template">The input formatted template as a Stream</param>
        /// <param name="identifier">The identifier of the template to convert</param>
        /// <returns>The output ProvisioningTemplate</returns>
        ProvisioningTemplate ToProvisioningTemplate(Stream template, String identifier);
    }
}
