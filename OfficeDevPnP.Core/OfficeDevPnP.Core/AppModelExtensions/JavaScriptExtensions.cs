using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.SharePoint.Client
{
    /// <summary>
    /// JavaScript related methods
    /// </summary>
    public static class JavaScriptExtensions
    {
        public const string SCRIPT_LOCATION = "ScriptLink";

        /// <summary>
        /// Injects links to javascript files via a adding a custom action to the site
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="key">Identifier (key) for the custom action that will be created</param>
        /// <param name="scriptLinks">semi colon delimited list of links to javascript files</param>
        /// <returns>True if action was ok</returns>
        public static bool AddJsLink(this Web web, string key, string scriptLinks)
        {
            return web.AddJsLink(key, new List<string>(scriptLinks.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)));
        }
	
        /// <summary>
        /// Injects links to javascript files via a adding a custom action to the site
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="key">Identifier (key) for the custom action that will be created</param>
        /// <param name="scriptLinks">IEnumerable list of links to javascript files</param>
        /// <returns>True if action was ok</returns>
        public static bool AddJsLink(this Web web, string key, IEnumerable<string> scriptLinks)
        {
            if (scriptLinks.Count() == 0)
            {
                throw new ArgumentException("Parameter scriptLinks can't be empty");
            }

            StringBuilder scripts = new StringBuilder(@" var headID = document.getElementsByTagName('head')[0]; 
var");
            foreach (var link in scriptLinks)
            {
                if (!string.IsNullOrEmpty(link))
                {
                    scripts.AppendFormat(@"
newScript = document.createElement('script');
newScript.type = 'text/javascript';
newScript.src = '{0}';
headID.appendChild(newScript);", link);
                }
                
            }

            bool ret = web.AddJsBlock(key, scripts.ToString());
            return ret;
        }

        /// <summary>
        /// Removes the custom action that triggers the execution of a javascript link
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="key">Identifier (key) for the custom action that will be deleted</param>
        /// <returns>True if action was ok</returns>
        public static bool DeleteJsLink(this Web web, string key)
        {
            var jsAction = new CustomActionEntity()
            {
                Name = key,
                Location = SCRIPT_LOCATION,
                Remove = true
            };
            bool ret = web.AddCustomAction(jsAction);
            return ret;
        }

        /// <summary>
        /// Injects javascript via a adding a custom action to the site
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="key">Identifier (key) for the custom action that will be created</param>
        /// <param name="scriptBlock">Javascript to be injected</param>
        /// <returns>True if action was ok</returns>
        public static bool AddJsBlock(this Web web, string key, string scriptBlock)
        {
            var jsAction = new CustomActionEntity()
            {
                Name = key,
                Location = SCRIPT_LOCATION,
                ScriptBlock = scriptBlock,
            };
            bool ret = web.AddCustomAction(jsAction);
            return ret;
        }

    }
}

