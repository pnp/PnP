using NLog;
using System;
using System.Net;
using System.Threading;
using System.Xml.Serialization;
using System.Collections.Generic;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;


// =======================================================
/// <author>
/// Simon-Pierre Plante (sp.plante@gmail.com)
/// </author>
// =======================================================
namespace PNP.Deployer
{
    [Serializable()]
    public class Sequence
    {
        #region Private Members

        private static Logger logger = LogManager.GetCurrentClassLogger();

        #endregion


        #region Public Members

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("description")]
        public string Description { get; set; }

        [XmlAttribute("webUrl")]
        public string WebUrl { get; set; }

        [XmlAttribute("ignore")]
        public bool Ignore { get; set; }

        [XmlArray("templates")]
        [XmlArrayItem("template", typeof(Template))]
        public List<Template> Templates { get; set; }

        #endregion


        #region Public Methods

        // ===========================================================================================================
        /// <summary>
        /// Launches the current sequence by executing all it's templates 
        /// </summary>
        /// <param name="credentials">The credentials required for creating the client context</param>
        /// <param name="templateProvider">The <b>XMLTemplatePRovider</b> that is mapped to the client's working directory</param>
        // ===========================================================================================================
        public void Launch(ICredentials credentials, XMLTemplateProvider templateProvider)
        {
            if(!this.Ignore)
            {
                logger.Info("Launching sequence '{0}' ({1})", this.Name, this.Description);

                using (ClientContext ctx = new ClientContext(this.WebUrl))
                {
                    // --------------------------------------------------
                    // Sets the context with the provided credentials
                    // --------------------------------------------------
                    ctx.Credentials = credentials;
                    ctx.RequestTimeout = Timeout.Infinite;

                    // --------------------------------------------------
                    // Loads the full web for futur references (providers)
                    // --------------------------------------------------
                    Web web = ctx.Web;
                    ctx.Load(web);
                    ctx.ExecuteQueryRetry();

                    // --------------------------------------------------
                    // Launches the templates
                    // --------------------------------------------------
                    foreach (Template template in this.Templates)
                    {
                        template.Apply(web, templateProvider);
                    }
                }
            }
            else
            {
                logger.Info("Ignoring sequence '{0}' ({1})", this.Name, this.Description);
            }
        }

        #endregion
    }
}
