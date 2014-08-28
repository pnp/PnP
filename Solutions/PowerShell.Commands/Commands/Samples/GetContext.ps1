$siteUrl = "https://<yourtenant>.sharepoint.com"
$credentials = Get-Credential
Connect-SPOnline -Url $siteUrl -Credentials $credentials
$ctx = Get-SPOContext
$w = $ctx.Web
$w.Lists.GetByTitle("TestList")
$ctx.Load($w)
Execute-SPOQuery # Or use $ctx.ExecuteQuery()
