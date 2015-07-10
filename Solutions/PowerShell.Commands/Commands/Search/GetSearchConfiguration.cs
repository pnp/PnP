using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Enums;

namespace OfficeDevPnP.PowerShell.Commands.Search
{
    [Cmdlet(VerbsCommon.Get, "SPOSearchConfiguration")]
    [CmdletHelp("Returns the search configuration", Category = "Search")]
    [CmdletExample(
        Code = @"PS:> Get-SPOSearchConfiguration",
        Remarks = "Returns the search configuration for the current web",
        SortOrder = 1)]
    [CmdletExample(
        Code = @"PS:> Get-SPOSearchConfiguration -Scope Site",
        Remarks = "Returns the search configuration for the current site collection",
        SortOrder = 2)]
    public class GetSearchConfiguration : SPOWebCmdlet
    {
        [Parameter(Mandatory = false)]
        public SearchConfigurationScope Scope = SearchConfigurationScope.Web;

        protected override void ExecuteCmdlet()
        {
            switch (Scope)
            {
                case SearchConfigurationScope.Web:
                    {
                        WriteObject(this.SelectedWeb.GetSearchConfiguration());
                        break;
                    }
                case SearchConfigurationScope.Site:
                    {
                        WriteObject(ClientContext.Site.GetSearchConfiguration());
                        break;
                    }
            }
        }
    }
}
