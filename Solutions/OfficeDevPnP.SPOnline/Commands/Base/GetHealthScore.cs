using OfficeDevPnP.SPOnline.CmdletHelpAttributes;
using System;
using System.Management.Automation;

namespace OfficeDevPnP.SPOnline.Commands.Base
{
    [Cmdlet("Get", "SPOHealthScore")]
    [CmdletHelp("Retrieves the current health score value of the server")]
    [CmdletExample(Code = "PS:> Get-SPOHealthScore")]
    public class GetHealthScore : PSCmdlet
    {
        [Parameter(Mandatory = false, HelpMessage = "The url of the WebApplication to retrieve the health score from", ValueFromPipeline = true)]
        public string Url { get; set; }

        protected override void ProcessRecord()
        {
            string url = string.Empty;
            if (Url != null)
            {
                url = Url;
            }
            else
            {
                if (SPOnlineConnection.CurrentConnection != null)
                {
                    url = SPOnlineConnection.CurrentConnection.Url;
                }
                else
                {
                    throw new Exception(Properties.Resources.NoContextPresent);
                }
            }
            WriteObject(SPOnline.Core.Utils.Health.GetHealthScore(url));
        }
    }
}
