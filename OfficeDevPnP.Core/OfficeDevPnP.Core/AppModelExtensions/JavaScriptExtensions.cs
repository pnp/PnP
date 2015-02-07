using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OfficeDevPnP.Core.Entities;

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
            return AddJsLinkImplementation(web, key, new List<string>(scriptLinks.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)));
        }

        /// <summary>
        /// Injects links to javascript files via a adding a custom action to the site
        /// </summary>
        /// <param name="site">Site to be processed</param>
        /// <param name="key">Identifier (key) for the custom action that will be created</param>
        /// <param name="scriptLinks">semi colon delimited list of links to javascript files</param>
        /// <returns>True if action was ok</returns>
        public static bool AddJsLink(this Site site, string key, string scriptLinks)
        {
            return AddJsLinkImplementation(site, key, new List<string>(scriptLinks.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)));
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
            return AddJsLinkImplementation(web,key,scriptLinks);
        }

        /// <summary>
        /// Injects links to javascript files via a adding a custom action to the site
        /// </summary>
        /// <param name="site">Site to be processed</param>
        /// <param name="key">Identifier (key) for the custom action that will be created</param>
        /// <param name="scriptLinks">IEnumerable list of links to javascript files</param>
        /// <returns>True if action was ok</returns>
        public static bool AddJsLink(this Site site, string key, IEnumerable<string> scriptLinks)
        {
            return AddJsLinkImplementation(site, key, scriptLinks);
        }

        private static bool AddJsLinkImplementation(ClientObject clientObject, string key, IEnumerable<string> scriptLinks)
        {
            var ret = false;
            if (clientObject is Web || clientObject is Site)
            {
                var scriptLinksEnumerable = scriptLinks as string[] ?? scriptLinks.ToArray();
                if (!scriptLinksEnumerable.Any())
                {
                    throw new ArgumentException("Parameter scriptLinks can't be empty");
                }

                var scripts = new StringBuilder(@" var headID = document.getElementsByTagName('head')[0]; 
var");
                foreach (var link in scriptLinksEnumerable)
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

                ret = AddJsBlockImplementation(clientObject, key, scripts.ToString());

            }
            else
            {
                throw new ArgumentException("Only Site or Web supported as clientObject");

            }
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
            return DeleteJsLinkImplementation(web, key);
        }

        /// <summary>
        /// Removes the custom action that triggers the execution of a javascript link
        /// </summary>
        /// <param name="site">Site to be processed</param>
        /// <param name="key">Identifier (key) for the custom action that will be deleted</param>
        /// <returns>True if action was ok</returns>
        public static bool DeleteJsLink(this Site site, string key)
        {
            return DeleteJsLinkImplementation(site, key);
        }

        private static bool DeleteJsLinkImplementation(ClientObject clientObject, string key)
        {
            var ret = false;
            if (clientObject is Web || clientObject is Site)
            {
                var jsAction = new CustomActionEntity()
                {
                    Name = key,
                    Location = SCRIPT_LOCATION,
                    Remove = true
                };
                if (clientObject is Web)
                {
                    ret = ((Web) clientObject).AddCustomAction(jsAction);
                }
                else
                {
                    ret = ((Site) clientObject).AddCustomAction(jsAction);
                }

            }
            else
            {
                throw new ArgumentException("Only Site or Web supported as clientObject");
            }
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
            return AddJsBlockImplementation(web, key, scriptBlock);
            
        }

        /// <summary>
        /// Injects javascript via a adding a custom action to the site
        /// </summary>
        /// <param name="site">Site to be processed</param>
        /// <param name="key">Identifier (key) for the custom action that will be created</param>
        /// <param name="scriptBlock">Javascript to be injected</param>
        /// <returns>True if action was ok</returns>
        public static bool AddJsBlock(this Site site, string key, string scriptBlock)
        {
            return AddJsBlockImplementation(site, key, scriptBlock);
        }

        private static bool AddJsBlockImplementation(ClientObject clientObject, string key, string scriptBlock)
        {
            var ret = false;
            if (clientObject is Web || clientObject is Site)
            {
                var jsAction = new CustomActionEntity()
                {
                    Name = key,
                    Location = SCRIPT_LOCATION,
                    ScriptBlock = scriptBlock,
                };
                if (clientObject is Web)
                {
                    ret = ((Web) clientObject).AddCustomAction(jsAction);
                }
                else
                {
                    ret = ((Site) clientObject).AddCustomAction(jsAction);
                }
            }
            else
            {
                throw new ArgumentException("Only Site or Web supported as clientObject");
            }
            return ret;
        }
    }
}

