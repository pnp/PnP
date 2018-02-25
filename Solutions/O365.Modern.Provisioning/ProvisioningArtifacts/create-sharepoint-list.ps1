# Set your own values here
$SiteCollectionUrl = "<your site collection url>"

Connect-PnPOnline -Url $SiteCollectionUrl -UseWebLogin

New-PnPList -Title 'SitesRequest' -Template GenericList -Url Lists/SitesRequest

Add-PnPField -List "SitesRequest" -DisplayName "Status" -InternalName "Status" -Type Choice -Group "spProvisioning" -AddToDefaultView -Choices "Requested","Approved","Ready" -Required
Add-PnPField -List "SitesRequest" -DisplayName "Owner" -InternalName "Owner" -Type Text -Group "spProvisioning" -AddToDefaultView -Required
Add-PnPField -List "SitesRequest" -DisplayName "Description" -InternalName "Description" -Type Text -Group "spProvisioning" -AddToDefaultView -Required
Add-PnPField -List "SitesRequest" -DisplayName "SiteType" -InternalName "SiteType" -Type Choice -Group "spProvisioning" -AddToDefaultView -Choices "TeamSite","CommunicationSite","Teams" -Required
Add-PnPField -List "SitesRequest" -DisplayName "Alias" -InternalName "Alias" -Type Text -Group "spProvisioning" -AddToDefaultView -Required
