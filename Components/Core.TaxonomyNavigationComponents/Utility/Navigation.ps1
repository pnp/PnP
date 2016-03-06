# Load SharePoint libraries
# You need to download the SharePoint Online Client Components SDK (https://www.microsoft.com/en-us/download/confirmation.aspx?id=42038)
# DLLs are installed in the GAC
Add-Type -AssemblyName "Microsoft.SharePoint.Client, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"
Add-Type -AssemblyName "Microsoft.SharePoint.Client.Runtime, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"
Add-Type -AssemblyName "Microsoft.SharePoint.Client.Taxonomy, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"
Add-Type -AssemblyName "Microsoft.SharePoint.Client.Publishing, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"

function MapTermSetToNavigationContext()
{
    Param(

        $Context,

        [Guid]$TermSetId,

        [Parameter(ParameterSetName=”GlobalNavigation", Mandatory=$true)] 
        [switch]
        $GlobalNavigation,

        [Parameter(ParameterSetName=”CurrentNavigation", Mandatory=$true)] 
        [switch]$CurrentNavigation
    )

    $Web = $Context.Web
    $Site = $Context.Site

    $Context.Load($Site)
    $Context.Load($Web)
    $Context.ExecuteQuery()

    # Don't use [Microsoft.SharePoint.Client.Publishing.Navigation.TaxonomyNavigation]::GetWebNavigationSettings($Context, $Web) or
    # you will get the "The operation failed because the object cannot be modified." error (don't know why yet)
    $NavigationSettings = New-Object Microsoft.SharePoint.Client.Publishing.Navigation.WebNavigationSettings -ArgumentList $Context, $Web

    if ($GlobalNavigation.IsPresent)
    {

        $NavigationContext = $NavigationSettings.GlobalNavigation
    }
    else
    {
        $NavigationContext = $NavigationSettings.CurrentNavigation
    }
    
    $TaxonomySession = [Microsoft.SharePoint.Client.Taxonomy.TaxonomySession]::GetTaxonomySession($Context)
    $TaxonomySession.UpdateCache()
    $Context.Load($TaxonomySession)
    $TermStores = $TaxonomySession.TermStores
    $Context.Load($TermStores)
    $Context.ExecuteQuery()
  
    # Set term set configuration for navigation
    $NavigationContext.Source = [Microsoft.SharePoint.Client.Publishing.Navigation.StandardNavigationSource]::TaxonomyProvider
    $NavigationContext.TermSetId = $TermSetId
    $NavigationContext.TermStoreId = $TermStores[0].Id # There is always only one termstore in SharePoint Online ;)

    # The term set is automatically flagged as navigation term set via the "Update" method
    $NavigationSettings.Update($TaxonomySession)  

    # Reset the cache
    [Microsoft.SharePoint.Client.Publishing.Navigation.TaxonomyNavigation]::FlushSiteFromCache($Context, $Site)
    $Context.ExecuteQuery()   
}