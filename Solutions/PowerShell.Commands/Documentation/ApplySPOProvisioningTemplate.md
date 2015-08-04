#Apply-SPOProvisioningTemplate
*Topic automatically generated on: 2015-08-04*

Applies a provisioning template to a web
##Syntax
```powershell
Apply-SPOProvisioningTemplate [-ResourceFolder [<String>]] [-OverwriteSystemPropertyBagValues [<SwitchParameter>]] [-Web [<WebPipeBind>]] -Path [<String>]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
OverwriteSystemPropertyBagValues|SwitchParameter|False|Specify this parameter if you want to overwrite and/or create properties that are known to be system entries (starting with vti_, dlc_, etc.)
Path|String|True|Path to the xml file containing the provisioning template.
ResourceFolder|String|False|Root folder where resources/files that are being referenced in the template are located. If not specified the same folder as where the provisioning template is located will be used.
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
##Examples

###Example 1
    
    PS:> Apply-SPOProvisioningTemplate -Path template.xml

Applies a provisioning template in XML format to the current web.

###Example 2
    
    PS:> Apply-SPOProvisioningTemplate -Path template.xml -ResourceFolder c:\provisioning\resources

Applies a provisioning template in XML format to the current web. Any resources like files that are referenced in the template will be retrieved from the folder as specified with the ResourceFolder parameter.
