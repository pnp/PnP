$basePath = "C:\DeploymentFiles"
$themePath = "/sites/psdemo/_catalogs/theme/15" # ServerRelativeUrl
$tenant = "<yourtenantname>"

$tenantAdmin  = Get-Credential -Message "Enter Tenant Administrator Credentials"
Connect-SPOnline -Url https://<tenant>-admin.sharepoint.com -Credentials $tenantAdmin

New-SPOTenantSite -Title "PS Site" -Url "https://$tenant.sharepoint.com/sites/psdemo" -Owner $tenantAdmin -Lcid 1033 -TimeZone 24 -Template STS#0 -RemoveDeletedSite -Wait

Connect-SPOnline -Url https://erwinmcm.sharepoint.com/sites/psdemo -Credentials $tenantAdmin

# Set Property Bag key to designate the type of site you're creating
Set-SPOPropertyBagValue -Key "PNP_SiteType" -Value "PROJECT"

# Upload a theme
Add-SPOFile -Path "$basePath\contoso.spcolor" -Url "$themePath/contoso.spcolor"
Add-SPOFile -Path "$basePath\contoso.spfont" -Url "$themePath/contoso.spfont"
Add-SPOFile -Path "$basePath\contosobg.jpg" -Url "$themePath/contosobg.jpg"
Set-SPOTheme -ColorPaletteUrl "$themePath/contoso.spcolor" -FontSchemeUrl "$themePath/contoso.spfont" -BackgroundImageUrl "$themePath/contosobg.jpg"

# Add a list and add a field to the list.
New-SPOList -Title "Projects" -Template GenericList -Url "lists/projects" -QuickLaunchOptions on
Add-SPOField -List "Projects" -InternalName "ProjectManager" -DisplayName "Project Manager" -StaticName "ProjectManager" -Type User -AddToDefaultView