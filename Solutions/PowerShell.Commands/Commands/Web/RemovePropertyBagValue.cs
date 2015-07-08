using System;
using Microsoft.SharePoint.Client;
using System.Management.Automation;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Remove, "SPOPropertyBagValue", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    [CmdletHelp("Removes a value from the property bag", Category = "Webs")]
    [CmdletExample(
        Code = @"PS:> Remove-SPOPropertyBagValue -Key MyKey",
        Remarks = "This will remove the value with key MyKey from the current web property bag",
        SortOrder = 1)]
    [CmdletExample(
        Code = @"PS:> Remove-SPOPropertyBagValue -Key MyKey -Folder /MyFolder",
        Remarks = "This will remove the value with key MyKey from the folder MyFolder which is located in the root folder of the current web",
        SortOrder = 2)]
    [CmdletExample(
        Code = @"PS:> Remove-SPOPropertyBagValue -Key MyKey -Folder /",
        Remarks = "This will remove the value with key MyKey from the root folder of the current web",
        SortOrder = 3)]
    public class RemovePropertyBagValue : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string Key;

        [Parameter(Mandatory = false, HelpMessage = "Site relative url of the folder. See examples for use.")]
        public string Folder;

        [Parameter(Mandatory = false)]
        public SwitchParameter Force;

        protected override void ExecuteCmdlet()
        {
            if (string.IsNullOrEmpty(Folder))
            {
                if (SelectedWeb.PropertyBagContainsKey(Key))
                {
                    if (Force || ShouldContinue(string.Format(Properties.Resources.Delete0, Key), Properties.Resources.Confirm))
                    {
                        SelectedWeb.RemovePropertyBagValue(Key);
                    }
                }
            }
            else
            {
                if (!SelectedWeb.IsPropertyAvailable("ServerRelativeUrl"))
                {
                    ClientContext.Load(SelectedWeb, w => w.ServerRelativeUrl);
                    ClientContext.ExecuteQueryRetry();
                }

                var folderUrl = UrlUtility.Combine(SelectedWeb.ServerRelativeUrl, Folder);
                var folder = SelectedWeb.GetFolderByServerRelativeUrl(folderUrl);
                if (!folder.IsPropertyAvailable("Properties"))
                {
                    ClientContext.Load(folder.Properties);
                    ClientContext.ExecuteQueryRetry();
                }
                if (folder.Properties[Key] != null)
                {
                    if (Force || ShouldContinue(string.Format(Properties.Resources.Delete0, Key), Properties.Resources.Confirm))
                    {

                        folder.Properties[Key] = null;
                        folder.Properties.FieldValues.Remove(Key);
                        folder.Update();
                        ClientContext.ExecuteQueryRetry();
                    }
                }
            }
        }
    }
}
