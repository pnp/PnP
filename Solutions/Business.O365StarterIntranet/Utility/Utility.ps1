# Thanks to this blog post https://blog.areflyen.no/2015/10/28/enabling-scheduling-on-publishing-pages-in-sharepoint-online-office-365-using-csom-and-powershell/
function Enable-CustomItemScheduling {
[CmdletBinding()]
    Param(
        [Parameter(Mandatory=$true)]
        [Microsoft.SharePoint.Client.Web]$Web,
        [Parameter(Mandatory=$true)]
        [String]$PagesLibraryName
    )

    $List = $Web.Lists.GetByTitle($PagesLibraryName)
    # Get function from: https://gist.github.com/aflyen/4a098b69b9faa43fd1a3
    $ListParameter = Get-CustomLoadParameter -Object $List -PropertyName "EventReceivers"
    $Web.Context.Load($List, $ListParameter)
    Execute-SPOQuery

    # Prerequisites for using scheduling
    $List.EnableModeration = $true
    $List.EnableMinorVersions = $true
    $List.Update()

    $Assembly = "Microsoft.SharePoint.Publishing, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"
    $FullName = "Microsoft.SharePoint.Publishing.Internal.ScheduledItemEventReceiver"
    $EventReceiverItemAddedExists = $false
    $EventReceiverItemAddedType = [Microsoft.SharePoint.Client.EventReceiverType]::ItemAdded
    $EventReceiverItemAddedName = "Item Added Event Handler For Scheduling"
    $EventReceiverItemUpdatingExists = $false
    $EventReceiverItemUpdatingType = [Microsoft.SharePoint.Client.EventReceiverType]::ItemUpdating
    $EventRecieverItemUpdatingName = "Item Updating Event Handler For Scheduling"

    # Check if the event receivers already exists
    foreach ($EventReceiver in $List.EventReceivers)
    {
        if ($EventReceiver.ReceiverName -eq $EventReceiverItemAddedName) 
        {
            $EventReceiverItemAddedExists = $true
        }
        elseif ($EventReceiver.ReceiverName -eq $EventRecieverItemUpdatingName) 
        {
            $EventReceiverItemUpdatingExists = $true
        }
    }

    # Add event receiver for ItemAdded
    if ($EventReceiverItemAddedExists -ne $true)
    {
        $EventReceiverItemAdded = New-Object Microsoft.SharePoint.Client.EventReceiverDefinitionCreationInformation
        $EventReceiverItemAdded.EventType = $EventReceiverItemAddedType
        $EventReceiverItemAdded.ReceiverName = $EventReceiverItemAddedName
        $EventReceiverItemAdded.ReceiverClass = $FullName
        $EventReceiverItemAdded.ReceiverAssembly = $Assembly

        $List.EventReceivers.Add($EventReceiverItemAdded) | Out-Null
    }

    # Add event receiver for ItemUpdating
    if ($EventReceiverItemUpdatingExists -ne $true)
    {
        $EventReceiverItemUpdating = New-Object Microsoft.SharePoint.Client.EventReceiverDefinitionCreationInformation
        $EventReceiverItemUpdating.EventType = $EventReceiverItemUpdatingType
        $EventReceiverItemUpdating.ReceiverName = $EventRecieverItemUpdatingName
        $EventReceiverItemUpdating.ReceiverClass = $FullName
        $EventReceiverItemUpdating.ReceiverAssembly = $Assembly

        $List.EventReceivers.Add($EventReceiverItemUpdating)| Out-Null
    }

    # Make fields for start and end date visible and add them to the default view
    if ($EventReceiverItemAddedExists -ne $true -or $EventReceiverItemUpdatingExists -ne $true)
    {
        $FieldPublishingStartDateName = "PublishingStartDate"
        $FieldPublishingExpirationDateName = "PublishingExpirationDate"

        $FieldPublishingStartDate = $List.Fields.GetByInternalNameOrTitle($FieldPublishingStartDateName)
        $FieldPublishingStartDate.Hidden = $false
        $FieldPublishingStartDate.Update()

        $FieldPublishingExpirationDate = $List.Fields.GetByInternalNameOrTitle($FieldPublishingExpirationDateName)
        $FieldPublishingExpirationDate.Hidden = $false
        $FieldPublishingExpirationDate.Update()

        $ListDefaultView = $List.DefaultView
        $ListDefaultView.ViewFields.Add($FieldPublishingStartDateName)
        $ListDefaultView.ViewFields.Add($FieldPublishingExpirationDateName)
        $ListDefaultView.Update()

        $List.Update()
    }

    Execute-SPOQuery
}

function Get-CustomLoadParameter {
    [CmdletBinding()]
    param(
       [Parameter(Mandatory=$true)]
       [Microsoft.SharePoint.Client.ClientObject]$Object,
       [Parameter(Mandatory=$true)]
       [string]$PropertyName
    )

    # Reference: http://sharepoint.stackexchange.com/questions/126221/spo-retrieve-hasuniqueroleassignements-property-using-powershell

    $Context = $Object.Context
    $Load = [Microsoft.SharePoint.Client.ClientContext].GetMethod("Load") 
    $Type = $Object.GetType()
    $ClientLoad = $Load.MakeGenericMethod($Type) 

    $Parameter = [System.Linq.Expressions.Expression]::Parameter(($Type), $Type.Name)
    $Expression = [System.Linq.Expressions.Expression]::Lambda(
        [System.Linq.Expressions.Expression]::Convert(
            [System.Linq.Expressions.Expression]::PropertyOrField($Parameter,$PropertyName),
            [System.Object]
        ),
        $($Parameter)
        )
    $ExpressionArray = [System.Array]::CreateInstance($Expression.GetType(), 1)
    $ExpressionArray.SetValue($Expression, 0)

    return $ExpressionArray
}

function Set-FolderContentTypesOrder() {

	Param(

		[Parameter(Mandatory=$True,Position=1)]
		[string]$FolderRelativePath,

		[Parameter(Mandatory=$True)]
		$ContentTypes

	)

	$Folder = Ensure-SPOFolder -SiteRelativePath $FolderRelativePath
	$ContentTypeOrder = New-Object System.Collections.Generic.List[Microsoft.SharePoint.Client.ContentTypeId]

	if ($ContentTypes.Count -gt 0) {

		$ContentTypes | Foreach-Object {

			$ContentTypeName = $_
			$Ct = Get-SPOContentType  -List Pages | Where-Object { $_.Name -eq $ContentTypeName }

			if ($Ct) {

				$CtOrder = $ContentTypeOrder.Add($Ct.Id)
			}
		}

		$Property = Get-SPOProperty -ClientObject $Folder -Property UniqueContentTypeOrder

		$Folder.UniqueContentTypeOrder = $ContentTypeOrder
		$Folder.Update()

		Execute-SPOQuery	
	}
}