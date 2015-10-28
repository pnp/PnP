#
# Install-MicrosurveyScripts
#

[CmdletBinding()]
Param(
   [Parameter(Mandatory=$False,Position=1)]
   [string]$Url,	

   [Parameter(ValueFromPipeline=$True)]
   [object]$Credentials
)

$settings = .\Get-Settings.ps1

# If installation URL is not provided, get it from the settings file
if ($Url -eq $null -or $Url -eq "") {
    $Url = $settings.ScriptSiteUrl
}

# If credentials were not provided, get them now
if ($credentials -eq $null) {
    $credentials  = Get-Credential -Message "Enter Site Administrator Credentials"
}

# Connect to the SharePoint site and add a folder for the app
Connect-SPOnline -Url $Url -Credentials $Credentials
New-SPOList -Title $settings.AppTitle -Template DocumentLibrary -Url $settings.ScriptLibrary

# Define source and destinations for the copy, as well as SharePoint app packaging files
# that we don't need
$localAppPath = (Get-Item -Path ..\MicroSurvey\SurveyApp).FullName
$spAppPath = "/" + $settings.ScriptLibrary
$filesToSkip = 'SampleFiles', 'Images', 'SharePointProjectItem.spdata'

# Copy the app files to the folder
$items = Get-ChildItem $localAppPath
foreach ($item in $items) 
{
    if ($filesToSkip -notcontains $item.Name)
    {
        $fullName = $item.FullName
        $fileName = $item.Name
        Add-SPOFile -Path $fullName -Folder "$spAppPath"
        Write-Host  "Deployed file: $spAppPath/$fileName"
    }
}
