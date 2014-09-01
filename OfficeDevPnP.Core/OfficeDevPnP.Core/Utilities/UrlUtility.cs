using System;
using System.Web;
using System.Web.UI;

namespace System
{
    /// <summary>
    /// Static methods to modify URL paths.
    /// </summary>
    public static class UrlUtility
    {
        const char PATH_DELIMITER = '/';

        #region [ Combine ]
        /// <summary>
        /// Combines a path and a relative path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="relativePaths"></param>
        /// <returns></returns>
        public static string Combine(string path, params string[] relativePaths) {
            string pathBuilder = path ?? string.Empty;

            if (relativePaths == null)
                return pathBuilder;

            foreach (string relPath in relativePaths) {
                pathBuilder = Combine(pathBuilder, relPath);
            }
            return pathBuilder;
        }
        /// <summary>
        /// Combines a path and a relative path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="relative"></param>
        /// <returns></returns>
        public static string Combine(string path, string relative) 
        {
            if(relative == null)
                relative = String.Empty;
            
            if(path == null)
                path = String.Empty;

            if(relative.Length == 0 && path.Length == 0)
                return String.Empty;

            if(relative.Length == 0)
                return path;

            if(path.Length == 0)
                return relative;

            path = path.Replace('\\', PATH_DELIMITER);
            relative = relative.Replace('\\', PATH_DELIMITER);

            return path.TrimEnd(PATH_DELIMITER) + PATH_DELIMITER + relative.TrimStart(PATH_DELIMITER);
        }
        #endregion

        #region [ AppendQueryString ]
        /// <summary>
        /// Adds query string parameters to the end of a querystring and guarantees the proper concatenation with <b>?</b> and <b>&amp;.</b>
        /// </summary>
        /// <param name="path"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public static string AppendQueryString(string path, string queryString)
        {
            string url = path;

            if (queryString != null && queryString.Length > 0)
            {
                char startChar = (path.IndexOf("?") > 0) ? '&' : '?';
                url = string.Concat(path, startChar, queryString.TrimStart('?'));
            }
            return url;
        }
        #endregion

        #region [ RelativeUrl ]

        public static string MakeRelativeUrl(string urlToProcess) {
            Uri uri = new Uri(urlToProcess);
            return uri.AbsolutePath;
        }

        /// <summary>
        /// Ensures that there is a trailing slash at the end of the url
        /// </summary>
        /// <param name="urlToProcess"></param>
        /// <returns></returns>
        public static string EnsureTrailingSlash(string urlToProcess) 
        {
            if (!urlToProcess.EndsWith("/"))
            {
                return urlToProcess + "/";
            }

            return urlToProcess;
        }
        #endregion

    }
}
