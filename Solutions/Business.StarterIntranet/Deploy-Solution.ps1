[CmdletBinding()]
Param(
	[Parameter(Mandatory=$True,Position=1)]
	[string]$SiteUrl,

	[Parameter(Mandatory=$True)]
	[string]$UserName,

	[Parameter(Mandatory=$True)]
	[string]$Password,
	
	[Parameter(Mandatory=$False)]
	[switch]$Prod=$false,

	[Parameter(Mandatory=$False)]
	[switch]$JsOnly=$false,
	
	[Parameter(Mandatory=$False)]
	[switch]$IncludeData=$false,

	[Parameter(Mandatory=$False)]
	$ExcludeHandlersRootSite,

	[Parameter(Mandatory=$False)]
	$ExcludeHandlersSubSites,

	[Parameter(Mandatory=$False)]
	[switch]$UpgradeSubSites=$false
)

$0 = $myInvocation.MyCommand.Definition
$CommandDirectory = [System.IO.Path]::GetDirectoryName($0)
$ErrorActionPreference = "Stop"

Push-Location $CommandDirectory

# Include utility scripts
 . "./utility/Utility.ps1"
 . "./Configuration.ps1"

# Configuration file paths
$ProvisioningRootSiteTemplateFile = Join-Path -Path $CommandDirectory -ChildPath "provisioning\RootSiteTemplate.xml"
$ImageRenditionsConfigurationFilePath = Join-Path -Path $CommandDirectory -ChildPath "provisioning\PublishingImageRenditions.xml"

# Get the version of the PnP Starter Intranet (from package.json file)
$PkgFile = Get-Content -Raw -Path (Join-Path -Path $CommandDirectory -ChildPath "app/package.json") | ConvertFrom-Json
$PnPStarterIntranetCurrentVersion = $PkgFile.version

# Connect to the site
$PasswordAsSecure = ConvertTo-SecureString $Password -AsPlainText -Force
$Credentials = New-Object System.Management.Automation.PSCredential ($UserName , $PasswordAsSecure)
Connect-PnPOnline -Url $SiteUrl -Credentials $Credentials
$RootSiteContext = Get-PnPContext

# Determine the SharePoint version
$ServerVersion = (Get-PnPContext).ServerLibraryVersion.Major

switch ($ServerVersion) 
{ 
	15 {$AssemblyVersion = "15.0.0.0"} 
	16 {$AssemblyVersion = "16.0.0.0"} 
    default {$AssemblyVersion = "16.0.0.0"}
}

Write-Header -AppVersion $PnPStarterIntranetCurrentVersion

# Set the current version of the solution in the property bag for future use (upgrades for instance)
Set-PnPPropertyBagValue -Key "PnPStarterIntranetVersion" -Value $PnPStarterIntranetCurrentVersion

$Date = Get-Date
Write-Section -Message "Installation started on $Date"
Write-Message -Message "Target site: '$SiteUrl'`n" -ForegroundColor Green

$ExecutionTime = [System.Diagnostics.Stopwatch]::StartNew()

# -------------------------------------------------------------------------------------
# Set the correct SharePoint assembly version in .aspx and .master files regarding the server version
# -------------------------------------------------------------------------------------
Get-ChildItem -Path ".\provisioning\artefacts" -Include "*.aspx","*.master" -Recurse | ForEach-Object {

    (Get-Content -Path $_.FullName) -replace "1[5|6]\.0\.0\.0",$AssemblyVersion | Out-File -FilePath $_.FullName
}

# -------------------------------------------------------------------------------------
# Upload files in the style library (folders are created automatically by the PnP cmdlet)
# -------------------------------------------------------------------------------------
if ($JsOnly.IsPresent) {
    Write-Message -Message "Selected mode: JS files only" -ForegroundColor Cyan
}

Push-Location ".\app"

if ($Prod.IsPresent) {

    Write-Message -Message "Bundling the application (production mode)..." -NoNewline

	# Bundle the project in production mode (the '2>$null' is to avoid PowerShell ISE errors)
	webpack -p 2>$null | Out-Null

    Write-Message -Message "`tDone!" -ForegroundColor Green
		
} else {

    Write-Message -Message "Bundling the application (development mode)..." -NoNewline
	
	# Bundle the project in dev mode
	webpack 2>$null | Out-Null

    Write-Message -Message "`tDone!" -ForegroundColor Green
}

Pop-Location

# Get Webpack output folder and upload all files in the style library (eventually will be replaced by CDN in the future)
$DistFolder = $CommandDirectory + "\app\dist"

Write-Message -Message "Uploading all files in the style library..." -NoNewline

Push-Location $DistFolder 

Get-ChildItem -Recurse $DistFolder -File | ForEach-Object {

    $TargetFolder = "Style Library\$AppFolderName\" + (Resolve-Path -relative $_.FullName) | Split-Path -Parent

	Add-PnPFile -Path $_.FullName -Folder ($TargetFolder.Replace("\","/")).Replace("./","").Replace(".","") -Checkout | Out-Null
}

Pop-Location

Write-Message -Message "`tDone!" -ForegroundColor Green

if ($JsOnly.IsPresent) {

    $ExecutionTime.Stop()
    $ElapsedMinutes = [System.Math]::Round($ExecutionTime.Elapsed.Minutes)
    $ElapsedSeconds = [System.Math]::Round($ExecutionTime.Elapsed.Seconds)

    Write-Section -Message "Deployment of JS files completed in $ElapsedMinutes minute(s) and $ElapsedSeconds second(s)"

    # Close the connection to the server
    Disconnect-PnPOnline

    exit
}

# -------------------------------------------------------------------------------------
# Apply root site template
# -------------------------------------------------------------------------------------
Write-Message -Message "Configuring the root site..." -NoNewline

$PagesLibraryName = (Get-PnPList -Identity (Get-PnPPropertyBag -Key __PagesListId)).Title

if (!$PagesLibraryName) {
    
    Write-Error "Pages library not found, make sure the target is a publishing site"    
    exit
}

$TemplateParameters = @{ 
	"CompanyName" = $AppFolderName; 
	"AssemblyVersion" = $AssemblyVersion; 
	"PagesLibraryName" = $PagesLibraryName;
	"DefaultLanguageLabel" = $SiteUrl + "/" + $Languages[0].Label
}

# Apply the root site provisioning template
if ($ExcludeHandlersRootSite) {
	Apply-PnPProvisioningTemplate -Path $ProvisioningRootSiteTemplateFile -Parameters $TemplateParameters -ExcludeHandlers $ExcludeHandlersRootSite
} else {
	Apply-PnPProvisioningTemplate -Path $ProvisioningRootSiteTemplateFile -Parameters $TemplateParameters
}

Write-Message -Message "`tDone!" -ForegroundColor Green

# -------------------------------------------------------------------------------------
# Configure sub webs according languages
# -------------------------------------------------------------------------------------
$Script = ".\Setup-Web.ps1" 
& $Script -RootSiteUrl $SiteUrl -UserName $UserName -Password $Password -ExcludeHandlers $ExcludeHandlersSubSites -UpgradeSubSites:$UpgradeSubSites -IncludeData:$IncludeData

# Switch back to the root site context
Set-PnPContext -Context $RootSiteContext

# -------------------------------------------------------------------------------------
# Add image renditions
# -------------------------------------------------------------------------------------
Write-Message -Message "Configuring image renditions..." -NoNewline

# Thanks to http://www.eliostruyf.com/provision-image-renditions-to-your-sharepoint-2013-site/
$File = Add-PnPFile -Path $ImageRenditionsConfigurationFilePath -Folder "_catalogs\masterpage\" -Checkout

Write-Message -Message "`tDone!" -ForegroundColor Green

# -------------------------------------------------------------------------------------
# Add sample data
# -------------------------------------------------------------------------------------
if ($IncludeData.IsPresent) {

    Write-Message -Message "Adding sample data for the carousel..." -NoNewline

    $CarouselItemsList = Get-PnPList -Identity "Carousel Items"

    $CarouselItemsEN = @(

	    @{ "Title"="PnP Starter Intranet official documentation";"CarouselItemURL"="https://transactions.sendowl.com/packages/48364/D024B326/view";"CarouselItemImage"="https://static.wixstatic.com/media/9b7fa1_d42f3bd96f5e40ed885a12eaca09513e~mv2_d_1920_1416_s_2.jpeg";"IntranetContentLanguage"="EN" },
	    @{ "Title"="Sample element";"CarouselItemURL"="https://dev.office.com/";"CarouselItemImage"="http://via.placeholder.com/640x360";"IntranetContentLanguage"="EN" },
        @{ "Title"="Sample element";"CarouselItemURL"="https://dev.office.com/";"CarouselItemImage"="http://via.placeholder.com/640x360";"IntranetContentLanguage"="EN" },
	    @{ "Title"="Sample element";"CarouselItemURL"="https://dev.office.com/";"CarouselItemImage"="http://via.placeholder.com/640x360";"IntranetContentLanguage"="EN" }
    )

    $CarouselItemsFR = @(

	    @{ "Title"="Documentation officielle de la solution PnP Starter Intranet";"CarouselItemURL"="https://transactions.sendowl.com/packages/48367/D92CFE56/view";"CarouselItemImage"="https://static.wixstatic.com/media/9b7fa1_d42f3bd96f5e40ed885a12eaca09513e~mv2_d_1920_1416_s_2.jpeg";"IntranetContentLanguage"="FR" },
	    @{ "Title"="Élement de démo";"CarouselItemURL"="https://dev.office.com/";"CarouselItemImage"="http://via.placeholder.com/640x360";"IntranetContentLanguage"="FR" },
    	@{ "Title"="Élement de démo";"CarouselItemURL"="https://dev.office.com/";"CarouselItemImage"="http://via.placeholder.com/640x360";"IntranetContentLanguage"="FR" },
	    @{ "Title"="Élement de démo";"CarouselItemURL"="https://dev.office.com/";"CarouselItemImage"="http://via.placeholder.com/640x360";"IntranetContentLanguage"="FR" }
    )

    $CarouselItemsEN | ForEach-Object {

		$Item = Add-PnPListItem -List $CarouselItemsList
    	$Item = Set-PnPListItem -Identity  $Item.Id -List $CarouselItemsList -Values $_ -ContentType "Carousel Item"
    }

    $CarouselItemsFR | ForEach-Object {

		$Item = Add-PnPListItem -List $CarouselItemsList
    	$Item = Set-PnPListItem -Identity  $Item.Id -List $CarouselItemsList -Values $_ -ContentType "Carousel Item"
    }

	Write-Message -Message "`tDone!" -ForegroundColor Green
}

$ExecutionTime.Stop()
$ElapsedMinutes = [System.Math]::Round($ExecutionTime.Elapsed.Minutes)
$ElapsedSeconds = [System.Math]::Round($ExecutionTime.Elapsed.Seconds)

Write-Section -Message "Installation completed in $ElapsedMinutes minute(s) and $ElapsedSeconds second(s)"

Pop-Location

# Close the connection to the server
Disconnect-PnPOnline




