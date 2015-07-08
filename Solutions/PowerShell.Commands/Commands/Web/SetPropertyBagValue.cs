using System;
using System.Linq;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Set, "SPOPropertyBagValue")]
    [CmdletHelp("Sets a property bag value", Category = "Webs")]
    [CmdletExample(
      Code = @"PS:> Set-SPOPropertyBagValue -Key MyKey -Value MyValue",
      Remarks = "This sets or adds a value to the current web property bag",
      SortOrder = 1)]
    [CmdletExample(
      Code = @"PS:> Set-SPOPropertyBagValue -Key MyKey -Value MyValue -Folder /",
      Remarks = "This sets or adds a value to the root folder of the current web",
      SortOrder = 2)]
    [CmdletExample(
      Code = @"PS:> Set-SPOPropertyBagValue -Key MyKey -Value MyValue -Folder /MyFolder",
      Remarks = "This sets or adds a value to the folder MyFolder which is located in the root folder of the current web",
      SortOrder = 3)]
    public class SetPropertyBagValue : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, ParameterSetName = "Web")]
        [Parameter(Mandatory = true, ParameterSetName = "Folder")]
        public string Key;

        [Parameter(Mandatory = true, ParameterSetName = "Web")]
        [Parameter(Mandatory = true, ParameterSetName = "Folder")]
        [Parameter(Mandatory = true)]
        public string Value;

        [Parameter(Mandatory = true, ParameterSetName = "Web")]
        public SwitchParameter Indexed;

        [Parameter(Mandatory = false, ParameterSetName = "Folder", HelpMessage = "Site relative url of the folder. See examples for use.")]
        public string Folder;

        protected override void ExecuteCmdlet()
        {
            if (ParameterSetName == "Web")
            {
                if (!Indexed)
                {
                    // If it is already an indexed property we still have to add it back to the indexed properties
                    Indexed = !string.IsNullOrEmpty(SelectedWeb.GetIndexedPropertyBagKeys().FirstOrDefault(k => k == Key));
                }

                SelectedWeb.SetPropertyBagValue(Key, Value);
                if (Indexed)
                {
                    SelectedWeb.AddIndexedPropertyBagKey(Key);
                }
                else
                {
                    SelectedWeb.RemoveIndexedPropertyBagKey(Key);
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
                folder.Properties[Key] = Value;
                folder.Update();
                ClientContext.ExecuteQueryRetry();
            }
        }
    }
}
