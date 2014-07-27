$hostUrl = "https://myapphost.net"

$projectDir = "$PSScriptRoot\Core.MailApps\"
$targetAppsDir = "$PSScriptRoot\Core.MailAppsWeb\Apps"
$manifests = Get-ChildItem -Path $projectDir -Filter "*.xml" -Recurse 

$manifests = $manifests | ? { ([xml](Get-Content $_.FullName)).OfficeApp -ne $null }

$manifests | % {
	Write-Host "Updating: $($_.Name)"
	[xml]$manifest = Get-Content $_.FullName
	$sourceLocation = $manifest.OfficeApp.DesktopSettings.SourceLocation.DefaultValue
	$newSourceLocation = $sourceLocation -replace "~remoteAppUrl",$hostUrl
	Write-Host "Original manifest source URL: $sourceLocation"
	Write-Host "New manifest source URL     : $newSourceLocation"
	$targetFileName = [IO.Path]::Combine($targetAppsDir, $_.Name)
	$manifest.OfficeApp.DesktopSettings.SourceLocation.DefaultValue = $newSourceLocation
	$manifest.Save($targetFileName)
	$targetPath = $targetFileName.Replace($PSScriptRoot, "")
	Write-Host "Manifest saved to $targetPath"
}