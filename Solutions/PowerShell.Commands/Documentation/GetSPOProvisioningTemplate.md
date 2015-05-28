#Get-SPOProvisioningTemplate
*Topic automatically generated on: 2015-05-27*

Generates a provisioning template from a web
##Syntax
```powershell
Get-SPOProvisioningTemplate [-IncludeAllTermGroups [<SwitchParameter>]] [-IncludeSiteCollectionTermGroup [<SwitchParameter>]] [-PersistComposedLookFiles [<SwitchParameter>]] [-Force [<SwitchParameter>]] [-Encoding <Encoding>] [-Web <WebPipeBind>] [-Out <String>] [-Schema <XMLPnPSchemaVersion>]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Encoding|Encoding|False|
Force|SwitchParameter|False|Overwrites the output file if it exists.
IncludeAllTermGroups|SwitchParameter|False|If specified, all term groups will be included. Overrides IncludeSiteCollectionTermGroup.
IncludeSiteCollectionTermGroup|SwitchParameter|False|If specified, all the site collection term groups will be included. Overridden by IncludeAllTermGroups.
Out|String|False|Filename to write to, optionally including full path
PersistComposedLookFiles|SwitchParameter|False|If specified the files making up the composed look (background image, font file and color file) will be saved.
Schema|XMLPnPSchemaVersion|False|The schema of the output to use, defaults to the latest schema
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
##Examples

###Example 1
    
    PS:> Get-SPOProvisioningTemplate -Out template.xml

Extracts a provisioning template in XML format from the current web.

###Example 2
    
    PS:> Get-SPOProvisioningTemplate -Out template.xml -PersistComposedLookFiles

Extracts a provisioning template in XML format from the current web and saves the files that make up the composed look to the same folder as where the template is saved.

###Example 3
    
    PS:> Get-SPOProvisioningTemplate -Out template.xml -Schema V201503

Extracts a provisioning template in XML format from the current web and saves it in the V201503 version of the schema.

###Example 4
    
    PS:> Get-SPOProvisioningTemplate -Out template.xml -IncludeAllTermGroups

Extracts a provisioning template in XML format from the current web and includes all term groups, term sets and terms from the Managed Metadata Service Taxonomy.

###Example 5
    
    PS:> Get-SPOProvisioningTemplate -Out template.xml -IncludeSiteCollectionTermGroup

Extracts a provisioning template in XML format from the current web and includes the term group currently (if set) assigned to the site collection.
<!-- Ref: B2DF795D522B4BBCFC6FC7099A77CDD6 -->