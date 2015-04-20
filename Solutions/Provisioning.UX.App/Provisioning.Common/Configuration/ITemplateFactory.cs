using Provisioning.Common.Configuration.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Configuration
{
    /// <summary>
    /// Interface for Creating the TemplateManager 
    /// </summary>
    public interface ITemplateFactory
    {
        /// <summary>
        /// Returns an TemplateManager for working with Site Templates
        /// </summary>
        /// <returns></returns>
        TemplateManager GetTemplateManager();
    }
}
