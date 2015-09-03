#
# Install-Microsurvey -Url <site URL> -Credentials <credentials object>
#

[CmdletBinding()]
Param(
   [Parameter(Mandatory=$True,Position=1)]
   [string]$Url,

   [Parameter(ValueFromPipeline=$True)]
   [object]$Credentials
)

$settings = .\Get-Settings.ps1

# If credentials were not provided, get them now
if ($credentials -eq $null) {
    $credentials  = Get-Credential -Message "Enter Site Administrator Credentials"
}

# Connect to the SharePoint site and add a folder for the app
Connect-SPOnline -Url $Url -Credentials $Credentials
Add-SPOFolder -Name $settings.ScriptLibrary -Folder "SiteAssets"

# Define source and destinations for the copy, as well as SharePoint app packaging files
# that we don't need
$localAppPath = (Get-Item -Path ..\MicroSurvey\SurveyAppCentralDeploy).FullName
$spAppPath = "SiteAssets/" + $settings.ScriptLibrary
$spScriptPath = $settings.ScriptSiteUrl + $settings.ScriptLibrary

# Copy the app files to the folder
$items = Get-ChildItem $localAppPath
foreach ($item in $items) 
{
    if ($item.Name.EndsWith(".template"))
    {
        # Remove .template from file name
        $itemFinalFullName = $item.FullName.Replace(".template", "")
        $itemFinalName = $item.Name.Replace(".template", "")

        # Replace tokens in template to build file
        (Get-Content $item.FullName) | 
         Foreach-Object {$_ -replace "%AppPath%", $spScriptPath} | 
         Set-Content $itemFinalName

        # Upload the file
        Add-SPOFile -Path $itemFinalName -Folder $spAppPath
        Write-Host  "Deployed file: $itemFinalName"

        # Clean up the file with the tokens replaced
        Remove-Item $itemFinalName
    }
}

# Disable Minimal Download Strategy to ensure the web part will work
Disable-SPOFeature -Identity 87294C72-F260-42f3-A41B-981A2FFCE37A

# Remove any old site settings links
$settingsPageUrl = $spAppPath + "/Default.aspx"
$actions = Get-SPOCustomAction | Where-Object {$_.Title -eq "Manage Microsurvey"}
foreach ($action in $actions)
{
    Write-Host "Removing "$action.Id
    Remove-SPOCustomAction -Identity $action.Id -Force
}

# Add the site settings link
Add-SPOCustomAction -Location "Microsoft.SharePoint.SiteSettings" -Title "Manage Microsurvey" `
 -Url $settingsPageUrl -Description "Manage Microsurvey" -Group "Customization" -Sequence 1000
 Write-Host "Added site settings link"

# Add the web part to the page
$homePageUrl = (Get-SPOWeb).ServerRelativeUrl + "/sitepages/home.aspx"
Add-SPOWebPartToWikiPage -PageUrl $homePageUrl -Path $localAppPath"\MicroSurvey.dwp" -Row 1 -Column 1
Write-Host "Added web part to home page"

#
# Provision the SharePoint storage that the app needs
# (NOTE: The app will attempt to create/repair its own storage, so this is really optional.)
#

# Set up questions list
New-SPOList -Title "Questions" -Template GenericList -Url "lists/questions" -QuickLaunchOptions off
$list = Get-SPOList -Identity "Questions"
Write-Host Created $list.Title "list"
Add-SPOField -List $list -InternalName "Answers" -DisplayName "Answers" -Type Text -AddToDefaultView
Write-Host Added $field.Title "to" $list.Title "list"

# Set up answers list
New-SPOList -Title "Answers" -Template GenericList -Url "lists/answers" -QuickLaunchOptions off
$list = Get-SPOList -Identity "Answers"
Write-Host Created $list.Title "List"
Add-SPOField -List $list -InternalName "Data" -DisplayName "Data" -Type Text -AddToDefaultView
Write-Host Added $field.Title "to" $list.Title "list"
