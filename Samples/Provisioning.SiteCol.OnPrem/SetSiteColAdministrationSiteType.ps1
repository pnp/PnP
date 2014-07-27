#
# Set admin site type property to the site collection using PS for any site collection type.
# This is needed to be set for the site collection which is used as the 
# "Connection point" for the CSOM when site collections are created in on-prem
#
$siteColUrl = "http://projects.contoso.com"

$snapin = Get-PSSnapin | Where-Object {$_.Name -eq 'Microsoft.SharePoint.Powershell'}
if ($snapin -eq $null) 
{
	Write-Host "Loading SharePoint Powershell Snapin"
	Add-PSSnapin "Microsoft.SharePoint.Powershell"
}

$site = get-spsite -Identity $siteColUrl
$site.AdministrationSiteType = [Microsoft.SharePoint.SPAdministrationSiteType]::TenantAdministration