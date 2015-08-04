using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Data.Metadata
{
    /// <summary>
    /// TODO
    /// </summary>
    public interface IMetadataFactory
    {
        /// <summary>
        /// Returns an interface for working with the Application Settings Metadata
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Provisioning.Common.Data.DataStoreException">Exception that occurs when interacting with the repository</exception>
        IMetadataManager GetManager();
    }
}
