using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Contoso.Intranet.Code;

namespace Contoso.Intranet.Features.WebDefault
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("ae822783-5f1d-4b8c-a00d-d7de4d060ad7")]
    public class WebDefaultEventReceiver : SPFeatureReceiver
    {

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            base.FeatureActivated(properties);
            try
            {
                if (properties.Feature.Parent is SPWeb)
                {
                    SPWeb web = (SPWeb)properties.Feature.Parent;
                    SiteManager.TeamSiteWebFeatureActivatedHandler(web, properties.Feature.Properties);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Exception occured during activation of UPM.Intranet.Workspace - Web - Team Default-feature.", ex);
            }
        }
    }
}
