#Uninstall-SPOAppInstance
*Topic automatically generated on: 2015-04-29*

Removes an add-in from a site
##Syntax
```powershell
Uninstall-SPOAppInstance -Identity <AppPipeBind> [-Force [<SwitchParameter>]] [-Web <WebPipeBind>]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Force|SwitchParameter|False|
Identity|AppPipeBind|True|Appinstance or Id of the add-in to remove.
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
##Examples

###Example 1
    PS:> Uninstall-SPOAppInstance -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe


###Example 2
    PS:> Uninstall-SPOAppInstance -Identity $appinstance

<!-- Ref: 8B209E5F919395582DB8EF1636287A7C -->