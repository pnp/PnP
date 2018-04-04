[CmdletBinding()]
Param(
	[Parameter(Mandatory=$True,Position=1)]
	[string]$SiteUrl,

	[Parameter(Mandatory=$True)]
	[string]$UserName,

	[Parameter(Mandatory=$True)]
    [string]$Password
)

$0 = $myInvocation.MyCommand.Definition
$CommandDirectory = [System.IO.Path]::GetDirectoryName($0)
$ErrorActionPreference = "Stop"

Push-Location $CommandDirectory

# Include utility scripts
. "./utility/Utility.ps1"

# Connect to the site
$PasswordAsSecure = ConvertTo-SecureString $Password -AsPlainText -Force
$Credentials = New-Object System.Management.Automation.PSCredential ($UserName , $PasswordAsSecure)
Connect-PnPOnline -Url $SiteUrl -Credentials $Credentials

# Get the version of the PnP Starter Intranet (from package.json file)
$PkgFile = Get-Content -Raw -Path (Join-Path -Path $CommandDirectory -ChildPath "app/package.json") | ConvertFrom-Json
$PnPStarterIntranetCurrentVersion = $PkgFile.version

$CurrentVersion = Get-PnPPropertyBag -Key "PnPStarterIntranetVersion"
$UpgradableVersions = @("2.0.0","2.1.0","2.2.0")

# Updates are always processed for all versions as follows
# - The search configuration is applied cumulatively by checking the applicable versions (greater than the current one, identified by file name convention)
# - PnP provisioning templates (root and sub sites) are applied excluding the taxonomy and search settings to avoid conflicts
# - Miscellaneous updates in sub site itself are done directly in the "Setup-Web" script by ensuring if a resource already exists before creating or recreating.
#By this way we are able to manage incremental updates without being too specific in scripts. 

if ($UpgradableVersions.IndexOf($CurrentVersion) -ne -1) {
    Write-Section -Message "Upgrading solution from '$CurrentVersion' to '$PnPStarterIntranetCurrentVersion'"

    # Apply search updates globally 
    # Be careful here. Since we use the native import/export SharePoint feature, configurations must not overlap themselves, so it means only new items have to be created)
    Get-ChildItem -Path ".\updates\search" -Include "*.xml" -Recurse | ForEach-Object {

        $FileVersionNumber = [regex]::match($_.FullName,'(\d+\.\d+\.\d+)').Groups[1].Value
        if ($FileVersionNumber -gt $CurrentVersion) {

            # Set up the search configuration
            Set-PnPSearchConfiguration -Path $_.FullName -Scope Site
        }
    }

    Push-Location (Join-Path -Path $CommandDirectory -ChildPath "app")

    # Update npm packages
    npm i --silent *>$null | Out-Null

    Pop-Location

    # The upgrade procedure will re-apply the PnP provisioning template on the root site and subsites (via the -UpgradeSubSites parameter)
    # When upgrading, taxonomy and search settings can't be overwritten  so they have to be excluded
    $Script = ".\Deploy-Solution.ps1" 
    & $Script -SiteUrl $SiteUrl -UserName $UserName -Password $Password -ExcludeHandlersRootSite TermGroups,SearchSettings -ExcludeHandlersSubSites TermGroups,Files -UpgradeSubSites
} else {
    Write-Message -Message "`tYou already have the latest version '$CurrentVersion' or your version does not support an upgrade." -ForegroundColor Green
}