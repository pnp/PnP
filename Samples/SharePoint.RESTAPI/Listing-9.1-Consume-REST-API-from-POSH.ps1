# Connect to SharePoint Online
$targetSite = "https://piasysdev.sharepoint.com/sites/ProgrammingOffice365/"
$targetSiteUri = [System.Uri]$targetSite

Connect-SPOnline $targetSite -Credentials PiaSysDev-Paolo

# Retrieve the client credentials and the related Authentication Cookies
$context = (Get-SPOWeb).Context
$credentials = $context.Credentials
$authenticationCookies = $credentials.GetAuthenticationCookie($targetSiteUri, $true)

# Set the Authentication Cookies and the Accept HTTP Header
$webSession = New-Object Microsoft.PowerShell.Commands.WebRequestSession  
$webSession.Cookies.SetCookies($targetSiteUri, $authenticationCookies)
$webSession.Headers.Add("Accept", "application/json;odata=verbose")

# Set request variables
$targetLibrary = "Documents"
$apiUrl = "$targetSite" + "_api/web/lists/getByTitle('$targetLibrary')"

# Make the REST request
$webRequest = Invoke-WebRequest -Uri $apiUrl -Method Get -WebSession $webSession

# Consume the JSON result
$jsonLibrary = $webRequest.Content | ConvertFrom-Json
Write-Host $jsonLibrary.d.Title
