using OfficeDevPnP.SPOnline.Commands.Base;
using OfficeDevPnP.SPOnline.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;
using OfficeDevPnP.SPOnline.Core;

namespace OfficeDevPnP.SPOnline.Commands
{
    [Cmdlet(VerbsCommon.Remove, "SPOPropertyBagValue", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemovePropertyBagValue : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Key;

        [Parameter(Mandatory = false)]
        public SwitchParameter Force;

        protected override void ExecuteCmdlet()
        {

            if (this.SelectedWeb.PropertyBagContainsKey(Key))
            {
                if (Force || ShouldContinue(string.Format(Properties.Resources.Delete0, Key), Properties.Resources.Confirm))
                {
                    this.SelectedWeb.RemovePropertyBagValue(Key);

                    // Due to some weird bug in CSOM the context will have to be reinitialized.
                    SPOnlineConnection.CurrentConnection = SPOnlineConnectionHelper.InstantiateSPOnlineConnection(
                        new Uri(ClientContext.Url),
                        SPOnlineConnection.CurrentConnection.PSCredential,
                        this.Host,
                        false,
                        SPOnlineConnection.CurrentConnection.OnPrem,
                        SPOnlineConnection.CurrentConnection.MinimalHealthScore,
                        SPOnlineConnection.CurrentConnection.RetryCount,
                        SPOnlineConnection.CurrentConnection.RetryWait,
                        ClientContext.RequestTimeout,
                        false);

                    ClientContext.ExecuteQuery();
                }
            }
        }
    }
}
