#Get-SPOProvisioningTemplate
*Topic automatically generated on: 2015-04-28*

Generates a provisioning template from a web
##Syntax
```powershell
Get-SPOProvisioningTemplate [-IncludeAllTermGroups [<SwitchParameter>]] [-IncludeSiteCollectionTermGroup [<SwitchParameter>]] [-PersistComposedLookFiles [<SwitchParameter>]] [-Force [<SwitchParameter>]] [-Encoding [<Encoding>]] [-Web [<WebPipeBind>]] [-Out [<String>]] [-Schema [<XMLPnPSchemaVersion>]]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Encoding|Encoding|False|
Force|SwitchParameter|False|Overwrites the output file if it exists.
IncludeAllTermGroups|SwitchParameter|False|If specified, all term groups will be included. Overrides IncludeSiteCollectionTermGroup.
IncludeSiteCollectionTermGroup|SwitchParameter|False|If specified, all the site collection term group will be included. Overridden by IncludeAllTermGroups.
Out|String|False|Filename to write to, optionally including full path
PersistComposedLookFiles|SwitchParameter|False|If specified the files making up the composed look (background image, font file and color file) will be saved.
Schema|XMLPnPSchemaVersion|False|The schema of the output to use, defaults to the latest schema
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
