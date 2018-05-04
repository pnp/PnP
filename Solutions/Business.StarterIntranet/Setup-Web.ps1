[CmdletBinding()]
Param(
	[Parameter(Mandatory=$True,Position=1)]
	[string]$RootSiteUrl,

	[Parameter(Mandatory=$True)]
	[string]$UserName,

	[Parameter(Mandatory=$True)]
    [string]$Password,

    [Parameter(Mandatory=$False)]
	[switch]$IncludeData=$false,
    
    [Parameter(Mandatory=$False)]
    $ExcludeHandlers,
    
    [Parameter(Mandatory=$False)]
	[switch]$UpgradeSubSites=$false
)

# Include utility scripts and supported languages configuration
 . "./Configuration.ps1"
 . "./utility/Utility.ps1"

$0 = $myInvocation.MyCommand.Definition
$CommandDirectory = [System.IO.Path]::GetDirectoryName($0)

# Connect to the root site
$PasswordAsSecure = ConvertTo-SecureString $Password -AsPlainText -Force
$Credentials = New-Object System.Management.Automation.PSCredential ($UserName , $PasswordAsSecure)
Connect-PnPOnline -Url $RootSiteUrl -Credentials $Credentials

# Determine the SharePoint version
$ServerVersion = (Get-PnPContext).ServerLibraryVersion.Major

switch ($ServerVersion) 
{ 
	15 {$AssemblyVersion = "15.0.0.0"} 
	16 {$AssemblyVersion = "16.0.0.0"} 
    default {$AssemblyVersion = "16.0.0.0"}
}

# -------------------------------------------------------------------------------------
# Set the correct SharePoint assembly version in .aspx regarding the server version
# -------------------------------------------------------------------------------------
Get-ChildItem -Path ".\provisioning\artefacts" -Include "*.aspx","*.master" -Recurse | ForEach-Object {

    (Get-Content -Path $_.FullName) -replace "1[5|6]\.0\.0\.0",$AssemblyVersion | Out-File -FilePath $_.FullName
}
 
$LanguageChoices = @()

# Get the language informations from the Languages.ps1 script
$Languages | ForEach-Object {

    # Add the language as an available language in the site (i.e. choice field value)
    $LanguageChoices += $_.Label.ToUpper()
}

# Set the language field according to the language settings in the root site
$field = Get-PnPField -Identity IntranetContentLanguage -ErrorAction SilentlyContinue

if ($field) {
    $field.Choices = $LanguageChoices
    $field.UpdateAndPushChanges($true)
    Execute-PnPQuery
}

# -------------------------------------------------------------------------------------
# Apply site template for webs according the languages
# -------------------------------------------------------------------------------------
function Configure-Web {

    Param(

		[Parameter(Mandatory=$True,Position=1)]
		$LanguageInfo
	)

    $Web = Get-PnPWeb
    $PagesLibraryName = $LanguageInfo.PagesLibraryName
    $LanguageLabelUpper = $LanguageInfo.Label.ToUpper()

    # -------------------------------------------------------------------------------------
    # Setup Pages library
    # -------------------------------------------------------------------------------------
    Write-Message -Message "`tConfiguring the pages library..." -NoNewline -ForegroundColor Gray

    # Enable Item Scheduling feature on the "Pages" library
    Enable-CustomItemScheduling -Web $Web -PagesLibraryName "$PagesLibraryName"

    # Create news and events folders in the "Pages" library of the web
    # Folder names have to be the same to ensure symetric peer copy
    $NewsFolder = Ensure-PnPFolder -SiteRelativePath "$PagesLibraryName/News"
    $EventsFolder = Ensure-PnPFolder -SiteRelativePath "$PagesLibraryName/Events"

    # Add the Hidden Side Bar column to the pages library.
    # There is a bug due to the delimiter character when provisioned by the root site XML template due to the site language
    $Formula =  "=IF([Hide Side Bar],1,0)"
    
    $FieldXml = 
    "<Field Type=""Calculated"" 
            DisplayName=""HideSideBarHidden"" 
            EnforceUniqueValues=""FALSE"" 
            Indexed=""FALSE"" 
            Format=""DateOnly""
            Decimals=""0""
            ResultType=""Number"" 
            ReadOnly=""TRUE"" 
            ID=""{2a26fbed-fc44-47b8-8d65-48f27f78a687}"" 
            SourceID=""{3b9f38ef-4cd3-4932-a203-b1cdcd8b5e51}"" 
            StaticName=""HideSideBarHidden"" 
            Name=""HideSideBarHidden"">
        <Formula>$Formula</Formula>
    </Field>"

    $HideSideBarHiddenCalcField = Get-PnPField -Identity HideSideBarHidden -List "$PagesLibraryName" -ErrorAction SilentlyContinue

    if (-not($HideSideBarHiddenCalcField)) {
        $HideSideBarHiddenCalcField = Add-PnPFieldFromXml -FieldXml $FieldXml -List "$PagesLibraryName"
    }

    # Same method for the "allow page comments" flag
    $Formula =  "=IF([Allow Page Comments],1,0)"
    
    $FieldXml = 
    "<Field Type=""Calculated"" 
            DisplayName=""IntranetAllowPageCommentsHidden"" 
            EnforceUniqueValues=""FALSE"" 
            Indexed=""FALSE"" 
            Format=""DateOnly""
            Decimals=""0""
            ResultType=""Number"" 
            ReadOnly=""TRUE"" 
            ID=""{57e57749-2bae-4cfa-bb39-d03a1b77ea51}"" 
            SourceID=""{e395aa87-1c71-45c9-be1a-a22ac1a38922}"" 
            StaticName=""IntranetAllowPageCommentsHidden"" 
            Name=""IntranetAllowPageCommentsHidden"">
        <Formula>$Formula</Formula>
    </Field>"

    $IntranetAllowPageCommentsHiddenCalcField = Get-PnPField -Identity IntranetAllowPageCommentsHidden -List "$PagesLibraryName" -ErrorAction SilentlyContinue

    if (-not($IntranetAllowPageCommentsHiddenCalcField)) {
        $IntranetAllowPageCommentsHiddenCalcField = Add-PnPFieldFromXml -FieldXml $FieldXml -List "$PagesLibraryName"
    }

    # Notification fields for colors
    $Fields = @{"IntranetNotificationBgColor"=@("#FCAF17","Notification Background Color");"IntranetNotificationTextColor"=@("#555555","Notification Text Color")}

    $Fields.Keys | ForEach-Object {

        $Field = Get-PnPField -Identity $_ -List "/Lists/Notifications" -ErrorAction SilentlyContinue

        if (-not($Field)) {
                
                $DisplayName = $Fields.Item($_)[1]
                $Guid = "{" + [guid]::NewGuid() + "}"
                $InternalName = $_
                $ValidationMessage = "The value must be a valid hexadecimal color (ex: #FFFFFF)"
                $DefaultValue = $Fields.Item($_)[0]
            
                $FieldXml = 
                    "<Field Type=""Text"" 
                            DisplayName=""$DisplayName"" 
                            Required=""FALSE"" 
                            EnforceUniqueValues=""FALSE"" 
                            Indexed=""FALSE"" 
                            MaxLength=""255"" 
                            Group=""Intranet"" 
                            ID=""$Guid"" 
                            SourceID=""$Guid"" 
                            StaticName=""$InternalName"" Name=""$InternalName"">
                            <Validation Message=""$ValidationMessage"">=NOT(ISERROR(SEARCH(""#??????"",[$DisplayName])=1))</Validation>
                            <Default>$DefaultValue</Default>
                    </Field>"

           $Field = Add-PnPFieldFromXml -List "/Lists/Notifications" -FieldXml $FieldXml
        }
    }

    # Content Types order
    $ContentTypesOrderRoot = @(

	    [PSCustomObject]@{FolderName="$PagesLibraryName";ContentTypes=@("Home Page","Static Page","Search Page")},
	    [PSCustomObject]@{FolderName="$PagesLibraryName/News";ContentTypes=@("News Page")}
	    [PSCustomObject]@{FolderName="$PagesLibraryName/Events";ContentTypes=@("Event Page")}
    )

    $ContentTypesOrderRoot | Foreach-Object { Set-FolderContentTypesOrder -FolderRelativePath $_.FolderName -ContentTypes $_.ContentTypes }

    # Approve folders
    $NewsFolder,$EventsFolder | ForEach-Object {
        $Item = Get-PnPProperty -ClientObject $_ -Property ListItemAllFields
        $ListItem = Get-PnPListItem -List $PagesLibraryName -Id $Item.Id
        $ListItem["_ModerationStatus"] = 0
        $ListItem.Update()
        Execute-PnPQuery
    }

    # Set the default value for the language column (i.e. 'EN', 'FR', etc.)
    $Field = Get-PnPField -List $PagesLibraryName -Identity IntranetContentLanguage
    $Field.DefaultValue = $LanguageInfo.Label.ToUpper()
    $Field.Update()
    Execute-PnPQuery

    Write-Message -Message "Done!" -ForegroundColor Green

    # -------------------------------------------------------------------------------------
    # Setup taxonomy
    # -------------------------------------------------------------------------------------
    Write-Message -Message "`tConfiguring the taxonomy..." -NoNewline -ForegroundColor Gray

    $CurrentSite = Get-PnPSite
    $Session = Get-PnPTaxonomySession
    $TermStore = $Session.GetDefaultSiteCollectionTermStore();
    $SiteCollectionTermGroup = $TermStore.GetSiteCollectionGroup($CurrentSite, $false)
    $IntranetTermGroupName = Get-PnPProperty -ClientObject $SiteCollectionTermGroup -Property Name
    
    $SiteMapTermSetName = "Site Map " + $LanguageLabelUpper
    $HeaderLinksTermSetName = "Header Links " + $LanguageLabelUpper
    $FooterLinksTermSetName = "Footer Links " + $LanguageLabelUpper

    # Get navigation term sets Id for the current language
    $SiteMapTermSet = Get-PnPTaxonomyItem -Term "$IntranetTermGroupName|$SiteMapTermSetName"
    $SiteMapTermSetId = $SiteMapTermSet.Id

    $HeaderLinksTermSet = Get-PnPTaxonomyItem -Term "$IntranetTermGroupName|$HeaderLinksTermSetName"
    $HeaderLinksTermSetId = $HeaderLinksTermSet.Id

    $FooterLinksTermSet = Get-PnPTaxonomyItem -Term "$IntranetTermGroupName|$FooterLinksTermSetName"
    $FooterLinksTermSetId = $FooterLinksTermSet.Id

    $SiteMapPositionField = Get-PnPField -Identity IntranetSiteMapPosition -List $PagesLibraryName -ErrorAction SilentlyContinue

    if (-not($SiteMapPositionField)) {

        # Link the Site Map Position Field to the according site map term set
        $SiteMapPositionField = Add-PnPTaxonomyField -List $PagesLibraryName -InternalName IntranetSiteMapPosition -TermSetPath "$IntranetTermGroupName|$SiteMapTermSetName" -DisplayName "Site Map Position" -Group Intranet -FieldOptions AddToDefaultContentType  
    
        "News Page","Event Page" | ForEach-Object {
        
            $FieldReferenceLink = New-Object Microsoft.SharePoint.Client.FieldLinkCreationInformation
            $FieldReferenceLink.Field = $SiteMapPositionField;
            $ContentType = Get-PnPContentType -List $PagesLibraryName -Identity $_
            $ContentType.FieldLinks.Add($FieldReferenceLink) | Out-Null
            $ContentType.Update($false) | Out-Null
        } 

        Execute-PnPQuery
    }
   
    Write-Message -Message "Done!" -ForegroundColor Green

    # -------------------------------------------------------------------------------------
    # Setup search navigation
    # -------------------------------------------------------------------------------------
    Write-Message -Message "`tConfiguring search navigation..." -NoNewline -ForegroundColor Gray

    $ExistingSrchNodes = Get-SearchNavigationNodes

    if ($ExistingSrchNodes.Count -eq 0) {

        $LanguageInfo.SearchNavigation | ForEach-Object {

            $Url = $Web.Url + "/" + $PagesLibraryName + "/" + $_.Url

            Add-PnPNavigationNode -Location SearchNav -Title $_.Title -Url $Url -External
        }
    }

    Write-Message -Message "Done!" -ForegroundColor Green

    if ($IncludeData.IsPresent) {
        # -------------------------------------------------------------------------------------
        # Add sample data
        # -------------------------------------------------------------------------------------
        Write-Message -Message "Adding sample data for the notifications banner..." -NoNewline

        $NotificationsList = Get-PnPList -Identity "Notifications"

        $Notification = @{ "Title"="PnP Starter Intranet official documentation";"IntranetNotificationDescription"='<div>Thanks for using the PnP Starter Intranet solution! You could also check the <a href="https&#58;//transactions.sendowl.com/packages/48364/D024B326/view">official documentation</a> if you want to know more ;) (FR &amp; EN available)</div>'}

        $Item = Add-PnPListItem -List $NotificationsList
        $Item = Set-PnPListItem -Identity  $Item.Id -List $NotificationsList -Values $Notification -ContentType "Notification"

        Write-Message -Message "`tDone!" -ForegroundColor Green
    }
    
    # -------------------------------------------------------------------------------------
    # Setup the configuration list
    # -------------------------------------------------------------------------------------
    Connect-PnPOnline -Url $CurrentSite.Url -Credentials $Credentials

    $ConfigurationList = Get-PnPList -Identity "Configuration"

    Write-Message -Message "`tAdding configuration item..." -NoNewline -ForegroundColor Gray

    $ConfigurationItemValues = @{ "Title"="Default $LanguageLabelUpper";"ForceCacheRefresh"=1;"SiteMapTermSetId"=$SiteMapTermSetId;"HeaderLinksTermSetId"=$HeaderLinksTermSetId;"FooterLinksTermSetId"=$FooterLinksTermSetId;"IntranetContentLanguage"="$LanguageLabelUpper";"AppInsightsInstrumentationKey"=$AppInsightsInstrumentationKey }

    # Check if the configuration item is already present for the current language
    $CamlQuery = "<View><Query><Where><Eq><FieldRef Name='IntranetContentLanguage'></FieldRef><Value Type='Text'>$LanguageLabelUpper</Value></Eq></Where></Query></View>"
    $ConfigItem = Get-PnPListItem -List "Configuration" -Query $CamlQuery

    if (-not($ConfigItem)) {
        # We create items in two steps because of a bug with the Add-PnPListItem since the February release https://github.com/SharePoint/PnP-PowerShell/issues/778
        $Item = Add-PnPListItem -List $ConfigurationList
        $Item = Set-PnPListItem -Identity  $Item.Id -List $ConfigurationList -Values $ConfigurationItemValues -ContentType "Item" 
    }
    
    Write-Message -Message "Done!" -ForegroundColor Green  
}

if ($Languages.Count -gt 1) {

    # If multiple languages are set, we create a sub web for each one (like SharePoint variations do). We do this to benefit of the SharePoint MUI.
    $Languages | ForEach-Object {

        $CurrentLanguage = $_
        $LanguageLabel = $CurrentLanguage.Label

        # Check if the sub web already exists
        $web = Get-PnPSubWebs | ForEach-Object { 

            try {
                Get-PnPWeb -Identity $LanguageLabel -ErrorAction Stop | Out-Null
                $IsWebExists = $true
            } catch {
                $IsWebExists = $false
            }
        }
        
        if ($IsWebExists -and -not($UpgradeSubSites.IsPresent)) {
        
            Write-Message -Message "Warning: sub web with label $LanguageLabel already exists. Use '-UpgradeSubSites' switch parameter to update it. Skipping..." -ForegroundColor White

        } else {

            Write-Message -Message "Creating and configuring the sub web for the language '$LanguageLabel'..."

            if (-not($IsWebExists)) {
                # Create subsites for languages with the corresponding template
                $SubWeb = New-PnPWeb -Title $CurrentLanguage.Title -Url $CurrentLanguage.Label -InheritNavigation -Locale $CurrentLanguage.LCID -Template CMSPUBLISHING#0
            } else {
                $SubWeb = Get-PnPWeb -Identity $CurrentLanguage.Label
            }

            Connect-PnPOnline -Url $SubWeb.Url -Credentials $Credentials

            $PagesLibraryName = (Get-PnPList -Identity (Get-PnPPropertyBag -Key __PagesListId)).Title
            $CurrentLanguage | Add-Member -MemberType NoteProperty -Name PagesLibraryName -Value $PagesLibraryName

            $Parameters = @{ 
                "CompanyName" = $AppFolderName; 
                "AssemblyVersion" = $AssemblyVersion; 
                "PagesLibraryName" = $PagesLibraryName; 
                "Language"= $LanguageLabel.ToUpper();
            }
            
            $TemplateFilePath = Join-Path -Path $CommandDirectory  -ChildPath ("provisioning\" + $CurrentLanguage.TemplateFileName)
            
            if ($ExcludeHandlers) {
                Apply-PnPProvisioningTemplate -Path $TemplateFilePath -Parameters $Parameters -ExcludeHandlers $ExcludeHandlers
            } else {
                Apply-PnPProvisioningTemplate -Path $TemplateFilePath -Parameters $Parameters
            }
            
            Configure-Web -LanguageInfo $CurrentLanguage

            # Switch back to the root site context
            Connect-PnPOnline -Url $RootSiteUrl -Credentials $Credentials
        }
    }
        
} else {

    if ($Languages.Count -eq 1) {

        $CurrentLanguage = $Languages[0]
        $LanguageLabel = $CurrentLanguage.Label

        Write-Message -Message "Just one language is configured ('$LanguageLabel'), applying the template at the root web level..."

        $PagesLibraryName = (Get-PnPList -Identity (Get-PnPPropertyBag -Key __PagesListId)).Title
        $CurrentLanguage | Add-Member -MemberType NoteProperty -Name PagesLibraryName -Value $PagesLibraryName

        $Parameters = @{ 
            "CompanyName" = $AppFolderName; 
            "AssemblyVersion" = $AssemblyVersion; 
            "PagesLibraryName" = $PagesLibraryName; 
            "Language"= $LanguageLabel.ToUpper();
        }

        # By default, apply the first template to the root site directly
        $TemplateFilePath = Join-Path -Path $CommandDirectory  -ChildPath ("provisioning\" + $CurrentLanguage.TemplateFileName)
        
        if ($ExcludeHandlers) {
            Apply-PnPProvisioningTemplate -Path $TemplateFilePath -Parameters $Parameters -ExcludeHandlers $ExcludeHandlers
        } else {
            Apply-PnPProvisioningTemplate -Path $TemplateFilePath -Parameters $Parameters
        }

        Configure-Web -LanguageInfo $CurrentLanguage

        # Switch back to the root site context
        Connect-PnPOnline -Url $RootSiteUrl -Credentials $Credentials
    }
}
