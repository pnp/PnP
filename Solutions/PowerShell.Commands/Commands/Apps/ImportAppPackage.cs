using System.IO;
using System.Management.Automation;
using System.Globalization;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsData.Import, "SPOAppPackage")]
    [CmdletHelp("Adds a SharePoint App to a site",
        Details = "This commands requires that you have an app package to deploy")]
    [CmdletExample(
        Code = @"PS:> Import-SPOAppPackage -Path c:\files\demo.app -LoadOnly",
        Remarks = @"This will load the app in the demo.app package, but will not install it to the site.
 ")]
    [CmdletExample(
        Code = @"PS:> Import-SPOAppPackage -Path c:\files\demo.app -Force",
        Remarks = @"This load first activate the app sideloading feature, upload and install the app, and deactivate the app sideloading feature.
    ")]
    public class ImportAppPackage : SPOWebCmdlet
    {
        [Parameter(Mandatory = false, HelpMessage = "Path pointing to the .app file")]
        public string Path = string.Empty;

        [Parameter(Mandatory = false, HelpMessage = "Will forcibly install the app by activating the app sideloading feature, installing the app, and deactivating the sideloading feature")]
        public SwitchParameter Force;

        [Parameter(Mandatory = false, HelpMessage = "Will only upload the app, but not install it")]
        public SwitchParameter LoadOnly = false;

        [Parameter(Mandatory = false, HelpMessage = "Will install the app for the specified locale")]
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
