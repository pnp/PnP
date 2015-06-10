using System.IO;
using System.Management.Automation;
using System.Globalization;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsData.Import, "SPOAppPackage")]
    
    [CmdletHelp("Adds a SharePoint Addin to a site",
        DetailedDescription = "This commands requires that you have an addin package to deploy", Category = "Apps")]
    [CmdletExample(
        Code = @"PS:> Import-SPOAppPackage -Path c:\files\demo.app -LoadOnly",
        Remarks = @"This will load the addin in the demo.app package, but will not install it to the site.
 ", SortOrder = 1)]
    [CmdletExample(
        Code = @"PS:> Import-SPOAppPackage -Path c:\files\demo.app -Force",
        Remarks = @"This load first activate the addin sideloading feature, upload and install the addin, and deactivate the addin sideloading feature.
    ", SortOrder = 2)]
    public class ImportAppPackage : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "Path pointing to the .app file")]
        public string Path = string.Empty;

        [Parameter(Mandatory = false, HelpMessage = "Will forcibly install the app by activating the addin sideloading feature, installing the addin, and deactivating the sideloading feature")]
        public SwitchParameter Force;

        [Parameter(Mandatory = false, HelpMessage = "Will only upload the addin, but not install it")]
        public SwitchParameter LoadOnly = false;

        [Parameter(Mandatory = false, HelpMessage = "Will install the addin for the specified locale")]
        public int Locale = -1;

        protected override void ExecuteCmdlet()
        {
            if (System.IO.File.Exists(Path))
            {
                if (Force)
                {
                    ClientContext.Site.ActivateFeature(Constants.APPSIDELOADINGFEATUREID);
                }
                AppInstance instance;

                if (!System.IO.Path.IsPathRooted(Path))
                {
                    Path = System.IO.Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path, Path);
                }

                var appPackageStream = new FileStream(Path, FileMode.Open, FileAccess.Read);
                if (Locale == -1)
                {
                    if (LoadOnly)
                    {
                        instance = SelectedWeb.LoadApp(appPackageStream, CultureInfo.CurrentCulture.LCID);
                    }
                    else
                    {
                        instance = SelectedWeb.LoadAndInstallApp(appPackageStream);
                    }
                }
                else
                {
                    if (LoadOnly)
                    {
                        instance = SelectedWeb.LoadApp(appPackageStream, Locale);
                    }
                    else
                    {
                        instance = SelectedWeb.LoadAndInstallAppInSpecifiedLocale(appPackageStream, Locale);
                    }
                }
                ClientContext.Load(instance);
                ClientContext.ExecuteQueryRetry();
                

                if (Force)
                {
                    ClientContext.Site.DeactivateFeature(Constants.APPSIDELOADINGFEATUREID);
                }
                WriteObject(instance);
            }
            else
            {
                WriteError(new ErrorRecord(new IOException(Properties.Resources.FileDoesNotExist), "1", ErrorCategory.InvalidArgument, null));
            }
        }
    }
}
