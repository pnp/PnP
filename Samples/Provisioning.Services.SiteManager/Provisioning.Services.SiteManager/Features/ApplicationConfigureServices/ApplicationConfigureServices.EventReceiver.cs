using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Contoso.Provisioning.Services.SiteManager.ApplicationLogic;

namespace Contoso.Services.SiteManager.Features.ApplicationConfigureServices
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("529590ed-c5bc-47fc-af94-36bedfc717c0")]
    public class ApplicationConfigureServicesEventReceiver : SPFeatureReceiver
    {
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            try
            {
                ConfigurationManager.ConfigureRemoteManagerTimeout("SiteManager.svc");
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Exception occured during activation of Contoso.Services.SiteManager.ApplicationConfigureServices", ex);
            }
        }

    }
}
