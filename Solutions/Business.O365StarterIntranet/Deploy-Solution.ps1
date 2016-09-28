[CmdletBinding()]
Param(
	[Parameter(Mandatory=$True,Position=1)]
	[string]$SiteUrl,

	[Parameter(Mandatory=$True)]
	[string]$UserName,

	[Parameter(Mandatory=$True)]
	[string]$Password,
	
	[Parameter(Mandatory=$False)]
	[switch]$Prod=$false,
	
	[Parameter(Mandatory=$False)]
	[switch]$IncludeData=$false
)

# -----------------
# Global parameters
# -----------------

# Include utility functions
 . "./Utility/Utility.ps1"

$0 = $myInvocation.MyCommand.Definition
$CommandDirectory = [System.IO.Path]::GetDirectoryName($0)

# Configuration file paths
$ProvisioningRootSiteTemplateFile = ($CommandDirectory  + ".\Provisioning\RootSiteTemplate.xml")
$SearchConfigurationFilePath = ($CommandDirectory  + ".\Provisioning\SearchConfiguration.xml")
$ImageRenditionsConfigurationFilePath = ($CommandDirectory + ".\Provisioning\PublishingImageRenditions.xml")

$CustomProviderDllPath = ($CommandDirectory + ".\Provisioning\Intranet.Providers\Intranet.Providers\bin\Release\Intranet.Providers.dll")

# This name will be used to create a separated folder in the style library and the master page catalog.
# If you change this name, don't forget to update :
# - Links in the master page (CSS and JS files)
# - Web Parts XML contents on the provisioning template (display templates paths)
# - Display templates files (relative paths to hover panel display template)
$AppFolderName = "PnP"

# Connect to the site
$PasswordAsSecure = ConvertTo-SecureString $Password -AsPlainText -Force
$Credentials = New-Object System.Management.Automation.PSCredential ($UserName , $PasswordAsSecure)
Connect-SPOnline -Url $SiteUrl -Credentials $Credentials

# -------------------------------------------------------------------------------------
# Upload files in the style library (folders are created automatically by the PnP cmdlet)
# -------------------------------------------------------------------------------------
Push-Location ".\App"

if ($Prod.IsPresent) {
		
	Write-Host "1# Bundling the application (production mode)..." -ForegroundColor Magenta
	
	# Bundle the project in production mode (the '2>$null' is to avoid PowerShell ISE errors)
	webpack -p 2>$null
		
} else {

	Write-Host "1# Bundling the application (development mode)..." -ForegroundColor Magenta
	
	# Bundle the project in dev mode
	webpack 2>$null
}

Pop-Location

# Get Webpack output folder and upload all files in the style library (eventually will be replaced by CDN in the future)
$DistFolder = $CommandDirectory + "\App\dist"

Write-Host "2# Uploading all files (non optimized)..." -ForegroundColor Magenta

Push-Location $DistFolder 

Get-ChildItem -Recurse $DistFolder -File | ForEach-Object {

    $TargetFolder = "Style Library\$AppFolderName\" + (Resolve-Path -relative $_.FullName) | Split-Path -Parent

	Add-SPOFile -Path $_.FullName -Folder ($TargetFolder.Replace("\","/")).Replace("./","").Replace(".","") -Checkout
}

Pop-Location

# -------------------------------------------------------------------------------------
# Apply root site template
# -------------------------------------------------------------------------------------
Write-Host "3# Apply the provisioning template to the root site..." -ForegroundColor Magenta

# Create news folder in the "Pages" library
Ensure-SPOFolder -SiteRelativePath "Pages/News" | Out-Null

# Load the custom extensibility provider type in the current PS session
Add-Type -Path $CustomProviderDllPath 

# Apply the root site provisioning template and set column default values (without files)
Apply-SPOProvisioningTemplate -Path $ProvisioningRootSiteTemplateFile -ExcludeHandlers Files,WebSettings -Parameters @{ "CompanyName" = $AppFolderName }

# Enable Item Scheduling feature on the "Pages" library
Enable-CustomItemScheduling -Web (Get-SPOWeb) -PagesLibraryName "Pages"

# Apply the global template for the root site (to get the right pages auto tagging)
Apply-SPOProvisioningTemplate -Path $ProvisioningRootSiteTemplateFile -Handlers Files,WebSettings -Parameters @{ "CompanyName" = $AppFolderName }
 
# Content Types order
$ContentTypesOrderRoot = @(

	[PSCustomObject]@{FolderName="Pages";ContentTypes=@("Home Page","Static Page","Search Page")},
	[PSCustomObject]@{FolderName="Pages/News";ContentTypes=@("News Page")}
)

$ContentTypesOrderRoot | Foreach-Object { Set-FolderContentTypesOrder -FolderRelativePath $_.FolderName -ContentTypes $_.ContentTypes }

# Set up the search configuration
# Be careful, in SharePoint Online, we can't update an automatically created managed property to be sortable. We have to use Refinable<Type>XX predefined property.
# For example, for the news list on the front page, we use the RefinableDate00 property for the publishing date. Use an alias instead of using the default name.
Set-SPOSearchConfiguration -Path $SearchConfigurationFilePath -Scope Site

Write-Host "4# Publishing artefacts..." -ForegroundColor Magenta

# Publishing artefacts
$Site = Get-SPOSite
$SiteServerRelativeUrl = Get-SPOProperty -ClientObject $Site -Property ServerRelativeUrl

$FilesToPublish = @(

	[PSCustomObject]@{Url="$SiteServerRelativeUrl/_catalogs/masterpage/$AppFolderName/portal.master"},
	[PSCustomObject]@{Url="$SiteServerRelativeUrl/_catalogs/masterpage/$AppFolderName/HomePageLayout.aspx"},
    [PSCustomObject]@{Url="$SiteServerRelativeUrl/_catalogs/masterpage/$AppFolderName/NewsPageLayout.aspx"},
    [PSCustomObject]@{Url="$SiteServerRelativeUrl/_catalogs/masterpage/$AppFolderName/SearchPageLayout.aspx"},
    [PSCustomObject]@{Url="$SiteServerRelativeUrl/_catalogs/masterpage/$AppFolderName/StaticPageLayout.aspx"}
    [PSCustomObject]@{Url="$SiteServerRelativeUrl/_catalogs/masterpage/Display Templates/Content Web Parts/$AppFolderName/Item_Intranet-News.html"},
    [PSCustomObject]@{Url="$SiteServerRelativeUrl/_catalogs/masterpage/Display Templates/Content Web Parts/$AppFolderName/Item_Intranet-Document.html"},
	[PSCustomObject]@{Url="$SiteServerRelativeUrl/_catalogs/masterpage/Display Templates/Content Web Parts/$AppFolderName/Item_Intranet-Contact.html"},
    [PSCustomObject]@{Url="$SiteServerRelativeUrl/_catalogs/masterpage/Display Templates/Content Web Parts/$AppFolderName/Control_Intranet-List.html"},	
    [PSCustomObject]@{Url="$SiteServerRelativeUrl/_catalogs/masterpage/Display Templates/Search/$AppFolderName/Item_Intranet-News_Search.html"},
    [PSCustomObject]@{Url="$SiteServerRelativeUrl/_catalogs/masterpage/Display Templates/Search/$AppFolderName/Item_Intranet-Page_Search.html"},
	[PSCustomObject]@{Url="$SiteServerRelativeUrl/_catalogs/masterpage/Display Templates/Search/$AppFolderName/Control_Intranet-SearchResults.html"},
	[PSCustomObject]@{Url="$SiteServerRelativeUrl/_catalogs/masterpage/Display Templates/Filters/$AppFolderName/Filter_Intranet-Item.html"},
	[PSCustomObject]@{Url="$SiteServerRelativeUrl/_catalogs/masterpage/Display Templates/Filters$AppFolderName/Filter_Intranet-SliderBarGraph.html"},
	[PSCustomObject]@{Url="$SiteServerRelativeUrl/_catalogs/masterpage/Display Templates/Filters/$AppFolderName/Control_Intranet-Refinement.html"},
	[PSCustomObject]@{Url="$SiteServerRelativeUrl/_catalogs/masterpage/Display Templates/Search/$AppFolderName/Item_Intranet-Document_HoverPanel.html"},  
	[PSCustomObject]@{Url="$SiteServerRelativeUrl/_catalogs/masterpage/Display Templates/Search/$AppFolderName/Item_Intranet-Document_Search.html"},
	[PSCustomObject]@{Url="$SiteServerRelativeUrl/_catalogs/masterpage/Display Templates/Search/$AppFolderName/Item_Intranet_CommonHoverPanel_Actions.html"},  
	[PSCustomObject]@{Url="$SiteServerRelativeUrl/_catalogs/masterpage/Display Templates/Search/$AppFolderName/Item_Intranet_CommonHoverPanel_Body.html"},  
	[PSCustomObject]@{Url="$SiteServerRelativeUrl/_catalogs/masterpage/Display Templates/Search/$AppFolderName/Item_Intranet_CommonHoverPanel_Header.html"},  		
	[PSCustomObject]@{Url="$SiteServerRelativeUrl/_catalogs/masterpage/Display Templates/Search/$AppFolderName/Item_Intranet_WebPage_HoverPanel.html"},  	
    [PSCustomObject]@{Url="$SiteServerRelativeUrl/Pages/Home.aspx"},  
	[PSCustomObject]@{Url="$SiteServerRelativeUrl/Pages/Search.aspx"},  
	[PSCustomObject]@{Url="$SiteServerRelativeUrl/Pages/SearchDocuments.aspx"},
	[PSCustomObject]@{Url="$SiteServerRelativeUrl/Pages/Accueil.aspx"},  
	[PSCustomObject]@{Url="$SiteServerRelativeUrl/Pages/Recherche.aspx"},  
	[PSCustomObject]@{Url="$SiteServerRelativeUrl/Pages/RechercheDocuments.aspx"}    	
)

$FilesToPublish | ForEach-Object {

    Set-SPOFileCheckedOut -Url $_.Url
    Set-SPOFileCheckedIn -Url $_.Url -CheckinType MajorCheckIn
}

# Approve all items 
Get-SPOListItem -List Pages | ForEach-Object { 
    $_["_ModerationStatus"] = 0
    $_.Update()
}

Execute-SPOQuery

# Reset the theme
Set-SPOTheme

# Set the theme
$Web = Get-SPOWeb
$bgImageUrl = Out-Null
$fontScheme = Out-Null
$Web.ApplyTheme("$SiteServerRelativeUrl/_catalogs/theme/15/intranet.spcolor", $fontScheme, $bgImageUrl, $true)

Execute-SPOQuery

# -------------------------------------------------------------------------------------
# 3) Taxonomy setup
# -------------------------------------------------------------------------------------
Write-Host "5# Set up taxonomy..." -ForegroundColor Magenta

# Get the site collection term group name
$CurrentSite = Get-SPOSite
$Session = Get-SPOTaxonomySession
$TermStore = $Session.GetDefaultSiteCollectionTermStore();
$SiteCollectionTermGroup = $TermStore.GetSiteCollectionGroup($CurrentSite, $false)
$IntranetTermGroupName = Get-SPOProperty -ClientObject $SiteCollectionTermGroup -Property Name 

$SiteMapTermSetName_EN = "Site Map EN"
$SiteMapTermSetName_FR = "Site Map FR"

$HeaderLinksTermSetName_EN = "Header Links EN"
$HeaderLinksTermSetName_FR = "Header Links FR"

# Get navigation term sets for each language (FR & EN)
$SiteMapTermSet_EN = Get-SPOTaxonomyItem -Term "$IntranetTermGroupName|$SiteMapTermSetName_EN"
$SiteMapTermSetId_EN = $SiteMapTermSet_EN.Id

$SiteMapTermSet_FR = Get-SPOTaxonomyItem -Term "$IntranetTermGroupName|$SiteMapTermSetName_FR"
$SiteMapTermSetId_FR = $SiteMapTermSet_FR.Id

# Duplicate the Site Map EN into Site Map FR to have a mirror structure (i.e pin terms with children)
$SiteMapTermSetTerms_EN = Get-SPOProperty -ClientObject $SiteMapTermSet_EN -Property Terms

$SiteMapTermSetTerms_EN | ForEach-Object {

	$NavTerm = Get-SPOTaxonomyItem -Term ("$IntranetTermGroupName|$SiteMapTermSetName_FR|" + $_.Name)

    if ($NavTerm -eq $null) {

		$Reuse = $SiteMapTermSet_FR.ReuseTermWithPinning($_)

		Execute-SPOQuery
	}
}

# Do the same thing for header links term set
$HeaderLinksTermSet_EN = Get-SPOTaxonomyItem -Term "$IntranetTermGroupName|$HeaderLinksTermSetName_EN"
$HeaderLinksTermSetId_EN = $HeaderLinksTermSet_EN.Id

$HeaderLinksTermSet_FR = Get-SPOTaxonomyItem -Term "$IntranetTermGroupName|$HeaderLinksTermSetName_FR"
$HeaderLinksTermSetId_FR = $HeaderLinksTermSet_FR.Id

$HeaderLinksTermSetTerms_EN = Get-SPOProperty -ClientObject $HeaderLinksTermSet_EN -Property Terms

$HeaderLinksTermSetTerms_EN | ForEach-Object {

	$NavTerm = Get-SPOTaxonomyItem -Term ("$IntranetTermGroupName|$HeaderLinksTermSetName_FR|" + $_.Name)

    if ($NavTerm -eq $null) {

		$Reuse = $HeaderLinksTermSet_FR.ReuseTermWithPinning($_)

		Execute-SPOQuery
	}
}

# -------------------------------------------------------------------------------------
# Setup the configuration list
# -------------------------------------------------------------------------------------
Write-Host "6# Setup the configuration list..." -ForegroundColor Magenta

$ConfigurationList = Get-SPOList -Identity "Configuration"

$ConfigurationItems = @(

	@{ "Title"="Default EN";"ForceCacheRefresh"=1;"SiteMapTermSetId"=$SiteMapTermSetId_EN;"HeaderLinksTermSetId"=$HeaderLinksTermSetId_EN;"IntranetContentLanguage"="EN" },
	@{ "Title"="Default FR";"ForceCacheRefresh"=1;"SiteMapTermSetId"=$SiteMapTermSetId_FR;"HeaderLinksTermSetId"=$HeaderLinksTermSetId_FR;"IntranetContentLanguage"="FR" }
)

# Create the configuration item for each language
$ConfigurationItems | ForEach-Object {

    $Item = Add-SPOListItem -List $ConfigurationList -ContentType Item  -Values $_
}

# -------------------------------------------------------------------------------------
# Add image renditions
# -------------------------------------------------------------------------------------
Write-Host "7# Configure image renditions..." -ForegroundColor Magenta

# Thanks to http://www.eliostruyf.com/provision-image-renditions-to-your-sharepoint-2013-site/
Add-SPOFile -Path $ImageRenditionsConfigurationFilePath -Folder "_catalogs\masterpage\" -Checkout

# -------------------------------------------------------------------------------------
# Add sample data
# -------------------------------------------------------------------------------------
if ($IncludeData.IsPresent) {

    $CarouselItemsList = Get-SPOList -Identity "Carousel Items"

    $ConfigurationItemsEN = @(

	    @{ "Title"="Part 1: Functional overview (How to use the solution?)";"CarouselItemURL"="http://thecollaborationcorner.com/2016/08/22/part-1-functional-overview-how-to-use-the-solution";"CarouselItemImage"="http://thecollaborationcorner.com/wp-content/uploads/2016/08/part1.png";"IntranetContentLanguage"="EN" },
	    @{ "Title"="Part 2: Frameworks and libraries used (How it is implemented?)";"CarouselItemURL"="http://thecollaborationcorner.com/2016/08/25/part-2-frameworks-and-libraries-used-how-it-is-implemented";"CarouselItemImage"="http://thecollaborationcorner.com/wp-content/uploads/2016/08/part2.png";"IntranetContentLanguage"="EN" },
        @{ "Title"="Part 3: Design and mobile implementation";"CarouselItemURL"="http://thecollaborationcorner.com/2016/08/29/part-3-design-and-mobile-implementation";"CarouselItemImage"="http://thecollaborationcorner.com/wp-content/uploads/2016/08/part3.png";"IntranetContentLanguage"="EN" },
        @{ "Title"="Part 4: The navigation implementation";"CarouselItemURL"="http://thecollaborationcorner.com/2016/08/31/part-4-the-navigation-implementation";"CarouselItemImage"="http://thecollaborationcorner.com/wp-content/uploads/2016/08/part4.png";"IntranetContentLanguage"="EN" },    
        @{ "Title"="Part 5: Localization";"CarouselItemURL"="http://thecollaborationcorner.com/2016/09/02/part-5-localization";"CarouselItemImage"="http://thecollaborationcorner.com/wp-content/uploads/2016/09/part5.png";"IntranetContentLanguage"="EN" },  
        @{ "Title"="Part 6: The search implementation";"CarouselItemURL"="http://thecollaborationcorner.com/2016/09/08/part-6-the-search-implementation";"CarouselItemImage"="http://thecollaborationcorner.com/wp-content/uploads/2016/09/part6.png";"IntranetContentLanguage"="EN" }  
    )

    $ConfigurationItemsFR = @(

	    @{ "Title"="Partie 1: Aperçu fonctionel (Comment utiliser cette solution?)";"CarouselItemURL"="http://thecollaborationcorner.com/2016/08/22/part-1-functional-overview-how-to-use-the-solution";"CarouselItemImage"="http://thecollaborationcorner.com/wp-content/uploads/2016/08/part1.png";"IntranetContentLanguage"="FR" },
	    @{ "Title"="Partie 2: Frameworks et librairies utilisées";"CarouselItemURL"="http://thecollaborationcorner.com/2016/08/25/part-2-frameworks-and-libraries-used-how-it-is-implemented";"CarouselItemImage"="http://thecollaborationcorner.com/wp-content/uploads/2016/08/part2.png";"IntranetContentLanguage"="FR" },
        @{ "Title"="Partie 3: Identité visuelle et implémentation mobile";"CarouselItemURL"="http://thecollaborationcorner.com/2016/08/29/part-3-design-and-mobile-implementation";"CarouselItemImage"="http://thecollaborationcorner.com/wp-content/uploads/2016/08/part3.png";"IntranetContentLanguage"="FR" },
        @{ "Title"="Partie 4: Implémentation de la navigation";"CarouselItemURL"="http://thecollaborationcorner.com/2016/08/31/part-4-the-navigation-implementation";"CarouselItemImage"="http://thecollaborationcorner.com/wp-content/uploads/2016/08/part4.png";"IntranetContentLanguage"="FR" },    
        @{ "Title"="Partie 5: Multilinguisme";"CarouselItemURL"="http://thecollaborationcorner.com/2016/09/02/part-5-localization";"CarouselItemImage"="http://thecollaborationcorner.com/wp-content/uploads/2016/09/part5.png";"IntranetContentLanguage"="FR" },  
        @{ "Title"="Partie 6: Implémentation de la recherche";"CarouselItemURL"="http://thecollaborationcorner.com/2016/09/08/part-6-the-search-implementation";"CarouselItemImage"="http://thecollaborationcorner.com/wp-content/uploads/2016/09/part6.png";"IntranetContentLanguage"="FR" }  
    )

    Write-Host "8# Add carousel data..." -ForegroundColor Magenta

    # Create the configuration item for each language
    $ConfigurationItemsEN | ForEach-Object {

        $Item = Add-SPOListItem -List $CarouselItemsList -ContentType "Carousel Item" -Values $_
    }

    $ConfigurationItemsFR | ForEach-Object {

        $Item = Add-SPOListItem -List $CarouselItemsList -ContentType "Carousel Item"  -Values $_
    }

    # Add promoted links
    $PromotedLinksList = Get-SPOList -Identity "Links"
    $PromotedLinks = @(

	    @{ "Title"="Link 1";"LinkLocation"="http://dev.office.com/patterns-and-practices"},
	    @{ "Title"="Link 2";"LinkLocation"="http://dev.office.com/patterns-and-practices"},
	    @{ "Title"="Link 3";"LinkLocation"="http://dev.office.com/patterns-and-practices"}
    )

    $PromotedLinks | ForEach-Object {

        $Item = Add-SPOListItem -List $PromotedLinksList -Values $_
    }
}

Write-Host "Done!" -ForegroundColor Green

# Close the connection to the server
Disconnect-SPOnline


