#Uninstall-SPOAppInstance
*Topic automatically generated on: 2015-05-28*

Removes an add-in from a site
##Syntax
```powershell
Uninstall-SPOAppInstance -Identity <AppPipeBind> [-Force [<SwitchParameter>]] [-Web <WebPipeBind>]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Force|SwitchParameter|False|
<<<<<<< HEAD
Identity|AppPipeBind|True|Appinstance or Id of the addin to remove.
=======
Identity|AppPipeBind|True|Appinstance or Id of the add-in to remove.
>>>>>>> 80f41dceaa3bcd5c3eb44a5dfcc3e3a4908809ab
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
##Examples

###Example 1
    PS:> Uninstall-SPOAppInstance -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe


###Example 2
    PS:> Uninstall-SPOAppInstance -Identity $appinstance

<!-- Ref: 5BEF6C0EB5535E4EA7A92AA39782206C -->
