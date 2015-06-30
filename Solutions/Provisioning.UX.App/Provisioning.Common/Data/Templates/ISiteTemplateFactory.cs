using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Data.Templates
{
    public interface ISiteTemplateFactory
    {
        /// <summary>
        /// Returns an interface for working Site Templates
        /// </summary>
        /// <returns></returns>
        ISiteTemplateManager GetManager();
    }
}
