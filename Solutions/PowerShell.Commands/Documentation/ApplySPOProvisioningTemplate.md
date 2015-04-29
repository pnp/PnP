#Apply-SPOProvisioningTemplate
*Topic automatically generated on: 2015-04-28*

Applies a provisioning template to a web
##Syntax
```powershell
Apply-SPOProvisioningTemplate [-Web [<WebPipeBind>]] -Path [<String>]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Path|String|True|Path to the xml file containing the provisioning template.
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
