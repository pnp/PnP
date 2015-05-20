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
    public static partial class JavaScriptExtensions
    {
        public const string SCRIPT_LOCATION = "ScriptLink";

        /// <summary>
        /// Injects links to javascript files via a adding a custom action to the site
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="key">Identifier (key) for the custom action that will be created</param>
        /// <param name="scriptLinks">semi colon delimited list of links to javascript files</param>
        /// <param name="sequence"></param>
        /// <returns>True if action was ok</returns>
        public static bool AddJsLink(this Web web, string key, string scriptLinks, int sequence = 0)
        {
            return AddJsLinkImplementation(web, key, new List<string>(scriptLinks.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)), sequence);
        }

        /// <summary>
        /// Injects links to javascript files via a adding a custom action to the site
        /// </summary>
        /// <param name="site">Site to be processed</param>
        /// <param name="key">Identifier (key) for the custom action that will be created</param>
        /// <param name="scriptLinks">semi colon delimited list of links to javascript files</param>
        /// <param name="sequence"></param>
        /// <returns>True if action was ok</returns>
        public static bool AddJsLink(this Site site, string key, string scriptLinks, int sequence = 0)
        {
            return AddJsLinkImplementation(site, key, new List<string>(scriptLinks.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)), sequence);
        }


        /// <summary>
        /// Injects links to javascript files via a adding a custom action to the site
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="key">Identifier (key) for the custom action that will be created</param>
        /// <param name="scriptLinks">IEnumerable list of links to javascript files</param>
        /// <param name="sequence"></param>
        /// <returns>True if action was ok</returns>
        public static bool AddJsLink(this Web web, string key, IEnumerable<string> scriptLinks, int sequence = 0)
        {
            return AddJsLinkImplementation(web, key, scriptLinks, sequence);
        }

        /// <summary>
        /// Injects links to javascript files via a adding a custom action to the site
        /// </summary>
        /// <param name="site">Site to be processed</param>
        /// <param name="key">Identifier (key) for the custom action that will be created</param>
        /// <param name="scriptLinks">IEnumerable list of links to javascript files</param>
        /// <param name="sequence"></param>
        /// <returns>True if action was ok</returns>
        public static bool AddJsLink(this Site site, string key, IEnumerable<string> scriptLinks, int sequence = 0)
        {
            return AddJsLinkImplementation(site, key, scriptLinks, sequence);
        }

        private static bool AddJsLinkImplementation(ClientObject clientObject, string key, IEnumerable<string> scriptLinks, int sequence)
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
var scripts = document.getElementsByTagName('script');
var scriptsSrc = [];
for(var i = 0; i < scripts.length; i++) {
    if(scripts[i].type === 'text/javascript'){
        scriptsSrc.push(scripts[i].src);
    }
}
");
                foreach (var link in scriptLinksEnumerable)
                {
                    if (!string.IsNullOrEmpty(link))
                    {
                        scripts.AppendFormat(@"
if (scriptsSrc.indexOf('{0}') === -1) {
    var newScript = document.createElement('script');
    newScript.type = 'text/javascript';
    newScript.src = '{0}';
    headID.appendChild(newScript);
    scriptsSrc.push('{0}');
}
", link);
                    }

                }

                ret = AddJsBlockImplementation(clientObject, key, scripts.ToString(), sequence);

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
                    Remove = true,
                };
                if (clientObject is Web)
                {
                    ret = ((Web)clientObject).AddCustomAction(jsAction);
                }
                else
                {
                    ret = ((Site)clientObject).AddCustomAction(jsAction);
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
        /// <param name="sequence"></param>
        /// <returns>True if action was ok</returns>
        public static bool AddJsBlock(this Web web, string key, string scriptBlock, int sequence = 0)
        {
            return AddJsBlockImplementation(web, key, scriptBlock, sequence);

        }

        /// <summary>
        /// Injects javascript via a adding a custom action to the site
        /// </summary>
        /// <param name="site">Site to be processed</param>
        /// <param name="key">Identifier (key) for the custom action that will be created</param>
        /// <param name="scriptBlock">Javascript to be injected</param>
        /// <param name="sequence"></param>
        /// <returns>True if action was ok</returns>
        public static bool AddJsBlock(this Site site, string key, string scriptBlock, int sequence = 0)
        {
            return AddJsBlockImplementation(site, key, scriptBlock, sequence);
        }

        private static bool AddJsBlockImplementation(ClientObject clientObject, string key, string scriptBlock, int sequence)
        {
            var ret = false;
            if (clientObject is Web || clientObject is Site)
            {
                var jsAction = new CustomActionEntity()
                {
                    Name = key,
                    Location = SCRIPT_LOCATION,
                    ScriptBlock = scriptBlock,
                    Sequence = sequence
                };
                if (clientObject is Web)
                {
                    ret = ((Web)clientObject).AddCustomAction(jsAction);
                }
                else
                {
                    ret = ((Site)clientObject).AddCustomAction(jsAction);
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

