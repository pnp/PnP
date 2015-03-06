using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Provisioning.Core.Utilities
{
    /// <summary>
    /// Helper Class for working with Paths for the Templates
    /// </summary>
    public static class PathHelper
    {
        /// <summary>
        /// Gets the Path to where the dlls are stored
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyDirectory()
        {
            string _codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder _uri = new UriBuilder(_codeBase);
            string path = Uri.UnescapeDataString(_uri.Path);
            return Path.GetDirectoryName(path);
        }
    }
}
