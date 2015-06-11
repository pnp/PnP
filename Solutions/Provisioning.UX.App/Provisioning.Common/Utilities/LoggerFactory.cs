using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Utilities
{
    /// <summary>
    /// This is a facade for working with Logging. You can modify this class to implement other logging frameworks.
    /// </summary>
    public static class LoggerFactory
    {
        public static ILog GetLogger()
        {
            return Logger.Instance;
        }
    }
}
