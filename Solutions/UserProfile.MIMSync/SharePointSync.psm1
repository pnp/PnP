
$namespace = @{dsml="http://www.dsml.org/DSML"; 'ms-dsml'="http://www.microsoft.com/MMS/DSML"}

function Install-SharePointSyncConfiguration
{
<#
.Synopsis
   Configures the Synchronization Service for SharePoint User Profile Synchronization
.DESCRIPTION
   Long description
.EXAMPLE
   Install-SharePointSyncConfiguration -Path C:\SharePointSync -ForestDnsName litware.ca -ForestCredental (Get-Credential LITWARE\Administrator) -OrganizationalUnit 'ou=Litwarians,dc=Litware,dc=ca' -SharePointUrl http://SharePointServer:5555 -SharePointCredential (Get-Credential LITWARE\Administrator)
.EXAMPLE
    $spProps = @{
        Path                 = 'C:\Temp\SharePointSync'
        ForestDnsName        = 'litware.ca'
        ForestCredential     = New-Object PSCredential ("LITWARE\administrator", (ConvertTo-SecureString 'J$p1ter' -AsPlainText -Force))
        OrganizationalUnit   = 'ou=Litwarians,dc=Litware,dc=ca'
        SharePointUrl        = 'http://cmvm38386:9140'
        SharePointCredential = New-Object PSCredential ("LITWARE\administrator", (ConvertTo-SecureString 'J$p1ter' -AsPlainText -Force))
    }
    Install-SharePointSyncConfiguration @spProps -Verbose

#>
    [CmdletBinding()]
    [OutputType([int])]
    Param
    (
        # Path to the configuration XML files
        [Parameter(Mandatory=$true, Position=0)]
        $Path,

        # DNS name of the Active Directory forest to synchronize (ie - litware.ca)
        [Parameter(Mandatory=$true, Position=1)]
        $ForestDnsName,

        # Credential for connecting to Active Directory
        [Parameter(Mandatory=$true, Position=2)]
        [PSCredential]
        $ForestCredential,

        # OU to synchronize to SharePoint
        [Parameter(Mandatory=$true, Position=3)]
        $OrganizationalUnit,

        # URL for SharePoint
        [Parameter(Mandatory=$true, Position=4)]
        [Uri]    
        $SharePointUrl,

        # Credential for connecting to SharePoint
        [Parameter(Mandatory=$true, Position=5)]
        [PSCredential]
        $SharePointCredential,

        # Flow Direction for Profile Pictures
        [Parameter(Mandatory=$false, Position=6)]
        [ValidateSet('Export only (NEVER from SharePoint)', 'Import only (ALWAYS from SharePoint)')]
        [String]
        $PictureFlowDirection = 'Export only (NEVER from SharePoint)'
    )

    #region Pre-requisites
    if (-not (Get-SynchronizationServiceRegistryKey))
    {
        throw "The Synchronization Service is not installed on this computer.  Please install the MIM Synchronization Service on this computer, or run this script on a computer where the MIM Synchronization Service is installed." 
    }

    if (-not (Get-Service -Name FimSynchronizationService))
    {
        throw "The Synchronization Service is installed but not running.  Please start the MIM Synchronization Service before running this script (Start-Service -Name FimSynchronizationService).  If the service fails to start please see the event log for details." 
    }

    if ((Test-SynchronizationServicePermission) -eq $false)
    {
        throw "The current user must be a member of the Synchronization Service Admins group before this command can be run.  You may need to logoff/logon before the group membership takes effect."
    }
    #endregion

    ### Load the Synchronization PowerShell snap-in
    #Add-PSSnapin miis.ma.config
    Import-Module -Name (Join-Path (Get-SynchronizationServicePath) UIShell\Microsoft.DirectoryServices.MetadirectoryServices.Config.dll) 



    Write-Verbose "Contacting AD to get the partition details"
    $RootDSE                = [ADSI]"LDAP://$ForestDnsName/RootDSE"
    $DefaultNamingContext   = [ADSI]"LDAP://$($RootDSE.defaultNamingContext)"
    $ConfigurationPartition = [ADSI]"LDAP://$($RootDSE.configurationNamingContext)"

    Write-Verbose "Configuring the Active Directory Connector"
    Write-Verbose "  AD Forest:               $ForestDnsName"
    Write-Verbose "  AD OU:                   $OrganizationalUnit"
    Write-Verbose "  AD Credential:           $($ForestCredential.UserName)" 
    Write-Verbose "  AD Naming Partition:     $($RootDSE.defaultNamingContext)"
    Write-Verbose "  AD Config Partition:     $($RootDSE.configurationNamingContext)"

    $admaXmlFilePath = Join-Path $Path MA-ADMA.XML
    [xml]$admaXml = Get-Content -Path $admaXmlFilePath
    $admaXml.Save("$admaXmlFilePath.bak")

    ### Fix up the Private Configuration
    $ForestCredentialParts = $ForestCredential.UserName -split '\\'
    $admaXml.'saved-ma-configuration'.'ma-data'.'private-configuration'.'adma-configuration'.'forest-name'         = $ForestDnsName
    $admaXml.'saved-ma-configuration'.'ma-data'.'private-configuration'.'adma-configuration'.'forest-login-user'   = $ForestCredentialParts[1]
    $admaXml.'saved-ma-configuration'.'ma-data'.'private-configuration'.'adma-configuration'.'forest-login-domain' = $ForestCredentialParts[0]

    ### Fix up the Domain partition
    $domainPartition = Select-Xml -Xml $admaXml -XPath "//ma-partition-data/partition[name='DC=Litware,DC=com']"
    $domainPartition.Node.name = $DefaultNamingContext.distinguishedName.ToString()
    $domainPartition.Node.'custom-data'.'adma-partition-data'.dn = $DefaultNamingContext.distinguishedName.ToString()
    $domainPartition.Node.'custom-data'.'adma-partition-data'.name = $ForestDnsName
    $domainPartition.Node.'custom-data'.'adma-partition-data'.guid = (New-Object guid $DefaultNamingContext.objectGUID).ToString('B').ToUpper() 
    $domainPartition.Node.filter.containers.inclusions.inclusion = $DefaultNamingContext.distinguishedName.ToString()
    $domainPartition.Node.filter.containers.exclusions.exclusion = $ConfigurationPartition.distinguishedName.ToString()
    $domainPartition.Node.filter.containers.inclusions.inclusion = $OrganizationalUnit

    ### Fix up the Configuration partition
    $configPartition = Select-Xml -Xml $admaXml -XPath "//ma-partition-data/partition[name='CN=Configuration,DC=Litware,DC=com']"
    $configPartition.Node.name = $ConfigurationPartition.distinguishedName.ToString()
    $configPartition.Node.'custom-data'.'adma-partition-data'.dn = $ConfigurationPartition.distinguishedName.ToString()
    $configPartition.Node.'custom-data'.'adma-partition-data'.name = $ForestDnsName
    $configPartition.Node.'custom-data'.'adma-partition-data'.guid = (New-Object guid $ConfigurationPartition.objectGUID).ToString('B').ToUpper() 
    $configPartition.Node.filter.containers.inclusions.inclusion = "CN=Partitions," + $ConfigurationPartition.distinguishedName.ToString()

    $admaXml.Save($admaXmlFilePath)

    Write-Verbose "Importing the Synchronization Service configuration"
    Write-Verbose "  Path: $Path"
    Import-MIISServerConfig -Path $Path -Verbose    

    #region BUG - Avoiding the call to Set-MIISADMAConfiguration because it deletes the ADMA partitions
    #TODO - fix this part of the function once we get an updated Set-MIISADMAConfiguration PowerShell cmdlet from MIM 
    Write-Warning "======================================================================================="
    Write-Warning "IMPORTANT: the Password must be set on the AD Connector before sychronization will work"
    Write-Warning "======================================================================================="
    #Write-Verbose "Set-MIISADMAConfiguration -MAName ADMA -Forest $ForestDnsName -Credentials $ForestCredential -Verbose"
    #Set-MIISADMAConfiguration -MAName ADMA -Credentials $ForestCredential -Forest $ForestDnsName -Verbose  
    #endregion  
    
    Write-Verbose "Configuring the SharePoint Connector"
    Write-Verbose "  SharePoint URL:          $SharePointUrl"
    Write-Verbose "  SharePoint Host:         $($SharePointUrl.Host)"
    Write-Verbose "  SharePoint Port:         $($SharePointUrl.Port)"
    Write-Verbose "  SharePoint Picture Flow: $PictureFlowDirection"
    Write-Verbose "  SharePoint Protocol:     $($SharePointUrl.Scheme)"
    Write-Verbose "  SharePoint Credential:   $($SharePointCredential.UserName)"
    Set-MIISECMA2Configuration -MAName SPMA -ParameterUse ‘connectivity’ -HTTPProtocol $SharePointUrl.Scheme -HostName $SharePointUrl.Host -Port $SharePointUrl.Port -PictureFlowDirection $PictureFlowDirection -Credentials $SharePointCredential -Verbose

    Write-Verbose "Publishing the Sync Rules Extension DLL to the Sychronization Service extensions folder"      
    Publish-SynchronizationAssembly -Path (Join-Path $Path SynchronizationRulesExtensions.cs) -Verbose
}##Closing: function Install-SharePointSyncConfiguration

function Start-SharePointSync
{
<#
.Synopsis
   Synchronize Active Directory to SharePoint by running the Synchronization Service management agents
.DESCRIPTION
   The sychronization service configuration consists on an Active Directory management agent, and a SharePoint management agent.  This function runs them in the following order:
   1. ADMA Import
   2. ADMA Sync
   3. SPMA Import
   4. SPMA Sync
   5. SPMA Export
   6. SPMA Import
   7. SPMA Sync
.EXAMPLE
   Run the management agents in full mode (full import, full sync)
   Start-SharePointSync -Verbose
.EXAMPLE
   Run the management agents in delta mode (delta import, delta sync)
   Start-SharePointSync -Delta -Verbose
#>

    [CmdletBinding(SupportsShouldProcess=$true, ConfirmImpact='High')]
    [OutputType([int])]
    Param
    (
        # Turn on Delta operations for the management agents
        [Switch]
        $Delta
    )

### Run the connectors
if ($Delta)
{
    Start-ManagementAgent -Name ADMA -RunProfile DELTAIMPORT
    Start-ManagementAgent -Name ADMA -RunProfile DELTASYNC
    Start-ManagementAgent -Name SPMA -RunProfile DELTAIMPORT
    Start-ManagementAgent -Name SPMA -RunProfile DELTASYNC
}
else
{
    Start-ManagementAgent -Name ADMA -RunProfile FULLIMPORT
    Start-ManagementAgent -Name ADMA -RunProfile FULLSYNC
    Start-ManagementAgent -Name SPMA -RunProfile FULLIMPORT
    Start-ManagementAgent -Name SPMA -RunProfile FULLSYNC
}

$spma = Get-ManagementAgent -Name spma
$confirmMessage = @"

$($spma.NumExportAdd().ReturnValue) Adds
$($spma.NumExportUpdate().ReturnValue) Updates
$($spma.NumExportDelete().ReturnValue) Deletes

"@
if ($PSCmdlet.ShouldProcess('SharePoint',$confirmMessage))
{
    ### Run the Export to SharePoint and Confirming Import
    Start-ManagementAgent -Name SPMA -RunProfile EXPORT
    Start-ManagementAgent -Name SPMA -RunProfile DELTAIMPORT
    Start-ManagementAgent -Name SPMA -RunProfile DELTASYNC
}

}##Closing: function Start-SharePointSync


function Publish-SynchronizationAssembly
{
<#
.Synopsis
   Build a Sychronization Service rules extension from a source code file and output it to the Extensions folder
.DESCRIPTION
   The sychronization service can be extended by calling out to a .NET assembly containing synchronization rules.
   This function builds that assembly and outputs it to the synchronization service 'Extensions' folder.
   
.EXAMPLE
   #Build and publish the assembly for the SharePoint Synchronization Solution
   Publish-SynchronizationAssembly -Path C:\Temp\SharePointSync\SynchronizationRulesExtensions.cs -Verbose
#>
    [CmdletBinding()]
    [OutputType([void])]
    Param
    (
        # Path to the source code file
        [ValidateScript({
        if (-not(Test-Path $_ -PathType Leaf))
        {
            throw "Source code file not found: $_"
        } 
        else
        {
            Write-Verbose "Verified $_ exists."
            return $true
        }      
        })] 
        [String]
        $Path
    )
    $ExtensionsFolder = Join-Path (Get-SynchronizationServicePath) Extensions
    Write-Verbose "Assembly will be output to: $ExtensionsFolder"

    if (-not(Test-Path -Path $ExtensionsFolder -PathType Container))
    {
        throw "Extensions folder not found: $ExtensionsFolder"
    }

    $SynchronizationAssembly = Join-Path (Get-SynchronizationServicePath) Bin\Assemblies\Microsoft.MetadirectoryServicesEx.dll
    Write-Verbose "Assembly will reference: $SynchronizationAssembly"
    if (-not(Test-Path -Path $SynchronizationAssembly -PathType Leaf))
    {
        throw "Microsoft.MetadirectoryServicesEx.dll assembly not found: $SynchronizationAssembly"
    }

    Write-Verbose "Calling Add-Type to build and output the assembly..."
    Add-Type -Path $Path -ReferencedAssemblies $SynchronizationAssembly -OutputType Library -OutputAssembly (Join-Path $ExtensionsFolder SharePointSynchronization.dll)
    Write-Verbose "Done."

}##Closing: function Publish-SynchronizationAssembly

function Get-SynchronizationServiceRegistryKey
{
<#
.Synopsis
   Gets the Registry Key of the Synchronization Service
.DESCRIPTION
   The Synchronization Service registry contains some useful detail for automation, such as the file path, logging level, database name, etc
.EXAMPLE
   Get-SynchronizationServiceRegistryKey
#>

	### The registry location depends on the version of the sync engine (it changed in FIM2010)
	$synchronizationServiceRegistryKey = Get-ItemProperty HKLM:\SYSTEM\CurrentControlSet\Services\miiserver -ErrorAction silentlycontinue
	if (-not $synchronizationServiceRegistryKey )
	{
	    $synchronizationServiceRegistryKey = Get-ItemProperty HKLM:\SYSTEM\CurrentControlSet\Services\FIMSynchronizationService -ErrorAction silentlycontinue
	}    

    ### Only output if we found what we were looking for
	if ($synchronizationServiceRegistryKey)
    {
        Write-Verbose ("Found the key: {0}" -F $synchronizationServiceRegistryKey.PSPath)
        Write-Output $synchronizationServiceRegistryKey
    }
    else
	{
	    Write-Warning "Synchronization Service does not seem to be installed on this computer."	    
	}
}##Closing: function Get-SynchronizationServiceRegistryKey


function Get-SynchronizationServicePath
{
<#
.Synopsis
   Get the Path for the Synchronization Service
.DESCRIPTION
   The path to the Synchronization Service is handy for placing DLLs in the 'Extensions' folder and for locating Synchronization Service assemblies
.EXAMPLE
   Get-SynchronizationServicePath
#>
    $synchronizationServiceRegistryKey = Get-SynchronizationServiceRegistryKey
    Get-ItemProperty -Path (Join-Path $synchronizationServiceRegistryKey.PSPath Parameters) | Select-Object -ExpandProperty Path
}##Closing: function Get-SynchronizationServicePath


function Start-ManagementAgent
{
<#
.Synopsis
   Executes a Run Profile on a Synchronization Service Management Agent
.DESCRIPTION
   Uses WMI to call the Execute method on the WMI MIIS_ManagementAgent Class
.EXAMPLE
   Start-ManagementAgent ADMA FULLIMPORT
.EXAMPLE
try{
    Start-ManagementAgent ADMA FULLIMPORT -StopOnError
    Start-ManagementAgent ADMA FULLSYNC   -StopOnError

    ### Export, Import and Sync
    Start-ManagementAgent SPMA EXPORT     -StopOnError
    Start-ManagementAgent SPMA FULLIMPORT -StopOnError
    Start-ManagementAgent SPMA FULLSYNC   -StopOnError
}
catch
{    
    ### Assign the Exception to a variable to play with
    $maRunException = $_

    ### Show the MA returnValue
    $maRunException.FullyQualifiedErrorId

    ### Show the details of the MA that failed
    $maRunException.TargetObject.MaGuid
    $maRunException.TargetObject.MaName
    $maRunException.TargetObject.RunNumber
    $maRunException.TargetObject.RunProfile
}
.OUTPUTS
   String ReturnValue - returned by the Execute() method of the WMI MIIS_ManagementAgent Class
#>
    [CmdletBinding()]
    [OutputType([String])]
    Param
    (
        # Management Agent Name
        [Parameter(Position = 0)]    
        [Alias("MA")] 
        [String]
        $Name,

        # RunProfile Name
        [Parameter(Position = 1)]
        [ValidateNotNull()]
        [String]
        $RunProfile,

        # StopOnError
        [Switch]
        $StopOnError
    )

        Write-Verbose "Using $Name as the MA name."
        Write-Verbose "Using $RunProfile as the RunProfile name."

        ### Get the WMI MA
        $ManagementAgent = Get-ManagementAgent $Name
        if (-not $ManagementAgent)
        {
            throw "MA not found: $Name"
        }

        ### Execute the Run Profile on the MA
        $ReturnValue = $ManagementAgent.Execute($RunProfile).ReturnValue 
        
        ### Construct a nice little parting gift for our callers
        $ReturnObject = [PSCustomObject] @{            
            MaName      = $ManagementAgent.Name            
            RunProfile  = $ManagementAgent.RunProfile().ReturnValue
            ReturnValue = $ReturnValue
            RunNumber   = $ManagementAgent.RunNumber().ReturnValue
        }       

		### Return our output - this will get sent to the caller when the MA finishes
        Write-output $ReturnObject

		### Throw according to $StopOnError
		if ($StopOnError -and $ReturnValue -ne 'success')
        {            
            throw New-Object Management.Automation.ErrorRecord @(
                New-Object InvalidOperationException "Stopping because the MA status was not 'success': $ReturnValue"
                $ReturnValue
                [Management.Automation.ErrorCategory]::InvalidResult
                $ReturnObject
            )
        }##Closing: if ($StopOnError...
}##Closing: function Start-ManagementAgent


function Get-ManagementAgent
{
<#
    .SYNOPSIS 
    Gets the Management Agent(s) from the Synchronization Service 

    .DESCRIPTION
    The Get-ManagementAgent function uses the MIIS WMI class to get the management agent   

    .PARAMETER Name
    Specifies the name of the MA to be retrieved.

    .OUTPUTS
    The WMI object containing the management agent
	
    .EXAMPLE
	Get-ManagementAgent -Name ADMA 
    This command will will retrieve a Management Agent named "ADMA" 

	.EXAMPLE
	Get-ManagementAgent -Verbose
    This command will will retrieve all Management Agents
#>
  Param
    (        
        [parameter(Mandatory=$false)] 
        $Name
    ) 

	if($PSBoundParameters.ContainsKey('Name'))
	{     
        Write-Verbose "Using WMI to query for a management agent with name: $Name"
        Get-WmiObject -Class MIIS_ManagementAgent -Namespace root/MicrosoftIdentityIntegrationServer -Filter "Name='$Name'"	
	}
    else
    {
        Write-Verbose "Using WMI to query for all management agents"
        Get-WmiObject -Class MIIS_ManagementAgent -Namespace root/MicrosoftIdentityIntegrationServer 
    } 
}##Closing: function Get-ManagementAgent


function Start-SynchronizationServiceManager
{
<#
.Synopsis
   Starts the User Interface for the Synchronization Service
.DESCRIPTION
   The Synchronization Service Manager (miisclient.exe) is the administration client for the Synchronization Service.  It can be used to configure and operate the Synchronization Service.  Synchronization Service permissions are required to run the UI.
.EXAMPLE
   Start-SynchronizationServiceManager
#>
    Start-Process -FilePath (Join-Path (Get-SynchronizationServicePath) UIShell\miisclient.exe)
}##Closing: function Start-SynchronizationServiceManager


function Test-SynchronizationServicePermission
{
<#
.Synopsis
   Test if the current user is able to access the Synchronization Service
.DESCRIPTION
   The Synchronization Service is secured by security groups specified when the Synchronization Service is installed. If the current user is not a member of the 'MIIS Admins' or 'MIIS Operators' then this function will return false.
.EXAMPLE
   Test-SynchronizationServicePermission -Verbose
#>
    try
    {
        Write-Verbose "Using WMI class 'MIIS_Server' to query the local Synchronization Serivce" 
        Get-WmiObject -Class MIIS_Server -Namespace root/MicrosoftIdentityIntegrationServer -ErrorAction Stop
    }
    catch [System.Management.ManagementException]
    {
        if ($_.Exception.ErrorCode -eq 'AccessDenied')
        {
            Write-Warning 'The WMI query returned with Access-Denied'
            return $false
        }
        else
        {
            Write-Warning 'The WMI query failed for some other reason.'
            throw
        }
    }
    Write-Verbose 'Congratulations!  You are an esteemed member of the Synchronization Service Admins or Operators group!'
    return $true
}##Closing: function Test-SynchronizationServicePermission


function ConvertTo-SharePointEcma2
{
<#
.Synopsis
   Converts a SharePoint ECMA1 based management agent to a SharePoint ECMA2 based management agent
.DESCRIPTION
   This function upgrades a Synchronization Service configuration by parsing the server configuration XML files to find the SharePoint ECMA1 management agent.
   The XML file representing that management agent is then updated to convert the management agent to the newer ECMA2-based SharePoint management agent.
.EXAMPLE
   Convert the file by name
   ConvertTo-SharePointEcma2 -Path C:\ManagementAgentFile.XML
.EXAMPLE
   Convert the file by path
   ConvertTo-SharePointEcma2 -Path C:\SyncServerExport\
#>
    [CmdletBinding()]
    Param
    (
        # Path to the Sychronization Service server export XML file to be converted
        [Parameter(Mandatory=$true,ValueFromPipeline=$true,Position=0)]
        [ValidateScript(
        {
            if (Test-Path $_) 
            {
                if (Test-Path -Path $_ -PathType Container)
                {
                    $_ = Join-Path $_ *.xml
                }
                if (Select-Xml -Path $_ -XPath "//ma-data[category='Extensible' and ma-listname='MOSS-UserProfile']")
                {
                    return $true
                } 
                else
                {
                    throw "Could not locate the management agent file using this XPath: //ma-data[category='Extensible' and ma-listname='MOSS-UserProfile'].  Verify the path contains the Synchronization Service configuration XML files."
                }
            }
            else
            {          
                throw 'The path could not be accessed (Test-Path failed).'
            }  
        })]
        $Path
    )

    ### Get the ma-data file with category == Extensible and ma-listname == MOSS-UserProfile
    if (Test-Path -Path $Path -PathType Container)
    {
        $Path = Join-Path $Path *.xml
    }

    Write-Verbose "Using XPath //ma-data[category='Extensible' and ma-listname='MOSS-UserProfile'] to find the SPMA in $Path"
    $spma1File = Select-Xml -Path $Path -XPath "//ma-data[category='Extensible' and ma-listname='MOSS-UserProfile']"

    ### Backup the original file
    #TODO: consider not clobbering the BAK file
    Write-Verbose "Copying original file to: $($spma1File.Path).BAK"
    Copy-Item -Path $spma1File.Path -Destination "$($spma1File.Path).BAK" -Force

    ### Load the ma-data XML into an XML Document
    Write-Verbose "Loading SPMA file: $($spma1File.Path)"
    [xml]$spmaToConvert = Get-Content -Path $spma1File.Path

    ### Get items we need to keep
    Write-Verbose "Getting details we need to keep:"
    $maPartitionID         = $spmaToConvert.'saved-ma-configuration'.'ma-data'.'ma-partition-data'.partition.id
    $maConnectTo           = $spmaToConvert.'saved-ma-configuration'.'ma-data'.'private-configuration'.MAConfig.'extension-config'.'connection-info'.'connect-to' -as [uri]
    $maDomain,$maUserName  = $spmaToConvert.'saved-ma-configuration'.'ma-data'.'private-configuration'.MAConfig.'extension-config'.'connection-info'.user -split '\\'
    Write-Verbose "  MA Partition ID: $maPartitionID"
    Write-Verbose "  MA Connect To:   $($maConnectTo.OriginalString)"
    Write-Verbose "  MA Domain:       $maDomain"
    Write-Verbose "  MA User Name:    $maUserName"

    <# ma-data/format-version
    No change
    #>

    <# ma-data/id 
    No change.
    This is the management agent GUID and must be kept the same.
    It is referenced througout the configuration.
    #>

    <# ma-data/name 
    No change.
    This is the management agent name and must be kept the same.
    It is likely used in scripts to automate the run profiles via WMI, changing the name would break those scripts.
    #>

    <# ma-data/category
    Must be changed to Extensible2 to match the ECMA2
    #>
    Write-Verbose "Updating ma-data/category to 'Extensible2'"
    $spmaToConvert.'saved-ma-configuration'.'ma-data'.category = 'Extensible2'

    <# ma-data/subtype
    Cosmetic.  Changed to match the ECMA2 SPMA
    #>
    Write-Verbose "Updating ma-data/subtype to 'SharePoint Profile Store'"
    $spmaToConvert.'saved-ma-configuration'.'ma-data'.subtype = 'SharePoint Profile Store'

    <# ma-data/ma-listname
    Cosmetic.  Changed to match the ECMA2 SPMA
    #>
    Write-Verbose "Updating ma-data/ma-listname to 'SharePoint Profile Store'"
    $spmaToConvert.'saved-ma-configuration'.'ma-data'.'ma-listname' = 'SharePoint Profile Store'


    <# ma-data/ma-companyname
    Cosmetic.  Changed to match the ECMA2 SPMA
    #>
    Write-Verbose "Updating ma-data/ma-companyname to 'Microsoft'"
    $spmaToConvert.'saved-ma-configuration'.'ma-data'.'ma-companyname' = 'Microsoft'

    <# ma-data/creation-time
    Not changed.
    #>

    <# ma-data/last-modification-time
    Not changed.
    #>

    <# ma-data/version
    Not changed.
    #>

    <# ma-data/internal-version
    Not changed.
    #>

    <# ma-data/password-sync-allowed
    Not changed.
    Both versions of the SPMA have a value of 0 for this.  Since they are the same, it did not need to be changed.
    #>

    <# ma-data/schema
    Copied completely from ECMA2 SPMA.
    Not user modifiable, so no attempt to merge was done.
    #>
    Write-Verbose "Replacing ma-data/schema with the SPMA V2 schema"
    $spmaToConvert.'saved-ma-configuration'.'ma-data'.schema.InnerXml = @'
    <dsml:dsml xmlns:ms-dsml="http://www.microsoft.com/MMS/DSML" xmlns:dsml="http://www.dsml.org/DSML">
      <dsml:directory-schema ms-dsml:no-objectclass-validation="true">
        <dsml:class id="group" type="structural" ms-dsml:locked="1">
          <dsml:name>group</dsml:name>
          <dsml:attribute ref="#ProfileIdentifier" required="false" ms-dsml:isAnchor="false" ms-dsml:allowedOperation="ExportOnly" />
          <dsml:attribute ref="#SID" required="false" ms-dsml:isAnchor="false" ms-dsml:allowedOperation="ExportOnly" />
          <dsml:attribute ref="#SourceReference" required="false" ms-dsml:isAnchor="false" ms-dsml:allowedOperation="ImportOnly" />
          <dsml:attribute ref="#GroupType" required="false" ms-dsml:isAnchor="false" ms-dsml:allowedOperation="ExportOnly" />
          <dsml:attribute ref="#PreferredName" required="false" ms-dsml:isAnchor="false" ms-dsml:allowedOperation="ExportOnly" />
          <dsml:attribute ref="#Description" required="false" ms-dsml:isAnchor="false" ms-dsml:allowedOperation="ExportOnly" />
          <dsml:attribute ref="#Url" required="false" ms-dsml:isAnchor="false" ms-dsml:allowedOperation="ExportOnly" />
          <dsml:attribute ref="#MailNickName" required="false" ms-dsml:isAnchor="false" ms-dsml:allowedOperation="ExportOnly" />
          <dsml:attribute ref="#domain" required="false" ms-dsml:isAnchor="false" ms-dsml:allowedOperation="ImportOnly" />
          <dsml:attribute ref="#Member" required="false" ms-dsml:isAnchor="false" ms-dsml:allowedOperation="ExportOnly" />
          <dsml:attribute ref="#Anchor" required="true" ms-dsml:isAnchor="true" />
          <dsml:attribute ref="#export_password" required="false" />
        </dsml:class>
        <dsml:class id="contact" type="structural" ms-dsml:locked="1">
          <dsml:name>contact</dsml:name>
          <dsml:attribute ref="#ProfileIdentifier" required="false" ms-dsml:isAnchor="false" ms-dsml:allowedOperation="ExportOnly" />
          <dsml:attribute ref="#SPS-DistinguishedName" required="false" ms-dsml:isAnchor="false" ms-dsml:allowedOperation="ExportOnly" />
          <dsml:attribute ref="#SPS-SourceObjectDN" required="false" ms-dsml:isAnchor="false" ms-dsml:allowedOperation="ExportOnly" />
          <dsml:attribute ref="#PreferredName" required="false" ms-dsml:isAnchor="false" ms-dsml:allowedOperation="ExportOnly" />
          <dsml:attribute ref="#UserName_Contact" required="false" ms-dsml:isAnchor="false" ms-dsml:allowedOperation="ImportOnly" />
          <dsml:attribute ref="#domain" required="false" ms-dsml:isAnchor="false" ms-dsml:allowedOperation="ImportOnly" />
          <dsml:attribute ref="#Anchor" required="true" ms-dsml:isAnchor="true" />
          <dsml:attribute ref="#export_password" required="false" />
        </dsml:class>
        <dsml:class id="user" type="structural" ms-dsml:locked="1">
          <dsml:name>user</dsml:name>
          <dsml:attribute ref="#ProfileIdentifier" required="false" ms-dsml:isAnchor="false" ms-dsml:allowedOperation="ExportOnly" />
          <dsml:attribute ref="#SID" required="false" ms-dsml:isAnchor="false" ms-dsml:allowedOperation="ExportOnly" />
          <dsml:attribute ref="#domain" required="false" ms-dsml:isAnchor="false" ms-dsml:allowedOperation="ImportOnly" />
          <dsml:attribute ref="#UserProfile_GUID" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#AccountName" required="false" ms-dsml:isAnchor="false" ms-dsml:allowedOperation="ImportOnly" />
          <dsml:attribute ref="#FirstName" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-PhoneticFirstName" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#LastName" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-PhoneticLastName" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#PreferredName" required="false" ms-dsml:isAnchor="false" ms-dsml:allowedOperation="ExportOnly" />
          <dsml:attribute ref="#SPS-PhoneticDisplayName" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#WorkPhone" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#Department" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#Title" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-JobTitle" required="false" ms-dsml:isAnchor="false" ms-dsml:allowedOperation="ExportOnly" />
          <dsml:attribute ref="#SPS-Department" required="false" ms-dsml:isAnchor="false" ms-dsml:allowedOperation="ExportOnly" />
          <dsml:attribute ref="#Manager" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#AboutMe" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#PersonalSpace" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#Picture" required="false" ms-dsml:isAnchor="false" ms-dsml:allowedOperation="ExportOnly" />
          <dsml:attribute ref="#UserName" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#QuickLinks" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#WebSite" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#PublicSiteRedirect" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-DataSource" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-MemberOf" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-Dotted-line" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-Peers" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-Responsibility" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-SipAddress" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-MySiteUpgrade" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-DontSuggestList" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-ProxyAddresses" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-HireDate" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-DisplayOrder" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-ClaimID" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-ClaimProviderID" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-ClaimProviderType" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-LastColleagueAdded" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-OWAUrl" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-SavedAccountName" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-ResourceAccountName" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-ObjectExists" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-MasterAccountName" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-UserPrincipalName" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-PersonalSiteCapabilities" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-O15FirstRunExperience" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-PersonalSiteInstantiationState" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-DistinguishedName" required="false" ms-dsml:isAnchor="false" ms-dsml:allowedOperation="ExportOnly" />
          <dsml:attribute ref="#SPS-SourceObjectDN" required="false" ms-dsml:isAnchor="false" ms-dsml:allowedOperation="ExportOnly" />
          <dsml:attribute ref="#SPS-LastKeywordAdded" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-FeedIdentifier" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#WorkEmail" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#CellPhone" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#Fax" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#HomePhone" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#Office" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-Location" required="false" ms-dsml:isAnchor="false" ms-dsml:allowedOperation="ExportOnly" />
          <dsml:attribute ref="#Assistant" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-PastProjects" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-Skills" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-School" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-Birthday" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-StatusNotes" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-Interests" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-HashTags" required="false" ms-dsml:isAnchor="false" ms-dsml:allowedOperation="ExportOnly" />
          <dsml:attribute ref="#SPS-PictureTimestamp" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-EmailOptin" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-PicturePlaceholderState" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-PrivacyPeople" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-PrivacyActivity" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-PictureExchangeSyncState" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-MUILanguages" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-ContentLanguages" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-TimeZone" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-RegionalSettings-FollowWeb" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-Locale" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-CalendarType" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-AltCalendarType" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-AdjustHijriDays" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-ShowWeeks" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-WorkDays" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-WorkDayStartHour" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-WorkDayEndHour" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-Time24" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-FirstDayOfWeek" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-FirstWeekOfYear" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#SPS-RegionalSettings-Initialized" required="false" ms-dsml:isAnchor="false" />
          <dsml:attribute ref="#Anchor" required="true" ms-dsml:isAnchor="true" />
          <dsml:attribute ref="#export_password" required="false" />
        </dsml:class>
        <dsml:attribute-type id="ProfileIdentifier" single-value="true" ms-dsml:export-only="true">
          <dsml:name>ProfileIdentifier</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SID" single-value="true" ms-dsml:export-only="true">
          <dsml:name>SID</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.5</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SourceReference" single-value="true" ms-dsml:immutable="true">
          <dsml:name>SourceReference</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="GroupType" single-value="true" ms-dsml:export-only="true">
          <dsml:name>GroupType</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.27</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="PreferredName" single-value="true" ms-dsml:export-only="true">
          <dsml:name>PreferredName</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="Description" single-value="true" ms-dsml:export-only="true">
          <dsml:name>Description</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="Url" single-value="true" ms-dsml:export-only="true">
          <dsml:name>Url</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="MailNickName" single-value="true" ms-dsml:export-only="true">
          <dsml:name>MailNickName</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="domain" single-value="true" ms-dsml:immutable="true">
          <dsml:name>domain</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="Member" single-value="false" ms-dsml:export-only="true">
          <dsml:name>Member</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.12</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="Anchor" single-value="true" ms-dsml:immutable="true">
          <dsml:name>Anchor</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-DistinguishedName" single-value="true" ms-dsml:export-only="true">
          <dsml:name>SPS-DistinguishedName</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-SourceObjectDN" single-value="true" ms-dsml:export-only="true">
          <dsml:name>SPS-SourceObjectDN</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="UserName_Contact" single-value="true" ms-dsml:immutable="true">
          <dsml:name>UserName_Contact</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="UserProfile_GUID" single-value="true">
          <dsml:name>UserProfile_GUID</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="AccountName" single-value="true" ms-dsml:immutable="true">
          <dsml:name>AccountName</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="FirstName" single-value="true">
          <dsml:name>FirstName</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-PhoneticFirstName" single-value="true">
          <dsml:name>SPS-PhoneticFirstName</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="LastName" single-value="true">
          <dsml:name>LastName</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-PhoneticLastName" single-value="true">
          <dsml:name>SPS-PhoneticLastName</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-PhoneticDisplayName" single-value="true">
          <dsml:name>SPS-PhoneticDisplayName</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="WorkPhone" single-value="true">
          <dsml:name>WorkPhone</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="Department" single-value="true">
          <dsml:name>Department</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="Title" single-value="true">
          <dsml:name>Title</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-JobTitle" single-value="true" ms-dsml:export-only="true">
          <dsml:name>SPS-JobTitle</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-Department" single-value="true" ms-dsml:export-only="true">
          <dsml:name>SPS-Department</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="Manager" single-value="true">
          <dsml:name>Manager</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.12</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="AboutMe" single-value="true">
          <dsml:name>AboutMe</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="PersonalSpace" single-value="true">
          <dsml:name>PersonalSpace</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="Picture" single-value="true" ms-dsml:export-only="true">
          <dsml:name>Picture</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.5</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="UserName" single-value="true">
          <dsml:name>UserName</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="QuickLinks" single-value="true">
          <dsml:name>QuickLinks</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="WebSite" single-value="true">
          <dsml:name>WebSite</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="PublicSiteRedirect" single-value="true">
          <dsml:name>PublicSiteRedirect</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-DataSource" single-value="true">
          <dsml:name>SPS-DataSource</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-MemberOf" single-value="false">
          <dsml:name>SPS-MemberOf</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-Dotted-line" single-value="true">
          <dsml:name>SPS-Dotted-line</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.12</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-Peers" single-value="true">
          <dsml:name>SPS-Peers</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-Responsibility" single-value="false">
          <dsml:name>SPS-Responsibility</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-SipAddress" single-value="true">
          <dsml:name>SPS-SipAddress</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-MySiteUpgrade" single-value="true">
          <dsml:name>SPS-MySiteUpgrade</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.7</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-DontSuggestList" single-value="false">
          <dsml:name>SPS-DontSuggestList</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.12</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-ProxyAddresses" single-value="false">
          <dsml:name>SPS-ProxyAddresses</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-HireDate" single-value="true">
          <dsml:name>SPS-HireDate</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-DisplayOrder" single-value="true">
          <dsml:name>SPS-DisplayOrder</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.27</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-ClaimID" single-value="true">
          <dsml:name>SPS-ClaimID</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-ClaimProviderID" single-value="true">
          <dsml:name>SPS-ClaimProviderID</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-ClaimProviderType" single-value="true">
          <dsml:name>SPS-ClaimProviderType</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-LastColleagueAdded" single-value="true">
          <dsml:name>SPS-LastColleagueAdded</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-OWAUrl" single-value="true">
          <dsml:name>SPS-OWAUrl</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-SavedAccountName" single-value="true">
          <dsml:name>SPS-SavedAccountName</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-ResourceAccountName" single-value="true">
          <dsml:name>SPS-ResourceAccountName</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.12</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-ObjectExists" single-value="true">
          <dsml:name>SPS-ObjectExists</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-MasterAccountName" single-value="true">
          <dsml:name>SPS-MasterAccountName</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.12</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-UserPrincipalName" single-value="true">
          <dsml:name>SPS-UserPrincipalName</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-PersonalSiteCapabilities" single-value="true">
          <dsml:name>SPS-PersonalSiteCapabilities</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.27</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-O15FirstRunExperience" single-value="true">
          <dsml:name>SPS-O15FirstRunExperience</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.27</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-PersonalSiteInstantiationState" single-value="true">
          <dsml:name>SPS-PersonalSiteInstantiationState</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.27</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-LastKeywordAdded" single-value="true">
          <dsml:name>SPS-LastKeywordAdded</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-FeedIdentifier" single-value="true">
          <dsml:name>SPS-FeedIdentifier</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="WorkEmail" single-value="true">
          <dsml:name>WorkEmail</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="CellPhone" single-value="true">
          <dsml:name>CellPhone</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="Fax" single-value="true">
          <dsml:name>Fax</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="HomePhone" single-value="true">
          <dsml:name>HomePhone</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="Office" single-value="true">
          <dsml:name>Office</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-Location" single-value="true" ms-dsml:export-only="true">
          <dsml:name>SPS-Location</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="Assistant" single-value="true">
          <dsml:name>Assistant</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.12</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-PastProjects" single-value="false">
          <dsml:name>SPS-PastProjects</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-Skills" single-value="false">
          <dsml:name>SPS-Skills</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-School" single-value="false">
          <dsml:name>SPS-School</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-Birthday" single-value="true">
          <dsml:name>SPS-Birthday</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-StatusNotes" single-value="true">
          <dsml:name>SPS-StatusNotes</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-Interests" single-value="false">
          <dsml:name>SPS-Interests</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-HashTags" single-value="false" ms-dsml:export-only="true">
          <dsml:name>SPS-HashTags</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-PictureTimestamp" single-value="true">
          <dsml:name>SPS-PictureTimestamp</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-EmailOptin" single-value="true">
          <dsml:name>SPS-EmailOptin</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.27</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-PicturePlaceholderState" single-value="true">
          <dsml:name>SPS-PicturePlaceholderState</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.27</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-PrivacyPeople" single-value="true">
          <dsml:name>SPS-PrivacyPeople</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.7</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-PrivacyActivity" single-value="true">
          <dsml:name>SPS-PrivacyActivity</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.27</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-PictureExchangeSyncState" single-value="true">
          <dsml:name>SPS-PictureExchangeSyncState</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.27</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-MUILanguages" single-value="true">
          <dsml:name>SPS-MUILanguages</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-ContentLanguages" single-value="true">
          <dsml:name>SPS-ContentLanguages</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-TimeZone" single-value="true">
          <dsml:name>SPS-TimeZone</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-RegionalSettings-FollowWeb" single-value="true">
          <dsml:name>SPS-RegionalSettings-FollowWeb</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.7</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-Locale" single-value="true">
          <dsml:name>SPS-Locale</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.27</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-CalendarType" single-value="true">
          <dsml:name>SPS-CalendarType</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.27</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-AltCalendarType" single-value="true">
          <dsml:name>SPS-AltCalendarType</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.27</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-AdjustHijriDays" single-value="true">
          <dsml:name>SPS-AdjustHijriDays</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.27</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-ShowWeeks" single-value="true">
          <dsml:name>SPS-ShowWeeks</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.7</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-WorkDays" single-value="true">
          <dsml:name>SPS-WorkDays</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.27</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-WorkDayStartHour" single-value="true">
          <dsml:name>SPS-WorkDayStartHour</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.27</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-WorkDayEndHour" single-value="true">
          <dsml:name>SPS-WorkDayEndHour</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.27</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-Time24" single-value="true">
          <dsml:name>SPS-Time24</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.7</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-FirstDayOfWeek" single-value="true">
          <dsml:name>SPS-FirstDayOfWeek</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.27</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-FirstWeekOfYear" single-value="true">
          <dsml:name>SPS-FirstWeekOfYear</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.27</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="SPS-RegionalSettings-Initialized" single-value="true">
          <dsml:name>SPS-RegionalSettings-Initialized</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.7</dsml:syntax>
        </dsml:attribute-type>
        <dsml:attribute-type id="export_password" single-value="true" ms-dsml:encrypted="true" ms-dsml:export-only="true">
          <dsml:name>export_password</dsml:name>
          <dsml:syntax>1.3.6.1.4.1.1466.115.121.1.15</dsml:syntax>
        </dsml:attribute-type>
      </dsml:directory-schema>
    </dsml:dsml>
'@

    <# ma-data/attribute-inclusion
    Changed to remove the 'ADGuid' attribute that is not present in ECMA2 SPMA schema.
    Changed to add 'Anchor' and 'ProfileIdentifier' attributes
    #TODO: (low priority) consider doing this dynamically instead of hard coding
    #>
    Write-Verbose "ma-data/attribute-inclusion by removing ADGuid"
    $spmaToConvert.'saved-ma-configuration'.'ma-data'.'attribute-inclusion'.RemoveChild($spmaToConvert.'saved-ma-configuration'.'ma-data'.'attribute-inclusion'.SelectSingleNode('./attribute[.="ADGuid"]'))
    
    $fraggle = $spmaToConvert.CreateDocumentFragment()
    $fraggle.InnerXML = '<attribute>Anchor</attribute><attribute>ProfileIdentifier</attribute>'
    $spmaToConvert.'saved-ma-configuration'.'ma-data'.'attribute-inclusion'.AppendChild($fraggle)

    <# ma-data/stay-disconnector
    Not changed.
    This node is for the Filter Rules, and can be mostly left alone.
    #TODO: scrub this for missing attributes
    #>

    <# ma-data/join
    Not changed.
    This node is for the Join Rules, and needs to be scrubbed for missing or export-only attributes.
    #TODO: scrub this for missing attributes
    #TODO: remove Join rules that depend on export-only attributes
    #>

    <# ma-data/projection
    Not changed.
    This node is for the Projection Rules, and can be left alone.
    Projection rules do not refer to attributes, just object types which are not changed.
    #>

    <# ma-data/export-attribute-flow
    This node is for the Export Attribute Flow Rules, and can be mostly left alone.
    We need to remove EAF rules when:
    1. the EAF CD attribute does not have a schema binding in the SPMA V2 schema
    2. the EAF CD attribute is marked as ImportOnly in the SPMA V2 schema
    3. the EAF CD attribute is not in the SPMA V2 attribute inclusion list
    #>
    Write-Verbose "Updating ma-data/export-attribute-flow"
    foreach ($exportFlowSet in $spmaToConvert.'saved-ma-configuration'.'ma-data'.'export-attribute-flow'.'export-flow-set')
    {
        Write-Verbose "  export-flow-set: cd-object-type[$($exportFlowSet.'cd-object-type')] mv-object-type[$($exportFlowSet.'mv-object-type')]"
        foreach ($exportFlow in $exportFlowSet.'export-flow')
        {
            Write-Verbose "    export-flow: cd-attribute [$($exportFlow.'cd-attribute')]"
            $deleteExportFlowRule = $false

            $xPathFilter = "//ma-data/schema/dsml:dsml/dsml:directory-schema/dsml:class[@id='$($exportFlowSet.'cd-object-type')']/dsml:attribute[@ref='#$($exportFlow.'cd-attribute')']"
            Write-Verbose "    XPath to find the schema binding: $xPathFilter"
            
            $SchemaBinding = Select-Xml -Xml $spmaToConvert -XPath $xPathFilter -Namespace $namespace
            
            #Check 1: is the EAF CD attribute bound to the object type?
            if (-not($SchemaBinding))
            {
                Write-Warning "No Schema binding: $xPathFilter"
                $deleteExportFlowRule = $true
            }

            #Check 2: is the EAF CD attribute bound to an attribute that is ImportOnly?
            if ($SchemaBinding.Node.allowedOperation -eq 'ImportOnly')
            {
                Write-Warning "Attribute is ImportOnly: $xPathFilter"
                $deleteExportFlowRule = $true
            }

            #Check 3: is the EAF CD attribute in the attribute inclusion list?
            if ($spmaToConvert.'saved-ma-configuration'.'ma-data'.'attribute-inclusion'.attribute -notcontains $exportFlow.'cd-attribute')
            {
                Write-Warning "Attribute is not in the attribute inclusion list: $($exportFlow.'cd-attribute')"
                $deleteExportFlowRule = $true
            }

            if ($deleteExportFlowRule -eq $true)
            {
                Write-Verbose "    Removing export flow rule: $xPathFilter"
                $exportFlowSet.RemoveChild($exportFlow)
            }
        }
    }

    Write-Verbose "Adding Export Attribute Flow Rule for MV.sAMAccountName --> CD.ProfileIdentifier"
    $fraggle = $spmaToConvert.CreateDocumentFragment()
    $fraggle.InnerXML = @'
    <export-flow cd-attribute="ProfileIdentifier" id="{994CF4BF-5DDA-43D3-B11F-AFEB0548CB5F}" suppress-deletions="true">
        <direct-mapping>
        <src-attribute>sAMAccountName</src-attribute>
        </direct-mapping>
    </export-flow>
'@
    $ExportFlowRulesForUsers = Select-Xml -Xml $spmaToConvert -XPath "//ma-data/export-attribute-flow/export-flow-set[@cd-object-type='user' and @mv-object-type='person']"
    $ExportFlowRulesForUsers.Node.AppendChild($fraggle)

    <# ma-data/provisioning-cleanup
    Not changed.
    This is the deprovisioning rule on the management agent and should be left alone.
    It does not refer to attributes or objects, so there is no work to be done.
    #>

    <# ma-data/extension
    Not changed.  
    This is the rules extension DLL that is still needed for sync rules to work.
    #>

    <# ma-data/controller-configuration
    Not changed.
    This node is the same for ECMA1 SPMA and ECMA2 SPMA
    #>

    <# ma-data/description
    Not changed.
    Cosmetic, and there was no value for either ECMA1 SPMA or ECMA2 SPMA
    #>

    <# ma-data/ma-ui-settings
    Not changed.
    Assuming this does not matter.  Let's go with hope until proven otherwise.
    #>

    <# ma-data/private-configuration
    Copied completely from ECMA2 SPMA.
    #>
    Write-Verbose "Replacing ma-data/private-configuration with the SPMA V2 private-configuration"
    $spmaToConvert.'saved-ma-configuration'.'ma-data'.'private-configuration'.InnerXml = @'
    <MAConfig>
      <ui-data>
        <xmlwizard>
          <properties>
            <sample_file>
            </sample_file>
            <code_page_description>
            </code_page_description>
          </properties>
          <partitions>
            <partition cd_name="default" guid="{0}" version="9">
              <object_class>contact</object_class>
              <object_class>group</object_class>
              <object_class>user</object_class>
            </partition>
          </partitions>
          <primary_class_mappings>
            <mapping object_class="contact" primary_class="contact" user_define="0">
              <attribute>ProfileIdentifier</attribute>
              <attribute>PreferredName</attribute>
              <attribute>domain</attribute>
              <attribute>Anchor</attribute>
              <attribute>export_password</attribute>
            </mapping>
            <mapping object_class="group" primary_class="group" user_define="0">
              <attribute>ProfileIdentifier</attribute>
              <attribute>SID</attribute>
              <attribute>PreferredName</attribute>
              <attribute>domain</attribute>
              <attribute>Anchor</attribute>
              <attribute>export_password</attribute>
            </mapping>
            <mapping object_class="user" primary_class="user" user_define="0">
              <attribute>ProfileIdentifier</attribute>
              <attribute>SID</attribute>
              <attribute>domain</attribute>
              <attribute>AccountName</attribute>
              <attribute>FirstName</attribute>
              <attribute>LastName</attribute>
              <attribute>PreferredName</attribute>
              <attribute>Picture</attribute>
              <attribute>UserName</attribute>
              <attribute>Anchor</attribute>
              <attribute>export_password</attribute>
            </mapping>
          </primary_class_mappings>
          <object_classes>
            <object_class cd_name="contact" selected="-1" user_define="0" configured="-1" anchor="" dn_as_anchor="0">
              <attribute mandatory="0">ProfileIdentifier</attribute>
              <attribute mandatory="0">PreferredName</attribute>
              <attribute mandatory="0">domain</attribute>
              <attribute mandatory="-1">Anchor</attribute>
              <attribute mandatory="0">export_password</attribute>
            </object_class>
            <object_class cd_name="group" selected="-1" user_define="0" configured="-1" anchor="" dn_as_anchor="0">
              <attribute mandatory="0">ProfileIdentifier</attribute>
              <attribute mandatory="0">SID</attribute>
              <attribute mandatory="0">PreferredName</attribute>
              <attribute mandatory="0">domain</attribute>
              <attribute mandatory="-1">Anchor</attribute>
              <attribute mandatory="0">export_password</attribute>
            </object_class>
            <object_class cd_name="user" selected="-1" user_define="0" configured="-1" anchor="" dn_as_anchor="0">
              <attribute mandatory="0">ProfileIdentifier</attribute>
              <attribute mandatory="0">SID</attribute>
              <attribute mandatory="0">domain</attribute>
              <attribute mandatory="0">AccountName</attribute>
              <attribute mandatory="0">FirstName</attribute>
              <attribute mandatory="0">LastName</attribute>
              <attribute mandatory="0">PreferredName</attribute>
              <attribute mandatory="0">Picture</attribute>
              <attribute mandatory="0">UserName</attribute>
              <attribute mandatory="-1">Anchor</attribute>
              <attribute mandatory="0">export_password</attribute>
            </object_class>
          </object_classes>
          <attributes>
            <attribute cd_name="AccountName" sample_data="" binary="0" multi_valued="0" file_reference="0" selected="-1" lower_bound="" upper_bound="" type="String" user_define="0" />
            <attribute cd_name="Anchor" sample_data="" binary="0" multi_valued="0" file_reference="0" selected="-1" lower_bound="" upper_bound="" type="String" user_define="0" />
            <attribute cd_name="domain" sample_data="" binary="0" multi_valued="0" file_reference="0" selected="-1" lower_bound="" upper_bound="" type="String" user_define="0" />
            <attribute cd_name="FirstName" sample_data="" binary="0" multi_valued="0" file_reference="0" selected="-1" lower_bound="" upper_bound="" type="String" user_define="0" />
            <attribute cd_name="LastName" sample_data="" binary="0" multi_valued="0" file_reference="0" selected="-1" lower_bound="" upper_bound="" type="String" user_define="0" />
            <attribute cd_name="Picture" sample_data="" binary="0" multi_valued="0" file_reference="0" selected="-1" lower_bound="" upper_bound="" type="Binary" user_define="0" />
            <attribute cd_name="PreferredName" sample_data="" binary="0" multi_valued="0" file_reference="0" selected="-1" lower_bound="" upper_bound="" type="String" user_define="0" />
            <attribute cd_name="ProfileIdentifier" sample_data="" binary="0" multi_valued="0" file_reference="0" selected="-1" lower_bound="" upper_bound="" type="String" user_define="0" />
            <attribute cd_name="SID" sample_data="" binary="0" multi_valued="0" file_reference="0" selected="-1" lower_bound="" upper_bound="" type="Binary" user_define="0" />
            <attribute cd_name="UserName" sample_data="" binary="0" multi_valued="0" file_reference="0" selected="-1" lower_bound="" upper_bound="" type="String" user_define="0" />
            <attribute cd_name="export_password" sample_data="" binary="0" multi_valued="0" file_reference="0" selected="-1" lower_bound="" upper_bound="" type="String" user_define="0" />
          </attributes>
          <anchor>
            <attribute object_class="contact">Anchor</attribute>
            <attribute object_class="group">Anchor</attribute>
            <attribute object_class="user">Anchor</attribute>
          </anchor>
        </xmlwizard>
        <ma-help-text>Using this Connector you can synchronize with SharePoint Profile Store</ma-help-text>
      </ui-data>
      <importing>
        <dn>
          <attribute object_class="contact">Anchor</attribute>
          <attribute object_class="group">Anchor</attribute>
          <attribute object_class="user">Anchor</attribute>
        </dn>
        <anchor>
          <attribute object_class="contact">Anchor</attribute>
          <attribute object_class="group">Anchor</attribute>
          <attribute object_class="user">Anchor</attribute>
        </anchor>
        <per-class-settings>
          <class>
            <name>contact</name>
            <anchor>
              <attribute>Anchor</attribute>
            </anchor>
          </class>
          <class>
            <name>group</name>
            <anchor>
              <attribute>Anchor</attribute>
            </anchor>
          </class>
          <class>
            <name>user</name>
            <anchor>
              <attribute>Anchor</attribute>
            </anchor>
          </class>
        </per-class-settings>
      </importing>
      <exporting>
      </exporting>
      <ldap-dn>0</ldap-dn>
      <change_type_attribute>
      </change_type_attribute>
      <add_change_type_value>Add</add_change_type_value>
      <modify_change_type_value>Modify</modify_change_type_value>
      <delete_change_type_value>Delete</delete_change_type_value>
      <primary_class_mappings>
        <mapping>
          <primary_class>contact</primary_class>
          <oc-value>contact</oc-value>
        </mapping>
        <mapping>
          <primary_class>group</primary_class>
          <oc-value>group</oc-value>
        </mapping>
        <mapping>
          <primary_class>user</primary_class>
          <oc-value>user</oc-value>
        </mapping>
      </primary_class_mappings>
      <enable-unapplied-merge>0</enable-unapplied-merge>
      <password-extension-config>
        <password-extension-enabled>0</password-extension-enabled>
        <dll data-owner="ISV">
        </dll>
        <password-set-enabled>
        </password-set-enabled>
        <password-change-enabled>
        </password-change-enabled>
        <connection-info>
          <connect-to>
          </connect-to>
          <user>
          </user>
        </connection-info>
        <timeout>
        </timeout>
      </password-extension-config>
      <file-type>Extensible2</file-type>
      <extension-config>
        <filename data-owner="ISV">Microsoft.IdentityManagement.Connector.Sharepoint.dll</filename>
        <import-default-page-size>50</import-default-page-size>
        <import-max-page-size>500</import-max-page-size>
        <export-default-page-size>50</export-default-page-size>
        <export-max-page-size>500</export-max-page-size>
        <export-mode data-owner="ISV">call-based</export-mode>
        <import-mode>call-based</import-mode>
        <export-enabled data-owner="ISV">1</export-enabled>
        <import-enabled data-owner="ISV">1</import-enabled>
        <capability-bits>2751862784</capability-bits>
        <export-type>2</export-type>
        <discovery-partition>
        </discovery-partition>
        <discovery-schema>extensibility</discovery-schema>
        <discovery-hierarchy>
        </discovery-hierarchy>
        <password-management-enabled>
        </password-management-enabled>
        <assembly-version>4.3.836.0</assembly-version>
        <supports-parameters-ex>0</supports-parameters-ex>
      </extension-config>
      <parameter-definitions refreshSchema="0" refreshPartition="0" refreshConnectivityParameters="0" refreshGlobalParameters="0" refreshOtherParameters="0" refreshSchemaParameters="0" refreshCapabilitiesParameters="0">
        <parameter>
          <name>
          </name>
          <use>connectivity</use>
          <type>label</type>
          <validation>
          </validation>
          <text>SharePoint Server Info:</text>
          <default-value>
          </default-value>
        </parameter>
        <parameter>
          <name>HTTP Protocol</name>
          <use>connectivity</use>
          <type>drop-down</type>
          <validation>HTTP,HTTPS,</validation>
          <text>
          </text>
          <default-value>HTTP</default-value>
          <dropdown-extensible>0</dropdown-extensible>
        </parameter>
        <parameter>
          <name>Host Name</name>
          <use>connectivity</use>
          <type>string</type>
          <validation>
          </validation>
          <text>
          </text>
          <default-value>
          </default-value>
        </parameter>
        <parameter>
          <name>Port</name>
          <use>connectivity</use>
          <type>string</type>
          <validation>
          </validation>
          <text>
          </text>
          <default-value>
          </default-value>
        </parameter>
        <parameter>
          <name>Application ID</name>
          <use>connectivity</use>
          <type>string</type>
          <validation>
          </validation>
          <text>
          </text>
          <default-value>
          </default-value>
        </parameter>
        <parameter>
          <name>
          </name>
          <use>connectivity</use>
          <type>divider</type>
          <validation>
          </validation>
          <text>
          </text>
          <default-value>
          </default-value>
        </parameter>
        <parameter>
          <name>
          </name>
          <use>connectivity</use>
          <type>label</type>
          <validation>
          </validation>
          <text>SharePoint User Credential:</text>
          <default-value>
          </default-value>
        </parameter>
        <parameter>
          <name>Domain</name>
          <use>connectivity</use>
          <type>string</type>
          <validation>
          </validation>
          <text>
          </text>
          <default-value>
          </default-value>
        </parameter>
        <parameter>
          <name>User Name</name>
          <use>connectivity</use>
          <type>string</type>
          <validation>
          </validation>
          <text>
          </text>
          <default-value>
          </default-value>
        </parameter>
        <parameter>
          <name>Password</name>
          <use>connectivity</use>
          <type>encrypted-string</type>
          <validation>
          </validation>
          <text>
          </text>
          <default-value>
          </default-value>
        </parameter>
        <parameter>
          <name>
          </name>
          <use>connectivity</use>
          <type>divider</type>
          <validation>
          </validation>
          <text>
          </text>
          <default-value>
          </default-value>
        </parameter>
        <parameter>
          <name>
          </name>
          <use>connectivity</use>
          <type>label</type>
          <validation>
          </validation>
          <text>User Picture Settings:</text>
          <default-value>
          </default-value>
        </parameter>
        <parameter>
          <name>Picture Flow Direction</name>
          <use>connectivity</use>
          <type>drop-down</type>
          <validation>Import only (ALWAYS from SharePoint),Export only (NEVER from SharePoint),</validation>
          <text>
          </text>
          <default-value>Import only (ALWAYS from SharePoint)</default-value>
          <dropdown-extensible>0</dropdown-extensible>
        </parameter>
      </parameter-definitions>
      <parameter-values>
        <parameter name="Password" type="encrypted-string" use="connectivity" encrypted="1">
        </parameter>
        <parameter name="HTTP Protocol" type="drop-down" use="connectivity">HTTP</parameter>
        <parameter name="Host Name" type="string" use="connectivity">{1}</parameter>
        <parameter name="Port" type="string" use="connectivity">{2}</parameter>
        <parameter name="Application ID" type="string" use="connectivity">
        </parameter>
        <parameter name="Domain" type="string" use="connectivity">{3}</parameter>
        <parameter name="User Name" type="string" use="connectivity">{4}</parameter>
        <parameter name="Picture Flow Direction" type="drop-down" use="connectivity">Export only (NEVER from SharePoint)</parameter>
      </parameter-values>
      <case_normalize_dn_for_anchor>1</case_normalize_dn_for_anchor>
      <default_visible_attributes>
        <attribute>Anchor</attribute>
        <attribute>ProfileIdentifier</attribute>
        <attribute>SID</attribute>
        <attribute>SourceReference</attribute>
        <attribute>GroupType</attribute>
        <attribute>PreferredName</attribute>
        <attribute>Description</attribute>
        <attribute>Url</attribute>
        <attribute>MailNickName</attribute>
        <attribute>domain</attribute>
        <attribute>Member</attribute>
        <attribute>SPS-DistinguishedName</attribute>
        <attribute>SPS-SourceObjectDN</attribute>
        <attribute>UserName_Contact</attribute>
        <attribute>UserProfile_GUID</attribute>
        <attribute>AccountName</attribute>
        <attribute>FirstName</attribute>
        <attribute>SPS-PhoneticFirstName</attribute>
        <attribute>LastName</attribute>
        <attribute>SPS-PhoneticLastName</attribute>
        <attribute>SPS-PhoneticDisplayName</attribute>
        <attribute>WorkPhone</attribute>
        <attribute>Department</attribute>
        <attribute>Title</attribute>
        <attribute>SPS-JobTitle</attribute>
        <attribute>SPS-Department</attribute>
        <attribute>Manager</attribute>
        <attribute>AboutMe</attribute>
        <attribute>PersonalSpace</attribute>
        <attribute>Picture</attribute>
        <attribute>UserName</attribute>
        <attribute>QuickLinks</attribute>
        <attribute>WebSite</attribute>
        <attribute>PublicSiteRedirect</attribute>
        <attribute>SPS-DataSource</attribute>
        <attribute>SPS-MemberOf</attribute>
        <attribute>SPS-Dotted-line</attribute>
        <attribute>SPS-Peers</attribute>
        <attribute>SPS-Responsibility</attribute>
        <attribute>SPS-SipAddress</attribute>
        <attribute>SPS-MySiteUpgrade</attribute>
        <attribute>SPS-DontSuggestList</attribute>
        <attribute>SPS-ProxyAddresses</attribute>
        <attribute>SPS-HireDate</attribute>
        <attribute>SPS-DisplayOrder</attribute>
        <attribute>SPS-ClaimID</attribute>
        <attribute>SPS-ClaimProviderID</attribute>
        <attribute>SPS-ClaimProviderType</attribute>
        <attribute>SPS-LastColleagueAdded</attribute>
        <attribute>SPS-OWAUrl</attribute>
        <attribute>SPS-SavedAccountName</attribute>
        <attribute>SPS-ResourceAccountName</attribute>
        <attribute>SPS-ObjectExists</attribute>
        <attribute>SPS-MasterAccountName</attribute>
        <attribute>SPS-UserPrincipalName</attribute>
        <attribute>SPS-PersonalSiteCapabilities</attribute>
        <attribute>SPS-O15FirstRunExperience</attribute>
        <attribute>SPS-PersonalSiteInstantiationState</attribute>
        <attribute>SPS-LastKeywordAdded</attribute>
        <attribute>SPS-FeedIdentifier</attribute>
        <attribute>WorkEmail</attribute>
        <attribute>CellPhone</attribute>
        <attribute>Fax</attribute>
        <attribute>HomePhone</attribute>
        <attribute>Office</attribute>
        <attribute>SPS-Location</attribute>
        <attribute>Assistant</attribute>
        <attribute>SPS-PastProjects</attribute>
        <attribute>SPS-Skills</attribute>
        <attribute>SPS-School</attribute>
        <attribute>SPS-Birthday</attribute>
        <attribute>SPS-StatusNotes</attribute>
        <attribute>SPS-Interests</attribute>
        <attribute>SPS-HashTags</attribute>
        <attribute>SPS-PictureTimestamp</attribute>
        <attribute>SPS-EmailOptin</attribute>
        <attribute>SPS-PicturePlaceholderState</attribute>
        <attribute>SPS-PrivacyPeople</attribute>
        <attribute>SPS-PrivacyActivity</attribute>
        <attribute>SPS-PictureExchangeSyncState</attribute>
        <attribute>SPS-MUILanguages</attribute>
        <attribute>SPS-ContentLanguages</attribute>
        <attribute>SPS-TimeZone</attribute>
        <attribute>SPS-RegionalSettings-FollowWeb</attribute>
        <attribute>SPS-Locale</attribute>
        <attribute>SPS-CalendarType</attribute>
        <attribute>SPS-AltCalendarType</attribute>
        <attribute>SPS-AdjustHijriDays</attribute>
        <attribute>SPS-ShowWeeks</attribute>
        <attribute>SPS-WorkDays</attribute>
        <attribute>SPS-WorkDayStartHour</attribute>
        <attribute>SPS-WorkDayEndHour</attribute>
        <attribute>SPS-Time24</attribute>
        <attribute>SPS-FirstDayOfWeek</attribute>
        <attribute>SPS-FirstWeekOfYear</attribute>
        <attribute>SPS-RegionalSettings-Initialized</attribute>
      </default_visible_attributes>
    </MAConfig>
'@ -F @(
        $maPartitionID    #{0} partition ID
        $maConnectTo.Host #{1} Host Name
        $maConnectTo.Port #{2} Port
        $maDomain         #{3} Domain
        $maUserName       #{4} User Name
    )

    <# ma-data/SyncConfig-refresh-schema
    Not changed.
    Pretty sure this is an operational attribute, so does not need to be changed.
    #>

    <# ma-data/ma-partition-data
    Copied completely from ECMA2 SPMA.
    #>
    Write-Verbose "Replacing ma-data/ma-partition-data with the SPMA V2 ma-partition-data"
    $spmaToConvert.'saved-ma-configuration'.'ma-data'.'ma-partition-data'.InnerXml = @'
    <partition>
      <id>{0}</id>
      <name>default</name>
      <creation-time>2015-07-14 14:18:16.617</creation-time>
      <last-modification-time>2015-07-14 14:22:06.090</last-modification-time>
      <version>8</version>
      <selected>1</selected>
      <filter>
        <object-classes>
          <object-class>contact</object-class>
          <object-class>group</object-class>
          <object-class>user</object-class>
        </object-classes>
        <containers>
          <exclusions />
          <inclusions>
            <inclusion>
            </inclusion>
          </inclusions>
        </containers>
      </filter>
      <custom-data>
        <ecma-partition-data>
          <dn>default</dn>
          <name>default</name>
          <is-domain>1</is-domain>
        </ecma-partition-data>
        <water-mark></water-mark>
      </custom-data>
      <allowed-operations>28</allowed-operations>
      <current>
        <batch-number>0</batch-number>
        <sequence-number>0</sequence-number>
      </current>
      <last-successful-batch>0</last-successful-batch>
      <filter-hints>
        <object-classes>
          <object-class>
            <name>contact</name>
            <hierarchy>
              <object-class>contact</object-class>
            </hierarchy>
            <included>1</included>
          </object-class>
          <object-class>
            <name>group</name>
            <hierarchy>
              <object-class>group</object-class>
            </hierarchy>
            <included>1</included>
          </object-class>
          <object-class>
            <name>user</name>
            <hierarchy>
              <object-class>user</object-class>
            </hierarchy>
            <included>1</included>
          </object-class>
        </object-classes>
      </filter-hints>
    </partition>
'@ -F $maPartitionID

    <# ma-data/ma-run-data
    Replaced the Run Step details with proper details from ECMA2 SPMA
    Replaced the Partition ID for each run profile step
    #>
    Write-Verbose "Updating the Run Profile: Export"
    $ExportRunProfile = Select-Xml -Xml $spmaToConvert -XPath "//ma-run-data/run-configuration/configuration/step[./step-type[@type='export']]"
    $ExportRunProfile.Node.InnerXml = @'
    <step-type type="export">
    </step-type>
    <threshold>
      <batch-size>50</batch-size>
    </threshold>
    <partition>{00000000-0000-0000-0000-000000000000}</partition>
    <custom-data>
      <extensible2-step-data>
        <timeout>0</timeout>
        <batch-size>50</batch-size>
      </extensible2-step-data>
      <parameter-values />
    </custom-data>
'@

    Write-Verbose "Updating the Run Profile: Full Sync"
    $FullSyncRunProfile = Select-Xml -Xml $spmaToConvert -XPath "//ma-run-data/run-configuration/configuration/step[./step-type[@type='apply-rules']/apply-rules-subtype[.='reevaluate-flow-connectors']]"
    $FullSyncRunProfile.Node.InnerXml = @'
    <step-type type="apply-rules">
        <apply-rules-subtype>reevaluate-flow-connectors</apply-rules-subtype>
    </step-type>
    <threshold>
        <batch-size>1</batch-size>
    </threshold>
    <partition>{00000000-0000-0000-0000-000000000000}</partition>
    <custom-data>
    </custom-data>
'@

    Write-Verbose "Updating the Run Profile: Delta Sync"
    $DeltaSyncRunProfile = Select-Xml -Xml $spmaToConvert -XPath "//ma-run-data/run-configuration/configuration/step[./step-type[@type='apply-rules']/apply-rules-subtype[.='apply-pending']]"
    $DeltaSyncRunProfile.Node.InnerXml = @'
    <step-type type="apply-rules">
      <apply-rules-subtype>apply-pending</apply-rules-subtype>
    </step-type>
    <threshold>
      <batch-size>1</batch-size>
    </threshold>
    <partition>{00000000-0000-0000-0000-000000000000}</partition>
    <custom-data>
    </custom-data>
'@

    Write-Verbose "Updating the Run Profile: Full Import"
    $FullImportRunProfile = Select-Xml -Xml $spmaToConvert -XPath "//ma-run-data/run-configuration/configuration/step[./step-type[@type='full-import']/import-subtype[.='to-cs']]"
    $FullImportRunProfile.Node.InnerXml = @'
    <step-type type="full-import">
      <import-subtype>to-cs</import-subtype>
    </step-type>
    <threshold>
      <batch-size>50</batch-size>
    </threshold>
    <partition>{00000000-0000-0000-0000-000000000000}</partition>
    <custom-data>
      <extensible2-step-data>
        <timeout>0</timeout>
        <batch-size>50</batch-size>
      </extensible2-step-data>
      <parameter-values />
    </custom-data>
'@

    Write-Verbose "Updating the Run Profile: Delta Import"
    $DeltaImportRunProfile = Select-Xml -Xml $spmaToConvert -XPath "//ma-run-data/run-configuration/configuration/step[./step-type[@type='delta-import']/import-subtype[.='to-cs']]"
    $DeltaImportRunProfile.Node.InnerXml = @'
    <step-type type="delta-import">
      <import-subtype>to-cs</import-subtype>
    </step-type>
    <threshold>
      <batch-size>50</batch-size>
    </threshold>
    <partition>{00000000-0000-0000-0000-000000000000}</partition>
    <custom-data>
      <extensible2-step-data>
        <timeout>0</timeout>
        <batch-size>50</batch-size>
      </extensible2-step-data>
      <parameter-values />
    </custom-data>
'@

    Write-Verbose "Updating ma-run-data/run-configuration/configuration/step/partition with the correct partition ID"
    foreach ($RunProfilePartitionGuid in Select-Xml -Xml $spmaToConvert -XPath '//ma-run-data/run-configuration/configuration/step/partition')
    {
        $RunProfilePartitionGuid.Node.'#text' = $maPartitionID
    }

    <# ma-data/capabilities-mask
    Copied the value from the ECMA2 SPMA.
    #>
    Write-Verbose "Updating ma-data/capabilities-mask to 'a4061801'"
    $spmaToConvert.'saved-ma-configuration'.'ma-data'.'capabilities-mask' = 'a4061801'

    <# ma-data/export-type
    Copied the value from the ECMA2 SPMA.
    #>
    Write-Verbose "Updating ma-data/export-type to '2'"
    $spmaToConvert.'saved-ma-configuration'.'ma-data'.'export-type' = '2'

    <# ma-data/dn-construction
    Copied from the ECMA2 SPMA.
    #>
    Write-Verbose "Replacing ma-data/dn-construction with the SPMA V2 dn-construction"
    $fraggle = $spmaToConvert.CreateDocumentFragment()
    $fraggle.InnerXML = @'
    <dn-construction>
    <dn object-type="contact">
      <attribute>Anchor</attribute>
    </dn>
    <dn object-type="group">
      <attribute>Anchor</attribute>
    </dn>
    <dn object-type="user">
      <attribute>Anchor</attribute>
    </dn>
    </dn-construction>
'@
    $spmaToConvert.'saved-ma-configuration'.'ma-data'.ReplaceChild($fraggle, $spmaToConvert.SelectSingleNode('//dn-construction'))

    <# ma-data/password-sync
    Copied from the ECMA2 SPMA.
    #>
    Write-Verbose "Replacing ma-data/password-sync with the SPMA V2 password-sync"
    $fraggle = $spmaToConvert.CreateDocumentFragment()
    $fraggle.InnerXML = @'
    <password-sync>
      <maximum-retry-count>10</maximum-retry-count>
      <retry-interval>60</retry-interval>
      <allow-low-security>0</allow-low-security>
    </password-sync>
'@
    $spmaToConvert.'saved-ma-configuration'.'ma-data'.ReplaceChild($fraggle, $spmaToConvert.SelectSingleNode('//password-sync'))

    <# ma-data/component_mappings
    Not changed.
    Not sure what this node does.  
    No need to worry because there was no value for either ECMA1 SPMA or ECMA2 SPMA
    #>

    ### Save
	Write-Verbose "Saving the management agent XML file: $($spma1File.Path)"
    $spmaToConvert.Save($spma1File.Path)
}##Closing: function ConvertTo-SharePointEcma2
