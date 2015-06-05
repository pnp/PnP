#Apply-SPOProvisioningTemplate
*Topic automatically generated on: 2015-06-03*

Applies a provisioning template to a web
##Syntax
```powershell
Apply-SPOProvisioningTemplate [-Web <WebPipeBind>] -Path <String>
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|Path|String|True|Path to the xml file containing the provisioning template.|
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
    
    PS:> Apply-SPOProvisioningTemplate -Path template.xml

Applies a provisioning template in XML format to the current web.
<!-- Ref: 625576F1A13C1C53CAA2471F8BB39B44 -->