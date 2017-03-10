using Microsoft.SharePoint;
using NLog;
using PNP.Deployer.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNP.Deployer.ExtensibilityProviders.SSOM
{
    // =================================================
    /// <author>
    /// Simon-Pierre Plante (sp.plante@gmail.com)
    /// </author>
    // =================================================
    public static class SPFieldExtensions
    {
        #region Private Members

        private static Logger logger = LogManager.GetCurrentClassLogger();

        #endregion


        #region Public Methods

        // =========================================================================================================
        /// <summary>
        /// Configures the TitleResource of the given <b>SPField</b>s
        /// </summary>
        /// <param name="field"></param>
        // =========================================================================================================
        public static void ConfigureFieldTitleResource(this SPField field, List<ResourceConfig> titleResources)
        {
            logger.Info("Configuring title resources for field '{0}'", field.InternalName);

            foreach(ResourceConfig titleResource in titleResources)
            {
                field.TitleResource.SetValueForUICulture(new CultureInfo(titleResource.LCID), titleResource.Value);
            }
            field.Update(true);
        }

        #endregion
    }
}
