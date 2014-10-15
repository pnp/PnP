using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Contoso.Intranet.Code;

namespace Contoso.Intranet.Features.WebDefaultPublishing
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("2cbd2dfd-6c77-47d6-a79f-12a8a757dc00")]
    public class WebDefaultPublishingEventReceiver : SPFeatureReceiver
    {
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            base.FeatureActivated(properties);
            try
            {
                if (properties.Feature.Parent is SPWeb)
                {
                    SPWeb web = (SPWeb)properties.Feature.Parent;
                    SiteManager.PublishingSiteWebFeatureActivatedHandler(web, properties.Feature.Properties);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Exception occured during activation of UPM.Intranet.Workspace - Web - Team Default-feature.", ex);
            }
        }
    }
}
