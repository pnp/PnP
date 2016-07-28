$global:webSession = New-Object Microsoft.PowerShell.Commands.WebRequestSession  

function Initialize-SPOSecuritySession {
    param ($targetSite)

    # Connect to SharePoint Online
    $targetSiteUri = [System.Uri]$targetSite
    Connect-SPOnline $targetSite -Credentials PiaSysDev-Paolo

    # Retrieve the client credentials and the related Authentication Cookies
    $context = (Get-SPOWeb).Context
    $credentials = $context.Credentials
    $authenticationCookies = $credentials.GetAuthenticationCookie($targetSiteUri, $true)

    # Set the Authentication Cookies and the Accept HTTP Header
    $global:webSession.Cookies.SetCookies($targetSiteUri, $authenticationCookies)
    $global:webSession.Headers.Add("Accept", "application/json;odata=verbose")
}

function Initialize-SPODigestValue {
    param ($targetSite)

    $contextInfoUrl = $targetSite + "_api/ContextInfo"

    $webRequest = Invoke-WebRequest -Uri $contextInfoUrl -Method Post -WebSession $global:webSession
    $jsonContextInfo = $webRequest.Content | ConvertFrom-Json

    $digestValue = $jsonContextInfo.d.GetContextWebInformation.FormDigestValue
    $global:webSession.Headers.Add("X-RequestDigest", $digestValue)
}

$targetSite = "https://piasysdev.sharepoint.com/sites/ProgrammingOffice365/"
Initialize-SPOSecuritySession -targetSite $targetSite
Initialize-SPODigestValue -targetSite $targetSite

# Define the EnsureUser REST API call
$ensureUserUrl = $targetSite + "_api/web/EnsureUser('paolo.pialorsi@sharepoint-camp.com')"

# Make the REST request
$webRequest = Invoke-WebRequest -Uri $ensureUserUrl -Method Post -WebSession $global:webSession

# Check the result
if ($webRequest.StatusCode -ne 200)
{
    Write-Host "Error:" $webRequest.StatusDescription
}
else
{
    Write-Host $webRequest.StatusDescription
}

